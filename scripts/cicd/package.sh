#!/usr/bin/env bash

set -eux -o pipefail

THIS_SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null && pwd)
source "${THIS_SCRIPT_DIR}/common.sh"

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

if ! command -v nuget >/dev/null 2>&1; then
    echo "nuget executable is required to pack Quasardb/Quasardb.nuspec while preserving native file layout" >&2
    exit 1
fi

nuget pack Quasardb/Quasardb.nuspec \
    -BasePath Quasardb \
    -OutputDirectory nuget-pack-out \
    -Properties "Configuration=${BUILD_CONFIGURATION}"

popd
