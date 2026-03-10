$ErrorActionPreference = 'Stop'

dotnet tool restore
Push-Location site
try {
    if (Test-Path '.lunet/build') {
        Remove-Item '.lunet/build' -Recurse -Force
    }

    dotnet tool run lunet --stacktrace build
}
finally {
    Pop-Location
}
