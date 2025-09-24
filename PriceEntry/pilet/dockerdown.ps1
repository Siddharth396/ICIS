param
(
    [Parameter(Mandatory = $false)]
    [string]$Setup = "authoring"
)

$dockerComposeFile = "docker-compose-$Setup.yml"

docker compose -f $dockerComposeFile down