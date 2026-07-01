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
DOCUMENTATION_PROJECT="${PROJECT_ROOT}/Quasardb.Documentation/Quasardb.Documentation.shfbproj"
DOCUMENTATION_OUTPUT_DIR="${PROJECT_ROOT}/Quasardb.Documentation/Help"

if [[ -f "${DOCUMENTATION_PROJECT}" ]]; then
    DOCUMENTATION_PROJECT_WIN=$(normalize_paths "${DOCUMENTATION_PROJECT}")
    "${MSBUILD_PATH}" "${DOCUMENTATION_PROJECT_WIN}" \
        /p:Configuration="${BUILD_CONFIGURATION}"

    if [[ -d "${DOCUMENTATION_OUTPUT_DIR}" ]]; then
        
        mkdir -p documentation-pack-out
        rm -f "documentation-pack-out/qdb-api-dotnet-help.zip"

        pushd "${DOCUMENTATION_OUTPUT_DIR}"
        case "$(uname)" in
            MINGW*|MSYS*|CYGWIN*)
                export MSYS2_ARG_CONV_EXCL=""
                export MSYS_NO_PATHCONV="0"
                7z a -tzip "documentation-pack-out/qdb-api-dotnet-help.zip" ./*
                export MSYS2_ARG_CONV_EXCL="*"
                export MSYS_NO_PATHCONV="1"
                ;;
            *)
                zip -r "documentation-pack-out/qdb-api-dotnet-help.zip" .
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
