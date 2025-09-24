FROM artifacts.cha.rbxd.ds/docker_images_proxy/node:20-alpine AS builder
RUN apk add --no-cache --update ca-certificates
WORKDIR /app

ARG SEM_VER="0.0.1"

# cache run
COPY [ \
  "package.json", \
  "yarn.lock", \
  ".yarnrc", \
  "/" \
]

COPY --from=artifacts.cha.rbxd.ds/icis-certificates /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt

RUN yarn install --ignore-scripts --frozen-lockfile
ENV PATH /node_modules/.bin:$PATH

# add source
COPY . ./

ARG IS_AUTHORING=false

# package the pilet
RUN yarn version --new-version "$SEM_VER" \
    && if [ "$IS_AUTHORING" = "true" ] ; then sed -i "s/price-entry-pilet/price-entry-pilet-authoring/ig" ./package.json && yarn build:authoring ; else yarn build; fi \
    && yarn package

# publish the pilet
FROM artifacts.cha.rbxd.ds/alpine:3.9
WORKDIR "/pilet"

RUN apk update && apk add curl bash

COPY --from=builder "/app/pilet-package.tgz" ./pilet-package.tgz
COPY --from=builder "/app/publish.sh" ./publish.sh

RUN chmod +x /pilet/publish.sh

ARG SEM_VER="0.0.1"
ARG IS_AUTHORING=false

ENV SEM_VER=${SEM_VER}
ENV IS_AUTHORING=${IS_AUTHORING}

CMD ["/pilet/publish.sh"]
