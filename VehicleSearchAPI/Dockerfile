# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Publish the app to the /out folder in Release configuration
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /out ./

# Environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:$PORT

# Expose port
EXPOSE $PORT

# Start app
ENTRYPOINT ["dotnet", "VehicleSearch.dll"]