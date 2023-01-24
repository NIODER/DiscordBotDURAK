using Discord;
using DiscordBotDurak.Enum.BotRoles;
using DiscordBotDurak.Enum.Commands;
using DiscordBotDurak.Enum.ModerationModes;
using System.Collections.Generic;

namespace DiscordBotDurak
{
    public class SlashCommandCreator
    {
        public List<SlashCommandBuilder> GetAllSlashCommands()
        {
            return new List<SlashCommandBuilder>()
            {
                RandomNumberSlashcommand(),
                RandomUserSlashCommand(),
                RandomDesideSlashCommand(),
                RandomDistributionSlashCommand(),
                SpamSlashCommand(),
                StopSlashCommand(),
                CybershokeSlashCommand(),
                DeleteSlashCommand(),
                RemoderateSlashCommand(),
                SetPrivelegeSlashCommand(),
                GetSymbolsListsSlashCommand(),
                GetSymbolsSlashCommand(),
                AddListSlashCommand(),
                AddSymbolsToListSlashCommand(),
                RemoveSymbolSlashCommand(),
                SetModerationSlashCommand(),
                RemoveListSlashCommand(),
                StopSlashCommand(),
                WarningMessageSlashCommand(),
                BaseRoleSlashCommand(),
                GiveimmunitySlashCommand(),
                SetSpyRegimeSlashCommand(),
                InfoSlashCommand(),
                GetCreateHelpSlashCommand()
            };
        }

        private SlashCommandBuilder RandomNumberSlashcommand()
        {
            return new SlashCommandBuilder().WithName("random-number")
                .WithDescription("Returns random number/numbers in range")
                .AddOption("min", ApplicationCommandOptionType.Integer, "Lower border (include)", isRequired: true)
                .AddOption("max", ApplicationCommandOptionType.Integer, "Upper border (include)", isRequired: true)
                .AddOption("count", ApplicationCommandOptionType.Integer, "Count of numbers need to generate. 1 by default", isRequired: false);
        }

        private SlashCommandBuilder RandomUserSlashCommand()
        {
            return new SlashCommandBuilder().WithName("random-user")
                .WithDescription("Choose user from list of users, voice and message channel")
                .AddOption("mentions", ApplicationCommandOptionType.String, "Mentions separated with \',\'", isRequired: true)
                .AddOption("count", ApplicationCommandOptionType.Integer, "Count (1 by default)", isRequired: false);
        }

        private SlashCommandBuilder RandomDesideSlashCommand()
        {
            return new SlashCommandBuilder().WithName("random-decide")
                .WithDescription("Sends \"yes\" or \"no\"");
        }

        private SlashCommandBuilder RandomDistributionSlashCommand()
        {
            return new SlashCommandBuilder().WithName("random-distribute")
                .WithDescription("Sends random distribution of users by users list")
                .AddOption("mentions", ApplicationCommandOptionType.String, "Mentions separated with \',\'", isRequired: true)
                .AddOption("teams-count", ApplicationCommandOptionType.Integer, "Count of teams", isRequired: true)
                .AddOption("team-size", ApplicationCommandOptionType.Integer, "Size of team", isRequired: false);
        }

        private SlashCommandBuilder SpamSlashCommand()
        {
            return new SlashCommandBuilder().WithName("spam")
                .WithDescription("Sends message n times")
                .AddOption("message", ApplicationCommandOptionType.String, "Message need to spam", isRequired: true)
                .AddOption("count", ApplicationCommandOptionType.Integer, "Count of messages need to send", isRequired: true);
        }

        private SlashCommandBuilder CybershokeSlashCommand()
        {
            return new SlashCommandBuilder().WithName("cybershoke")
                .WithDescription("Sends menu of cybershoke servers");
        }

        private SlashCommandBuilder DeleteSlashCommand()
        {
            return new SlashCommandBuilder().WithName("delete")
                .WithDescription("Removes all messages contains specified symbol")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("type")
                    .WithDescription("Delete messages by...")
                    .AddChoice("by-symbol", 0)
                    .AddChoice("by-author", 1)
                    .WithType(ApplicationCommandOptionType.Integer)
                    .WithRequired(true))
                .AddOption("content", ApplicationCommandOptionType.String, "Content or author need to clear", isRequired: true);
        }

