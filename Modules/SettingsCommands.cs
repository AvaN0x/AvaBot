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
    [Group("settings")]
    [Alias("set", "s")]
    public class SettingsCommands : ModuleBase
    {
        [Command("")]
        [RequireOwner]
        public async Task NoSettingFoundCommand()
        {
            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithDescription("No setting found")
                .WithColor(255, 0, 0);
            await ReplyAsync("", false, embedMessage.Build());
        }

        [Command("all")]
        [RequireOwner]
        public async Task SetAllCommand(string value = null)
            => await SetBoolean("all", Program.settings.Get(Context.Guild.Id).all, value);


        public async Task SetBoolean(string settingName, bool setting, string value)
        {
            bool flag;
            if (Boolean.TryParse(value, out flag))
            {
                setting = flag;
                Program.settings.SaveSettings();
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **" + settingName + "** set to *" + flag + "*")
                    .WithColor(255, 241, 185);
                await ReplyAsync("", false, embedMessage.Build());
            }
            else
            {
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **" + settingName + "** is *" + setting + "*")
                    .WithColor(255, 241, 185);
                await ReplyAsync("", false, embedMessage.Build());
            }
        }
    }
}