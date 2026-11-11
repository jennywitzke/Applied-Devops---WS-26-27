# syntax=docker/dockerfile:1

ARG DOTNET_VERSION=10.0

############ Stage 1: build ############
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG APP_VERSION=0.1.0
WORKDIR /src

COPY Directory.Build.props ./
COPY src/ClinicApi/ClinicApi.csproj src/ClinicApi/
RUN dotnet restore src/ClinicApi/ClinicApi.csproj

COPY src/ClinicApi/ src/ClinicApi/
RUN dotnet publish src/ClinicApi/ClinicApi.csproj -c Release -o /app/publish --no-restore -p:Version=${APP_VERSION}

############ Stage 2: test ############
FROM build AS test
COPY tests/ClinicApi.Tests/ tests/ClinicApi.Tests/
RUN dotnet test tests/ClinicApi.Tests/ClinicApi.Tests.csproj -c Release

############ Stage 3: runtime ############
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
ARG APP_VERSION=0.1.0
WORKDIR /app

LABEL org.opencontainers.image.source="https://github.com/jennywitzke/Applied-Devops---WS-26-27" \
      org.opencontainers.image.description="Clinic Patient and Appointment Management API" \
      org.opencontainers.image.licenses="MIT" \
      org.opencontainers.image.version=${APP_VERSION}

COPY --from=build /app/publish .

EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "ClinicApi.dll"]
