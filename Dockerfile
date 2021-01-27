# https://docs.docker.com/engine/examples/dotnetcore/
# https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/docker/building-net-docker-images?view=aspnetcore-5.0

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/MovieLibrary/MovieLibrary.csproj src/MovieLibrary/
COPY src/MovieLibrary.Dal.MongoDB/MovieLibrary.Dal.MongoDB.csproj src/MovieLibrary.Dal.MongoDB/
COPY src/MovieLibrary.Logic/MovieLibrary.Logic.csproj src/MovieLibrary.Logic/
RUN dotnet restore src/MovieLibrary

# Copy everything else and build
COPY src/ ./src/
COPY MovieLibrary.src.ruleset LICENSE.txt ./
RUN dotnet publish src/MovieLibrary --no-restore -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "MovieLibrary.dll" ]
