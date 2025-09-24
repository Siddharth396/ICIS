FROM mcr.microsoft.com/devcontainers/javascript-node:1-20-bullseye

RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends git ca-certificates


COPY --from=artifacts.cha.rbxd.ds/icis-certificates /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

RUN npm config set cafile /etc/ssl/certs/ca-certificates.crt
RUN su node -c "npm install -g migrate-mongo"