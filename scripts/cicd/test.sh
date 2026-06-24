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

cp -r Quasardb/linux/ Quasardb.Tests/bin/${BUILD_CONFIGURATION}/${DOTNET_FRAMEWORK}
# cp -r Quasardb/windows/* Quasardb.Tests/bin/${BUILD_CONFIGURATION}/${DOTNET_FRAMEWORK}

set +e
"${DOTNET}" vstest \
  "Quasardb.Tests/bin/${BUILD_CONFIGURATION}/${DOTNET_FRAMEWORK}/Quasardb.Tests.dll" \
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
  "Quasardb.Tests/bin/${BUILD_CONFIGURATION}/${DOTNET_FRAMEWORK}/Quasardb.Tests.dll" \
  /Settings:"Quasardb.Tests/secure.runsettings" \
  /Platform:x64 \
  /Framework:".NETCoreApp,Version=v${DOTNET_FRAMEWORK#net}" \
  /Blame \
  /ResultsDirectory:"${INSECURE_RESULTS_DIR}" \
  /Logger:"junit;LogFilePath=${JUNIT_RESULTS_DIR}/insecure.xml;MethodFormat=Class;FailureBodyFormat=Verbose"
secure_status=$?
set -e

if [[ ${secure_status} -ne 0 && ${status} -eq 0 ]]; then
    status=${secure_status}
fi

popd

exit "${status}"
