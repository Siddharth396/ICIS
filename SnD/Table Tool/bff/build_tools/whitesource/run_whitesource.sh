#!/usr/bin/env bash
curl -LJO https://github.com/whitesource/unified-agent-distribution/raw/master/standAlone/wss_agent.sh

export WS_CHECKPOLICIES="true"
export WS_FORCECHECKALLDEPENDENCIES="true"
export WS_FORCEUPDATE="true"
export WS_FORCEUPDATE_FAILBUILDONPOLICYVIOLATION="true"
export WS_RESOLVEALLDEPENDENCIES="false"

export WS_PRODUCTNAME="ICIS"
export WS_PROJECTNAME="${WS_PROJECT_NAME}"

export WS_INCLUDES="${WS_INCLUDES_PATTERN}"

export WS_WSS_URL="https://app-eu.whitesourcesoftware.com/agent"

if [ -z "$WS_USER_KEY" ]
then
  bash wss_agent.sh -apiKey $WS_API_KEY -noConfig "false" || true
else
  bash wss_agent.sh -apiKey $WS_API_KEY -userKey $WS_USER_KEY -noConfig "false" || true
fi
