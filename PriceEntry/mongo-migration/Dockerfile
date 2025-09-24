# Start with a base image
FROM artifacts.cha.rbxd.ds/docker_images_proxy/node:20.10.0-alpine

RUN apk --no-cache add ca-certificates bash \
     && rm -rf /var/cache/apk/*

COPY --from=artifacts.cha.rbxd.ds/icis-certificates:latest /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt

# install mongo-migrate globally
RUN npm install -g migrate-mongo
# Set the working directory
WORKDIR /app

# Copy package.json and package-lock.json to the container
COPY ["package*.json", "./docker/run.sh" , "./"]

# Install dependencies
RUN npm install

# Copy the rest of the application code to the container
COPY . .

RUN npm run lint

RUN chmod +x ./run.sh
ENTRYPOINT ["./run.sh"]
CMD ["up"]
