# qdb-api-dotnet Buildkite Migration Plan

> For Hermes: use subagent-driven-development if executing this plan task-by-task.

Goal: replace the TeamCity pipeline from `origin/teamcity` with a repository-owned Buildkite pipeline for qdb-api-dotnet, following the pattern currently used in `/home/igorn/kafka-connect-qdb` branch `sc-18926/buildkite-add-kafka-connect-pipeline`.

Architecture: keep Buildkite YAML small and move build/test/package behavior into checked-in `scripts/cicd/` scripts. Use `.buildkite/tools` (`bureau14/qdb-cicd-tools`) as a submodule for dynamic pipeline generation, platform metadata, artifact plugin option injection, and validation. Buildkite downloads the same QuasarDB dependency artifacts TeamCity used, stages them into the same repo paths, runs qdb-test-setup services, executes net6.0/net8.0 tests in secure and insecure modes, packages on Windows, and always stops services through a Buildkite hook.

Tech stack: Buildkite dynamic pipeline (`buildkite-sdk`), qdb-cicd-tools, qdb-artifacts Buildkite plugin, qdb-test-report Buildkite plugin, Bash, dotnet SDK/VSTest, NuGet pack, existing `scripts/tests/setup` submodule.

Assumptions & feedback to confirm before implementation:
- The initial Buildkite matrix should match active TeamCity builds only: linux x86_64 core2 and windows x86_64 core2, each on dotnet 6.0 and 8.0.
- TeamCity has an AArch64 Linux project folder but no active build type under it; do not add Linux AArch64 until explicitly requested.
- TeamCity artifact project IDs map to qdb-artifacts `project_id: "quasardb-build"`; exact Buildkite variants are expected to be `linux-core2-release` and `windows-core2-release`, but verify by generating/downloading once.
- Keep current TeamCity dependency semantics first, even though .NET jobs need both Linux and Windows C API outputs for packaging/runtime layout.
- Documentation build (`Quasardb.Documentation/Quasardb.Documentation.shfbproj`) is Windows-only, matching TeamCity.
- NuGet packaging is Windows-only, matching TeamCity.
- There is no `StreamDemo` directory in the current working tree; preserve TeamCity’s artifact intent only if investigation confirms it exists on the target branch or was intentionally removed.

---

## Source behavior already identified

TeamCity source branch: `origin/teamcity`.

Relevant TeamCity files:
- `.teamcity/Api_DotNET/buildTypes/Api_DotNET_Build.xml`
- `.teamcity/Api_DotNET/buildTypes/Api_DotNET_LinuxBuild.xml`
- `.teamcity/Api_DotNET_Windows/buildTypes/Api_DotNET_Windows_Build.xml`
- `.teamcity/Api_DotNET_Linux_X8664Core2/buildTypes/Api_DotNET_Linux_X8664Core2_DotNET6.xml`
- `.teamcity/Api_DotNET_Linux_X8664Core2/buildTypes/Api_DotNET_Linux_X8664Core2_DotNET8.xml`
- `.teamcity/Api_DotNET_Windows_X8664Core2/buildTypes/Api_DotNET_Windows_DotNET6.xml`
- `.teamcity/Api_DotNET_Windows_X8664Core2/buildTypes/Api_DotNET_Windows_X8664Core2_DotNET8.xml`

TeamCity matrix:
- Linux x86_64 core2: dotnet 6.0, dotnet 8.0.
- Windows x86_64 core2: dotnet 6.0, dotnet 8.0.

TeamCity step behavior:
1. NuGet restore on Windows.
2. Start services: `bash scripts/tests/setup/start-services.sh`.
3. Build library: `dotnet build --configuration Release Quasardb/Quasardb.csproj`.
4. Build tests: `dotnet build --configuration Release Quasardb.Tests/Quasardb.Tests.csproj`.
5. Test insecure: `dotnet test Quasardb.Tests/Quasardb.Tests.csproj --framework net<version> --settings Quasardb.Tests/insecure.runsettings --logger:junit ... --blame`.
6. Test secure: `dotnet test Quasardb.Tests/Quasardb.Tests.csproj --framework net<version> --settings Quasardb.Tests/secure.runsettings --logger:junit ... --blame`.
7. Windows only: build Sandcastle docs with MSBuild 17 on `Quasardb.Documentation/Quasardb.Documentation.shfbproj`.
8. Windows only: create NuGet package from `Quasardb/Quasardb.csproj` / nuspec semantics, output under `nuget-pack-out`.
9. Always stop services: `bash scripts/tests/setup/stop-services.sh`.

