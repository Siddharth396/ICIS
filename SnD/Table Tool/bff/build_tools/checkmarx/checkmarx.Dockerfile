FROM artifacts.cha.rbxd.ds/utils/alpine/checkmarx:9.4.0_GA as copy
COPY source/ /source

FROM copy as runner
RUN chmod +x /opt/CxConsolePlugin/runCxConsole.sh
ENTRYPOINT ["/opt/CxConsolePlugin/runCxConsole.sh"]
