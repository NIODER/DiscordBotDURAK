using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DiscordBotDURAK
{
    static class DataBase
    {
        private static string connectionString = @"Data Source=DESKTOP-OVCM484\SQLEXPRESS;Initial Catalog=DiscordBotDURAKDataBase;Integrated Security=True";
        /// <summary>
        /// Добавляет id админа в БД, если его там еще нет.
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="adminId"></param>
        public static void Add(ulong ulongguildId, ulong ulongadminId)
        {
            string guildId = Convert.ToString(ulongguildId);
            string adminId = Convert.ToString(ulongadminId);
            string sqlExpression = $"INSERT INTO [DiscordBotDURAKDataBase].[dbo].[IdTable] ([GuildId], [Admins]) VALUE ('{guildId}', '{adminId}')";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                if (!Search(guildId, adminId))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Вытаскивает всех админов из БД
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns>Возвращает строку с id всех админов</returns>
        public static string[] Get(ulong ulongguildId)
        {
            string guildId = Convert.ToString(ulongguildId);
            string adminsId = null;
            string sqlExpression = $"SELECT Admins FROM IdTable WHERE GuildId = '{guildId}'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    adminsId = reader.GetString(0);
                }
            }
            return adminsId?.Split(' ');
            
        }
        public static string[] Get(string guildId)
        {
            string adminsId = null;
            string sqlExpression = $"SELECT Admins FROM [DiscordBotDURAKDataBase].[dbo].[IdTable] WHERE GuildId = '{guildId}'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                adminsId = reader.GetString(1);
            }
            return adminsId.Split(' ');
        }
        private static bool Search(string guildId, string adminId) => Get(guildId).Contains(adminId);
        public static bool Search(ulong ulongguildId)
        {
            string guildId = Convert.ToString(ulongguildId);
            string sqlExpression = $"SELECT GuildId FROM [DiscordBotDURAKDataBase].[dbo].[IdTable]";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.HasRows)
                {
                    if (reader.GetString(0) == guildId)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
