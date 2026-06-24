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

if [[ -f Quasardb.Documentation/Quasardb.Documentation.shfbproj ]]; then
    "${DOTNET}" msbuild Quasardb.Documentation/Quasardb.Documentation.shfbproj -c "${BUILD_CONFIGURATION}"
fi

mkdir -p nuget-pack-out

nuget pack Quasardb/Quasardb.nuspec \
    -BasePath Quasardb \
    -OutputDirectory nuget-pack-out \
    -Properties "Configuration=${BUILD_CONFIGURATION}"

popd
