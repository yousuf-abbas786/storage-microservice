FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["StorageService.API/StorageService.API.csproj", "StorageService.API/"]
COPY ["StorageService.Applicatiom/StorageService.Application.csproj", "StorageService.Applicatiom/"]
COPY ["StorageService.Domain/StorageService.Domain.csproj", "StorageService.Domain/"]
COPY ["StorageService.Infrastructure/StorageService.Infrastructure.csproj", "StorageService.Infrastructure/"]
RUN dotnet restore "StorageService.API/StorageService.API.csproj"
COPY . .
WORKDIR "/src/StorageService.API"
RUN dotnet build "StorageService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StorageService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StorageService.API.dll"]

