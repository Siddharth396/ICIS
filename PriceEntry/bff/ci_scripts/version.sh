#!/bin/bash
VERSION="$(/tools/dotnet-gitversion | sed --silent --regexp-extended 's/^\s*"SemVer": "([^"]+)".*$/\1/p')"
if [[ -z $VERSION ]]
then
    chmod +x ./ci_scripts/version_backup.sh
    VERSION=$(./ci_scripts/version_backup.sh)
fi

REVISION=$((${GITHUB_RUN_ID} % 65535))

echo "SEM_VERSION=${VERSION}-${REVISION}" >> build.env

echo "${VERSION}-${REVISION}"