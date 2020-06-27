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
        [Command]
        [RequireOwner]
        public async Task NoSettingFoundCommand()
        {
            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithDescription("No setting found")
                .WithColor(255, 0, 0);
            await ReplyAsync("", false, embedMessage.Build());
        }

        // TODO need optimisation, reflection?! :)
        [Command("textscan")]
        [Alias("scan")]
        [RequireOwner]
        public async Task SetTextScanCommand(string value = null)
        {
            bool flag;
            if (Boolean.TryParse(value, out flag))
            {
                Program.settings.Get(Context.Guild.Id).modpackScan = flag;
                Program.settings.Get(Context.Guild.Id).chehScan = flag;
                Program.settings.Get(Context.Guild.Id).gf1Scan = flag;
                Program.settings.Get(Context.Guild.Id).ineScan = flag;
                Program.settings.SaveSettings();
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **modpack** set to *" + flag + "*" +
                        "\nValue of **cheh** set to *" + flag + "*" +
                        "\nValue of **gf1** set to *" + flag + "*" +
                        "\nValue of **ine** set to *" + flag + "*")
                    .WithColor(255, 241, 185);
                await ReplyAsync("", false, embedMessage.Build());
            }
            else
            {
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **modpack** is *" + Program.settings.Get(Context.Guild.Id).modpackScan + "*" +
                        "\nValue of **cheh** is *" + Program.settings.Get(Context.Guild.Id).chehScan + "*" +
                        "\nValue of **gf1** is *" + Program.settings.Get(Context.Guild.Id).gf1Scan + "*" +
                        "\nValue of **ine** is *" + Program.settings.Get(Context.Guild.Id).ineScan + "*")
                    .WithColor(255, 241, 185);
                await ReplyAsync("", false, embedMessage.Build());
            }
        }

            [Command("modpack")]
            [RequireOwner]
            public async Task SetModpackScanCommand(string value = null)
            {
                var setting = Program.settings.Get(Context.Guild.Id).modpackScan;
                SetBoolean("modpack", ref setting, value);
                Program.settings.Get(Context.Guild.Id).modpackScan = setting;
            }

            [Command("cheh")]
            [RequireOwner]
            public async Task SetChehScanCommand(string value = null)
            {
                var setting = Program.settings.Get(Context.Guild.Id).chehScan;
                SetBoolean("cheh", ref setting, value);
                Program.settings.Get(Context.Guild.Id).chehScan = setting;
            }

            [Command("gf1")]
            [RequireOwner]
            public async Task SetGf1ScanCommand(string value = null)
            {
                var setting = Program.settings.Get(Context.Guild.Id).gf1Scan;
                SetBoolean("gf1", ref setting, value);
                Program.settings.Get(Context.Guild.Id).gf1Scan = setting;
            }

            [Command("ine")]
            [RequireOwner]
            public async Task SetIneScanCommand(string value = null)
            {
                var setting = Program.settings.Get(Context.Guild.Id).ineScan;
                SetBoolean("ine", ref setting, value);
                Program.settings.Get(Context.Guild.Id).ineScan = setting;
            }

        public void SetBoolean(string settingName, ref bool setting, string value)
        {
            bool flag;
            if (Boolean.TryParse(value, out flag))
            {
                setting = flag;
                Program.settings.SaveSettings();
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **" + settingName + "** set to *" + flag + "*")
                    .WithColor(255, 241, 185);
                ReplyAsync("", false, embedMessage.Build());
            }
            else
            {
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **" + settingName + "** is *" + setting + "*")
                    .WithColor(255, 241, 185);
                ReplyAsync("", false, embedMessage.Build());
            }
        }
    }
}