#!/bin/bash

CONTAINER_NAME=$1

IMAGE=$2

docker run -d --name $CONTAINER_NAME -t $IMAGE