FROM artifacts.cha.rbxd.ds/node:14.18.0-alpine AS builder

ARG SEM_VER="0.0.1"
RUN mkdir /certificates
RUN apk update && apk add curl bash
RUN npm config set strict-ssl false
RUN yarn config set "strict-ssl" false
# cache run


WORKDIR /app

COPY [ \
    "package.json", \
    "yarn.lock", \
    ".yarnrc", \
    "/" \
    ]
RUN yarn install --ignore-scripts --frozen-lockfile
ENV PATH /node_modules/.bin:$PATH

# add source 
COPY . ./
CMD ["yarn", "start:mock"]