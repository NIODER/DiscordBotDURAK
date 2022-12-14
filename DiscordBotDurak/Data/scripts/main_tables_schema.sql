
/* Drop Tables */

--DROP TABLE IF EXISTS BanwordsLists;
--DROP TABLE IF EXISTS Channel;
--DROP TABLE IF EXISTS Users;
--DROP TABLE IF EXISTS Guilds;




/* Create Tables */

-- Banwords
CREATE TABLE Banwords
(
	-- id : идентификатор запрещенного символа
	id bigserial NOT NULL UNIQUE,
	-- symbol : запрещённый символ
	symbol varchar NOT NULL,
	banwordlist_id bigserial NOT NULL,
	PRIMARY KEY (id)
) WITHOUT OIDS;

-- BanwordsLists
CREATE TABLE BanwordsLists
(
	-- channel_id : uint64 snowflake встроенный идентификатор discord.
	channel_id numeric(20) NOT NULL,
	-- banwordlist_id
	banwordlist_id bigserial NOT NULL
) WITHOUT OIDS;


-- Канал, чат
CREATE TABLE Channel
(
	-- channel_id : uint64 snowflake встроенный идентификатор discord.
	channel_id numeric(20) NOT NULL UNIQUE,
	-- guild_id : uint64 snowflake встроенный идентификатор discord.
	guild_id numeric(20) NOT NULL UNIQUE,
	-- banword_reaction : Соответствует перечислению:
	-- 0 - Ignore
	-- 1 - Delete
	-- 2 - Warn
	-- 3 - Resend
	banword_reaction int,
	-- resend_to_id : Идентификатор канала, куда будут пересылаться сообщения с запрещенными символами
	resend_to_id numeric(20),
	-- warning_message : Сообщение для предупреждения об использовании запрещенных символов.
	warning_message varchar DEFAULT 'Вы использовали запрещенный символ. Пожалуйста, воздержитесь от применения слов, содержащих запрещеные конструкции. Для уточнения списка запрещенных слов и символов обратитесь к администраторам сервера.',
	-- is_moderated : Показывает, модерируется ли канал ботом
	is_moderated boolean,
	PRIMARY KEY (channel_id)
) WITHOUT OIDS;


-- Сервера, гильдии
CREATE TABLE Guilds
(
	-- guild_id : uint64 snowflake встроенный идентификатор discord.
	id numeric(20) NOT NULL UNIQUE,
	-- spy_regime : 0 - просто сбор статистики
	-- 1 - предупреждения
	-- 2 - администрирование пользователей
	spy_regime int DEFAULT 0 NOT NULL,
	PRIMARY KEY (id)
) WITHOUT OIDS;

-- Users
CREATE TABLE Users
(
	-- user_id
	id numeric(20) NOT NULL UNIQUE,
	-- guild_id : uint64 snowflake встроенный идентификатор discord.
	guild_id numeric(20) NOT NULL,
	-- active_at
	active_at date NOT NULL,
	-- role : 0 - бан
	-- 1 - обычный
	-- 2 - модер
	-- 3 - админ
	-- 4 - владелец
	role int DEFAULT 1 NOT NULL,
	-- immunity
	immunity boolean DEFAULT 'false' NOT NULL,
	-- invited
	invited varchar DEFAULT 'unknown' NOT NULL,
	-- introduced
	introduced varchar DEFAULT 'unknown' NOT NULL,
	PRIMARY KEY (id, guild_id)
) WITHOUT OIDS;



/* Create Foreign Keys */

ALTER TABLE BanwordsLists
	ADD FOREIGN KEY (channel_id)
	REFERENCES Channel (channel_id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;

ALTER TABLE BanwordsLists
	ADD FOREIGN KEY (banwordlist_id)
	REFERENCES Banwords (banwordlist_id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;


ALTER TABLE Channel
	ADD FOREIGN KEY (guild_id)
	REFERENCES Guilds (id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;


ALTER TABLE Users
	ADD FOREIGN KEY (guild_id)
	REFERENCES Guilds (id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;


/* Comments */

COMMENT ON TABLE BanwordsLists IS 'BanwordsLists';
COMMENT ON COLUMN BanwordsLists.channel_id IS 'channel_id : uint64 snowflake встроенный идентификатор discord.';
COMMENT ON COLUMN BanwordsLists.symbol IS 'symbol';
COMMENT ON COLUMN BanwordsLists.banwordlist_id IS 'banwordlist_id';
COMMENT ON TABLE Channel IS 'Канал, чат';
COMMENT ON COLUMN Channel.channel_id IS 'channel_id : uint64 snowflake встроенный идентификатор discord.';
COMMENT ON COLUMN Channel.guild_id IS 'guild_id : uint64 snowflake встроенный идентификатор discord.';
COMMENT ON COLUMN Channel.banword_reaction IS 'banword_reaction : Соответствует перечислению:
0 - Ignore
1 - Delete
2 - Warn
3 - Resend';
COMMENT ON COLUMN Channel.resend_to_id IS 'resend_to_id : Идентификатор канала, куда будут пересылаться сообщения с запрещенными символами';
COMMENT ON COLUMN Channel.warning_message IS 'warning_message : Сообщение для предупреждения об использовании запрещенных символов.';
COMMENT ON COLUMN Channel.is_moderated IS 'is_moderated : Показывает, модерируется ли канал ботом';
COMMENT ON TABLE Guilds IS 'Сервера, гильдии';
COMMENT ON COLUMN Guilds.id IS 'guild_id : uint64 snowflake встроенный идентификатор discord.';
COMMENT ON COLUMN Guilds.spy_regime IS 'spy_regime : 0 - просто сбор статистики
1 - предупреждения
2 - администрирование пользователей';
COMMENT ON TABLE Users IS 'Users';
COMMENT ON COLUMN Users.id IS 'user_id';
COMMENT ON COLUMN Users.guild_id IS 'guild_id : uint64 snowflake встроенный идентификатор discord.';
COMMENT ON COLUMN Users.active_at IS 'active_at';
COMMENT ON COLUMN Users.role IS 'role : 0 - бан
1 - обычный
2 - модер
3 - админ
4 - владелец';
COMMENT ON COLUMN Users.immunity IS 'immunity';
COMMENT ON COLUMN Users.invited IS 'invited';
COMMENT ON COLUMN Users.introduced IS 'introduced';



