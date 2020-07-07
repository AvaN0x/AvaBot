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
    [Group("help")]
    [Alias("h")]
    public class HelpCommands : ModuleBase
    {

        [Command]
        public async Task HelpCommand()
        {
            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithTitle("Commands help")
                .WithDescription("" + 
                    "The prefix is `//`." + 
                    "\nUse `//help settings` for informations about setting commands.")
                .AddField("Randoms", "" +
                    "• `github` or `avan0x` : send informations about the bot owner" +
                    "", false)
                .AddField("Text scan", "" +
                    "Those are not commands, they just scan every message posted." + 
                    "\n• `cheh` : answer if the message contains \"cheh\"" +
                    "\n• `gf1` : answer if the message contains \"gf1\" or \"j'ai faim\"" +
                    "\n• `ine` : answer if the message contains a word that end with \"ine\"" +
                    "\n• `reacttouser` : react if an emote exists with the same name as the user" +
                    "", false)
                .AddField("User", "" +
                    "• `info [username]` : give information about the user, or yourself if there is no parameter" +
                    "", false)
                .AddField("Admin", "" +
                    "• `mute [username] [duration in minutes]` : mute the user for the expected time (default 5 minutes)" +
                    "\n• `unmute [username]` : unmute the user" +
                    "", false)
                .WithFooter("github.com/AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            await ReplyAsync("", false, embedMessage.Build());
        }

        [Command("settings")]
        [Alias("set" , "s")]
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