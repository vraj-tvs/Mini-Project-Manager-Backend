# Use the official .NET 9.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 9.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["ProjectManagerAPI.csproj", "./"]
RUN dotnet restore "ProjectManagerAPI.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "ProjectManagerAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "ProjectManagerAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM base AS final
WORKDIR /app

# Copy the published application
COPY --from=publish /app/publish .

# Create a non-root user for security and set proper permissions
RUN adduser --disabled-password --gecos '' appuser && \
    chown -R appuser:appuser /app && \
    chmod -R 755 /app
USER appuser

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Create directory for database
RUN mkdir -p /app/data

# Expose the port
EXPOSE 5096

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD dotnet --info > /dev/null || exit 1

# Start the application
ENTRYPOINT ["dotnet", "ProjectManagerAPI.dll"]
