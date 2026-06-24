#!/usr/bin/env bash

SCRIPT_DIR="$(cd "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null && pwd)"

source ${SCRIPT_DIR}/common.sh

git config --global --add safe.directory '*'

set -u -x

BASE_DIR="${SCRIPT_DIR}/../.."
INSECURE_RESULTS_DIR="${BASE_DIR}/test-results/insecure"
SECURE_RESULTS_DIR="${BASE_DIR}/test-results/secure"
JUNIT_RESULTS_DIR="${BASE_DIR}/test-results/junit"

mkdir -p "${INSECURE_RESULTS_DIR}" "${SECURE_RESULTS_DIR}" "${JUNIT_RESULTS_DIR}"

pushd "${BASE_DIR}"

status=0

TEST_OUTPUT_DIR="${BASE_DIR}/Quasardb.Tests/bin/${BUILD_CONFIGURATION}/${DOTNET_FRAMEWORK}"
INSECURE_SETTINGS="${BASE_DIR}/Quasardb.Tests/insecure.runsettings"
SECURE_SETTINGS="${BASE_DIR}/Quasardb.Tests/secure.runsettings"

stage_native_libraries() {
    case "$(uname)" in
        MINGW*|MSYS*|CYGWIN*)
            mkdir -p "${TEST_OUTPUT_DIR}/win64"
            cp "${BASE_DIR}/Quasardb/win64/qdb_api.dll" "${TEST_OUTPUT_DIR}/win64/qdb_api.dll"
            ;;
        *)
            mkdir -p "${TEST_OUTPUT_DIR}/linux"
            cp "${BASE_DIR}/Quasardb/linux/libqdb_api.so" "${TEST_OUTPUT_DIR}/linux/libqdb_api.so"

            # .NET's DllImport("qdb_api") probes the assembly directory, while the
            # legacy qdb_api static loader probes ./linux/libqdb_api.so. Keep both
            # layouts available so Linux vstest works outside a NuGet package.
            cp "${BASE_DIR}/Quasardb/linux/libqdb_api.so" "${TEST_OUTPUT_DIR}/libqdb_api.so"
            ;;
    esac
}

stage_native_libraries

DOTNET_TEST_DLL="${TEST_OUTPUT_DIR}/Quasardb.Tests.dll"
DOTNET_INSECURE_SETTINGS="${INSECURE_SETTINGS}"
DOTNET_SECURE_SETTINGS="${SECURE_SETTINGS}"
DOTNET_INSECURE_RESULTS_DIR="${INSECURE_RESULTS_DIR}"
DOTNET_SECURE_RESULTS_DIR="${SECURE_RESULTS_DIR}"
DOTNET_INSECURE_JUNIT="${JUNIT_RESULTS_DIR}/insecure.xml"
DOTNET_SECURE_JUNIT=$"${JUNIT_RESULTS_DIR}/secure.xml"

set +e
"${DOTNET}" vstest \
  "${DOTNET_TEST_DLL}" \
  /Settings:"${DOTNET_INSECURE_SETTINGS}" \
  /Platform:x64 \
  /Framework:".NETCoreApp,Version=v${DOTNET_FRAMEWORK#net}" \
  /Blame \
  /ResultsDirectory:"${DOTNET_INSECURE_RESULTS_DIR}" \
  /Logger:"junit;LogFilePath=${DOTNET_INSECURE_JUNIT};MethodFormat=Class;FailureBodyFormat=Verbose"
insecure_status=$?
set -e

if [[ ${insecure_status} -ne 0 ]]; then
    status=${insecure_status}
fi

set +e
"${DOTNET}" vstest \
  "${DOTNET_TEST_DLL}" \
  /Settings:"${DOTNET_SECURE_SETTINGS}" \
  /Platform:x64 \
  /Framework:".NETCoreApp,Version=v${DOTNET_FRAMEWORK#net}" \
  /Blame \
  /ResultsDirectory:"${DOTNET_SECURE_RESULTS_DIR}" \
  /Logger:"junit;LogFilePath=${DOTNET_SECURE_JUNIT};MethodFormat=Class;FailureBodyFormat=Verbose"
secure_status=$?
set -e

if [[ ${secure_status} -ne 0 && ${status} -eq 0 ]]; then
    status=${secure_status}
fi

popd

exit "${status}"
