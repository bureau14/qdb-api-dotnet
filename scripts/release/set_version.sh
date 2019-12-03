#!/bin/sh
set -eu -o pipefail
IFS=$'\n\t'

if [[ $# -ne 1 ]] ; then
    >&2 echo "Usage: $0 <new_version>"
    exit 1
fi

INPUT_VERSION=$1; shift

MAJOR_VERSION=${INPUT_VERSION%%.*}
WITHOUT_MAJOR_VERSION=${INPUT_VERSION#${MAJOR_VERSION}.}
MINOR_VERSION=${WITHOUT_MAJOR_VERSION%%.*}
WITHOUT_MINOR_VERSION=${INPUT_VERSION#${MAJOR_VERSION}.${MINOR_VERSION}.}
PATCH_VERSION=${WITHOUT_MINOR_VERSION%%.*}
WITHOUT_PATCH_VERSION=${INPUT_VERSION#${MAJOR_VERSION}.${MINOR_VERSION}.${PATCH_VERSION}.}
REVISION_VERSION=${WITHOUT_PATCH_VERSION%%-*}

XYZ_VERSION="${MAJOR_VERSION}.${MINOR_VERSION}.${PATCH_VERSION}"
XYZR_VERSION="${MAJOR_VERSION}.${MINOR_VERSION}.${PATCH_VERSION}.${REVISION_VERSION}"

SUB_RELEASE_VERSION=${INPUT_VERSION#*-}
SUB_RELEASE_TYPE=${SUB_RELEASE_VERSION%%.*}
SUB_RELEASE_MINOR_VERSION=${SUB_RELEASE_VERSION#*.}
if [[ "${SUB_RELEASE_TYPE}" == "rc" ]] ; then
    SUB_RELEASE_TYPE="rc"
    FULL_XYZ_VERSION="${XYZ_VERSION}-${SUB_RELEASE_TYPE}${SUB_RELEASE_MINOR_VERSION}"
elif [[ "${SUB_RELEASE_TYPE}" == "nightly" ]] ; then
    SUB_RELEASE_TYPE="alpha"
    FULL_XYZ_VERSION="${XYZ_VERSION}-${SUB_RELEASE_TYPE}0"
else
    FULL_XYZ_VERSION="${XYZ_VERSION}"
fi

CURRENT_YEAR=`date +%Y`

cd $(dirname -- $0)
cd ${PWD}/../..

# [assembly: AssemblyCopyright("Copyright © quasardb SAS 2017")]
sed -i -e 's/\[assembly: AssemblyCopyright("\([^"]*\) [0-9]\+")\]/[assembly: AssemblyCopyright("\1 '"${CURRENT_YEAR}"'")]/' Quasardb/Properties/AssemblyInfo.cs

# [assembly: AssemblyVersion("2.1.0.0")]
# [assembly: AssemblyFileVersion("2.1.0.0")]
sed -i -e 's/\[assembly: Assembly\(File\)\?Version("[^"]*")\]/[assembly: Assembly\1Version("'"${XYZR_VERSION}"'")]/' Quasardb/Properties/AssemblyInfo.cs

# Copyright (c) 2009-2019, quasardb SAS
sed -i -e 's/Copyright (c) 2009-[0-9]\+, quasardb SAS/Copyright (c) 2009-'"${CURRENT_YEAR}"', quasardb SAS/' LICENSE.md

# <version>3.5.0</version>
sed -i -e 's|<version>[^<]\+</version>|<version>'"${FULL_XYZ_VERSION}"'</version>|' Quasardb/Quasardb.nuspec
