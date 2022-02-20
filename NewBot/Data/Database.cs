using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotDURAK.NewBot.Data
{
    class Database : CRUD
    {
        private const string connectionString = @"Data Source=DESKTOP-OVCM484\SQLEXPRESS;Initial Catalog=DiscordBotDURAKDataBase;Integrated Security=True";
        SqlConnection connection;

        public Database()
        {
            connection = new(connectionString);
        }

        ~Database()
        {
            connection.Close();
        }

        /// <summary>
        /// Puts data in database
        /// </summary>
        /// <param name="action">
        /// Type DiscordBotDURAK.NewBot.Data.Actions
        /// </param>
        /// <param name="guildId">
        /// Type string
        /// </param>
        /// <param name="ulong channelId | ulong adminsId | string radio">
        /// Actions.Guild:
        ///     none
        /// Actions.Admin:
        ///     ulong adminId
        /// Actions.Channel:
        ///     ulong channelId
        /// Actions.Radio:
        ///     string radioUrl
        /// </param>
        public void Create(params object[] parameters)
        {
            try
            {
                Actions action = (Actions)parameters[0];
                switch (action)
                {
                    case Actions.Guild:
                        {
                            string guildId = (string)parameters[1];
                            string createSchema = $"CREATE SCHEMA [{guildId}]";
                            string sqlExpression =
                                $"INSERT INTO [DiscordBotDURAKDataBase].[dbo].[Guilds] (guild) VALUES ('{guildId}') " +
                                $"CREATE TABLE [{guildId}].[Admins] (Admin NVARCHAR(50) NOT NULL) " +
                                $"CREATE TABLE [{guildId}].[Channels] (Channel NVARCHAR(50) NOT NULL, Summary NVARCHAR(50 NOT NULL) " +
                                $"CREATE TABLE [{guildId}].[Radio] (Reference NVARCHAR(50) NOT NULL) ";
                            if (!inDatabase(guildId))
                            {
                                SqlCommand schema = new(createSchema, connection);
                                SqlCommand command = new(sqlExpression, connection);
                                connection.Open();
                                try
                                {
                                    schema.ExecuteNonQuery();
                                }
                                catch (SqlException) { }
                                command.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                        break;
                    case Actions.Admin:
                        {
                            string guildId = (string)parameters[1];
                            ulong adminId = (ulong)parameters[2];
                            string sqlExpression =
                                $"INSERT INTO [DiscordBotDURAKDataBase].[{guildId}].[Admins] (Admin) VALUES ('{Convert.ToString(adminId)}');";
                            SqlCommand command = new(sqlExpression, connection);
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        break;
                    case Actions.Channel:
                        {
                            string guildId = (string)parameters[1];
                            ulong channelId = (ulong)parameters[2];
                            string sqlExpression = $"INSERT INTO [DiscordBotDURAKDataBase].[{guildId}].[Channels] (Channel, Type) VALUES ('{Convert.ToString(channelId)}', '{ChannelSeverity.Flood}');";
                            SqlCommand command = new(sqlExpression, connection);
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        break;
                    case Actions.Radio:
                        {
                            string guildId = (string)parameters[1];
                            string radioUrl = (string)parameters[2];
                            string sqlExpression = $"INSERT INTO [DiscordBotDURAKDataBase].[{guildId}].[Radio] (Reference) VALUES ('{radioUrl}');";
                            SqlCommand command = new(sqlExpression, connection);
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        break;
                    default:
                        throw new Exception("invalid action");
                }
            }
            catch(InvalidCastException e)
            {
                _ = new Log(
                    new Discord.LogMessage(
                        Discord.LogSeverity.Error, 
                        "Database create", 
                        "Invalid cast", 
                        e)
                    );
            }
            catch(Exception e)
            {
                _ = new Log(
                    new Discord.LogMessage(
                        Discord.LogSeverity.Error,
                        "Database create",
                        "default worked",
                        e)
                    );
            }
        }

        /// <summary>
        /// Deletes data from database
        /// </summary>
        /// <param name="action">
        /// Type DiscordBotDURAK.NewBot.Data.Actions
        /// </param>
        /// <param name="guildId">
        /// Type string
        /// </param>
        /// <param name="adminsId | reference">
        /// Actions.Guild:
        ///     none
        /// Actions.Admin:
        ///     ulong adminId
        /// Actions.Channel:
        ///     none
        /// Actions.Radio:
        ///     string reference
        /// </param>
        public void Delete(params object[] parameters)
        {
            Actions action = (Actions)parameters[0];
            switch (action)
            {
                case Actions.Guild:
                    {
                        string guildId = (string)parameters[1];
                        string sqlExpression =
                            $"DROP TABLE [DiscordBotDURAKDataBase].[{guildId}].[Admins]; " +
                            $"DROP TABLE [DiscordBotDURAKDataBase].[{guildId}].[Channels]; " +
                            $"DROP TABLE [DiscordBotDURAKDataBase].[{guildId}].[Radio]; " +
                            $"DROP SCHEMA [{guildId}]; " +
                            $"DELETE FROM [DiscordBotDURAKDataBase].[dbo].[Guilds] WHERE (guild = '{guildId}');";
                        SqlCommand command = new(sqlExpression, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    break;
                case Actions.Admin:
                    {
                        string guildId = (string)parameters[1];
                        ulong adminId = (ulong)parameters[2];
                        string sqlExpression = 
                            $"DELETE FROM [DiscordBotDURAKDataBase].[{guildId}].[Admins] WHERE (Admin = '{Convert.ToString(adminId)}');";
                        SqlCommand command = new(sqlExpression, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    break;
                case Actions.Channel:
                    break;
                case Actions.Radio:
                    {
                        string guildId = (string)parameters[1];
                        string reference = (string)parameters[2];
                        string sqlExpression = 
                            $"DELETE FROM [DiscordBotDURAKDataBase].[{guildId}].[Radio] WHERE Reference = '{reference}'";
                        SqlCommand command = new(sqlExpression, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reads content from database
        /// params: Actions action, string guildId, bool isType, ulong channelId
        /// </summary>
        /// <param name="actions">
        /// Type DiscordBotDURAK.NewBot.Data.Actions
        /// </param>
        /// <param name="guildId">
        /// Type string
        /// null if Actions.Guild
        /// </param>
        /// <param name="isType">
        /// Type bool
        /// true if you need get channel type
        /// false if you need get reference-channel id
        /// </param>
        /// <param name="channelid">
        /// Type ulong
        /// null if parameter isType is false
        /// </param>
        /// <returns>
        /// Actions.Guilds:
        ///     ulong List of all guilds in database
        /// Actions.Admin:
        ///     ulong List of all admins in current guild
        /// Actions.Channel:
        ///     if isType true:
        ///         ChannelSeverity
        ///     else ulong channelId (0 if no reference channel in this guild)
        /// Actions.Radio:
        ///     string List of all radios
        /// </returns>
        public object Read(params object[] parameters)
        {
            Actions action = (Actions)parameters[0];
            switch (action)
            {
                case Actions.Guild:
                    {
                        List<ulong> guilds = new();
                        string sqlExpression = $"SELECT * FROM [DiscordBotDURAKDataBase].[dbo].[Guilds]";
                        SqlCommand command = new(sqlExpression, connection);
                        connection.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            guilds.Add(Convert.ToUInt64(reader.GetString(0)));
                        }
                        connection.Close();
                        return guilds;
                    }
                case Actions.Admin:
                    {
                        string guildId = (string)parameters[1];
                        List<ulong> admins = new();
                        string sqlExpression =
                            $"SELECT Admin FROM [DiscordBotDURAKDataBase].[{guildId}].[Admins];";
                        SqlCommand command = new(sqlExpression, connection);
                        connection.Open();
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            admins.Add(Convert.ToUInt64(reader.GetString(0)));
                        }
                        connection.Close();
                        return admins;
                    }
                case Actions.Channel:
                    {
                        string guildId = (string)parameters[1];
                        bool isType = (bool)parameters[2];
                        if (!isType)
                        {
                            string sqlExpression =
                                $"SELECT Channel FROM [DiscordBotDURAKDataBase].[{guildId}].[Channels] WHERE (Type = '{ChannelSeverity.References}')";
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
                            connection.Close();
                            return 0;
                        }
                        else
                        {
                            ulong channelId = (ulong)parameters[3];
                            string sqlExpression =
                                $"SELECT Type FROM [DiscordBotDURAKDataBase].[{guildId}].[Channels] WHERE Channel = '{channelId}'";
                            SqlCommand command = new(sqlExpression, connection);
                            connection.Open();
                            var reader = command.ExecuteReader();
                            connection.Close();
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
                case Actions.Radio:
                    {
                        string guildId = (string)parameters[1];
                        List<string> radio = new();
                        string sqlExpression =
                            $"SELECT Reference FROM [DiscordBotDURAKDataBase].[{guildId}].[Radio]";
                        SqlCommand command = new(sqlExpression, connection);
                        connection.Open();
                        var reader = command.ExecuteReader();
                        connection.Close();
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
                default:
                    throw new Exception("no such activity");
            }
        }

        /// <summary>
        /// Updates data in database. Works only with action Actions.Channel
        /// </summary>
        /// <param name="action">
        /// Type Type DiscordBotDURAK.NewBot.Data.Actions
        /// </param>
        /// <param name="guildId">
        /// Type string
        /// </param>
        /// <param name="channelId">
        /// Type ulong
        /// </param>
        /// <param name="channelSeverity">
        /// Type DiscordBotDURAK.NewBot.Data.ChannelSeverity
        /// </param>
        public void Update(params object[] parameters)
        {
            Actions action = (Actions)parameters[0];
            switch (action)
            {
                case Actions.Guild:
                    {

                    }
                    break;
                case Actions.Admin:
                    break;
                case Actions.Channel:
                    {
                        string guildId = (string)parameters[1];
                        ulong channelId = (ulong)parameters[2];
                        ChannelSeverity severity = (ChannelSeverity)parameters[3];
                        string sqlExpession =
                            $"UPDATE [DiscordBotDURAKDataBase].[{guildId}].[Channels] " +
                            $"SET Type = '{severity}' WHERE (Channel = '{Convert.ToString(channelId)}');";
                        SqlCommand command = new(sqlExpession, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    break;
                case Actions.Radio:
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Check if guild exists in database
        /// </summary>
        /// <param name="guild">guild id</param>
        /// <returns>true if in database</returns>
        public bool inDatabase(string guild)
        {
            string sqlExpression =
                $"SELECT * FROM [DiscordBotDURAKDataBase].[dbo].[Guilds] WHERE guild = '{guild}'";
            connection.Open();
            var reader = new SqlCommand(sqlExpression, connection).ExecuteReader();
            connection.Close();
            return reader.HasRows;
        }

        /// <summary>
        /// Check if user is admin
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="guildId"></param>
        /// <returns>true if user is admin</returns>
        public bool isAdmin(ulong userId, ulong guildId)
        {
            string sqlExpression = $"SELECT Admin FROM [DiscordBotDURAKDataBase].[{guildId}].[Admins] WHERE (Admin = '{Convert.ToString(userId)}');";
            SqlCommand command = new(sqlExpression, connection);
            connection.Open();
            var reader = command.ExecuteReader();
            connection.Close();
            return reader.HasRows;
        }
    }
}
