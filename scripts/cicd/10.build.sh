#!/usr/bin/env bash

SCRIPT_DIR="$(cd "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null && pwd)"

source ${SCRIPT_DIR}/common.sh

git config --global --add safe.directory '*'

set -e -u -x

pushd "${PROJECT_ROOT}"

case "$(uname)" in
    MINGW*|MSYS*|CYGWIN*)
        "${DOTNET}" restore Quasardb.sln
        ;;
esac

"${DOTNET}" build --configuration "${BUILD_CONFIGURATION}" Quasardb/Quasardb.csproj
"${DOTNET}" build --configuration "${BUILD_CONFIGURATION}" --framework "${DOTNET_FRAMEWORK}" Quasardb.Tests/Quasardb.Tests.csproj

popd
