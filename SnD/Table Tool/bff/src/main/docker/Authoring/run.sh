#!/usr/bin/env bash

# load env variables from external files and export them to the child process
set -o allexport

    source $SECRET_FILE_PATH

set +o allexport

dotnet Authoring.dll