TeamCity artifact dependencies:
- Linux jobs download Linux C API/server/utils to `qdb`/`qdb/bin`, Linux C API libs to `Quasardb/linux`, and Windows C API bin files to `Quasardb/win64`.
- Windows jobs download Linux C API libs to `Quasardb/linux`, and Windows C API/server/utils to `qdb`/`qdb/bin` plus `Quasardb/win64`.

Existing repo facts:
- `Quasardb/Quasardb.csproj` targets `netstandard2.0` and copies `Quasardb/linux/libqdb_api.so` and `Quasardb/win64/qdb_api.dll` to output.
- `Quasardb.Tests/Quasardb.Tests.csproj` currently targets `net8.0;net6.0;net5.0;netcoreapp3.1`; Buildkite should call explicit framework paths for net6.0/net8.0 to match TeamCity, not rely on all target frameworks.
- `Quasardb.Tests/insecure.runsettings` sets `useSecurity=false`; `secure.runsettings` sets `useSecurity=true`.
- Existing `scripts/tests/setup` submodule expects qdb binaries under `./qdb/bin`.

## Target file layout

Create/modify:
- `.gitmodules` — add `.buildkite/tools` submodule pointing to `https://github.com/bureau14/qdb-cicd-tools.git`.
- `.buildkite/pipeline.yml` — bootstrap venv and upload generated dynamic pipeline.
- `.buildkite/requirements.txt` — `buildkite-sdk==0.8.0` and `-r tools/requirements.txt`.
- `.buildkite/pipeline.py` — generate Buildkite matrix from platform × dotnet version.
- `.buildkite/steps/_build.yml` — one build/test/package step template.
- `.buildkite/steps/_test_report.yml` — aggregate test report template.
- `.buildkite/hooks/pre-exit` — always stop qdb services.
- `scripts/cicd/common.sh` — shared env/path/tool resolution.
- `scripts/cicd/stage-artifacts.sh` — normalize qdb-artifacts downloads into TeamCity-equivalent paths.
- `scripts/cicd/build.sh` — build library and tests.
- `scripts/cicd/test.sh` — run secure/insecure VSTest and emit test results.
- `scripts/cicd/package.sh` — Windows-only docs and NuGet package behavior.

Do not modify product C# code unless local Buildkite verification exposes a real CI-only issue.

## Implementation tasks

### Task 1: Capture TeamCity behavior in a migration note

Objective: add a short checked-in note or plan section that records the exact TeamCity behavior being preserved.

Files:
- Create or keep: `.hermes/plans/buildkite-migration-from-teamcity.md`

Steps:
1. Re-run source inspection:
   - `git ls-tree -r --name-only origin/teamcity .teamcity`
   - `git show origin/teamcity:.teamcity/Api_DotNET/buildTypes/Api_DotNET_Build.xml`
   - `git show origin/teamcity:.teamcity/Api_DotNET_Windows_X8664Core2/buildTypes/Api_DotNET_Windows_X8664Core2_DotNET8.xml`
2. Verify no active TeamCity AArch64 build type exists:
   - `git ls-tree -r --name-only origin/teamcity .teamcity | grep -i AArch64`
   - Expected: project config only, no active build type XML.
3. Keep the matrix and artifact-dependency mapping in this plan updated if the inspection changes.

Verification:
- `git diff -- .hermes/plans/buildkite-migration-from-teamcity.md`

### Task 2: Add the qdb-cicd-tools submodule

Objective: mirror kafka-connect-qdb and sibling API repositories by using shared Buildkite helpers instead of copying them into this repo.

Files:
- Modify: `.gitmodules`
- Create: `.buildkite/tools` submodule pointer

