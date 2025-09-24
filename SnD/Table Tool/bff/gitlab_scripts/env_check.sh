set -e

# Check all environment variables that to be set before running the pipeline
echo "Checking WHITESOURCE_APIKEY"
if [[ -z "$WHITESOURCE_APIKEY" || "$WHITESOURCE_APIKEY" = "DEFAULT_VALUE" ]]
then 
    echo "WHITESOURCE_APIKEY variable is not set"
    exit 1
fi

echo "Checking CHECKMARX_PASS"
if [[ -z "$CHECKMARX_PASS" || "$CHECKMARX_PASS" = "DEFAULT_VALUE" ]]
then 
    echo "CHECKMARX_PASS variable is not set"
    exit 1
fi

echo "Checking CHECKMARX_USER"
if [[ -z "$CHECKMARX_USER" || "$CHECKMARX_USER" = "DEFAULT_VALUE" ]]
then 
    echo "CHECKMARX_USER variable is not set"
    exit 1
fi

echo "Checking DOCKER_REGISTRY_CHA"
if [[ -z "$DOCKER_REGISTRY_CHA" || "$DOCKER_REGISTRY_CHA" = "DEFAULT_VALUE" ]]
then
  echo "DOCKER_REGISTRY_CHA is not set"
  exit 1
fi

echo "Checking DOCKER_USERNAME"
if [[ -z "$DOCKER_USERNAME" || "$DOCKER_USERNAME" = "DEFAULT_VALUE" ]]
then 
    echo "DOCKER_USERNAME variable is not set"
    exit 1
fi

echo "Checking DOCKER_PASSWORD"
if [[ -z "$DOCKER_PASSWORD" || "$DOCKER_PASSWORD" = "DEFAULT_VALUE" ]]
then 
    echo "DOCKER_PASSWORD variable is not set"
    exit 1
fi

echo "Checking PROJECT_DEPLOYERPATH_FLUX"
if [[ -z "$PROJECT_DEPLOYERPATH_FLUX" || "$PROJECT_DEPLOYERPATH_FLUX" = "DEFAULT_VALUE" ]]
then
  echo "\$PROJECT_DEPLOYERPATH_FLUX is not set"
  exit 1
fi


echo "All environment variables are set"
