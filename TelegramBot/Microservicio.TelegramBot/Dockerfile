FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TelegramBot/Microservicio.TelegramBot/Microservicio.TelegramBot.csproj", "Microservicio.TelegramBot/"]
RUN dotnet restore "./Microservicio.TelegramBot/Microservicio.TelegramBot.csproj"
COPY TelegramBot/Microservicio.TelegramBot/. ./Microservicio.TelegramBot/
WORKDIR "/src/Microservicio.TelegramBot"
RUN dotnet publish "./Microservicio.TelegramBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Microservicio.TelegramBot.dll"]