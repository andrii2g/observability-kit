FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ObservabilityKit.Example.Api.csproj ./
RUN dotnet restore ObservabilityKit.Example.Api.csproj

COPY . ./
RUN dotnet publish ObservabilityKit.Example.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "ObservabilityKit.Example.Api.dll"]
