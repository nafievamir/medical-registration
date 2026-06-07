# \# 🏥 Медицинская регистратура

# 

# Интеграционная система для записи пациентов к врачам. Разработана на .NET 8 с использованием ASP.NET Core, SQLite, кэширования и CI/CD.

# 

# \---

# 

# \## 📋 Требования

# 

# \- \[.NET 8 SDK](https://dotnet.microsoft.com/download) (обязательно)

# \- Git (опционально)

# \- Браузер (Chrome/Firefox/Edge)

# 

# \---

# 

# \## 🚀 Быстрый запуск

# 

# 1\. Скачать проект

# git clone https://github.com/nafievamir/medical-registration.git

# cd medical-registration/MedicalWeb

# 

# 2\. Запустить

# dotnet run

# 

# 3\. Открыть в браузере

# http://localhost:5167

# 

# 4\. Swagger документация

# http://localhost:5167/swagger

# 

# \---

# 

# \## 🧪 Запуск тестов

# 

# cd MedicalWeb.Tests

# dotnet test

# 

# \---

# 

# \## 📁 Структура проекта

# 

# MedicalWeb/                 # Основной API

# ├── Program.cs              # Все эндпоинты (врачи, слоты, записи)

# ├── medical.db              # База данных SQLite (создаётся автоматически)

# └── wwwroot/index.html      # Веб-интерфейс

# 

# MedicalWeb.Tests/           # Тесты

# ├── UnitTests.cs            # Модульные тесты

# ├── IntegrationTests.cs     # Интеграционные тесты

# └── E2ETests.cs             # Сквозные тесты

# 

# \---

# 

# \## 🔧 API Эндпоинты

# 

# GET    /api/doctors          - Получить всех врачей

# POST   /api/doctors          - Добавить врача

# POST   /api/schedule/add     - Добавить слот расписания

# POST   /api/appointment/create - Создать запись

# 

# \---

# 

# \## 🛠 Технологии

# 

# \- .NET 8

# \- ASP.NET Core Minimal API

# \- SQLite + EF Core

# \- IDistributedMemoryCache (кэш)

# \- Polly (Retry)

# \- Serilog (логи)

# \- xUnit + WebApplicationFactory (тесты)

# \- GitHub Actions (CI/CD)

# 

# \---

# 

# \## 📊 Логи (пример)

# 

# info: GET /api/doctors (CACHE MISS) - 0 врачей, время 39 мс

# info: GET /api/doctors (CACHE HIT) - 0 врачей, время 4 мс

# warn: Повторная попытка 1 через 2 сек. Ошибка: 500

# 

# \---

# 

# \## ❓ Если ничего не работает

# 

# 1\. Убедись что установлен .NET 8:

# dotnet --version

# 

# 2\. Убедись что порт 5167 свободен:

# netstat -ano | findstr :5167

# 

# 3\. Если ошибка - напиши в Issues на GitHub

# 

# \---

# 

# \## 📝 Автор

# 

# Нафиев Амир

# 

# \## 🔗 Ссылки

# 

# GitHub: https://github.com/nafievamir/medical-registration

# Swagger: http://localhost:5167/swagger

