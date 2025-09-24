FROM artifacts.cha.rbxd.ds/icis-certificates:latest as certs

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

COPY --from=certs /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

# Load dependencies into the NuGet cache, so this can be added to a docker build cache layer
COPY ["NuGet.Config", "."]
COPY ["src/main/Subscriber/Subscriber.csproj", "src/main/Subscriber/"]
COPY ["src/main/BusinessLayer/BusinessLayer.csproj", "src/main/BusinessLayer/"]
COPY ["src/main/Infrastructure/Infrastructure.csproj", "src/main/Infrastructure/"]
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
RUN dotnet restore "src/main/Subscriber/Subscriber.csproj" --ignore-failed-sources --locked-mode

# Copy application files into the container
COPY . .
WORKDIR "/app/src/main/Subscriber"
RUN dotnet build "Subscriber.csproj" \
  /p:RestoreLockedMode=true \
  -c Release \
  -o /app/build


FROM build AS publish
ARG FULL_SEM_VERSION

WORKDIR "/app/src/main/Subscriber"
RUN dotnet publish "Subscriber.csproj" \
    -c Release \
    -o /app/publish \
    /p:Version=$FULL_SEM_VERSION \
    /p:Product=Subscriber-$FULL_SEM_VERSION

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final

# Ensure certificates are added before anything else
COPY --from=certs /certificates/* /usr/local/share/ca-certificates/
RUN apk update && apk add ca-certificates && rm -rf /var/cache/apk/*
RUN update-ca-certificates

WORKDIR /opt/bff
RUN apk add bash icu-libs

COPY --from=publish /app/publish .
COPY src/main/docker/Subscriber/run.sh .
COPY src/main/Subscriber/.env dev.env
COPY /SqlScripts/. ../../SqlScripts/

EXPOSE 80

# set default values
ENV DEV_MODE=false
ENV HOSTINGS_URLS=http://*:80/
ENV LOG_LEVEL=Warning

# used in run.sh script and posibly overriden from Helm charts
ENV SECRET_FILE_PATH=/etc/secrets/.env

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN chmod +x /opt/bff/run.sh
CMD /opt/bff/run.sh
