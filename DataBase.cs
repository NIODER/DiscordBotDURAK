using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace DiscordBotDURAK
{
    static class DataBase
    {
        private static string connectionString = @"Data Source=DESKTOP-OVCM484\SQLEXPRESS;Initial Catalog=DiscordBotDURAKDataBase;Integrated Security=True";
        /// <summary>
        /// Добавляет id админа в БД, если его там еще нет.
        /// </summary>
        /// <param name="guildId">Id of guild</param>
        /// <param name="adminId">Id of admin</param>
        public static void AddAdminIDB(ulong ulongGuildId, ulong ulongAdminId)
        {
            string guildId = Convert.ToString(ulongGuildId);
            string adminId = Convert.ToString(ulongAdminId);
            string sqlExpression = $"INSERT INTO [DiscordBotDURAKDataBase].[dbo].[IdTable] ([GuildId], [Admins]) VALUES ('{guildId}', '{adminId}')";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (!SearchAdminIDB(guildId, adminId))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }
            }
            
        }
        /// <summary>
        /// Get all admins from DB
        /// </summary>
        /// <param name="guildId">Id of guild</param>
        /// <returns>IEnumerable<string> adminsId</returns>
        public static IEnumerable<string> GetAdminsFDB(ulong ulongguildId)
        {
            string guildId = Convert.ToString(ulongguildId);
            string admins = "";
            string sqlExpression = $"SELECT Admins FROM [DiscordBotDURAKDataBase].[dbo].[IdTable] WHERE GuildId = '{guildId}'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        admins = admins + reader.GetString(0) + " ";
                    }
                }
                
            }
            admins = admins.Trim();
            if (admins == "")
            {
                return null;
            }
            else
            {
                IEnumerable<string> adminsId = admins.Split(' ');
                return adminsId;
            }
        }
        /// <summary>
        /// Get all admins from DB
        /// </summary>
        /// <param name="guildId">Id of guild</param>
        /// <returns>IEnumerable<string> adminsId</returns>
        public static IEnumerable<string> GetAdminsFDB(string guildId)
        {
            string admins = "";
            string sqlExpression = $"SELECT Admins FROM [DiscordBotDURAKDataBase].[dbo].[IdTable] WHERE GuildId = '{guildId}'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        admins += reader.GetString(0) + " ";
                    }
                }

            }
            admins = admins.Trim();
            if (admins == "")
            {
                return null;
            }
            else
            {
                IEnumerable<string> adminsId = admins.Split(' ');
                return adminsId;
            }
        }
        /// <summary>
        /// Search admin in DB
        /// </summary>
        /// <param name="guildId">Id of the guild</param>
        /// <param name="adminId">Id of admin</param>
        /// <returns>true if admin was found in DB</returns>
        private static bool SearchAdminIDB(string guildId, string adminId) => GetAdminsFDB(guildId)?.Contains(adminId) ?? false;
        /// <summary>
        /// Search guild in DB
        /// </summary>
        /// <param name="ulongguildId">Id of guild</param>
        /// <returns>true if guild was found in DB</returns>
        public static bool SearchGuildIDB(ulong ulongguildId)
        {
            string guildId = Convert.ToString(ulongguildId);
            string sqlExpression = $"SELECT GuildId FROM [DiscordBotDURAKDataBase].[dbo].[Channels] WHERE GuildId = '{guildId}'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Adds a channel id DB
        /// </summary>
        /// <param name="ulongGuildID">guild id</param>
        /// <param name="ulongChannelID">channel id</param>
        /// <param name="severity">channel severity</param>
        public static void AddChannelInDB(ulong ulongGuildID, ulong ulongChannelID, ChannelSeverity severity)
        {
            string channelId = Convert.ToString(ulongChannelID);
            string guildId = Convert.ToString(ulongGuildID);
            string sqlExpression =
                $"INSERT INTO [DiscordBotDURAKDataBase].[dbo].[Channels] " +
                $"(GuildId, ChannelId, [Type]) VALUES ('{guildId}', '{channelId}', '{severity}')";
            using (SqlConnection connection = new(connectionString))
            {
                if (!SearchChannelInDB(ulongChannelID))
                {
                    connection.Open();
                    SqlCommand command = new(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Adds a channel id DB with flood type
        /// </summary>
        /// <param name="ulongGuildID">guild id</param>
        /// <param name="ulongChannelID">channel id</param>
        public static void AddChannelInDB(ulong ulongGuildID, ulong ulongChannelID)
        {
            string channelId = Convert.ToString(ulongChannelID);
            string guildId = Convert.ToString(ulongGuildID);
            string sqlExpression =
                $"INSERT INTO [DiscordBotDURAKDataBase].[dbo].[Channels] " +
                $"(GuildId, ChannelId, [Type]) VALUES ('{guildId}', '{channelId}', '{ChannelSeverity.Flood}')";
            using (SqlConnection connection = new(connectionString))
            {
                if (!SearchChannelInDB(ulongChannelID))
                {
                    connection.Open();
                    SqlCommand command = new(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }
            }
        }
        public static ChannelSeverity GetChannelType(ulong ulongChannelId)
        {
            string channelId = Convert.ToString(ulongChannelId);
            string sqlExpression = $"SELECT [Type] FROM [DiscordBotDURAKDataBase].[dbo].[Channels] WHERE ChannelId = '{channelId}'";

            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                SqlCommand command = new(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string severity = reader.GetString(0);
                    return (ChannelSeverity)Enum.Parse(typeof(ChannelSeverity), severity);
                }
                else
                {
                    throw new Exception("No such a channel in DB"); 
                }
        }
        }
        public static ulong GetReferenceChannel(ulong ulongGuildId)
        {
            string guildId = Convert.ToString(ulongGuildId);
            string sqlExpression =
                $"SELECT [ChannelId] FROM [DiscordBotDURAKDataBase].[dbo].[Channels] " +
                $"WHERE [GuildId] = '{guildId}' AND [Type] = '{ChannelSeverity.References}'";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                SqlCommand command = new(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return Convert.ToUInt64(reader.GetString(0));
                }
                else
                {
                    Program.Log(new Discord.LogMessage(Discord.LogSeverity.Error,
                                                       Constants.Sources.internal_function,
                                                       $"No reference channel for guild {guildId}"));
                    return 0;
                }
            }
        }
        /// <summary>
        /// Adds type to the channel
        /// </summary>
        /// <param name="ulongChannelId">channel id</param>
        /// <param name="severity">type of channel</param>
        public static void AddChannelType(ulong ulongChannelId, ChannelSeverity severity)
        {
            string channelId = Convert.ToString(ulongChannelId);
            string sqlExpression =
                $"UPDATE [DiscordBotDURAKDataBase].[dbo].[Channels] " +
                $"SET [Type] = '{severity}' WHERE ChannelId = '{channelId}'";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                SqlCommand command = new(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Deletes a channel from DB
        /// </summary>
        /// <param name="ulongChannelId">channel id</param>
        public static void DeleteChannelFDB(ulong ulongChannelId)
        {
            string channelId = Convert.ToString(ulongChannelId);
            string sqlExpression =
                $"DELETE FROM [DiscordBotDURAKDataBase].[dbo].[Channels] WHERE ChannelId = '{channelId}'";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                SqlCommand command = new(sqlExpression, connection);
                command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Search a channel in DB
        /// </summary>
        /// <param name="ulongChannelID">channel id</param>
        /// <returns>true if channel is already in DB</returns>
        public static bool SearchChannelInDB(ulong ulongChannelID)
        {
            string channelId = Convert.ToString(ulongChannelID);
            string sqlExpression =
                $"SELECT TOP (1000) [GuildID]" +
                $", [ChannelId]" +
                $", [Type]" +
                $" FROM [DiscordBotDURAKDataBase].[dbo].[Channels] WHERE ChannelId = '{channelId}'";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                SqlCommand command = new(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Get All guild ids from DB
        /// </summary>
        /// <returns>list of ids</returns>
        public static List<ulong> GetAll()
        {
            List<ulong> all = new List<ulong>();
            string sqlExpression = "SELECT GuildId FROM DiscordBotDURAKDataBase.dbo.IdTable";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();
                SqlCommand command = new(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var str = reader.GetString(0);
                        all.Add(Convert.ToUInt64(str));
                    }
                }
            }
            return all;
        }
        public static void DeleteGuilds(IReadOnlyCollection<ulong> guilds)
        {
            foreach (var guildId in guilds)
            {
                string sqlExpression1 = $"DELETE FROM DiscordBotDURAKDataBase.dbo.Channels WHERE GuildId = '{guildId}'";
                string sqlExpression2 = $"DELETE FROM DiscordBotDURAKDataBase.dbo.IdTable WHERE GuildId = '{guildId}'";
                using (SqlConnection connection = new(connectionString))
                {
                    connection.Open();
                    SqlCommand command1 = new(sqlExpression1, connection);
                    SqlCommand command2 = new(sqlExpression2, connection);
                    command1.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
                }
            }
            
        }
    }
}
