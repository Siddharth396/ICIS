FROM artifacts.cha.rbxd.ds/docker_images_proxy/grafana/k6:1.1.0 AS base

USER root
RUN apk add --update bash nodejs npm
RUN npm install -g yarn

RUN apk add --no-cache --update ca-certificates
COPY --from=artifacts.cha.rbxd.ds/icis-certificates /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates
RUN cat /usr/local/share/ca-certificates/*.crt >> /usr/local/share/ca-certificates/b2b_bundle.crt
RUN mkdir certificates
RUN cat /usr/local/share/ca-certificates/*.crt >> ./certificates/b2b_bundle.crt
ENV NODE_EXTRA_CA_CERTS=/usr/local/share/ca-certificates/b2b_bundle.crt

# Build the App
WORKDIR /app

COPY ./tsconfig.json ./tsconfig.json
COPY ./package.json ./package.json
COPY ./yarn.lock ./yarn.lock
COPY ./scripts ./scripts
COPY ./.yarnrc ./.yarnrc
COPY ./environments.ts ./environments.ts
COPY ./utils ./utils
COPY ./fixtures ./fixtures
COPY ./javascripts ./javascripts


RUN yarn install --frozen-lockfile


RUN mkdir ./html-results

# blank, so it can be set by docker compose
ENTRYPOINT []
