VERSION="$(/tools/dotnet-gitversion | sed --silent --regexp-extended 's/^\s*"SemVer": "([^"]+)".*$/\1/p')"

if [[ -z $VERSION ]]
then
    echo "*************************************************************"
    echo "****** Using backup script for determining the version ******"
    echo "*************************************************************"

    chmod +x ./gitlab_scripts/version_backup.sh
    VERSION=$(./gitlab_scripts/version_backup.sh)
fi

REVISION=$((${CI_PIPELINE_ID} % 65535)) 

echo "SEM_VERSION=${VERSION}.${REVISION}" >> build.env
cat build.env