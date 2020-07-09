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
            => await ShowModuleHelp("Commands help", _commands.Modules.Where(m => !m.Aliases.First().Contains("settings")));


        [Command("settings")]
        [Alias("set" , "s")]
        [Summary("Give informations about settings commands")]
        public async Task HelpSettingsCommand()
            => await ShowModuleHelp("Commands help - Settings", _commands.Modules.Where(m => m.Aliases.First().Contains("settings")));

        private async Task ShowModuleHelp(string title, IEnumerable<ModuleInfo> modules)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle(title)
                .WithFooter("github.com/AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            foreach (var module in modules)
            {
                var fieldContent = "";
                foreach (var command in module.Commands)
                {
                    fieldContent += "• `" + _config["Prefix"] + command.Aliases.OrderBy(a => a.Length).First() + string.Join("", command.Parameters.Select(p => " [" + p.Name + "]")) +
                        "` : " + (command.Summary ?? "No description available") +
                        string.Concat(command.Parameters.Select(p => "\n[" + p.Name + "] : *" + (p.Summary ?? "No description available") + "*")) + "\n";
                }
                embedBuilder.AddField(module.Summary ?? "No summary available", fieldContent);
            }
            await ReplyAsync("", false, embedBuilder.Build());
        }
    }
}