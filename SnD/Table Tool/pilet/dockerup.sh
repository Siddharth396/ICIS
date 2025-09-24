#!/usr/bin/env bash
#Add references to your bff wherever comments referencing yourbff are found.

all=false
mkdir /tmp/feed

# Parse args
for arg in "$@"
do
  case "${arg}" in
    -coreapp=*)
      coreapp="${arg#*=}"
      ;;
    -corebff=*)
      corebff="${arg#*=}"
      ;;
    -coredb=*)
      coredb="${arg#*=}"
      ;;
    -nodedb=*)
      nodedb="${arg#*=}"
      ;;
    -identityserver=*)
      identityserver="${arg#*=}"
      ;;
    -homepagebff=*)
      homepagebff="${arg#*=}"
      ;;
    -appshell=*)
      appshell="${arg#*=}"
      ;;
    -feedservice=*)
      feedservice="${arg#*=}"
      ;;
    # -yourbff=*)
    #   yourbff="${arg#*=}"
    #   ;;
    -all)
      all=1
      ;;
    *)
  esac
done

function main {
  get_tag "genesis-app-dev" "${coreapp}"
  export CORE_APP_TAG="${tag}"
  echo "Found core app tag: ${tag}"

  get_tag "genesis-identityserver-dev" "${identityserver}"
  export IDENTITYSERVER_TAG="${tag}"
  echo "Found identityserver tag: ${tag}"

  get_tag "genesis-bff-dev" "${corebff}"
  export CORE_BFF_TAG="${tag}"
  echo "Found core bff tag: ${tag}"

  get_tag "genesis-db" "${coredb}"
  export CORE_DB_TAG="${tag}"
  echo "Found core db tag: ${tag}"

  get_tag "sbcore/sbcore-node-db-api" "${nodedb}"
  export NODE_DB_TAG="${tag}"
  echo "Found node db tag: ${tag}"

  get_tag "sbcore/sbcore-bff" "${homepagebff}"
  export HOMEPAGE_BFF_TAG="${tag}"
  echo "Found homepage bff tag: ${tag}"

  get_tag "sbcore/sbcore-app" "${appshell}"
  export APP_SHELL_TAG="${tag}"
  echo "Found app shell tag: ${tag}"

  get_tag "sbcore/sbcore-mfe-feed-service" "${feedservice}"
  export FEED_SERVICE_TAG="${tag}"
  echo "Found feed service tag: ${tag}"

  # get_tag "your-bff" "${yourbff}"
  # export YOUR_BFF_TAG="${tag}"
  # echo "Found your bff tag: ${tag}"

  # Pull the latest docker images
  docker-compose pull

  if [[ $all == 1 ]] ;
  then
    docker-compose up -d
  else
    # Start all the containers apart from the app shell and feed service
    docker-compose up -d
  fi
}

function get_results_page {
  name="${1}"
  query="${2}"
  page="${3}"
  if [[ ! -z "$page" ]]
  then
    page="&continuationToken=${page}"
  fi

  # get raw JSON outrput from nexus search
  output=$(curl -s "http://nexus.cha.rbxd.ds/service/rest/v1/search?repository=docker-hosted&name=${name}&q=\"*${query}*\"${page}" 2>&1 )

  # only take the version param (this matches the docker tag )
  version_list=$(sed -n -E  's/.*"version" : "(.*)".*/\1/p' <<<  "${output}")

  if [[ -z "$version_list" ]]
  then
    # get raw JSON outrput from nexus search
    output=$(curl -s --insecure "https://artifacts.cha.rbxd.ds/service/rest/v1/search?repository=docker_images&name=${name}&q=\"*${query}*\"${page}" 2>&1 )

    # only take the version param (this matches the docker tag )
    version_list=$(sed -n -E  's/.*"version" : "(.*)".*/\1/p' <<<  "${output}")
  fi

  # append to the versions list with a trailing newline
  versions="${versions}${version_list}"$'\n'

  next_page=$(sed -n -E  's/.+"continuationToken" : "(.*)".*/\1/p' <<<  "${output}")
  if [[ ! -z "$next_page" ]]
  then
    get_results_page "${name}" "${query}" "${next_page}"
  fi
}

function get_tag {
  name="${1}"
  query="${2}"

  if [[ -z "${query}" || "${query}" == "latest" ]]
  then
    tag=":latest"
  else
    versions=
    get_results_page "${name}" "${query//[^a-zA-Z0-9\.\-]/.}*"

    # get the metadata (last part of the tag) and insert it in the front of the tag line to sort
    sorted_versions=$(echo "$versions" | awk -F "." '{print $NF, $0}' | sort)

    # remove the extra sorting part
    versions=$(sed -n -E 's/.* //p' <<< "${sorted_versions}")

    if [[ ! -z "${versions}" ]]
    then
      tag=":"$(echo "${versions}" | tail -n 1 )
    else
      tag=":latest"
    fi
  fi
}

main
