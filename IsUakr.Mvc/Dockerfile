FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["IsUakr.Mvc/IsUakr.Mvc.csproj", "IsUakr.Mvc/"]
RUN dotnet restore "IsUakr.Mvc/IsUakr.Mvc.csproj"
COPY . .
WORKDIR "/src/IsUakr.Mvc"
RUN dotnet build "IsUakr.Mvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IsUakr.Mvc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IsUakr.Mvc.dll"]