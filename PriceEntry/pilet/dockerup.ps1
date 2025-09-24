param
(
    [Parameter(Mandatory = $false)]
    [string]$Setup = "authoring",

    [Parameter(Mandatory = $false)] 
    [switch]$UseMockAppShell,

    [Parameter(Mandatory = $false)] 
    [switch]$OmitRichText,

    [Parameter(Mandatory = $false)] 
    [switch]$OmitPeriodGenerator
)

# Path to the docker-compose file
$dockerComposeFile = "docker-compose-$Setup.yml"

if ($UseMockAppShell) {
    $OmitRichText = $true
}

if ($OmitRichText) {
    docker compose -f $dockerComposeFile rm -sfv feedservice rich-text-pilet-authoring richtext-authoring-bff-authproxy richtext-authoring-bff
}

if ($OmitPeriodGenerator) {
    docker compose -f $dockerComposeFile rm -sfv period-generator-service period-generator-service-authproxy
}

docker compose -f $dockerComposeFile pull price-entry-authoring-pilet-authproxy mongodb-seeder prcent-mongodb-migration price-entry-authoring-bff-authproxy price-entry-authoring-bff
docker compose -f $dockerComposeFile up -d price-entry-authoring-pilet-authproxy mongodb-seeder prcent-mongodb-migration price-entry-authoring-bff-authproxy price-entry-authoring-bff

if ($UseMockAppShell -and $OmitRichText) {
    docker compose -f $dockerComposeFile rm -sfv authoring-core-bff authoring-core-bff-authproxy
    docker compose -f $dockerComposeFile pull authoring-mock-bff
    docker compose -f $dockerComposeFile up -d authoring-mock-bff
} else {
    docker compose -f $dockerComposeFile rm -sfv authoring-mock-bff
    docker compose -f $dockerComposeFile pull authoring-core-bff authoring-core-bff-authproxy
    docker compose -f $dockerComposeFile up -d authoring-core-bff authoring-core-bff-authproxy
}

if (-not $OmitPeriodGenerator) {
    docker compose -f $dockerComposeFile pull period-generator-seeder period-generator-service period-generator-service-authproxy
    docker compose -f $dockerComposeFile up -d period-generator-seeder period-generator-service period-generator-service-authproxy
}

if (-not $OmitRichText) {
    docker compose -f $dockerComposeFile pull feedservice rich-text-pilet-authoring richtext-authoring-bff-authproxy richtext-authoring-bff
    docker compose -f $dockerComposeFile up -d feedservice rich-text-pilet-authoring richtext-authoring-bff-authproxy richtext-authoring-bff
}