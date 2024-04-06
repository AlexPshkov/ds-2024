param([int]$valuatorInstances, [int]$calculatorInstances, [int]$loggerInstances)

if ($valuatorInstances -eq 0) {
    $valuatorInstances = Read-Host "Enter amount of valuator instances"
}
if ($calculatorInstances -eq 0) {
    $calculatorInstances = Read-Host "Enter amount of calculator instances"
}

if ($loggerInstances -eq 0) {
    $loggerInstances = Read-Host "Enter amount of logger instances"
}

Set-Location -LiteralPath $PSScriptRoot
docker compose -f "../docker-compose.yml" up --scale valuator=$valuatorInstances --scale rank-calculator=$calculatorInstances --scale similarity-calculator=$calculatorInstances --scale events-logger=$loggerInstances -d
