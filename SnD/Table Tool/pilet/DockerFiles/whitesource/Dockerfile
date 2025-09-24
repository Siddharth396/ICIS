FROM artifacts.cha.rbxd.ds/node:18.3.0 as build
RUN apt-get update
RUN apt-get -y install default-jre
COPY ./wss_agent.sh /wss_agent.sh

FROM build as runner
RUN mkdir /source

COPY --from=artifacts.cha.rbxd.ds/icis-certificates /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

RUN cd /source
RUN yarn install --production --ignore-engines --frozen-lockfile --ignore-scripts

ENV WS_CHECKPOLICIES="true"
ENV WS_FORCECHECKALLDEPENDENCIES="true"
ENV WS_FORCEUPDATE="true"
ENV WS_FORCEUPDATE_FAILBUILDONPOLICYVIOLATION="true"
ENV WS_RESOLVEALLDEPENDENCIES="false"
ENV WS_SHOWPROGRESSBAR="false"
ENV WS_PRODUCTNAME="ICIS"
ENV HTTPS_PROXY=${https_proxy}
ENV WS_PROXY_HOST=${PROXY_HOST}
ENV WS_PROXY_PORT=${PROXY_HOST_PORT}
ENV WS_EXCLUDES='**/.*, **/src/test, **/testdata, **/*sources.jar, **/*javadoc.jar **/node_modules'
ENV WS_INCLUDES="**/*.js **/*.jsx **/*.ts **/*.tsx"
ENV WS_WSS_URL="https://app-eu.whitesourcesoftware.com/agent"
ENV WS_NPM_YARNPROJECT=true
CMD bash wss_agent.sh \
  -apiKey ${APIKEY} \
  -userKey ${USERKEY} \
  -noConfig "true" \
  -d /source
