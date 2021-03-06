﻿Проект представляет из себя простейшую распределенную систему хранения кэша

Кэш представлен в виде пары ключ-значение (написан универсальный тип, однако для простоты используется
хранилище Storage<string,string>)

Экземпляр приложения представляет собой:
1. Сервер, хранящий данные кэша и предоставляющий WebAPI для работы с этим кэшем
    - GET /data - получить список всех собственных ключей сервера
    - GET /data/{key} - получить значение по ключу 
    - POST /data/{key} [body]value - добавить пару ключ-значение. Ограничение - при наличии ключа его значение просто обновляется
    - DELETE /data/{key} - удалить пару ключ-значение
Основной контроллер - DataController
2. Сервер (NotificationHub, Broadcast) - прослушивающий сервер для уведомления об изменениях в хранилище
3. Клиенты (NotificationClient - от 0 объектов) - подключения к другим серверам в сети для уведомления их об изменениях в хранилище

При запуске сканируется локальная сеть, устанавливаются клиентские (NotificationClient) подключения к уже запущенным серверам.
Эти сервера при коннекте отправляют свои списки ключей, которые сохраняются в локальном хранилище чужих ключей.
Разворачивается хаб для соединения с новыми клиентами. Таким образом устанавливаются соединения в сети "все-со-всеми".
Допущения:
- TODO: реконнект при сбоях не реализован, однако его можно легко дореализовать (при успешном реконнекте еще раз 
обменяться ключами)
- TODO: при дисконнекте (клиент или сервер нормально завершает работу) можно отправить всем сообщения "удалите все мои ключи"
- добавление ключа-значения всегда происходит на точке обращения. TODO: можно реализовать сценарий
типа "посмотреть у других и обновить, если есть". В текущей реализации может возникнуть коллизия (зацикливание).
- ключ всегда сначала ищется в своем хранилище (данное хранилище приоритетнее)
- при запросе/удалении ключа, не найденного в своем хранилище, смотрится хранилище чужих ключей, выполняется обычный
запрос к WebApi, клиенту возвращается его результат

Приложение работает при запуске от администратора - консольное окно с небольшим логированием.
Приложение разворачивает серверы на http://localIP:5000, порт можно изменить в App.config.
Если извне апи недоступно, в настройках брандмауэра необходимо разрешить подключения на указанный порт.

Чтобы установить и запустить как службу, требуется скопировать в папку с exe и запустить от имени 
администратора InstallAndStart.bat (для удаления аналогично для Uninstall.bat).

Пример работающего приложения
![Image of working](https://sun1-89.userapi.com/WoVkCfsV1G4eCvct2AMkCziKA2UAZLquFoTeUw/97UzVB3CsHg.jpg)