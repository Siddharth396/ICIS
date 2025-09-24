FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base

RUN apk update && apk add ca-certificates && rm -rf /var/cache/apk/* \
  mkdir /usr/local/share/ca-certificates/
COPY --from=artifacts.cha.rbxd.ds/icis-certificates /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
WORKDIR /src

COPY --from=artifacts.cha.rbxd.ds/icis-certificates /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

COPY ["NuGet.Config", "Directory.Packages.props", "Directory.Build.props", "./"]
COPY ["src/main/MongoDbSeeder/MongoDbSeeder.csproj", "src/main/MongoDbSeeder/"]
COPY ["src/main/Infrastructure/Infrastructure.csproj", "src/main/Infrastructure/"]

RUN dotnet restore "src/main/Infrastructure/Infrastructure.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/main/MongoDbSeeder/MongoDbSeeder.csproj" --ignore-failed-sources --locked-mode

# Copy application files into the container
COPY . .
WORKDIR "/src/src/main/MongoDbSeeder"
RUN dotnet build "MongoDbSeeder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MongoDbSeeder.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN apk add bash

COPY --from=publish /app/publish .
COPY src/main/docker/MongoDbSeeder/run.sh .
COPY src/main/MongoDbSeeder/.env dev.env

# used in run.sh script and posibly overriden from Helm charts
ENV SECRET_FILE_PATH=/etc/secrets/.env

RUN chmod +x /app/run.sh
CMD /app/run.sh