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
    [Summary("⚙️ Settings Commands")]
    [RequireAdminRole(Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public class SettingsCommands : ModuleBase
    {
        [Command("enable")]
        [Summary("Enable a command")]
        public async Task EnableCommand([Summary("Command name. Use \"all\" to select all of them.")]string command = null)
        {
            await AbleCommand(command, true);
        }

        [Command("disable")]
        [Summary("Disable a command")]
        public async Task DisableCommand([Summary("Command name. Use \"all\" to select all of them.")]string command = null)
        {
            await AbleCommand(command, false);
        }

        private async Task AbleCommand(string command, bool value)
        {
            var settings = Utils.GetSettings(Context.Guild.Id);
            var type = settings.GetType();
            var props = type.GetProperties().Where(p => p.GetValue(settings) is bool);

            if (command != null)
            {
                var focused = props.Where(p => command == "all" ? true : p.Name.ToLower() == command.ToLower());
                bool anyChange = false;
                foreach (var f in focused)
                {
                    if (!f.GetValue(settings).Equals(value))
                    {
                        if (command == "admin_mute" && !value) // unmute everyone if needed
                        {
                            Utils.GetSettings(Context.Guild.Id).muted.Clear();
                            await Utils.LogAsync("Every muted got unmuted on `" + Context.Guild.Name + "`");
                        }
                        f.SetValue(settings, value);
                        await Utils.LogAsync(f.Name + " " + (value ? "enabled" : "disabled") + " on `" + Context.Guild.Name + "`");
                        anyChange = true;
                    }
                }
                if (anyChange) Utils.SaveData(); // save only if changements were made
            }

            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithTitle("Commands states")
                .WithDescription(string.Join(", ", props.Select(p => (bool)p.GetValue(settings) ? "*" + p.Name + "*" : "~~`" + p.Name + "`~~")))
                .WithFooter("github.com/AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            await ReplyAsync("", false, embedMessage.Build());

        }


        [Command("values")]
        [Summary("A command that give you every value for the different commands")]
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

        // //s role [role name/id/mention]
        [Command("adminrole")]
        [Summary("set the needed role to access setting and admin commands")]
        [RequireOwner] // Only the owner can change the role
        public async Task SetAdminRoleCommand([Summary("Role mention/name/id to set")]SocketRole role = null)
        {
            if (role != null)
                await SetObject("adminRoleId", true, role.Id, Context);
            else
                await SetObject("adminRoleId", false, null, Context);
        }

        private static async Task SetObject(string settingName, bool tryparse, object flag, ICommandContext context)
        // use like : await SetObject(settingName, bool.TryParse(boolean, out var flag), flag, Context);
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
                if (focused.GetValue(settings).Equals(flag))
                {
                    embedMessage = new EmbedBuilder()
                        .WithDescription("Value of **" + settingName + "** is already *" + flag + "*")
                        .WithColor(255, 241, 185);
                } 
                else
                {
                    focused.SetValue(settings, flag);
                    await Utils.LogAsync("Value of `" + settingName + "` set to `" + flag + "` on `" + context.Guild.Name + "`");
                    Utils.SaveData();
                    embedMessage = new EmbedBuilder()
                        .WithDescription("Value of **" + settingName + "** set to *" + flag + "*")
                        .WithColor(255, 241, 185);
                }
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