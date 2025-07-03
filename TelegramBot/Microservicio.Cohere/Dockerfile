FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TelegramBot/Microservicio.Cohere/Microservicio.Cohere.csproj", "Microservicio.Cohere/"]
RUN dotnet restore "./Microservicio.Cohere/Microservicio.Cohere.csproj"
COPY TelegramBot/Microservicio.Cohere/. ./Microservicio.Cohere/
WORKDIR "/src/Microservicio.Cohere"
RUN dotnet publish "./Microservicio.Cohere.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Microservicio.Cohere.dll"]