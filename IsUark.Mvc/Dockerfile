FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["IsUark.Mvc/IsUark.Mvc.csproj", "IsUark.Mvc/"]
COPY ["IsUakr.DAL/IsUakr.DAL.csproj", "IsUakr.DAL/"]
COPY ["IsUakr.MessageBroker/IsUakr.MessageBroker.csproj", "IsUakr.MessageBroker/"]
COPY ["IsUakr.Entities/IsUakr.Entities.csproj", "IsUakr.Entities/"]
RUN dotnet restore "IsUark.Mvc/IsUark.Mvc.csproj"
COPY . .
WORKDIR "/src/IsUark.Mvc"
RUN dotnet build "IsUark.Mvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IsUark.Mvc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IsUakr.Mvc.dll"]