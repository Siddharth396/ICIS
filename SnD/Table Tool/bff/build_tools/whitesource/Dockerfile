# define all args at the beginning to have a clear view what is required
ARG DOCKER_TAGNAME=latest
ARG DOCKER_IMAGE

# the code has to be from some docker image
FROM ${DOCKER_IMAGE}:${DOCKER_TAGNAME} as published

FROM artifacts.cha.rbxd.ds/adoptopenjdk:8 AS whitesource
ARG WS_API_KEY
ARG WS_USER_KEY
ARG WS_PROJECT_NAME
ARG WS_INCLUDES_PATTERN
ARG CODE_FOLDER_PATH
WORKDIR /build

COPY --from=published ${CODE_FOLDER_PATH} .
COPY run_whitesource.sh .
RUN chmod +x /build/run_whitesource.sh
RUN /build/run_whitesource.sh
