using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Discord.WebSocket;

namespace DiscordBotDURAK
{
    public static class MyDatabase
    {
        private static readonly string connectionString = @"Data Source=DESKTOP-OVCM484\SQLEXPRESS;Initial Catalog=DiscordBotDURAKDataBase;Integrated Security=True";
        public static void AddGuild(string guildId)
        {
            string createSchema = $"CREATE SCHEMA [{guildId}]";
            string sqlExpression =
                $"INSERT INTO [DiscordBotDURAKDataBase].[dbo].[Guilds] (guild) VALUES ('{guildId}') " +
                $"CREATE TABLE [{guildId}].[Admins] (Admin NVARCHAR(50) NOT NULL) " +
                $"CREATE TABLE [{guildId}].[Channels] (Channel NVARCHAR(50) NOT NULL, Type NVARCHAR(50) NOT NULL) " +
                $"CREATE TABLE [{guildId}].[Radio] (Reference NVARCHAR(50) NOT NULL) ";
            if (!inDatabase(guildId))
            {
                using (SqlConnection connection = new(connectionString))
                {
                    SqlCommand schema = new(createSchema, connection);
                    SqlCommand command = new(sqlExpression, connection);
                    connection.Open();
                    try
                    {
                        schema.ExecuteNonQuery();
                    }
                    catch (System.Data.SqlClient.SqlException) { }
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void AddAdmin(string guildId, ulong adminId)
        {
            string sqlExpression =
                $"INSERT INTO [DiscordBotDURAKDataBase].[{guildId}].[Admins] (Admin) VALUES ('{Convert.ToString(adminId)}');";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static void AddChannel(string guildId, ulong channelId)
        {
            string sqlExpression =
                $"INSERT INTO [DiscordBotDURAKDataBase].[{guildId}].[Channels] (Channel, Type) VALUES ('{Convert.ToString(channelId)}', '{ChannelSeverity.Flood}');";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static void AddRadio(string guildId, string radio)
        {
            string sqlExpression =
                $"INSERT INTO [DiscordBotDURAKDataBase].[{guildId}].[Radio] (Reference) VALUES ('{radio}');";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static void UpdateChannelType(string guildId, ulong channelId, ChannelSeverity severity)
        {
            string sqlExpession =
                $"UPDATE [DiscordBotDURAKDataBase].[{guildId}].[Channels] " +
                $"SET Type = '{severity}' WHERE (Channel = '{Convert.ToString(channelId)}');";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpession, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static void DeleteAdmin(string guildId, ulong adminId)
        {
            string sqlExpression =
                $"DELETE FROM [DiscordBotDURAKDataBase].[{guildId}].[Admins] WHERE (Admin = '{Convert.ToString(adminId)}');";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static bool isAdmin(this SocketUser user, ulong guildId)
        {
            string sqlExpression =
                $"SELECT Admin FROM [DiscordBotDURAKDataBase].[{guildId}].[Admins] WHERE (Admin = '{Convert.ToString(user.Id)}');";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                return reader.HasRows;
            }
        }
        public static void DeleteGuild(string guildId)
        {
            string sqlExpression =
                $"DROP TABLE [DiscordBotDURAKDataBase].[{guildId}].[Admins]; " +
                $"DROP TABLE [DiscordBotDURAKDataBase].[{guildId}].[Channels]; " +
                $"DROP TABLE [DiscordBotDURAKDataBase].[{guildId}].[Radio]; " +
                $"DROP SCHEMA [{guildId}]; " +
                $"DELETE FROM [DiscordBotDURAKDataBase].[dbo].[Guilds] WHERE (guild = '{guildId}');";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static IEnumerable<ulong> GetGuilds()
        {
            List<ulong> guilds = new();
            string sqlExpression =
                $"SELECT * FROM [DiscordBotDURAKDataBase].[dbo].[Guilds]";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    guilds.Add(Convert.ToUInt64(reader.GetString(0)));
                }
            }
            return guilds;
        }
        public static bool inDatabase(string guild)
        {
            string sqlExpression =
                $"SELECT * FROM [DiscordBotDURAKDataBase].[dbo].[Guilds] WHERE guild = '{guild}'";
            var connection = new SqlConnection(connectionString);
            connection.Open();
            var reader = new SqlCommand(sqlExpression, connection).ExecuteReader();
            return reader.HasRows;
        }
        public static IEnumerable<ulong> GetAdmins(string guildId)
        {
            List<ulong> admins = new();
            string sqlExpression =
                $"SELECT Admin FROM [DiscordBotDURAKDataBase].[{guildId}].[Admins];";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    admins.Add(Convert.ToUInt64(reader.GetString(0)));
                }
            }
            return admins;
        }
        public static ulong GetReferencesChannel(string guildId)
        {
            string sqlExpression =
                $"SELECT Channel FROM [DiscordBotDURAKDataBase].[{guildId}].[Channels] WHERE (Type = '{ChannelSeverity.References}')";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        return Convert.ToUInt64(reader.GetString(0));
                    }
                }
                return 0;
            }
        }
        public static ChannelSeverity ChannelType(this SocketChannel channel)
        {
            string sqlExpression =
                $"SELECT Type FROM [DiscordBotDURAKDataBase].[{((SocketGuildChannel)channel).Guild.Id}].[Channels] WHERE Channel = '{channel.Id}'";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    if (Enum.TryParse<ChannelSeverity>(reader.GetString(0), out var result))
                    {
                        return result;
                    }
                    else
                    {
                        throw new Exception("wrong channel type");
                    }
                }
                else
                {
                    return ChannelSeverity.NoSuchChannel;
                }
            }
        }
        public static ChannelSeverity ChannelType(this ISocketMessageChannel channel)
        {
            string sqlExpression =
                $"SELECT Type FROM [DiscordBotDURAKDataBase].[{((SocketGuildChannel)channel).Guild.Id}].[Channels] WHERE Channel = '{channel.Id}'";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                SqlDataReader reader;
                try
                {
                    reader = command.ExecuteReader();
                }
                catch (Exception)
                {
                    return ChannelSeverity.NoSuchChannel;
                }
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (Enum.TryParse<ChannelSeverity>(reader.GetString(0), out var result))
                        {
                            return result;
                        }
                        else
                        {
                            throw new Exception("wrong channel type");
                        }
                    }
                }
                return ChannelSeverity.NoSuchChannel;
            }
        }
        public static IEnumerable<string> GetRadio(string guildId)
        {
            List<string> radio = new();
            string sqlExpression =
                $"SELECT Reference FROM [DiscordBotDURAKDataBase].[{guildId}].[Radio]";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        radio.Add(reader.GetString(0));
                    }
                    return radio;
                }
                else
                {
                    return null;
                }
            }
        }
        public static void DeleteRadio(string guildId, string reference)
        {
            string sqlExpression =
                $"DELETE FROM [DiscordBotDURAKDataBase].[{guildId}].[Radio] WHERE Reference = '{reference}'";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="guild"></param>
        /// <returns>true if guild in database</returns>
        public static bool Check(this SocketGuild guild)
        {
            string sqlExpression =
                $"SELECT guild FROM [DiscordBotDURAKDataBase].[dbo].[Guilds] WHERE guild = '{guild.Id}'";
            using (SqlConnection connection = new(connectionString))
            {
                SqlCommand command = new(sqlExpression, connection);
                connection.Open();
                var reader = command.ExecuteReader();
                return reader.HasRows;
            }
        }
    }
}
