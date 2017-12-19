#!/bin/sh
set -eu -o pipefail
IFS=$'\n\t'

if [[ $# -ne 1 ]] ; then
    >&2 echo "Usage: $0 <new_version>"
    exit 1
fi

INPUT_VERSION=$1; shift

XYZB_VERSION=${INPUT_VERSION%%-*}

cd $(dirname -- $0)
cd ${PWD}/../..

# [assembly: AssemblyVersion("2.1.0.0")]
# [assembly: AssemblyFileVersion("2.1.0.0")]
sed -i -e 's/\[assembly: Assembly\(File\)\?Version("[^"]*")\]/[assembly: Assembly\1Version("'"${XYZB_VERSION}"'")]/' Quasardb/Properties/AssemblyInfo.cs