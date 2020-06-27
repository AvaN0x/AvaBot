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

//#pragma warning disable CS1998
namespace AvaBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    [Group("settings")]
    [Alias("set", "s")]
    [RequireOwner]
    public class SettingsCommands : ModuleBase
    {
        [Command]
        public async Task NoSettingFoundCommand()
        {
            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithDescription("No setting found")
                .WithColor(255, 0, 0);
            await ReplyAsync("", false, embedMessage.Build());
        }

        [Group("textscan")]
        [Alias("scan")]
        public class SettingsCommands_Scan : ModuleBase
        {
            // TODO need optimisation, reflection?! :)
            // //s scan --> display values
            // //s scan bool --> set all to bool value
            [Command]
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

            // //s scan modpack --> display value
            // //s scan modpack bool --> set modpackScan to bool value
            [Command("modpack")]
            [RequireOwner]
            public async Task SetModpackScanCommand(string value = null)
            {
                var setting = Program.settings.Get(Context.Guild.Id).modpackScan;
                var embedMessage = SetBoolean("modpack", ref setting, value);
                Program.settings.Get(Context.Guild.Id).modpackScan = setting;
                Program.settings.SaveSettings();
                await ReplyAsync("", false, embedMessage.Build());
            }

            // //s scan cheh --> display value
            // //s scan cheh bool --> set chehScan to bool value
            [Command("cheh")]
            [RequireOwner]
            public async Task SetChehScanCommand(string value = null)
            {
                var setting = Program.settings.Get(Context.Guild.Id).chehScan;
                var embedMessage = SetBoolean("cheh", ref setting, value);
                Program.settings.Get(Context.Guild.Id).chehScan = setting;
                Program.settings.SaveSettings();
                await ReplyAsync("", false, embedMessage.Build());
            }

            // //s scan gf1 --> display value
            // //s scan gf1 bool --> set gf1Scan to bool value
            [Command("gf1")]
            [RequireOwner]
            public async Task SetGf1ScanCommand(string value = null)
            {
                var setting = Program.settings.Get(Context.Guild.Id).gf1Scan;
                var embedMessage = SetBoolean("gf1", ref setting, value);
                Program.settings.Get(Context.Guild.Id).gf1Scan = setting;
                Program.settings.SaveSettings();
                await ReplyAsync("", false, embedMessage.Build());
            }

            // //s scan ine --> display value
            // //s scan ine bool --> set ineScan to bool value
            [Command("ine")]
            [RequireOwner]
            public async Task SetIneScanCommand(string value = null)
            {
                var setting = Program.settings.Get(Context.Guild.Id).ineScan;
                var embedMessage = SetBoolean("ine", ref setting, value);
                Program.settings.Get(Context.Guild.Id).ineScan = setting;
                Program.settings.SaveSettings();
                await ReplyAsync("", false, embedMessage.Build());
            }

        }



        public static EmbedBuilder SetBoolean(string settingName, ref bool setting, string value)
        {
            EmbedBuilder embedMessage;
            bool flag;
            if (Boolean.TryParse(value, out flag))
            {
                setting = flag;
                embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **" + settingName + "** set to *" + flag + "*")
                    .WithColor(255, 241, 185);
            }
            else
            {
                embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **" + settingName + "** is *" + setting + "*")
                    .WithColor(255, 241, 185);
            }
            return embedMessage;
        }
    }
}