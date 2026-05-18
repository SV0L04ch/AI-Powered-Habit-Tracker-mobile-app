param(
    [string]$Model = $(if ($env:LLM_TEST_MODEL) { $env:LLM_TEST_MODEL } else { "gemma3:4b" }),
    [string]$OllamaImage = $(if ($env:LLM_TEST_OLLAMA_IMAGE) { $env:LLM_TEST_OLLAMA_IMAGE } else { "ollama/ollama:latest" }),
    [int]$ResponseTimeoutSeconds = $(if ($env:LLM_TEST_RESPONSE_TIMEOUT_SECONDS) { [int]$env:LLM_TEST_RESPONSE_TIMEOUT_SECONDS } else { 180 }),
    [int]$ModelPullTimeoutSeconds = $(if ($env:LLM_TEST_MODEL_PULL_TIMEOUT_SECONDS) { [int]$env:LLM_TEST_MODEL_PULL_TIMEOUT_SECONDS } else { 1200 }),
    [switch]$NoPullModel
)

$ErrorActionPreference = "Stop"

$previous = @{
    RUN_LLM_INTEGRATION_TESTS = $env:RUN_LLM_INTEGRATION_TESTS
    LLM_TEST_MODEL = $env:LLM_TEST_MODEL
    LLM_TEST_OLLAMA_IMAGE = $env:LLM_TEST_OLLAMA_IMAGE
    LLM_TEST_RESPONSE_TIMEOUT_SECONDS = $env:LLM_TEST_RESPONSE_TIMEOUT_SECONDS
    LLM_TEST_MODEL_PULL_TIMEOUT_SECONDS = $env:LLM_TEST_MODEL_PULL_TIMEOUT_SECONDS
    LLM_TEST_PULL_MODEL = $env:LLM_TEST_PULL_MODEL
    TESTCONTAINERS_RYUK_DISABLED = $env:TESTCONTAINERS_RYUK_DISABLED
}

try {
    $env:RUN_LLM_INTEGRATION_TESTS = "1"
    $env:LLM_TEST_MODEL = $Model
    $env:LLM_TEST_OLLAMA_IMAGE = $OllamaImage
    $env:LLM_TEST_RESPONSE_TIMEOUT_SECONDS = "$ResponseTimeoutSeconds"
    $env:LLM_TEST_MODEL_PULL_TIMEOUT_SECONDS = "$ModelPullTimeoutSeconds"
    $env:LLM_TEST_PULL_MODEL = $(if ($NoPullModel) { "0" } else { "1" })
    $env:TESTCONTAINERS_RYUK_DISABLED = "true"

    Write-Host "Running LLM integration tests"
    Write-Host "Image: $OllamaImage"
    Write-Host "Model: $Model"
    Write-Host "Response budget: $ResponseTimeoutSeconds seconds"
    Write-Host "Pull model: $(-not $NoPullModel)"

    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    dotnet test HabitApi.Tests/HabitApi.Tests.csproj --filter "Category=Integration" --logger "console;verbosity=normal"
    $exitCode = $LASTEXITCODE
    $stopwatch.Stop()

    Write-Host ("Total integration test time: {0}" -f $stopwatch.Elapsed)

    if ($exitCode -ne 0) {
        exit $exitCode
    }
}
finally {
    foreach ($name in $previous.Keys) {
        [Environment]::SetEnvironmentVariable($name, $previous[$name], "Process")
    }
}
