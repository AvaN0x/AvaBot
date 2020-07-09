using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace AvaBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    [Summary("Help Commands")]
    [Group("help")]
    [Alias("h")]
    public class HelpCommands : ModuleBase
    {
        private readonly CommandService _commands;
        private readonly IConfiguration _config;

        public HelpCommands(CommandService commands, IConfiguration config)
        {
            _commands = commands;
            _config = config;
        }

        [Command]
        [Summary("Give informations about every commands")]
        public async Task HelpCommand()
        {
            List<CommandInfo> commands = _commands.Commands.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle("Commands help")
                .WithDescription("" +
                    "The prefix is `" + _config["Prefix"] + "`.")
                .WithFooter("github.com/AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            foreach (var module in _commands.Modules)
            {
                var fieldContent = "";
                foreach (var command in module.Commands)
                {
                    fieldContent += "• `" + _config["Prefix"] + command.Aliases.First() + string.Join("", command.Parameters.Select(p => " [" + p.Name + "]")) +
                        "` : " + (command.Summary ?? "No description available") +
                        string.Concat(command.Parameters.Select(p => "\n[" + p.Name + "] : *" + (p.Summary ?? "No description available") + "*")) + "\n";
                }
                embedBuilder.AddField(module.Summary ?? "No summary available", fieldContent);
            }
            await ReplyAsync("", false, embedBuilder.Build());
        }


        [Command("settings")]
        [Alias("set" , "s")]
        [Summary("Give informations about settings commands")]
        public async Task HelpSettingsCommand()
        {
            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithTitle("Commands help - settings")
                .WithDescription("Actual permission needed : Owner")
                .AddField("See value", "`//s [category / command]`" +
                    "\nExample : " +
                    "\n`//s` -> Value of every command" +
                    "\n`//s scan` -> Value of every command in scan category" +
                    "\n`//s scan ine` -> Value of ine command" +
                    "\n`//s mute` -> Value of mute //and unmute command" +
                    "", false)
                .AddField("Edit value", "`//s [category / command] value`" +
                    "\nExample : " +
                    "\n`//s scan false` -> Value of every command in scan category set to false" +
                    "\n`//s scan ine false` -> Value of ine command set to true" +
                    "\n`//s mute false` -> Value of mute and unmute command set to false" +
                    "", false)
                .WithFooter("github.com/AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            await ReplyAsync("", false, embedMessage.Build());
        }
    }
}