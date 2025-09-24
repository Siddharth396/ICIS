FROM artifacts.cha.rbxd.ds/icis-certificates:latest as certs
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /app

# certs has all the certificates required for docker.
# dotnet restore may fail due to certificate issue. 
# To resolve this, certificates are added before the restore
COPY --from=certs /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

COPY ["NuGet.Config", "."]
COPY ["src/SnD.EventProcessor.Poller/SnD.EventProcessor.Poller.csproj", "src/SnD.EventProcessor.Poller/"]
COPY ["src/SnD.EventProcessor.Poller.Tests/SnD.EventProcessor.Poller.Tests.csproj", "src/SnD.EventProcessor.Poller.Tests/"]

RUN dotnet restore "src/SnD.EventProcessor.Poller/SnD.EventProcessor.Poller.csproj"
RUN dotnet restore "src/SnD.EventProcessor.Poller.Tests/SnD.EventProcessor.Poller.Tests.csproj"

COPY src/. src/.
COPY ["SnD.EventProcessor.Poller.sln", "."]

RUN dotnet build -c Release -o /app/build

#Set the flag to tell TeamCity that these are unit tests:
ENV TEAMCITY_PROJECT_NAME = ${TEAMCITY_PROJECT_NAME}

RUN dotnet test "src/SnD.EventProcessor.Poller.Tests/SnD.EventProcessor.Poller.Tests.csproj" --verbosity=normal

#RUN dotnet tool install dotnet-reportgenerator-globaltool --tool-path /tools

#RUN dotnet test "SnD.EventProcessor.Poller.Tests.csproj" \
#--verbosity=normal
#/p:AltCover=true \
#/p:AltCoverOpenCover=true \
#/p:AltCoverXmlReport="/app/coverage/coverage.xml" \
#/p:AltCoverAttributeFilter=ExcludeFromCodeCoverage \
#/p:AltCoverAssemblyExcludeFilter="^(?!EventProcessor$).*" \
#/p:AltCoverForce=true \
#/p:RestoreLockedMode=true

#RUN /tools/reportgenerator "-reports:/app/coverage/coverage.xml" "-targetdir:/app/coverage/report" "-reporttypes:HTMLInline;TeamCitySummary"

RUN dotnet publish "src/SnD.EventProcessor.Poller/SnD.EventProcessor.Poller.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS final
WORKDIR /app
EXPOSE 12000
EXPOSE 12001
COPY --from=build /app/publish .
COPY --from=certs /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

ENTRYPOINT ["dotnet", "SnD.EventProcessor.Poller.dll"]
