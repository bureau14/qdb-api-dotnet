#!/usr/bin/env bash

SCRIPT_DIR="$(cd "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null && pwd)"

source ${SCRIPT_DIR}/common.sh

git config --global --add safe.directory '*'

# No more errors should occur after here
set -e -u -x

case "$(uname)" in
    MINGW*|MSYS*|CYGWIN*)
        ;;
    *)
        echo "Skipping Windows-only documentation and NuGet packaging on $(uname)."
        exit 0
        ;;
esac

pushd "${PROJECT_ROOT}"

MSBUILD_PATH="/c/Program Files (x86)/Microsoft Visual Studio/2022/BuildTools/MSBuild/Current/Bin/MSBuild.exe"
DOCUMENTATION_PROJECT="${PROJECT_ROOT}/Quasardb.Documentation/Quasardb.Documentation.shfbproj"
if [[ -f "${DOCUMENTATION_PROJECT}" ]]; then
    DOCUMENTATION_PROJECT_WIN=$(path_for_windows_native "${DOCUMENTATION_PROJECT}")
    run_without_msys_path_conversion "${MSBUILD_PATH}" "${DOCUMENTATION_PROJECT_WIN}" \
        /p:Configuration="${BUILD_CONFIGURATION}"
fi

mkdir -p nuget-pack-out

nuget pack Quasardb/Quasardb.nuspec \
    -BasePath Quasardb \
    -OutputDirectory nuget-pack-out \
    -Properties "Configuration=${BUILD_CONFIGURATION}"

popd
