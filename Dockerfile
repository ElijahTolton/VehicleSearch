# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and projects
COPY VehicleSearch.sln ./
COPY VehicleSearch/ VehicleSearch/
COPY VehicleSearchAPI/ VehicleSearchAPI/

# Restore and publish API project
RUN dotnet restore VehicleSearchAPI/VehicleSearchAPI.csproj
RUN dotnet publish VehicleSearchAPI/VehicleSearchAPI.csproj -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

ENV ASPNETCORE_URLS=http://+:$PORT
EXPOSE $PORT

ENTRYPOINT ["dotnet", "VehicleSearchAPI.dll"]