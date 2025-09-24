#!/usr/bin/env bash

# Exit immediately if a command exits with a non-zero status
set -e

# Define a variable that takes the value of the first argument sent to the script or defaults to "up"
command=${1:-up}

echo "Starting the migration process"

migrate-mongo $command --file 'migrate-mongo-repeatable-config.js'
migrate-mongo $command --file 'migrate-mongo-config.js'

echo "Migration process finished"
