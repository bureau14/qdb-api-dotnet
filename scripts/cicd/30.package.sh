#!/usr/bin/env bash
set -e -u -x

SCRIPT_DIR="$(cd "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null && pwd)"
source ${SCRIPT_DIR}/common.sh

archive_documentation() {
    mkdir -p "${DOCUMENTATION_ARCHIVE_DIR}"
    rm -f "${DOCUMENTATION_ARCHIVE}"

    pushd "${DOCUMENTATION_OUTPUT_DIR}"
    case "$(uname)" in
        MINGW*|MSYS*|CYGWIN*)
            7z a -tzip "${DOCUMENTATION_ARCHIVE}" ./*
            ;;
        *)
            zip -r "${DOCUMENTATION_ARCHIVE}" .
            ;;
    esac
    popd
}

case "$(uname)" in
    MINGW*|MSYS*|CYGWIN*)
        ;;
    *)
        echo "Skipping documentation and packaging on $(uname)."
        exit 0
        ;;
esac

pushd "${PROJECT_ROOT}"

MSBUILD_PATH="/c/Program Files (x86)/Microsoft Visual Studio/2022/BuildTools/MSBuild/Current/Bin/MSBuild.exe"
DOCUMENTATION_PROJECT="${PROJECT_ROOT}/Quasardb.Documentation/Quasardb.Documentation.shfbproj"
DOCUMENTATION_OUTPUT_DIR="${PROJECT_ROOT}/Quasardb.Documentation/Help"
PACKAGE_OUTPUT_DIR="${PROJECT_ROOT}/nuget-pack-out"
DOCUMENTATION_ARCHIVE_DIR="${PACKAGE_OUTPUT_DIR}"
DOCUMENTATION_ARCHIVE="${DOCUMENTATION_ARCHIVE_DIR}/qdb-api-dotnet-help.zip"

mkdir -p "${PACKAGE_OUTPUT_DIR}"

if [[ -f "${DOCUMENTATION_PROJECT}" ]]; then
    DOCUMENTATION_PROJECT_WIN=$(normalize_paths "${DOCUMENTATION_PROJECT}")
    "${MSBUILD_PATH}" "${DOCUMENTATION_PROJECT_WIN}" \
        /p:Configuration="${BUILD_CONFIGURATION}"

    if [[ -d "${DOCUMENTATION_OUTPUT_DIR}" ]]; then
        archive_documentation
    fi
fi

"${NUGET}" pack Quasardb/Quasardb.nuspec \
    -BasePath Quasardb \
    -OutputDirectory "${PACKAGE_OUTPUT_DIR}" \
    -Properties "Configuration=${BUILD_CONFIGURATION}"

popd
