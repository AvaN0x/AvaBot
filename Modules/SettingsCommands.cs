using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Reflection;
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
    [RequireAdminRole(Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public class SettingsCommands : ModuleBase
    {
        [Command]
        public async Task AllValueCommand()
        {
            var guildSettings = Utils.GetSettings(Context.Guild.Id);
            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithTitle("Commands states")
                .WithDescription("*Always* mean that the value cannot be changed.")
                .AddField("Randoms", "" +
                    "• `github` or `avan0x` : *Always*" +
                    "", false)
                .AddField("Text scan", "" +
                "• `modpack` : *" + (guildSettings.modpackScan ? "Activated" : "Disabled") + "*" +
                "\n• `cheh` : *" + (guildSettings.chehScan ? "Activated" : "Disabled") + "*" +
                "\n• `gf1` : *" + (guildSettings.gf1Scan ? "Activated" : "Disabled") + "*" +
                "\n• `ine` : *" + (guildSettings.ineScan ? "Activated" : "Disabled") + "*" +
                "\n• `reactuser` : *" + (guildSettings.reactToUserScan ? "Activated" : "Disabled") + "*" +
                "", false)
                .AddField("User", "" +
                    "• `info` : *Always*" +
                    "", false)
                .AddField("Admin", "" +
                    "• `mute` : *" + (guildSettings.admin_mute ? "Activated" : "Disabled") + "*" +
                    "", false)
                .WithFooter("github.com/AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            await ReplyAsync("", false, embedMessage.Build());

        }

        [Group("textscan")]
        [Alias("scan")]
        public class SettingsCommands_Scan : ModuleBase
        {
            // //s scan --> display values
            // //s scan bool --> set all to bool value
            [Command]
            public async Task SetTextScanCommand(string value = null)
            {
                //TODO add a method to change a full category ( foreach( var foo in foos.Where()) )
                var guildSettings = Utils.GetSettings(Context.Guild.Id);
                bool flag;
                if (Boolean.TryParse(value, out flag))
                {
                    guildSettings.modpackScan = flag;
                    guildSettings.chehScan = flag;
                    guildSettings.gf1Scan = flag;
                    guildSettings.ineScan = flag;
                    guildSettings.reactToUserScan = flag;
                    await Utils.LogAsync("Value of all scan settings set to `" + flag + "` on `" + Context.Guild.Name + "`");

                    Utils.SaveData();
                    EmbedBuilder embedMessage = new EmbedBuilder()
                        .WithDescription("" + 
                            "Value of **modpack** set to *" + flag + "*" +
                            "\nValue of **cheh** set to *" + flag + "*" +
                            "\nValue of **gf1** set to *" + flag + "*" +
                            "\nValue of **ine** set to *" + flag + "*" +
                            "\nValue of **reactuser** set to *" + flag + "*" +
                            "")
                        .WithColor(255, 241, 185);
                    await ReplyAsync("", false, embedMessage.Build());
                }
                else
                {
                    EmbedBuilder embedMessage = new EmbedBuilder()
                        .WithDescription("" + 
                            "Value of **modpack** is *" + guildSettings.modpackScan + "*" +
                            "\nValue of **cheh** is *" + guildSettings.chehScan + "*" +
                            "\nValue of **gf1** is *" + guildSettings.gf1Scan + "*" +
                            "\nValue of **ine** is *" + guildSettings.ineScan + "*" +
                            "\nValue of **reactuser** is *" + guildSettings.reactToUserScan + "*" +
                            "")
                        .WithColor(255, 241, 185);
                    await ReplyAsync("", false, embedMessage.Build());
                }
            }

            // //s scan modpack --> display value
            // //s scan modpack bool --> set modpackScan to bool value
            [Command("modpack")]
            public async Task SetModpackScanCommand(string value = null)
                => await SetObject("modpackScan", bool.TryParse(value, out var flag), flag, Context);

            // //s scan cheh --> display value
            // //s scan cheh bool --> set chehScan to bool value
            [Command("cheh")]
            public async Task SetChehScanCommand(string value = null)
                //=> await SetBoolean("chehScan", Utils.GetSettings(Context.Guild.Id), value, Context);
                => await SetObject("chehScan", bool.TryParse(value, out var flag), flag, Context);


            //// //s scan gf1 --> display value
            //// //s scan gf1 bool --> set gf1Scan to bool value
            [Command("gf1")]
            public async Task SetGf1ScanCommand(string value = null)
                => await SetObject("gf1Scan", bool.TryParse(value, out var flag), flag, Context);

            // //s scan ine --> display value
            // //s scan ine bool --> set ineScan to bool value
            [Command("ine")]
            public async Task SetIneScanCommand(string value = null)
                => await SetObject("ineScan", bool.TryParse(value, out var flag), flag, Context);

            // //s scan reactuser --> display value
            // //s scan reactuser bool --> set reactionToUsername to bool value
            [Command("reactuser")]
            public async Task SetReactUserCommand(string value = null)
                => await SetObject("reactToUserScan", bool.TryParse(value, out var flag), flag, Context);
        }

        // //s role [role name/id/mention]
        [Command("adminrole")]
        [Alias("role")]
        [RequireOwner] // Only the owner can change the role
        public async Task SetAdminRoleCommand(SocketRole role = null)
        {
            if (role != null)
                await SetObject("adminRoleId", true, role.Id, Context);
            else
                await SetObject("adminRoleId", false, null, Context);
        }

        // //s mute [true/false]
        [Command("mute")]
        public async Task MuteCommand(string value = null)
        {
            bool flag;
            var tryparse = Boolean.TryParse(value, out flag);
            if (tryparse && !flag)
            {
                Utils.GetSettings(Context.Guild.Id).muted.Clear();
                await Utils.LogAsync("Every muted got unmuted on `" + Context.Guild.Name + "`");
            }
            await SetObject("admin_mute", tryparse, flag, Context);
        }

        public static async Task SetObject(string settingName, bool tryparse, object flag, ICommandContext context)
        {
            var settings = Utils.GetSettings(context.Guild.Id);
            EmbedBuilder embedMessage;
            var type = settings.GetType();
            var props = type.GetProperties();
            var focused = props.FirstOrDefault(prop => prop.Name == settingName);

            if (focused == null)
            {
                await Utils.LogAsync("There's no property named `" + settingName + "` in `" + type.Name + "`", "ERROR");
                embedMessage = new EmbedBuilder()
                    .WithDescription("There's an error with this property, you can create an issue on [github](https://github.com/AvaN0x/AvaBot/issues) or contact me on Discord at AvaN0x#6348.")
                    .WithColor(255, 0, 0);
                await context.Channel.SendMessageAsync("", false, embedMessage.Build());
                return;
            }

            if (tryparse)
            {
                focused.SetValue(settings, flag);
                await Utils.LogAsync("Value of `" + settingName + "` set to `" + flag + "` on `" + context.Guild.Name + "`");
                // TODO save only if different of actual value
                Utils.SaveData();
                embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **" + settingName + "** set to *" + flag + "*")
                    .WithColor(255, 241, 185);
            }
            else
            {
                embedMessage = new EmbedBuilder()
                    .WithDescription("Value of **" + settingName + "** is *" + (focused.GetValue(settings) != null ? focused.GetValue(settings) : "null" ) + "*")
                    .WithColor(255, 241, 185);
            }
            await context.Channel.SendMessageAsync("", false, embedMessage.Build());
        }
    }
}