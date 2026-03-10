---
title: "Docs Pipeline"
---

# Docs Pipeline

RibbonControl uses Lunet for the static docs site and generated .NET API reference.

Important files:

- `site/config.scriban`
- `site/menu.yml`
- `site/articles/**`
- `.config/dotnet-tools.json`
- `build-docs.sh` / `build-docs.ps1`
- `serve-docs.sh` / `serve-docs.ps1`
- `.github/workflows/docs.yml`

Local commands:

```bash
dotnet tool restore
./build-docs.sh
./serve-docs.sh
```

PowerShell equivalents:

```powershell
dotnet tool restore
./build-docs.ps1
./serve-docs.ps1
```

CI behavior:

- the main CI workflow builds the docs site to validate structure and API generation,
- the main CI workflow uploads the built docs site as an artifact,
- the docs workflow publishes `site/.lunet/build/www` to GitHub Pages on pushes to `main` and `master`.
