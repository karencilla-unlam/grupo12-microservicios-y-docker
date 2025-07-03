FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TelegramBot/TelegramBot.Web/TelegramBot.Web.csproj", "TelegramBot.Web/"]
COPY ["TelegramBot/TelegramBot.Logica/TelegramBot.Logica.csproj", "TelegramBot.Logica/"]
COPY ["TelegramBot/TelegramBot.Entidades/TelegramBot.Data.csproj", "TelegramBot.Entidades/"]

RUN dotnet restore "TelegramBot.Web/TelegramBot.Web.csproj"

COPY TelegramBot/TelegramBot.Web/. ./TelegramBot.Web/
COPY TelegramBot/TelegramBot.Logica/. ./TelegramBot.Logica/
COPY TelegramBot/TelegramBot.Entidades/. ./TelegramBot.Entidades/

WORKDIR "/src/TelegramBot.Web"
RUN dotnet publish "TelegramBot.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TelegramBot.Web.dll"]