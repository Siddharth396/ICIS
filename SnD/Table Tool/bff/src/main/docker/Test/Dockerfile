FROM artifacts.cha.rbxd.ds/icis-certificates:latest as certs

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

COPY --from=certs /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

# Load dependencies into the NuGet cache, so this can be added to a docker build cache layer
COPY ["NuGet.Config", "."]
COPY ["src/main/Authoring/Authoring.csproj", "src/main/Authoring/"]
COPY ["src/main/Subscriber/Subscriber.csproj", "src/main/Subscriber/"]
COPY ["src/main/BusinessLayer/BusinessLayer.csproj", "src/main/BusinessLayer/"]
COPY ["src/main/Infrastructure/Infrastructure.csproj", "src/main/Infrastructure/"]
COPY ["src/test/unit/Test.Infrastructure/Test.Infrastructure.csproj", "src/test/unit/Test.Infrastructure/"]

ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false

RUN dotnet restore "src/main/Authoring/Authoring.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/main/Subscriber/Subscriber.csproj" --ignore-failed-sources --locked-mode

# Copy application files into the container
COPY . .

FROM build AS coverage
WORKDIR "/app/src/test/unit/Authoring.Tests"
RUN dotnet tool install dotnet-reportgenerator-globaltool --tool-path /tools

RUN dotnet test "Authoring.Tests.csproj" \
  --test-adapter-path:. \
  --logger:"junit;LogFilePath=/artifacts/{assembly}-test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose" \
  --verbosity=minimal \
  /p:AltCover=true \
  /p:AltCoverOpenCover=true \
  /p:AltCoverXmlReport="/app/coverage/Authoring/coverage.xml" \
  /p:AltCoverAttributeFilter=ExcludeFromCodeCoverage \
  /p:AltCoverAssemblyExcludeFilter="^(?!(Authoring$||BusinessLayer||Infrastructure)).*" \
  /p:AltCoverForce=true \
  /p:RestoreLockedMode=true

WORKDIR "/app/src/test/unit/BusinessLayer.Tests"

RUN dotnet test "BusinessLayer.Tests.csproj" \
  --test-adapter-path:. \
  --logger:"junit;LogFilePath=/artifacts/{assembly}-test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose" \
  --verbosity=minimal \
  /p:AltCover=true \
  /p:AltCoverOpenCover=true \
  /p:AltCoverXmlReport="/app/coverage/BusinessLayer/coverage.xml" \
  /p:AltCoverAttributeFilter=ExcludeFromCodeCoverage \
  /p:AltCoverAssemblyExcludeFilter="^(?!(BusinessLayer$||Infrastructure)).*" \
  /p:AltCoverForce=true \
  /p:RestoreLockedMode=true

WORKDIR "/app/src/test/unit/Test.Infrastructure"

RUN dotnet test "Test.Infrastructure.csproj" \
  --test-adapter-path:. \
  --logger:"junit;LogFilePath=/artifacts/{assembly}-test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose" \
  --verbosity=minimal \
  /p:AltCover=true \
  /p:AltCoverOpenCover=true \
  /p:AltCoverXmlReport="/app/coverage/Infrastructure/coverage.xml" \
  /p:AltCoverAttributeFilter=ExcludeFromCodeCoverage \
  /p:AltCoverAssemblyExcludeFilter="^(?!(Infrastructure$)).*" \
  /p:AltCoverForce=true \
  /p:RestoreLockedMode=true

WORKDIR "/app/src/test/unit/Subscriber.Tests"

RUN dotnet test "Subscriber.Tests.csproj" \
  --test-adapter-path:. \
  --logger:"junit;LogFilePath=/artifacts/{assembly}-test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose" \
  --verbosity=minimal \
  /p:AltCover=true \
  /p:AltCoverOpenCover=true \
  /p:AltCoverXmlReport="/app/coverage/Subscriber/coverage.xml" \
  /p:AltCoverAttributeFilter=ExcludeFromCodeCoverage \
  /p:AltCoverAssemblyExcludeFilter="^(?!(Subscriber$||BusinessLayer||Infrastructure)).*" \
  /p:AltCoverForce=true \
  /p:RestoreLockedMode=true

RUN /tools/reportgenerator "-reports:/app/coverage/*/coverage.xml" "-targetdir:/app/coverage/report" "-reporttypes:Cobertura;HTMLInline;XmlSummary" "-verbosity:Error"
