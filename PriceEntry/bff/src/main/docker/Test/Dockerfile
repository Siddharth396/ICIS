FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV SNAPSHOOTER_STRICT_MODE=true
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
RUN dotnet tool install dotnet-reportgenerator-globaltool --tool-path /tools

# https://github.com/asimmon/ephemeral-mongo/issues/51#issuecomment-1855964241
ENV LIBSSL_DEB=libssl1.1_1.1.1f-1ubuntu2.24_amd64.deb
RUN wget http://security.ubuntu.com/ubuntu/pool/main/o/openssl/$LIBSSL_DEB
RUN dpkg -i $LIBSSL_DEB && rm $LIBSSL_DEB
WORKDIR /app

COPY --from=artifacts.cha.rbxd.ds/icis-certificates:latest /certificates/* /usr/local/share/ca-certificates/
RUN update-ca-certificates

# Load dependencies into the NuGet cache, so this can be added to a docker build cache layer
COPY ["NuGet.Config", "Directory.Packages.props", "Directory.Build.props", "./"]
COPY ["src/main/Authoring/Authoring.csproj", "src/main/Authoring/packages.lock.json", "src/main/Authoring/"]
COPY ["src/main/Subscriber/Subscriber.csproj", "src/main/Subscriber/packages.lock.json", "src/main/Subscriber/"]
COPY ["src/main/BusinessLayer/BusinessLayer.csproj", "src/main/BusinessLayer/"]
COPY ["src/main/Infrastructure/Infrastructure.csproj", "src/main/Infrastructure/"]
COPY ["src/main/MongoDbSeeder/MongoDbSeeder.csproj", "src/main/MongoDbSeeder/"]
COPY ["src/test/unit/Test.Infrastructure/Test.Infrastructure.csproj", "src/test/unit/Test.Infrastructure/"]
COPY ["src/test/unit/Authoring.Tests/Authoring.Tests.csproj", "src/test/unit/Authoring.Tests/"]
COPY ["src/test/unit/Subscriber.Tests/Subscriber.Tests.csproj", "src/test/unit/Subscriber.Tests/"]
COPY ["src/test/unit/Architecture.Tests/Architecture.Tests.csproj", "src/test/unit/Architecture.Tests/"]

RUN dotnet restore "src/main/Infrastructure/Infrastructure.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/main/BusinessLayer/BusinessLayer.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/main/Authoring/Authoring.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/main/Subscriber/Subscriber.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/main/MongoDbSeeder/MongoDbSeeder.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/test/unit/Test.Infrastructure/Test.Infrastructure.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/test/unit/Authoring.Tests/Authoring.Tests.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/test/unit/Subscriber.Tests/Subscriber.Tests.csproj" --ignore-failed-sources --locked-mode
RUN dotnet restore "src/test/unit/Architecture.Tests/Architecture.Tests.csproj" --ignore-failed-sources --locked-mode

# Copy the rest of the files into the container
COPY . .

FROM build AS coverage
WORKDIR "/app/src/test/unit/Authoring.Tests"

RUN dotnet test "Authoring.Tests.csproj" \
  --test-adapter-path:. \
  --logger:"junit;LogFilePath=/artifacts/{assembly}-test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose" \
  --verbosity=minimal \
  /p:AltCover=true \
  /p:AltCoverReportFormat="OpenCover" \
  /p:AltCoverReport="/app/coverage/Authoring/coverage.xml" \
  /p:AltCoverAttributeFilter=ExcludeFromCodeCoverage \
  /p:AltCoverAssemblyExcludeFilter="^(?!(Authoring$||BusinessLayer||Infrastructure)).*" \
  /p:AltCoverForce=true \
  /p:RestoreLockedMode=true


WORKDIR "/app/src/test/unit/Subscriber.Tests"

RUN dotnet test "Subscriber.Tests.csproj" \
  --test-adapter-path:. \
  --logger:"junit;LogFilePath=/artifacts/{assembly}-test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose" \
  --verbosity=minimal \
  /p:AltCover=true \
  /p:AltCoverReportFormat="OpenCover" \
  /p:AltCoverReport="/app/coverage/Subscriber/coverage.xml" \
  /p:AltCoverAttributeFilter=ExcludeFromCodeCoverage \
  /p:AltCoverAssemblyExcludeFilter="^(?!(Subscriber$||BusinessLayer||Infrastructure)).*" \
  /p:AltCoverForce=true \
  /p:RestoreLockedMode=true

RUN /tools/reportgenerator "-reports:/app/coverage/*/coverage.xml" "-targetdir:/app/coverage/report" "-reporttypes:Cobertura;HTMLInline;XmlSummary;MarkdownSummaryGithub" "-verbosity:Error"


WORKDIR "/app/src/test/unit/Architecture.Tests"

RUN dotnet test "Architecture.Tests.csproj" \
  --test-adapter-path:. \
  --logger:"junit;LogFilePath=/artifacts/{assembly}-test-result.xml;MethodFormat=Class;FailureBodyFormat=Verbose" \
  --verbosity=minimal \
  /p:RestoreLockedMode=true