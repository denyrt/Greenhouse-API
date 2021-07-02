# Greenhouse.
AspNetCore backend.

## Описание
Бекенд приложения для управления теплицами, сбора температурных данных, влажности и освещения.

## Стек технологий
Технологии используемые в данном проекте:
* ASP.NET Core WEB API;
* Microsoft SQL Server (EF Core);
* Sendgrid API (url: https://sendgrid.com/) для максимально простой отправки писем на почту.

## Немного о проекте
В основном проект использует REST архитектуру.
Контроллеры предоставляют основной CRUD функционал для клиента.
В проекте используется самая простая JWT аутентификация.
Все запросы, кроме авторизации и регистрации предоставляются только для авторизированных пользователей.
Пользователи не прошедшие аутентификацию будут получать 401 код ответата.
Так же аутентификация позволяет пользователю получать именно свои данные, которые создал именно он (теплицы, контроллеры).

Возьмем например GreenhousesController с роутом _api/greenhouses_.
Имеем следущие endpoints:

_**POST** /api/greenhouses_ \
Запрос для создания сущности теплицы.

_**GET** /api/greenhouses_ \
Запрос для получения своих списка теплиц.

_**GET** /api/greenhouses/{greenhouseUUID}_ \
Запрос для получения по айди контретной теплицы.

_**DELETE** /api/greenhouses/{greenhouseUUID}_ \
Удаление контретной теплицы.

_**PUT** /api/greenhouses/{greenhouseUUID}_ \
Обновление теплицы.

Весь список API controllers и endpoints: [some link].
