FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CryptoTrader/CryptoTrader.csproj", "CryptoTrader/"]
RUN dotnet restore "CryptoTrader/CryptoTrader.csproj"
COPY . .
WORKDIR "/src/CryptoTrader"
RUN dotnet build "CryptoTrader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CryptoTrader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoTrader.dll"]
