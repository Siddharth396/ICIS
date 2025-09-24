# The commands in this file are written in a sequence that helps make the best use of Docker caching.
# See Readme file for more information.

# ------------------------------------ #
# STAGE - Base                         #
# Contains all the dev dependencies for the project and is able to run any of the
# scripts from package.json
# ------------------------------------ #

FROM artifacts.cha.rbxd.ds/ubuntu:latest AS base

WORKDIR /app


RUN apt-get update && apt-get install bash python make g++ jq curl  -y


RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -

RUN apt-get update && apt-get install nodejs -y

RUN npm install -g yarn

RUN yarn global add mongodb-realm-cli@2.3.2

# Copy just the files needed to install the dependencies as they doesn't change often
COPY [ "package.json", "./" ]

RUN yarn install

COPY src src
COPY /deployer.sh /app/deployer.sh
COPY /realm-auth.crt /app/realm-auth.crt
COPY /secrets.json /app/secrets.json
COPY /version.json /app/version.json

# ------------------------------------ #
# Run artifacts for realm.
# ------------------------------------ #

FROM base as deployer

WORKDIR /app

RUN chmod 777 /app/deployer.sh

CMD ["sh", "-c", "/app/deployer.sh"]

