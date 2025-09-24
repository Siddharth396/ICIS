FROM artifacts.cha.rbxd.ds/utils/alpine/checkmarx:9.4.0_GA_ENV as copy
RUN mkdir /source
WORKDIR /
COPY ./run.sh /run.sh
RUN chmod +x /run.sh
FROM copy as runner

RUN chmod +x /opt/CxConsolePlugin/runCxConsole.sh
ENTRYPOINT ["sh", "/run.sh"]