Steps:
1. Add the submodule if it is not already staged:
   - `git submodule add https://github.com/bureau14/qdb-cicd-tools.git .buildkite/tools`
2. Initialize/update it:
   - `git submodule update --init --recursive .buildkite/tools`
3. Confirm `.gitmodules` contains both `scripts/tests/setup` and `.buildkite/tools`.

Verification:
- `git submodule status --recursive`
- `git status --short --untracked-files=all`

### Task 3: Add Buildkite bootstrap files

Objective: create the static Buildkite entry point that prepares a temporary Python venv and uploads the dynamic pipeline.

Files:
- Create: `.buildkite/pipeline.yml`
- Create: `.buildkite/requirements.txt`

Implementation shape:
- Match kafka-connect-qdb:
  - queue: `default-debian-amd64`
  - env: `BUILDKITE_PIPELINE_VENV=/tmp/qdb-api-dotnet-buildkite-venv`
  - commands:
    - `python3 -m venv "$$BUILDKITE_PIPELINE_VENV"`
    - `"$$BUILDKITE_PIPELINE_VENV/bin/python" -m pip install -r .buildkite/requirements.txt`
    - `"$$BUILDKITE_PIPELINE_VENV/bin/python" .buildkite/pipeline.py | buildkite-agent pipeline upload`
- Requirements:
  - `buildkite-sdk==0.8.0`
  - `-r tools/requirements.txt`

Verification:
- `python3 -m venv /tmp/qdb-api-dotnet-buildkite-plan-venv`
- `/tmp/qdb-api-dotnet-buildkite-plan-venv/bin/python -m pip install -r .buildkite/requirements.txt`

### Task 4: Add shared cicd script foundation

Objective: put all repeated path/tool/env logic in one script, following existing repo style.

Files:
- Create: `scripts/cicd/common.sh`

Implementation requirements:
- `set -eux -o pipefail`.
- Resolve:
  - `THIS_SCRIPT_DIR`
  - `PROJECT_ROOT`
  - `BUILD_CONFIGURATION=${BUILD_CONFIGURATION:-Release}`
  - `DOTNET_VERSION=${DOTNET_VERSION:?DOTNET_VERSION is required}`
  - `DOTNET_FRAMEWORK=net${DOTNET_VERSION}`
  - `DOTNET=${DOTNET_PATH:-dotnet}` or `DOTNET=${DOTNET:-dotnet}`.
- Export all values used by child scripts.
- Print `dotnet --info` once for diagnostic parity.
- Keep script names lowercase and direct (`build.sh`, `test.sh`, `package.sh`) like kafka-connect-qdb.

Verification:
- `bash -n scripts/cicd/common.sh`
- `DOTNET_VERSION=8.0 bash -c 'source scripts/cicd/common.sh; test "$DOTNET_FRAMEWORK" = net8.0'`

### Task 5: Add artifact staging script

Objective: translate qdb-artifacts downloads into the paths TeamCity produced before build/test.

Files:
- Create: `scripts/cicd/stage-artifacts.sh`

Implementation requirements:
- Source `scripts/cicd/common.sh`.
- Validate required inputs exist after qdb-artifacts download.
- Ensure these destination directories exist:
  - `qdb/bin`
  - `Quasardb/linux`
  - `Quasardb/win64`
- Stage Linux C API library files into `Quasardb/linux`.
- Stage Windows C API DLL files into `Quasardb/win64`.
- Stage qdb server/utils binaries into `qdb/bin` for the current OS so `scripts/tests/setup/start-services.sh` works.
- Avoid hard-coding exact qdb version filenames; use artifact directory contents/patterns.

Suggested download layout from `.buildkite/steps/_build.yml`:
- Download Linux release artifacts under `artifacts/linux`.
- Download Windows release artifacts under `artifacts/windows`.
- Let this script copy from those normalized directories into TeamCity-compatible paths.

Verification:
- `bash -n scripts/cicd/stage-artifacts.sh`
- Add a temporary fake artifact tree under `/tmp`, run the script with override env if implemented, and assert files land at `qdb/bin`, `Quasardb/linux`, `Quasardb/win64`.

