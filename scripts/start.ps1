param([int]$valuatorInstances, [int]$rankCalculatorInstances)

if ($valuatorInstances -eq 0) {
    $valuatorInstances = Read-Host "Enter amount of valuator instances"
}
if ($rankCalculatorInstances -eq 0) {
    $rankCalculatorInstances = Read-Host "Enter amount of rank calculator instances"
}

Set-Location -LiteralPath $PSScriptRoot
docker compose -f "../docker-compose.yml" up --scale valuator=$valuatorInstances --scale rank-calculator=$rankCalculatorInstances -d
