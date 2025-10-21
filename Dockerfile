# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR ./

COPY *.sln .
COPY FishFarmAPI_v2/FishFarmAPI_v2.csproj FishFarmAPI_v2/
COPY BusinessObjects/FishFarm.BusinessObjects.csproj BusinessObjects/
COPY Repositories/FishFarm.Repositories.csproj Repositories/
COPY Services/FishFarm.Services.csproj Services/
RUN dotnet restore

COPY . .
WORKDIR /FishFarmAPI_v2
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FishFarmAPI_v2.dll"]
