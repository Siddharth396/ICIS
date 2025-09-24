#!/bin/bash
VERSION="$(/tools/dotnet-gitversion | sed --silent --regexp-extended 's/^\s*"SemVer": "([^"]+)".*$/\1/p')"
if [ -z $VERSION ]
then
    # echo "**********************************************************************"
    # echo "****** Using backup script for determining the semantic version ******"
    # echo "**********************************************************************"

    chmod +x ./github_scripts/gh_version_backup.sh
    VERSION=$(./github_scripts/gh_version_backup.sh)
fi

REVISION=$((${GITHUB_RUN_ID} % 65535))

echo "SEM_VERSION=${VERSION}-${REVISION}" >> build.env

echo "${VERSION}-${REVISION}"