        private SlashCommandBuilder RemoderateSlashCommand()
        {
            return new SlashCommandBuilder().WithName("moderate")
                .WithDescription("Remoderates specified number of messages")
                .AddOption("count", ApplicationCommandOptionType.Integer, "Number of messages need to remoderate", isRequired: true);
        }

        private SlashCommandBuilder SetPrivelegeSlashCommand()
        {
            return new SlashCommandBuilder().WithName("set-privelege")
                .WithDescription("Gets to specified user privelege")
                .AddOption("user", ApplicationCommandOptionType.User, "User", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("privelege")
                    .WithDescription("Privelege")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoice("ban", (int)BotRole.Ban)
                    .AddChoice("user", (int)BotRole.User)
                    .AddChoice("moderator", (int)BotRole.Moderator)
                    .AddChoice("admin", (int)BotRole.Admin));
        }
        
        private SlashCommandBuilder GetSymbolsListsSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("get-lists")
                .WithDescription("Returns lists from specified scope.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("scope")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .WithDescription("Scope of lists need to return.")
                    .AddChoice("guild", 0)
                    .AddChoice("channel", 1)
                    .WithRequired(true));
        }

        private SlashCommandBuilder GetSymbolsSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("get-symbols")
                .WithDescription("Returns symbols from specified list")
                .AddOption("lists", ApplicationCommandOptionType.String, "Lists ids separated by \',\'", isRequired: true);
        }

        private SlashCommandBuilder AddListSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("list")
                .WithDescription("Add list to channel or guild. Creates new empty list if list-id is not specified.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("scope")
                    .WithDescription("Guild or channel")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoice("guild", 0)
                    .AddChoice("channel", 1)
                    .WithRequired(true))
                .AddOption("list-id", ApplicationCommandOptionType.Number, "List id need to add. Leave empty to create new list.", isRequired: false)
                .AddOption("title", ApplicationCommandOptionType.String, "List title. \"Untitled\" by default.", isRequired: false)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("moderation")
                    .WithDescription("Sets moderation regime to all of specified lists. (Non moderated by default)")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoice("Send warnings", (int)ModerationMode.OnlyWarnings)
                    .AddChoice("Resend", (int)ModerationMode.OnlyResend)
                    .AddChoice("Delete", (int)ModerationMode.OnlyDelete)
                    .WithRequired(false))
                .AddOption("resend-channel", ApplicationCommandOptionType.Channel, "Channel you want to resend symbols to. (Requred if moderation is OnlyResend)", isRequired: false);
        }

        private SlashCommandBuilder AddSymbolsToListSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("add-symbol")
                .WithDescription("Add symbols to list. When added, it overwrites the old ones with the same content")
                .AddOption("content", ApplicationCommandOptionType.String, "Symbol content", isRequired: true)
                .AddOption("excluded", ApplicationCommandOptionType.Boolean, "If an excluded character is encountered, the bot will forcibly stop message moderation.", isRequired: true)
                .AddOption("list", ApplicationCommandOptionType.String, "Lists ids need to add this symbol (separated by \',\')", isRequired: true);
        }

        private SlashCommandBuilder RemoveSymbolSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("remove-symbol")
                .WithDescription("Removes symbol from list by symbol id.")
                .AddOption("symbol-id", ApplicationCommandOptionType.Number, "Id of symbol need to remove.", isRequired: true)
                .AddOption("lists", ApplicationCommandOptionType.String, "Lists ids separated by \',\'", isRequired: true);
        }

        private SlashCommandBuilder SetModerationSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("set-moderation")
                .WithDescription("Sets moderation to list in this channel (if list already added to channel)")
                .AddOption("list-id", ApplicationCommandOptionType.Number, "Lists ids separated by \',\'", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("moderation")
                    .WithDescription("Sets moderation regime to all of specified lists. (Non moderated by default)")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoice("Send warnings", (int)ModerationMode.OnlyWarnings)
                    .AddChoice("Resend", (int)ModerationMode.OnlyResend)
                    .AddChoice("Delete", (int)ModerationMode.OnlyDelete)
                    .WithRequired(true));
        }

        private SlashCommandBuilder RemoveListSlashCommand()
        {
            return new SlashCommandBuilder()
                .WithName("remove-list")
                .WithDescription("Removes list from channel or guild")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("scope")
                    .WithDescription("Channel or guild")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoice("guild", 0)
                    .AddChoice("channel", 1)
                    .WithRequired(true))
                .AddOption("lists", ApplicationCommandOptionType.String, "Lists ids need to remove (separated by \',\')", isRequired: true);
        }

        private SlashCommandBuilder StopSlashCommand()
        {
            return new SlashCommandBuilder().WithName("stop")
                .WithDescription("Stop specified command")
                .AddOption(new SlashCommandOptionBuilder()
                .WithType(ApplicationCommandOptionType.Integer)
                .WithName("command")
                .WithDescription("Command need to stop")
                .AddChoice("spam", (int)CommandType.Spam)
                .AddChoice("delete", (int)CommandType.Delete)
                .WithRequired(true));
        }

        private SlashCommandBuilder WarningMessageSlashCommand()
        {
            return new SlashCommandBuilder().WithName("warning-message")
                .WithDescription("Set message parameter to change warning message in this channel or leave it empty to see current.")
                .AddOption("message", ApplicationCommandOptionType.String, "Message", isRequired: false);
        }

        private SlashCommandBuilder BaseRoleSlashCommand()
        {
            return new SlashCommandBuilder().WithName("base-role")
                .WithDescription("Set role parameter to change base role or leave it empty to see current.")
                .AddOption("role", ApplicationCommandOptionType.Role, "Base role", isRequired: false);
        }

        private SlashCommandBuilder GiveimmunitySlashCommand()
        {
            return new SlashCommandBuilder().WithName("set-immunity")
                .WithDescription("Set immunity (enable or disable) to specified user.")
                .AddOption("user", ApplicationCommandOptionType.User, "User ypu want to set immunity.", isRequired: true)
                .AddOption("enable-immunity", ApplicationCommandOptionType.Boolean, "True if set enable.", isRequired: true);
        }

        private SlashCommandBuilder SetSpyRegimeSlashCommand()
        {
            return new SlashCommandBuilder().WithName("spy-mode")
                .WithDescription("Set mode parameter to change spy mode or leave it empty to see current.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("mode")
                    .WithDescription("Spy mode")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoice("saves-statistics", (int)SpyModesEnum.CollectInfo)
                    .AddChoice("sends-warnings", (int)SpyModesEnum.SendTips)
                    .AddChoice("deletes-users", (int)SpyModesEnum.DeleteUsers)
                    .WithRequired(false));
        }

        private SlashCommandBuilder InfoSlashCommand()
        {
            return new SlashCommandBuilder().WithName("info")
                .WithDescription("Gets info about user or chat")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("guild")
                    .WithDescription("Info about this guild.")
                    .WithType(ApplicationCommandOptionType.SubCommand))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("channel")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .WithDescription("Info about channel.")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("channel-mention")
                        .WithDescription("Select channel")
                        .WithType(ApplicationCommandOptionType.Channel)
                        .WithRequired(true)))
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("user")
                    .WithDescription("Info about user.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("user-mention")
                        .WithDescription("Select user.")
                        .WithType(ApplicationCommandOptionType.User)
                        .WithRequired(true)));
        }

        private SlashCommandBuilder GetCreateHelpSlashCommand()
        {
            return new SlashCommandBuilder().WithName("help")
                .WithDescription("Sends help message")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("type")
                    .WithDescription("Type of help message")
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoice("SpyMode", 0)
                    .AddChoice("ForbiddenSymbols", 1)
                    .AddChoice("User", 2)
                    .WithRequired(true));
        }
    }
}
