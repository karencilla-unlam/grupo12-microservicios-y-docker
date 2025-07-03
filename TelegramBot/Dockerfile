# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos solución y proyectos
COPY TelegramBot.sln ./
COPY TelegramBot.Web/ TelegramBot.Web/
COPY TelegramBot.Logic/ TelegramBot.Logic/
COPY TelegramBot.Entities/ TelegramBot.Entities/

# Restaurar dependencias
RUN dotnet restore

# Build
RUN dotnet publish TelegramBot.Web/TelegramBot.Web.csproj -c Release -o /app/publish

# Imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TelegramBot.Web.dll"]
