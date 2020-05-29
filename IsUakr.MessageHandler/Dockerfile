FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["IsUakr.MessageHandler/IsUakr.MessageHandler.csproj", "IsUakr.MessageHandler/"]
RUN dotnet restore "IsUakr.MessageHandler/IsUakr.MessageHandler.csproj"
COPY . .
WORKDIR "/src/IsUakr.MessageHandler"
RUN dotnet build "IsUakr.MessageHandler.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IsUakr.MessageHandler.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IsUakr.MessageHandler.dll"]