# Docker Build Documentation – ClinicApi

## 1. Exact Commands to Build, Test, Run, and Tag

### Build the Production Image
```bash
docker build --build-arg APP_VERSION=0.1.0 -t clinic-api:0.1.0 .
```

### Run the Unit Test Stage
```bash
docker build --target test -t clinic-api:test .
```

### Run the Container Locally
```bash
docker run --rm -d -p 8080:8080 --name clinic-api clinic-api:0.1.0
curl http://localhost:8080/health
curl http://localhost:8080/patients
docker stop clinic-api
```

### Tag and Push to GitHub Container Registry (GHCR)
```bash
docker tag clinic-api:0.1.0 ghcr.io/jennywitzke/clinic-api:0.1.0
docker tag clinic-api:0.1.0 ghcr.io/jennywitzke/clinic-api:latest
docker push ghcr.io/jennywitzke/clinic-api:0.1.0
docker push ghcr.io/jennywitzke/clinic-api:latest
```

## 2. Image Size & Security Verification
- **Final Image Size:** 343 MB (Disk Usage) / 98 MB (Content Size)
- **Non-Root Execution:** Runs as unprivileged user `1654` (`$APP_UID`), verified via `docker image inspect clinic-api:0.1.0 --format '{{.Config.User}}'`.
- **Build Context Size:** < 2 KB (filtered via `.dockerignore`, excluding `bin/`, `obj/`, `.git/`, `.vscode/`, `.env`, and `docs/`).

## 3. Dockerfile Stages Explained
1. **Stage 1 (`build` – `mcr.microsoft.com/dotnet/sdk:10.0`):**
   Copies `Directory.Build.props` and `ClinicApi.csproj` first and runs `dotnet restore` so NuGet dependencies are cached in a dedicated layer. Then copies the C# source files and runs `dotnet publish -c Release --no-restore` to produce optimized release assemblies in `/app/publish`.
2. **Stage 2 (`test` – inherits from `build`):**
   Copies the xUnit test project (`tests/ClinicApi.Tests/`) and executes `dotnet test -c Release`. When invoked with `docker build --target test`, it returns exit code `0` if all tests pass and a non-zero exit code if any test fails. BuildKit skips this stage when building the final runtime image.
3. **Stage 3 (`final` – `mcr.microsoft.com/dotnet/aspnet:10.0`):**
   Starts from a clean, pinned ASP.NET runtime image without the SDK or compilers. Applies OpenContainers metadata labels (`source`, `description`, `licenses`, `version`), copies only the published artifacts from the `build` stage, exposes port `8080`, switches to non-root user `$APP_UID` (`1654`), and launches the application using the exec-form `ENTRYPOINT ["dotnet", "ClinicApi.dll"]`.

## 4. Base Image Choice & Rationale
- **Build Stage (`mcr.microsoft.com/dotnet/sdk:10.0`):** Pinned to version `10.0` (never `:latest`). Chosen because compiling C# source code, resolving NuGet packages, and running xUnit tests require the MSBuild engine, the Roslyn compiler (`csc`), and the .NET CLI tools.
- **Runtime Stage (`mcr.microsoft.com/dotnet/aspnet:10.0`):** Pinned to version `10.0`. Chosen because at runtime the compiler and SDK tools are unnecessary; only the lightweight CoreCLR and ASP.NET Core shared web framework libraries are needed to host the compiled `ClinicApi.dll`. This reduces the image size by ~74% (from 1.31 GB to 343 MB) and minimizes the security attack surface.
