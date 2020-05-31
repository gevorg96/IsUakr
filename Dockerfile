FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build-env
WORKDIR /app

COPY *.Mvc/*.Mvc.csproj ./
RUN dotnet restore *.Mvc.csproj

COPY . ./
RUN dotnet publish *.Mvc/*.Mvc.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=build-env /app/out .
CMD dotnet IsUark.Mvc.dll