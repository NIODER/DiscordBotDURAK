using System.IO;
using Discord;

namespace Constants
{
    public static class Constants
    {
        public static object locker = new object();
        public static readonly string logPath = @"E:\log.TXT";
        public static readonly string tokenPath = @"E:\TOKEN.txt";
        public static readonly string tabulation = "\n__________________________________________\n";
        public static readonly string helpPath = @"E:\MyProgs\Home\c sharp\DiscordBotDURAK\helpMessage.txt";
        public static readonly string ownerMessagePath = @"E:\MyProgs\Home\c sharp\DiscordBotDURAK\ownerMessage.txt";
    }

    public static class Sources
    {
        public static readonly string command = "Command";
        public static readonly string internal_function = "Internal";
    }

    public static class Commands
    {
        public static readonly string joinedAt = $"$joinedAt";
        public static readonly string joke = $"$joke";
        public static readonly string surf = $"$surf";
        public static readonly string quote = $"$цитата"; 
        public static readonly string owner = "$owner";
        public static readonly string setChannelType = "$set";
        public static readonly string delete = "$delete";
        public static readonly string random = "$random"; 
        public static readonly string spam = "$spam"; 
        public static readonly string moderate = "$moderate"; 
        public static readonly string clean = "$clean"; 
        public static readonly string decide = "реши"; 
        public static readonly string id = "$ID";
        public static readonly string giveAdmin = "$admin";
        public static readonly string help = "$help";
        public static readonly string deleteAdmin = "$delete";
        public static readonly string search = "$search";
        public static readonly string commandsHelp = new StreamReader(File.OpenRead(Constants.helpPath)).ReadToEnd();
    }
}