### Task 6: Add build script

Objective: preserve TeamCity build steps with explicit Release configuration and selected target framework.

Files:
- Create: `scripts/cicd/build.sh`

Implementation requirements:
- Source `scripts/cicd/common.sh`.
- On Windows only, run NuGet restore behavior if required by current agents/project:
  - Prefer `dotnet restore Quasardb.sln` first.
  - Only use `nuget restore Quasardb.sln` if dotnet restore does not reproduce TeamCity.
- Run:
  - `dotnet build --configuration "$BUILD_CONFIGURATION" Quasardb/Quasardb.csproj`
  - `dotnet build --configuration "$BUILD_CONFIGURATION" --framework "$DOTNET_FRAMEWORK" Quasardb.Tests/Quasardb.Tests.csproj`

Verification:
- `bash -n scripts/cicd/build.sh`
- If the local machine has required dotnet SDKs and qdb native deps staged:
  - `DOTNET_VERSION=8.0 bash scripts/cicd/build.sh`

### Task 7: Add test script with JUnit output

Objective: preserve TeamCity’s two secure/insecure test phases and make results consumable by `qdb-test-report`.

Files:
- Create: `scripts/cicd/test.sh`
- Modify: `Quasardb.Tests/Quasardb.Tests.csproj`

Implementation requirements:
- Add `JunitXml.TestLogger` to the test project.
- Source `scripts/cicd/common.sh`.
- Run insecure then secure with `dotnet test`, explicit `--framework "${DOTNET_FRAMEWORK}"`, `--no-build`, the matching runsettings file, and `--logger:"junit;LogFilePath=${JUNIT_RESULTS_DIR}/<mode>.xml;MethodFormat=Class;FailureBodyFormat=Verbose"`.
- Capture each `dotnet test` exit code, run both phases when possible, and exit non-zero if either phase failed.
- Name JUnit output paths deterministically under `test-results/junit/`.

Verification:
- `bash -n scripts/cicd/test.sh`
- Local smoke if dotnet/qdb deps are available:
  - `DOTNET_VERSION=8.0 bash scripts/cicd/test.sh`
- Confirm JUnit result files exist under `test-results/junit/`.

### Task 8: Add package/docs script

Objective: preserve TeamCity’s Windows-only package/doc behavior without running it on Linux.

Files:
- Create: `scripts/cicd/package.sh`

Implementation requirements:
- Source `scripts/cicd/common.sh`.
- If not Windows, print a clear skip message and exit 0.
- On Windows:
  - build docs with `dotnet msbuild Quasardb.Documentation/Quasardb.Documentation.shfbproj -c Release` or the exact MSBuild 17 command available on agents.
  - package with NuGet/dotnet pack into `nuget-pack-out` preserving `Quasardb/Quasardb.nuspec` files for `linux/libqdb_api.so` and `win64/qdb_api.dll`.
  - support TeamCity’s nightly suffix equivalent via env, e.g. `QDB_NUGET_PACKAGE_SUFFIX`, only if current NuGet packaging accepts it.
- Do not invent `StreamDemo` upload behavior unless the directory exists or the user confirms it still matters.

Verification:
- `bash -n scripts/cicd/package.sh`
- On a Windows Buildkite agent or local Git Bash:
  - `DOTNET_VERSION=8.0 bash scripts/cicd/package.sh`
- Confirm `.nupkg` lands under `nuget-pack-out`.

### Task 9: Add Buildkite pre-exit cleanup hook

Objective: preserve TeamCity `execute_always` stop-services and swabra cleanup behavior enough to avoid leftover qdbd processes.

Files:
- Create: `.buildkite/hooks/pre-exit`

Implementation requirements:
- Match kafka-connect-qdb/qdb-api-python style:
  - `set +e`
  - if `scripts/tests/setup/stop-services.sh` exists, run it.
  - exit 0.
- Make it executable.

Verification:
- `bash -n .buildkite/hooks/pre-exit`
- `chmod +x .buildkite/hooks/pre-exit`
- `git diff --stat .buildkite/hooks/pre-exit`

