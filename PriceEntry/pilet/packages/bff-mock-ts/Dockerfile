FROM artifacts.cha.rbxd.ds/node:20-alpine AS builder

ARG SEM_VER="0.0.1"
RUN apk --no-cache add ca-certificates openssl \
    && rm -rf /var/cache/apk/*

COPY --from=artifacts.cha.rbxd.ds/icis-certificates /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt

WORKDIR /app

COPY --from=artifacts.cha.rbxd.ds/local-ssl-certificate /downloaded_local_ssl_certs/* /localhost/ssl/
RUN openssl rsa -in /localhost/ssl/tls.key -out ./key.pem
RUN openssl x509 -in /localhost/ssl/tls.crt -out ./cert.pem

COPY [ \
    "package.json", \
    "yarn.lock", \
    ".yarnrc", \
    "/" \
    ]
RUN yarn install --ignore-scripts --frozen-lockfile
ENV PATH /node_modules/.bin:$PATH

# add source 
COPY . .
CMD ["yarn", "start:mock"]