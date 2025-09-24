#!/bin/bash

##########################################################################
# This is a Docker image build script.
# It versions builds with Git Version.
##########################################################################

# Makes the script return immediately when a command fails rather than continue the execution
set -e

# A name or a repository URI that will be used for the image.
# If given a plain name, the image will be created locally.
# If given a URI, the image will also be pushed to that repository.
IMAGE_NAME=$1

# Semantic version of the application
SEM_VER=$2

# Proxy settings for Teamcity agents 
PROXY=$3

docker build --file Dockerfile \
  --tag $IMAGE_NAME:$SEM_VER \
  --build-arg semVersion=$SEM_VER \
  --build-arg http_proxy=$PROXY \
  --build-arg https_proxy=$PROXY \
  . # Use current dir for Docker context