# Multi-stage Dockerfile for HushKey Backend (ASP.NET Core)

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY . .
WORKDIR /app/src/HushKeyApi
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (default for ASP.NET Core)
EXPOSE 8080

# Set environment variables (optional, can be overridden)
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "HushKeyApi.dll"]
