param(
    [string]$PostgresImage = $(if ($env:BACKEND_TEST_POSTGRES_IMAGE) { $env:BACKEND_TEST_POSTGRES_IMAGE } else { "postgres:15-alpine" })
)

$ErrorActionPreference = "Stop"

$previous = @{
    RUN_BACKEND_INTEGRATION_TESTS = $env:RUN_BACKEND_INTEGRATION_TESTS
    BACKEND_TEST_POSTGRES_IMAGE = $env:BACKEND_TEST_POSTGRES_IMAGE
    TESTCONTAINERS_RYUK_DISABLED = $env:TESTCONTAINERS_RYUK_DISABLED
}

try {
    $env:RUN_BACKEND_INTEGRATION_TESTS = "1"
    $env:BACKEND_TEST_POSTGRES_IMAGE = $PostgresImage
    $env:TESTCONTAINERS_RYUK_DISABLED = "true"

    Write-Host "Running backend integration tests"
    Write-Host "Postgres image: $PostgresImage"

    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    dotnet test HabitApi.Tests/HabitApi.Tests.csproj --filter "Category=Integration&Dependency=Postgres" --logger "console;verbosity=normal"
    $exitCode = $LASTEXITCODE
    $stopwatch.Stop()

    Write-Host ("Total backend integration test time: {0}" -f $stopwatch.Elapsed)

    if ($exitCode -ne 0) {
        exit $exitCode
    }
}
finally {
    foreach ($name in $previous.Keys) {
        [Environment]::SetEnvironmentVariable($name, $previous[$name], "Process")
    }
}
