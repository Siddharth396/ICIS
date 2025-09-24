FROM artifacts.cha.rbxd.ds/mcr.microsoft.com/playwright:v1.43.0-focal as test

COPY --from=artifacts.cha.rbxd.ds/icis-certificates:latest /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates
ENV NODE_EXTRA_CA_CERTS=/etc/ssl/certs/ca-certificates.crt

WORKDIR /app

COPY ["./packages/e2e-tests/", "./"]

RUN npm ci
RUN npm install -D @playwright/test@latest
RUN npx playwright install

ENV DISPLAY=:99

CMD ["npm", "run", "test:smoke"]
