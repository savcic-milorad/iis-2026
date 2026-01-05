# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["TransportSystem.sln", "."]
COPY ["src/TransportSystem.Domain/TransportSystem.Domain.csproj", "src/TransportSystem.Domain/"]
COPY ["src/TransportSystem.Application/TransportSystem.Application.csproj", "src/TransportSystem.Application/"]
COPY ["src/TransportSystem.Infrastructure/TransportSystem.Infrastructure.csproj", "src/TransportSystem.Infrastructure/"]
COPY ["src/TransportSystem.API/TransportSystem.API.csproj", "src/TransportSystem.API/"]

# Restore dependencies
RUN dotnet restore "src/TransportSystem.API/TransportSystem.API.csproj"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/src/TransportSystem.API"
RUN dotnet build "TransportSystem.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TransportSystem.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransportSystem.API.dll"]