### Task 10: Add Buildkite build step template

Objective: define one reusable step with artifact downloads, repo scripts, uploads, and per-job test-report plugin config.

Files:
- Create: `.buildkite/steps/_build.yml`

Implementation requirements:
- Agents queue placeholder: `default-{queue}`.
- Label/key placeholders:
  - label: `{name} ({slug})`
  - key: `build-{slug}`
- Timeout: start with TeamCity’s 20 minutes.
- Commands:
  1. `bash scripts/cicd/stage-artifacts.sh`
  2. `bash scripts/tests/setup/start-services.sh`
  3. `bash scripts/cicd/build.sh`
  4. `bash scripts/cicd/test.sh`
  5. `bash scripts/cicd/package.sh`
- Plugins:
  - `bureau14/qdb-artifacts#master` download entries for Linux and Windows `quasardb-build` artifacts into normalized directories.
  - Upload deliverables only, likely `nuget-pack-out/*.nupkg` and generated docs archive if present.
  - `bureau14/qdb-test-report#master` job report with `variant: {slug}` and `junit_input_path: "test-results/junit/*.xml"`.

Artifact download details to encode and verify:
- Linux dependency variant: `linux-core2-release`.
- Windows dependency variant: `windows-core2-release`.
- Required files:
  - `*-c-api.tar.zst!*`
  - `*-server.tar.zst!*`
  - `*-utils*.tar.zst!*` or exact plugin-compatible pattern that covers TeamCity `*-utils.*` / `*-utils.tar.zst`.
- The staging script is responsible for filtering/copying to `qdb/bin`, `Quasardb/linux`, and `Quasardb/win64`.

Verification:
- YAML parse after file exists:
  - `python3 - <<'PY'
import yaml
for p in ['.buildkite/pipeline.yml', '.buildkite/steps/_build.yml']:
    yaml.safe_load(open(p))
print('ok')
PY`

### Task 11: Add aggregate test-report template

Objective: aggregate all per-job reports at the end, mirroring kafka-connect-qdb.

Files:
- Create: `.buildkite/steps/_test_report.yml`

Implementation requirements:
- queue: `default-debian-amd64`.
- label/key: `:bar_chart: Aggregate test report` / `full-test-report`.
- command: `true`.
- `allow_dependency_failure: true`.
- `depends_on` filled by `pipeline.py` with all generated build step keys.
- plugin:
  - `bureau14/qdb-test-report#master`
  - title: `Full test report`
  - aggregate: `{}`.

Verification:
- YAML parse together with Task 10.

### Task 12: Add dynamic pipeline generator

Objective: generate the exact platform × dotnet matrix and inject artifact/report variants using qdb-cicd-tools.

Files:
- Create: `.buildkite/pipeline.py`

Implementation requirements:
- Follow `/home/igorn/kafka-connect-qdb/.buildkite/pipeline.py` structure.
- Import from `.buildkite/tools/qdb_pipeline`:
  - `Platform`
  - `apply_docker` if Linux runs inside container, otherwise omit Docker.
  - `get_git_ref`
  - `load_template`
  - `merge_env`
  - `select_platforms`
  - `set_artifact_plugin_options`
  - `validate_pipeline`
- `PLATFORMS` initially:
  - `linux-amd64-core2`
  - `windows-amd64-core2`
- `DOTNET_VERSIONS = ["6.0", "8.0"]`.
- `BUILD_TYPES = ["Release"]`.
- Slug shape should be stable and readable, for example:
  - `linux-core2-release-dotnet6`
  - `linux-core2-release-dotnet8`
  - `windows-core2-release-dotnet6`
  - `windows-core2-release-dotnet8`
- Env per step:
  - `BUILD_CONFIGURATION=Release`
  - `DOTNET_VERSION=<6.0|8.0>`
  - `DOTNET_FRAMEWORK=net<version>` can be derived by scripts, not necessarily injected.
  - `NUGET_ENABLE_LEGACY_CSPROJ_PACK=true` for Windows packaging if still needed.
