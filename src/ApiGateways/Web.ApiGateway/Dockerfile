﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ApiGateways/Web.ApiGateway/Web.ApiGateway.csproj", "src/ApiGateways/Web.ApiGateway/"]
RUN dotnet restore "src/ApiGateways/Web.ApiGateway/Web.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/ApiGateways/Web.ApiGateway"
RUN dotnet build "Web.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.ApiGateway.dll"]
