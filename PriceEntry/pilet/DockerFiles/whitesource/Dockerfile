FROM artifacts.cha.rbxd.ds/docker_images_proxy/ubuntu:24.04 as build

RUN apt-get update && apt-get install -y default-jre curl && apt-get clean

COPY ./wss_agent.sh /wss_agent.sh

FROM build as runner
RUN mkdir /source

RUN cd /source

ENV https_proxy=${https_proxy}
ENV http_proxy=${http_proxy}
ENV no_proxy=${no_proxy}
ENV WS_PROXY_HOST=${PROXY_HOST}
ENV WS_PROXY_PORT=${PROXY_HOST_PORT}

ENV WS_CHECKPOLICIES="true"
ENV WS_FORCECHECKALLDEPENDENCIES="true"
ENV WS_FORCEUPDATE="true"
ENV WS_FORCEUPDATE_FAILBUILDONPOLICYVIOLATION="true"
ENV WS_RESOLVEALLDEPENDENCIES="false"
ENV WS_SHOWPROGRESSBAR="false"
ENV WS_PRODUCTNAME="ICIS"
ENV WS_EXCLUDES="**/packages/bff-mock-ts/**/*, **/packages/e2e-tests/**/*"
ENV WS_INCLUDES="**/packages/price-entry-pilet/**/*.js **/packages/price-entry-pilet/**/*.jsx **/packages/price-entry-pilet/**/*.ts **/packages/price-entry-pilet/**/*.tsx"
ENV WS_WSS_URL="https://app-eu.whitesourcesoftware.com/agent"
ENV WS_NPM_YARNPROJECT="true"
ENV WS_NPM_RESOLVEDEPENDENCIES="true"
ENV WS_NPM_INCLUDEDEVDEPENDENCIES="true"

CMD bash wss_agent.sh \
  -apiKey ${APIKEY} \
  -userKey ${USERKEY} \
  -noConfig "true" \
  -d /source