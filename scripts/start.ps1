param([int]$instances)

if ($instances -eq 0) {
    $instances = Read-Host "Enter amount of valuator instances"
}

Set-Location -LiteralPath $PSScriptRoot
docker compose -f "../docker-compose.yml" up --scale valuator=$instances -d
