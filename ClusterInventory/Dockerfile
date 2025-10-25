# RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
# Escuchar en 8080 dentro del contenedor
ENV ASPNETCORE_URLS=http://+:8080
# Si esta línea te rompe permisos, comenta USER
# USER $APP_UID

# BUILD
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 👇 OJO: el csproj está en la raíz del contexto
COPY ["ClusterInventory.csproj", "."]
RUN dotnet restore "ClusterInventory.csproj"

# Copia el resto del código
COPY . .
RUN dotnet build "ClusterInventory.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ClusterInventory.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FINAL
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClusterInventory.dll"]
