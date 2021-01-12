param (
    [string]
    [Parameter(Mandatory = $true)]
    $BaseDirectory
)

Write-Output "Copying Playwright runtimes..."
Write-Output "BaseDirectory: $BaseDirectory"

$sourceDirectory = "runtimes"
$targetDirectory = "bin/runtimes"

New-Item -ItemType Directory -Path $BaseDirectory$targetDirectory -Force
Copy-Item -Path $BaseDirectory$sourceDirectory/* -Destination $BaseDirectory$targetDirectory/ -Recurse -Force
