# The commands in this file are written in a sequence that helps make the best use of Docker caching.
# See Readme file for more information.

# ------------------------------------ #
# STAGE - Base                         #
# Contains all the dev dependencies for the project and is able to run any of the
# scripts from package.json
# ------------------------------------ #
FROM artifacts.cha.rbxd.ds/docker_images_proxy/node:20-alpine AS base
RUN apk add --no-cache --update ca-certificates
WORKDIR /app

COPY --from=artifacts.cha.rbxd.ds/icis-certificates /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt

# Copy just the files needed to install the dependencies as they doesn't change often
COPY [ "/packages/price-entry-pilet/package.json", \
  "/packages/price-entry-pilet/.yarnrc", \
  "/packages/price-entry-pilet/yarn.lock", \
  "./" ]

RUN yarn --frozen-lockfile --ignore-scripts

FROM base AS runner

COPY /packages/price-entry-pilet ./

RUN chmod 777 /app/run_tests.sh

CMD ["sh", "-c", "/app/run_tests.sh"]
