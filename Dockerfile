# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution file
COPY *.sln ./

# Copy all project files separately to optimize restore caching
COPY FishFarmAPI_v2/FishFarmAPI_v2.csproj FishFarmAPI_v2/
COPY BusinessObjects/FishFarm.BusinessObjects.csproj BusinessObjects/
COPY Repositories/FishFarm.Repositories.csproj Repositories/
COPY Services/FishFarm.Services.csproj Services/
COPY DataAccessLayer/FishFarm.DataAccessLayer.csproj DataAccessLayer/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish release
WORKDIR /app/FishFarmAPI_v2
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "FishFarmAPI_v2.dll"]
