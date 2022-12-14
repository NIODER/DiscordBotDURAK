
/* Drop Tables */

DROP TABLE IF EXISTS SymbolsToLists;
DROP TABLE IF EXISTS BanSymbols;
DROP TABLE IF EXISTS ListsToChannels;
DROP TABLE IF EXISTS BanwordsLists;
DROP TABLE IF EXISTS BotGuildChannels;
DROP TABLE IF EXISTS BotGuildUsers;
DROP TABLE IF EXISTS BotGuilds;




/* Create Tables */

CREATE TABLE BanSymbols
(
	id bigserial NOT NULL UNIQUE,
	exclude boolean NOT NULL,
	symbol text NOT NULL,
	PRIMARY KEY (id)
) WITHOUT OIDS;


CREATE TABLE BanwordsLists
(
	banword_list_id bigserial NOT NULL,
	title varchar(32) DEFAULT '''Untitled''' NOT NULL,
	PRIMARY KEY (banword_list_id)
) WITHOUT OIDS;


CREATE TABLE BotGuildChannels
(
	channel_id numeric(20) NOT NULL UNIQUE,
	guild_id numeric(20) NOT NULL,
	-- 0 - non-modetated
	-- 1 - only warnings
	-- 2 - only resend
	-- 3 - only delete
	moderation smallint DEFAULT 0 NOT NULL,
	resend_channel numeric(20),
	warning text DEFAULT '''banword detected''' NOT NULL,
	is_modearated boolean DEFAULT 'FALSE' NOT NULL,
	PRIMARY KEY (channel_id)
) WITHOUT OIDS;


CREATE TABLE BotGuilds
(
	id numeric(20) NOT NULL UNIQUE,
	-- 0 - stats
	-- 1 - warnings
	-- 2 - kicks
	spy_mode smallint DEFAULT 0 NOT NULL,
	base_role numeric(20) DEFAULT 0 NOT NULL,
	immunity_role numeric(20) DEFAULT 0 NOT NULL,
	PRIMARY KEY (id)
) WITHOUT OIDS;


CREATE TABLE BotGuildUsers
(
	id numeric(20) NOT NULL,
	guild_id numeric(20) NOT NULL,
	active date DEFAULT now() NOT NULL,
	-- 0 - ban
	-- 1 - user
	-- 2 - moderator
	-- 3 - admin
	-- 4 - owner
	role smallint DEFAULT 1 NOT NULL,
	immunity boolean DEFAULT 'FALSE' NOT NULL,
	invited text DEFAULT '''empty''' NOT NULL,
	introduced text DEFAULT '''empty''' NOT NULL,
	PRIMARY KEY (id, guild_id)
) WITHOUT OIDS;


CREATE TABLE ListsToChannels
(
	banword_list_id bigint NOT NULL,
	channel_id numeric(20) NOT NULL UNIQUE
) WITHOUT OIDS;


CREATE TABLE SymbolsToLists
(
	id bigint NOT NULL UNIQUE,
	banword_list_id bigint NOT NULL
) WITHOUT OIDS;



/* Create Foreign Keys */

ALTER TABLE SymbolsToLists
	ADD FOREIGN KEY (id)
	REFERENCES BanSymbols (id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;


ALTER TABLE ListsToChannels
	ADD FOREIGN KEY (banword_list_id)
	REFERENCES BanwordsLists (banword_list_id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;


ALTER TABLE SymbolsToLists
	ADD FOREIGN KEY (banword_list_id)
	REFERENCES BanwordsLists (banword_list_id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;


ALTER TABLE ListsToChannels
	ADD FOREIGN KEY (channel_id)
	REFERENCES BotGuildChannels (channel_id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;


--ALTER TABLE BanwordsLists
--	ADD FOREIGN KEY (guild_id)
--	REFERENCES BotGuilds (id)
--	ON UPDATE CASCADE
--	ON DELETE CASCADE
--;


ALTER TABLE BotGuildChannels
	ADD FOREIGN KEY (guild_id)
	REFERENCES BotGuilds (id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;


ALTER TABLE BotGuildUsers
	ADD FOREIGN KEY (guild_id)
	REFERENCES BotGuilds (id)
	ON UPDATE CASCADE
	ON DELETE CASCADE
;



