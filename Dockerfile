# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build

# Копируем только файлы проектов для кэширования слоев
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Восстанавливаем зависимости
RUN dotnet restore "Application/Application.csproj"

# Копируем весь исходный код
COPY . .

# Собираем и публикуем
RUN dotnet publish "Application/Application.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Копируем собранное приложение
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Application.dll"]