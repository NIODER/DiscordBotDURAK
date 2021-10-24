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
        public static void AddAdminIDB(ulong ulongguildId, ulong ulongadminId)
        {
            string guildId = Convert.ToString(ulongguildId);
            string adminId = Convert.ToString(ulongadminId);
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
            string sqlExpression = $"SELECT GuildId FROM [DiscordBotDURAKDataBase].[dbo].[IdTable] WHERE GuildId = '{guildId}'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
