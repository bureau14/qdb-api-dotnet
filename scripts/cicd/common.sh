#!/usr/bin/env bash
set -eux -o pipefail

THIS_SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null && pwd)
PROJECT_ROOT=$(cd -- "${THIS_SCRIPT_DIR}/../.." >/dev/null && pwd)

BUILD_CONFIGURATION=${BUILD_CONFIGURATION:-Release}
DOTNET_VERSION=${DOTNET_VERSION:?DOTNET_VERSION is required, e.g. 6.0 or 8.0}
DOTNET_FRAMEWORK=${DOTNET_FRAMEWORK:-net${DOTNET_VERSION}}

if [[ -n "${DOTNET_PATH:-}" ]]; then
    DOTNET="${DOTNET_PATH}"
elif [[ -n "${DOTNET:-}" ]]; then
    DOTNET="${DOTNET}"
else
    DOTNET=dotnet
fi

export PROJECT_ROOT
export BUILD_CONFIGURATION
export DOTNET_VERSION
export DOTNET_FRAMEWORK
export DOTNET

path_for_windows_native() {
    case "$(uname)" in
        MINGW*|MSYS*|CYGWIN*)
            cygpath -w "$1"
            ;;
        *)
            printf '%s\n' "$1"
            ;;
    esac
}

# run_without_msys_path_conversion() {
#     # Some dotnet tooling requires options to be specified with `/`
#     # on windows, when using mingw this is treated as unix path and automatically escaped
#     # this function executes passed call with this feature disabled
#     env \
#       MSYS2_ARG_CONV_EXCL="*" \
#       MSYS_NO_PATHCONV="1" \
#       "$@"
# }

echo "BUILD_CONFIGURATION: ${BUILD_CONFIGURATION}"
echo "DOTNET_VERSION: ${DOTNET_VERSION}"
echo "DOTNET_FRAMEWORK: ${DOTNET_FRAMEWORK}"
echo "Detected dotnet:"
"${DOTNET}" --info
