FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["IsUakr.HighLoad.Ui/IsUakr.HighLoad.Ui.csproj", "IsUakr.HighLoad.Ui/"]
COPY ["IsUakr.DAL/IsUakr.DAL.csproj", "IsUakr.DAL/"]
COPY ["IsUakr.Entities/IsUakr.Entities.csproj", "IsUakr.Entities/"]
RUN dotnet restore "IsUakr.HighLoad.Ui/IsUakr.HighLoad.Ui.csproj"
COPY . .
WORKDIR "/src/IsUakr.HighLoad.Ui"
RUN dotnet build "IsUakr.HighLoad.Ui.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IsUakr.HighLoad.Ui.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IsUakr.HighLoad.Ui.dll"]