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
            await ReplyAsync("No setting found");
        }

        [Command("all")]
        [RequireOwner]
        public async Task SetAllCommand(string value = null)
        {
            SetBoolean("all", Program.settings.Get(Context.Guild.Id).all, value);
            //bool flag;
            //if (Boolean.TryParse(value, out flag))
            //{
            //    Program.settings.Get(Context.Guild.Id).all = flag;
            //    Program.settings.SaveSettings();
            //    EmbedBuilder embedMessage = new EmbedBuilder()
            //        .WithDescription("Value of **" + "all" + "** set to *" + flag + "*")
            //        .WithColor(255, 241, 185);
            //    await ReplyAsync("", false, embedMessage.Build());
            //}
            //else
            //{
            //    EmbedBuilder embedMessage = new EmbedBuilder()
            //        .WithDescription("Value of **" + "all" + "** is *" + Program.settings.Get(Context.Guild.Id).all + "*")
            //        .WithColor(255, 241, 185);
            //    await ReplyAsync("", false, embedMessage.Build());
            //}
        }

        public async void SetBoolean(string settingName, bool setting, string value)
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