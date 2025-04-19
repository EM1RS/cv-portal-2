# === BYGGESTEG ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
#RUN dotnet publish -c Release -o /app/publish
RUN dotnet publish CvApi2.csproj -c Release -o /app/publish

# === RUNTIME ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Kopier alt som ble publisert, inkl. libwkhtmltox.so
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CvApi2.dll"]
