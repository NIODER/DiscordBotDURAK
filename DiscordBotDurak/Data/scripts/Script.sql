INSERT INTO botguilds (id, spy_mode) VALUES (1, 1);
INSERT INTO botguilds (id, spy_mode) VALUES (2, 0);
INSERT INTO botguilds (id, spy_mode) VALUES (3, 2);
-- INSERT INTO botguilds (id, spy_mode) VALUES (4, 1);
-- INSERT INTO botguilds (id, spy_mode) VALUES (5, 0);

INSERT INTO botguildchannels (
	channel_id,
	guild_id,
	moderation,
	resend_channel,
	warning,
	is_modearated
) VALUES (
	11,
	1,
	1,
	12,
	'abobg',
	false
);
INSERT INTO botguildchannels (
	channel_id,
	guild_id,
	moderation,
	resend_channel,
	warning,
	is_modearated
) VALUES (
	12,
	1,
	1,
	12,
	'asdfdasf',
	true
);
INSERT INTO botguildchannels (
	channel_id,
	guild_id,
	moderation,
	resend_channel,
	warning,
	is_modearated
) VALUES (
	21,
	2,
	1,
	12,
	'yujtyj',
	false
);
INSERT INTO botguildchannels (
	channel_id,
	guild_id,
	moderation,
	resend_channel,
	warning,
	is_modearated
) VALUES (
	31,
	3,
	1,
	12,
	't78i878t6i',
	false
);

INSERT INTO botguildusers (
	id,
	guild_id
) VALUES (
	111,
	1
);
INSERT INTO botguildusers (
	id,
	guild_id
) VALUES (
	221,
	2
);
INSERT INTO botguildusers (
	id,
	guild_id
) VALUES (
	112,
	1
);
INSERT INTO botguildusers (
	id,
	guild_id
) VALUES (
	331,
	3
);
INSERT INTO botguildusers (
	id,
	guild_id
) VALUES (
	222,
	2
);
