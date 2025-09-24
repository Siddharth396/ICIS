FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
WORKDIR /app

COPY --from=artifacts.cha.rbxd.ds/icis-certificates:latest /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

# Load dependencies into the NuGet cache, so this can be added to a docker build cache layer
COPY ["NuGet.Config", "Directory.Packages.props", "Directory.Build.props", "./"]
COPY ["src/main/Authoring/Authoring.csproj", "src/main/Authoring/packages.lock.json", "src/main/Authoring/"]
COPY ["src/main/BusinessLayer/BusinessLayer.csproj", "src/main/BusinessLayer/"]
COPY ["src/main/Infrastructure/Infrastructure.csproj", "src/main/Infrastructure/"]

RUN dotnet restore "src/main/Infrastructure/Infrastructure.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/main/BusinessLayer/BusinessLayer.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/main/Authoring/Authoring.csproj" --ignore-failed-sources --locked-mode

# Copy application files into the container
COPY . .
WORKDIR "/app/src/main/Authoring"
RUN dotnet build "Authoring.csproj" \
  /p:RestoreLockedMode=true \
  -c Release \
  -o /app/build

FROM build AS publish
ARG FULL_SEM_VERSION

WORKDIR "/app/src/main/Authoring"
RUN dotnet publish "Authoring.csproj" \
    -c Release \
    -o /app/publish \
    /p:Version=$FULL_SEM_VERSION \
    /p:Product=Authoring-$FULL_SEM_VERSION

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Ensure certificates are added before anything else
RUN apk update && apk add ca-certificates && rm -rf /var/cache/apk/* \
  mkdir /usr/local/share/ca-certificates/
COPY --from=artifacts.cha.rbxd.ds/icis-certificates:latest /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

WORKDIR /opt/bff
RUN apk add bash

COPY --from=publish /app/publish .
COPY src/main/docker/Authoring/run.sh .
COPY src/main/Authoring/.env dev.env

EXPOSE 80

# set default values
ENV DEV_MODE=false
ENV HOSTINGS_URLS=http://*:80/
ENV LOG_LEVEL=Warning

# Set ASPNETCORE_HTTP_PORTS to nothing to not get warning when application start, and keep using the HOSTINGS_URLS
# Default port changed in dotnet 8 https://learn.microsoft.com/en-us/dotnet/core/compatibility/containers/8.0/aspnet-port
ENV ASPNETCORE_HTTP_PORTS=

# used in run.sh script and posibly overriden from Helm charts
ENV SECRET_FILE_PATH=/etc/secrets/.env

RUN chmod +x /opt/bff/run.sh
CMD /opt/bff/run.sh