- Artifact vars per step:
  - upload/promote variant: the generated slug.
  - download git-ref: `get_git_ref()`.
  - by-project or per-entry options for `quasardb-build` Linux/Windows variants; verify plugin behavior with generated YAML.
- Add aggregate report step depending on all `build-{slug}` keys.
- Support `pipeline.py check` and `pipeline.py generate` like kafka-connect-qdb.

Verification:
- `python3 -m py_compile .buildkite/pipeline.py`
- `BUILDKITE_BRANCH=$(git branch --show-current) python3 .buildkite/pipeline.py check`
- `BUILDKITE_BRANCH=$(git branch --show-current) python3 .buildkite/pipeline.py > /tmp/qdb-api-dotnet-pipeline.yml`
- Inspect generated YAML for exactly 4 build steps plus aggregate report.

### Task 13: Local syntax and generation validation

Objective: verify the repository-owned CI definition without requiring a full Buildkite run.

Files:
- All new Buildkite and script files.

Commands:
- `bash -n scripts/cicd/common.sh scripts/cicd/stage-artifacts.sh scripts/cicd/build.sh scripts/cicd/test.sh scripts/cicd/package.sh .buildkite/hooks/pre-exit`
- `python3 -m venv /tmp/qdb-api-dotnet-buildkite-venv`
- `/tmp/qdb-api-dotnet-buildkite-venv/bin/python -m pip install -r .buildkite/requirements.txt`
- `BUILDKITE_BRANCH=$(git branch --show-current) /tmp/qdb-api-dotnet-buildkite-venv/bin/python .buildkite/pipeline.py check`
- `BUILDKITE_BRANCH=$(git branch --show-current) /tmp/qdb-api-dotnet-buildkite-venv/bin/python .buildkite/pipeline.py > /tmp/qdb-api-dotnet-generated.yml`

Expected:
- Bash syntax passes.
- Python compile/check passes.
- Generated pipeline has 4 build steps and 1 aggregate report step.
- Build steps have queues for linux amd64 and windows amd64.
- Each build step has `DOTNET_VERSION` set to 6.0 or 8.0.

### Task 14: Optional local dotnet smoke

Objective: catch obvious .NET command issues before Buildkite, without requiring qdb-artifacts credentials.

Files:
- `scripts/cicd/build.sh`
- `scripts/cicd/test.sh`

Steps:
1. If native qdb artifacts are already present under `qdb/bin`, `Quasardb/linux`, and `Quasardb/win64`, run:
   - `DOTNET_VERSION=8.0 bash scripts/cicd/build.sh`
2. If services can start locally, run:
   - `bash scripts/tests/setup/start-services.sh`
   - `DOTNET_VERSION=8.0 bash scripts/cicd/test.sh`
   - `bash scripts/tests/setup/stop-services.sh`

Expected:
- Build succeeds for net8.0.
- Tests either pass or fail with a real runtime dependency issue that should be fixed before pushing.

### Task 15: Final diff review

Objective: ensure the migration is scoped and ready for Buildkite trial.

Commands:
- `git status --short --untracked-files=all`
- `git diff -- .gitmodules .buildkite scripts/cicd .hermes/plans/buildkite-migration-from-teamcity.md`
- `git submodule status --recursive`

Checklist:
- No product code changes unless justified by verification.
- No secrets or generated qdb artifacts committed.
- `.buildkite/tools` is a submodule pointer, not a vendored copy.
- `scripts/tests/setup` remains intact.
- All new scripts have executable bits where needed.
- Buildkite files follow kafka-connect-qdb structure.

## Open questions for first Buildkite run

- Confirm exact qdb-artifacts plugin behavior for downloading two variants of the same `project_id` in one step.
- Confirm `qdb-test-report` accepts the JUnit XML emitted by `JunitXml.TestLogger`.
- Confirm Windows agent has the expected MSBuild/Sandcastle tooling for `Quasardb.Documentation.shfbproj`.
- Confirm whether TeamCity `StreamDemo` artifacts are obsolete or should be reintroduced from another branch.
- Confirm package suffix behavior for nightly/release NuGet packages in Buildkite environment variables.
