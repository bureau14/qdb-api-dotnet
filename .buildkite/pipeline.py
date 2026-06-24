#!/usr/bin/env python3
"""Buildkite dynamic pipeline generator for qdb-api-dotnet.

Step templates in steps/*.yml define nearly-complete Buildkite steps with
{placeholder} variables.  This script loads them, substitutes variables, and
overlays environment variables and the Docker plugin per platform.

Usage:
    python3 pipeline.py           # emit pipeline YAML to stdout
    python3 pipeline.py check     # validate without emitting
"""

from __future__ import annotations

import dataclasses
import sys
from pathlib import Path

from buildkite_sdk import CommandStep, GroupStep, Pipeline

sys.path.insert(0, str(Path(__file__).parent / "tools"))
from qdb_pipeline import (  # noqa: E402
    Platform,
    apply_docker,
    get_git_ref,
    load_template,
    merge_env,
    select_platforms,
    set_artifact_plugin_options,
    validate_pipeline,
)

STEPS_DIR = Path(__file__).parent / "steps"

# Quasardb-specific toolchain overlays on top of shared infrastructure platforms.
_LINUX = dict(
    docker_image="bureau14/builder:rhel7",
    docker_volumes=("/var/lib/ccache:/var/lib/ccache",),
)
_WIN = dict()
_MACOS = dict()
_OS_OVERLAY = {"linux": _LINUX, "windows": _WIN, "macos": _MACOS}
PLATFORMS: list[Platform] = [
    dataclasses.replace(p, **_OS_OVERLAY.get(p.os, {}))
    for p in select_platforms(
        "linux-amd64-core2",
        "windows-amd64-core2",
    )
]

BUILD_TYPES = ["Release"]
DOTNET_VERSIONS = ["6.0", "8.0"]

# Environment variable layering: global → step → os → os+step → platform compilers.
GLOBAL_ENV: dict[str, str] = {
    "AWS_DEFAULT_REGION": "eu-west-1",
    "NUGET_ENABLE_LEGACY_CSPROJ_PACK": "true",
}
STEP_ENV: dict[str, dict[str, str]] = {}
OS_ENV: dict[str, dict[str, str]] = {
    "linux": {
        "DOTNET_PATH": "/usr/share/dotnet/dotnet"
    },
    "windows": {},
}
OS_STEP_ENV: dict[str, dict[str, str]] = {}
CPU_ENV: dict[str, dict[str, str]] = {}


def _env(p: Platform, step_name: str, build_type: str, dotnet_version: str) -> dict[str, str]:
    """Compose the full environment dict for one step."""
    return merge_env(
        GLOBAL_ENV,
        STEP_ENV.get(step_name, {}),
        OS_ENV.get(p.os, {}),
        OS_STEP_ENV.get(f"{p.os}/{step_name}", {}),
        CPU_ENV.get(p.cpu, {}),
        {
            "BUILD_CONFIGURATION": build_type,
            "DOTNET_VERSION": dotnet_version,
        },
        platform=p,
    )


def _artifact_plugin_config(step: dict) -> dict | None:
    """Return the qdb-artifacts plugin config for a generated step."""
    for plugin_dict in step.get("plugins", []):
        for plugin_name, plugin_config in plugin_dict.items():
            if plugin_name.startswith("bureau14/qdb-artifacts#"):
                return plugin_config
    return None


def _configure_artifact_downloads(step: dict, p: Platform, git_ref: str) -> None:
    """Keep only the native artifacts needed by this platform."""
    plugin_config = _artifact_plugin_config(step)
    if not plugin_config:
        return

    projects = plugin_config.get("download", {}).get("projects", [])
    dependency_slug = p.slug("release")

    required_output_dirs = {"qdb/bin", "Quasardb/linux"}
    if p.os == "windows":
        required_output_dirs.add("Quasardb/win64")

    selected_projects = []
    for project in projects:
        output_dir = project.get("output-dir")
        if output_dir not in required_output_dirs:
            continue

        project.setdefault("git-ref", git_ref)
        if output_dir == "qdb/bin" and p.os == "windows":
            project["files"] = ["*-server.tar.zst!bin/*", "*-utils.tar.zst!bin/*"]

        if output_dir == "Quasardb/linux":
            project.setdefault("variant", "linux-core2-release")
        elif output_dir == "Quasardb/win64":
            project.setdefault("variant", "windows-core2-release")
        else:
            project.setdefault("variant", dependency_slug)
        selected_projects.append(project)

    plugin_config["download"]["projects"] = selected_projects


def _disable_artifact_upload(step: dict) -> None:
    """Remove qdb-artifacts upload/promote sections for non-packaging jobs."""
    plugin_config = _artifact_plugin_config(step)
    if plugin_config:
        plugin_config.pop("upload", None)
        plugin_config.pop("promote", None)


def _apply_step_shape(step: dict, p: Platform) -> None:
    """Apply platform-specific template pruning shared by all .NET matrix rows."""
    # apply_docker(step, p.docker_image, p.docker_volumes)
    if p.os != "windows":
        _disable_artifact_upload(step)


def generate_pipeline() -> Pipeline:
    """Load templates, expand across platforms × build_types, overlay env and docker."""
    pipeline = Pipeline()
    git_ref = get_git_ref()
    group_steps = {}
    variants: list[str] = []

    for p in PLATFORMS:
        for bt in BUILD_TYPES:
            for dotnet_version in DOTNET_VERSIONS:
                dotnet_slug = f"dotnet{dotnet_version.split('.', 1)[0]}"
                slug = p.slug(bt.lower(), dotnet_slug)
                variants.append(slug)

                tvars = {
                    "slug": slug,
                    "queue": f"{p.queue_os}-{p.arch}",
                    "name": slug.replace("-", " ").title(),
                }

                artifact_vars_per_step = {
                    "upload": {"variant": slug, "git-ref": git_ref},
                    "promote": {"variant": slug, "git-ref": git_ref},
                }

                step = load_template(STEPS_DIR / "_build.yml", **tvars)
                env = _env(p, "build", bt, dotnet_version)
                env.update(step.get("env") or {})
                step["env"] = env
                _configure_artifact_downloads(step, p, git_ref)
                _apply_step_shape(step, p)
                set_artifact_plugin_options(step, artifact_vars_per_step)

                # add step to group
                group_name = p.slug(bt.lower()).replace("-", " ").title()
                if group_name not in group_steps:
                    group_steps[group_name] = []
                group_steps[group_name].append(step)

    # create groups and add to pipeline
    for group, steps in group_steps.items():
        group_step = GroupStep(group=group, steps=steps)
        pipeline.add_step(group_step)

    step = load_template(STEPS_DIR / "_test_report.yml")
    step["depends_on"] = [f"build-{variant}" for variant in variants]
    pipeline.add_step(CommandStep.from_dict(step))

    return pipeline


def main() -> None:
    command = sys.argv[1] if len(sys.argv) > 1 else "generate"

    try:
        pipeline = generate_pipeline()
    except Exception as e:
        print(f"[FAIL] Pipeline generation failed: {e}", file=sys.stderr)
        sys.exit(1)

    if command == "generate":
        print(pipeline.to_yaml())
    elif command == "check":
        errors = validate_pipeline(pipeline)
        if errors:
            for e in errors:
                print(f"[FAIL] {e}", file=sys.stderr)
            sys.exit(1)
        print(f"[OK] Pipeline valid: {len(pipeline.steps)} steps")
    else:
        print(f"Unknown command: {command}", file=sys.stderr)
        print("Usage: pipeline.py [generate|check]", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
