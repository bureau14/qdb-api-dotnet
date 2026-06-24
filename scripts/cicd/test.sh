#!/usr/bin/env bash

set -eux -o pipefail

THIS_SCRIPT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" >/dev/null && pwd)
source "${THIS_SCRIPT_DIR}/common.sh"

INSECURE_RESULTS_DIR="${PROJECT_ROOT}/test-results/insecure"
SECURE_RESULTS_DIR="${PROJECT_ROOT}/test-results/secure"
JUNIT_RESULTS_DIR="${PROJECT_ROOT}/test-results/junit"

mkdir -p "${INSECURE_RESULTS_DIR}" "${SECURE_RESULTS_DIR}" "${JUNIT_RESULTS_DIR}"

pushd "${PROJECT_ROOT}"

status=0

TEST_OUTPUT_DIR="Quasardb.Tests/bin/${BUILD_CONFIGURATION}/${DOTNET_FRAMEWORK}"

stage_native_libraries() {
    case "$(uname)" in
        MINGW*|MSYS*|CYGWIN*)
            mkdir -p "${TEST_OUTPUT_DIR}/win64"
            cp Quasardb/win64/qdb_api.dll "${TEST_OUTPUT_DIR}/win64/qdb_api.dll"
            ;;
        *)
            mkdir -p "${TEST_OUTPUT_DIR}/linux"
            cp Quasardb/linux/libqdb_api.so "${TEST_OUTPUT_DIR}/linux/libqdb_api.so"

            # .NET's DllImport("qdb_api") probes the assembly directory, while the
            # legacy qdb_api static loader probes ./linux/libqdb_api.so. Keep both
            # layouts available so Linux vstest works outside a NuGet package.
            cp Quasardb/linux/libqdb_api.so "${TEST_OUTPUT_DIR}/libqdb_api.so"
            # export LD_LIBRARY_PATH="${PROJECT_ROOT}/${TEST_OUTPUT_DIR}:${PROJECT_ROOT}/${TEST_OUTPUT_DIR}/linux:${PROJECT_ROOT}/qdb/bin${LD_LIBRARY_PATH:+:${LD_LIBRARY_PATH}}"
            ;;
    esac
}

stage_native_libraries

set +e
"${DOTNET}" vstest \
  "${TEST_OUTPUT_DIR}/Quasardb.Tests.dll" \
  /Settings:"Quasardb.Tests/insecure.runsettings" \
  /Platform:x64 \
  /Framework:".NETCoreApp,Version=v${DOTNET_FRAMEWORK#net}" \
  /Blame \
  /ResultsDirectory:"${INSECURE_RESULTS_DIR}" \
  /Logger:"junit;LogFilePath=${JUNIT_RESULTS_DIR}/insecure.xml;MethodFormat=Class;FailureBodyFormat=Verbose"
insecure_status=$?
set -e

if [[ ${insecure_status} -ne 0 ]]; then
    status=${insecure_status}
fi

set +e
"${DOTNET}" vstest \
  "${TEST_OUTPUT_DIR}/Quasardb.Tests.dll" \
  /Settings:"Quasardb.Tests/secure.runsettings" \
  /Platform:x64 \
  /Framework:".NETCoreApp,Version=v${DOTNET_FRAMEWORK#net}" \
  /Blame \
  /ResultsDirectory:"${SECURE_RESULTS_DIR}" \
  /Logger:"junit;LogFilePath=${JUNIT_RESULTS_DIR}/secure.xml;MethodFormat=Class;FailureBodyFormat=Verbose"
secure_status=$?
set -e

if [[ ${secure_status} -ne 0 && ${status} -eq 0 ]]; then
    status=${secure_status}
fi

popd

exit "${status}"
