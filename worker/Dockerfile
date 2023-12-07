# https://hub.docker.com/_/microsoft-dotnet

#FROM mcr.microsoft.com/dotnet/sdk:8.0.100-preview.7-bookworm-slim-amd64 AS build

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build


WORKDIR /app

COPY . .

RUN dotnet publish worker.csproj -c release -o /out


# final stage/image
#FROM mcr.microsoft.com/dotnet/aspnet:8.0.0-preview.7-bookworm-slim-amd64

FROM mcr.microsoft.com/dotnet/aspnet:7.0

ARG PROJECT_PATH
ARG PROJECT_NAME
ARG USE_FFMPEG

WORKDIR /app
COPY --from=build /out ./

EXPOSE 80

RUN chmod +x worker.dll


ENTRYPOINT ["dotnet", "worker.dll"]