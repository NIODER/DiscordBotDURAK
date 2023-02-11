# DURAK
## Вступление
DURAK - многофункциональный дискорд бот для помощи в администрировании Discord гильдий.

### Бот умеет:
1.  Выдавать базовую роль новым участникам.
2.  Модерировать текстовые каналы (Удалять, пересылать и отправлять предупреждения, содержащие [запрещенные символы](#Symbol).)
3.  Администрировать [неактивных пользователей](#активность-пользователей) 
4.  [Слэш-команды](#слэш-команды)

## Термины

<a name="Symbol"></a>**Запрещенный символ** - структура, состоящая из символа (строки), на содержание которой проверяются входящие сообщения, и флага исключения (Если флаг исключения (excluded) включен, то при нахождении указанного символа модерация сообщения прекращается.)

***Пример***

Модерация ссылок как раз использует exluded-флаг. Мы хотим, чтобы любые сообщения, содержащие ссылки (кроме ссылко на [Youtube](https://www.youtube.com/)), удалялись. Тогда мы создаем [список](#Lists) в чате, в который добавляем символ "http", с exlcuded-флагом false. Теперь сообщения, содержащие любые ссылки, будут модерироваться, в том числе и ссылки на youtube. Чтобы ссылки на youtube не модерировались, нужно добавить в список символ "youtube" с excluded-флагом true. Теперь сообщения, содержащие подстроку "youtube" модерироваться не будут.

---

<a name="Lists"></a>**Списки запрещенных символов** - список [запрещенных символов](#Symbol), для которого можно настроить [режим модерации](#режимы-модерации), канал для пересылки сообщений и название. Списки общие для нескольких гильдий и каналов, при изменении списка, фигурирующего в нескольких гильдиях, для вашей гильдии создается новый, он и изменяется. Доступ к списку можно получить через его id. Посмотреть все списки в гильдии и канале можно с помощью команды. Список можно добавить в гильдию, но не добавлять в каналы. Посмотреть содержащиеся в списке символы можно используя команду [list](#list-scope-list-id-title-moderation-resend-channel)

---

<a name="BaseRole"></a>**Базовая роль** - роль, выдающаяся всем новым участникам по умолчанию.

## Выдача ролей

[Базовая роль](#BaseRole) выдаётся ботом при условии, что новый пользователь ответит на два вопроса, бот попросит новичка представиться и сказать, кем он был приглашен на сервер. Эти данные нужны для аутентификации пользователя, их можно просмотреть, используя команду [info](#info).

## Режимы модерации

Есть 4 режима модерации:

- NonModerated (Бот игнорирует сообщения, модерация отключена)
- Warnings (Бот присылает заданное сообщение в чат, где был зафиксирован запрещенный символ)
- Resend (Бот перешлет сообщение с запрещенным символом в указаный канал)
- Delete (Бот удалит сообщение с запрещенным символом)

## Активность пользователей

Бот сохраняет время последней активности пользователя. Пользователи, которые были активны более полугода назад. <a name="Spy"></a>Этим занимается так называемый "Шпион (Spy)".

Любому пользователю можно выдать [иммунитет](#set-immunity-user-enable-immunity) от работы этой функции (Пользователь не будет исключен и сообщение о его неактивности не будет послано.)

Активностью считается одно из следующих действий:

- Вход/выход в голосовой канал
- Отключение/включение микрофона/звука
- Отправка сообщений в текстовый канал
- Использование слэш команд бота

### Есть 3 вида модерации пользователей:

#### CollectInfo

Бот собирает информацию об активности пользователя, просмотреть её можно используя слэш-команду [info](#info)

#### SendTips

Бот собирает информацию об активности пользователя, и если пользователь признается неактивным, бот делает рассылку пользователям, указаных в [mailing списке](#рассылка)

#### DeleteUsers

Бот собирает информацию об активности пользователя, и если пользователь признается неактивным, бот кикает пользователя.

## Рассылка

Список пользователей (обычно администраторов), которым будут присылаться сообщения, уведомлящие о том, что некоторые пользователи неактивны. По-умолчанию в список вносится владелец гильдии.

Изменить список можно используя команду [mailing](#mailing-action-user)

# Слэш-команды

Бот имеет 4 роли для пользователей бота:

- Владелец сервера
- Админ
- Модератор
- Пользователь
- Бан

Вышестоящим ролям доступны слэш-команды всех нижестоящих ролей. Владелец может исполльзовать все команды, Бан - никакие.

## Команды, доступные пользователям

### help

Справка

### random-number

Рандомайдер, выдает случайное число в указанных пределах.

### random-user mentions count

Рандомайзер, случайным образом выбириает одного или count пользователей, упомянутых в mentions через запятую. В mentions можно указать не только пользователей через @, но и текстовые каналы через #, и голосовые через #!. При упоминании канала в список упоминаний включаются все пользователи, которым доступен указанный канал. 

### random-decide 

Рандомайзер, выводит "yes" или "no" случайным образом.

### random-distibute mentions teams-count teams-size

Рандомайзер, случайным образом распределяет n пользователей по teams-count командам, в каждой из которой может быть teams-size участников.

### csgo

Позволяет получить ip адрес сервера для Counter-Strike: Global Offensive с сайта [cybershoke](https://cybershoke.net/).

## Команды, доступные модераторам

### stop

Останавливает выполнение длящихся команд, таких как: [delete](#delete), [spam](#spam).

Если пользователь, использовавший команду имеет роль Админ или выше, то команда остановит выполнение всех длящихся команд для этого канала. Если пользователь имеет роль Модератор, остановится только выполнение того, что он и запустил.

### delete

Удаляет все сообщения в канале, по авторству или по содержанию.

### spam message count

Отправляет message count раз.

### info

Выводит информацию об гильдии, канале, пользователе или списке.

### moderate count

Заново модерирует последние count сообщений.

## Команды, доступные администраторам

### set-privelege user privelege

Устанавливает роль privelege пользователю user.

### set-immunity user enable-immunity

Включает или выключает иммунитет от "[Шпион](#Spy)". 

### set-moderation list-id moderation

Устанавливает moderation для списка с id list-id для этого канала.

### get-lists 

Возвращает списки, связанные с этим каналом или гильдией.

### list scope list-id title moderation resend-channel

В зависимости от того, указан ли list-id функция будет либо создавать новый список, либо редактировать существующий. **Если кроме list-id никакие параметры более не указаны, выводится информация о списке, в том числе содержащиеся в нем символы.**

Если параметр не указан, будет присвоено значение по-умолчанию:

- moderation - NonModerated
- title - "untitled"
- resend-channel - null, если указан moderation=Resend, параметр resend-channel становится необходимым.

### add-symbol content excluded list(id)

Добавляет в список list символ с excluded-флагом excluded и символом content. см. [Запрещенный символ](#Symbol).

### remove-list scope lists

Удаляет указанный list из гильдии или из канала. При удалении из гильдии, список удалится из всех каналов. 

### remove-symbols symbol-id lists

Удаляет указанный символ из указанных списков.

### warning-message message

Устанавливает текст предупреждения для данного канала. Этот текст будет отправлять бот, если указан moderation=Warnings для этого канала.

### base-role

Устанавливает базовую роль для гильдии. (Эту роль будет выдавать бот новым пользователям).

### spy-mode mode

Устанавливает режим работы "Шпиона". см. [Шпион](#Spy)

### mailing action user

Включает или исключает user из mailing-list.

При указании action=get-mailing-list указывать user не обязательно.

