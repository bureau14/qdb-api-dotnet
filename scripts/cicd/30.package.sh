#!/usr/bin/env bash
set -e -u -x

SCRIPT_DIR="$(cd "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null && pwd)"
source ${SCRIPT_DIR}/common.sh


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
# MSBUILD_PATH="/c/Program Files (x86)/Microsoft Visual Studio/18/BuildTools/MSBuild//Current/Bin/MSBuild.exe"
DOCUMENTATION_PROJECT="${PROJECT_ROOT}/Quasardb.Documentation/Quasardb.Documentation.shfbproj"
DOCUMENTATION_OUTPUT_DIR="${PROJECT_ROOT}/Quasardb.Documentation/Help"
DOCUMENTATION_ARCHIVE="${PROJECT_ROOT}/documentation-pack-out/qdb-api-dotnet-help.zip"
# z7z="/c/Program Files/7-Zip/7z.exe"

if [[ -f "${DOCUMENTATION_PROJECT}" ]]; then
    DOCUMENTATION_PROJECT_WIN=$(normalize_paths "${DOCUMENTATION_PROJECT}")
    "${MSBUILD_PATH}" "${DOCUMENTATION_PROJECT_WIN}" \
        /p:Configuration="${BUILD_CONFIGURATION}"

    if [[ -d "${DOCUMENTATION_OUTPUT_DIR}" ]]; then
        
        mkdir -p "$(dirname "${DOCUMENTATION_ARCHIVE}")"
        rm -f "${DOCUMENTATION_ARCHIVE}"

        pushd "${DOCUMENTATION_OUTPUT_DIR}"
        case "$(uname)" in
            MINGW*|MSYS*|CYGWIN*)
                DOCUMENTATION_ARCHIVE_WIN=$(normalize_paths "${DOCUMENTATION_ARCHIVE}")
                "${z7z}" a -tzip "${DOCUMENTATION_ARCHIVE_WIN}" ./*
                ;;
            *)
                zip -r "${DOCUMENTATION_ARCHIVE}" .
                ;;
        esac
        popd
    fi
fi

mkdir -p nuget-pack-out

"${NUGET}" pack Quasardb/Quasardb.nuspec \
    -BasePath Quasardb \
    -OutputDirectory nuget-pack-out \
    -Properties "Configuration=${BUILD_CONFIGURATION}"

popd

ls -l documentation-pack-out
ls -l nuget-pack-out
