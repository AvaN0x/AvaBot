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
using System.Text.RegularExpressions;

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
            var settings = Utils.GetSettings(Context.Guild.Id);

            var type = settings.GetType();
            var props = type.GetProperties();
            var scanText = "";
            foreach (var prop in props.Where(prop => new Regex("(Scan)$").IsMatch(prop.Name)))
            {
                var value = prop.GetValue(settings);
                if (value is bool boolValue)
                    scanText += "• `" + prop.Name[0..^4] + "` : *" + (boolValue ? "Activated" : "Disabled") + "*\n";
                else
                    scanText += "• `" + prop.Name[0..^4] + "` : *" + value + "*\n";
            }

            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithTitle("Commands states")
                .WithDescription("*Always* mean that the value cannot be changed.")
                .AddField("Randoms", "" +
                    "• `github` or `avan0x` : *Always*" +
                    "", false)
                .AddField("Text scan", scanText, false)
                .AddField("User", "" +
                    "• `info` : *Always*" +
                    "", false)
                .AddField("Admin", "" +
                    "• `mute` : *" + (settings.admin_mute ? "Activated" : "Disabled") + "*" +
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
                var settings = Utils.GetSettings(Context.Guild.Id);

                var type = settings.GetType();
                var props = type.GetProperties();
                var scanText = "";

                bool flag;
                if (Boolean.TryParse(value, out flag))
                {
                    foreach (var prop in props.Where(prop => new Regex("(Scan)$").IsMatch(prop.Name)))
                    {
                        var propValue = prop.GetValue(settings);
                        if (propValue is bool)
                        {
                            prop.SetValue(settings, flag);
                            scanText += "• Value of **" + prop.Name[0..^4] + "** set to *" + flag + "*\n";
                            await Utils.LogAsync("Value of `" + prop.Name[0..^4] + "` set to `" + flag + "` on `" + Context.Guild.Name + "`");
                        } else
                            scanText += "• Value of **" + prop.Name[0..^4] + "** is still *" + propValue + "*\n";
                    }
                    Utils.SaveData();
                }
                else
                {
                    foreach (var prop in props.Where(prop => new Regex("(Scan)$").IsMatch(prop.Name)))
                    {
                        scanText += "• Value of **" + prop.Name[0..^4] + "** is *" + prop.GetValue(settings) + "*\n";
                    }
                }
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription(scanText)
                    .WithColor(255, 241, 185);
                await ReplyAsync("", false, embedMessage.Build());

            }

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
            [Command("reacttouser")]
            [Alias("reactuser")]
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
        // use like that : await SetObject(settingName, bool.TryParse(value, out var flag), flag, Context);
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