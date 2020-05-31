FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.Mvc/*.Mvc.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish *.Mvc/*.Mvc.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
WORKDIR /app
COPY --from=build-env /app/out .
CMD dotnet IsUark.Mvc.dll