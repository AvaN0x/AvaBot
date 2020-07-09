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
using System.Globalization;

namespace AvaBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    [Summary("😁 Random Commands")]
    public class RandomCommands : ModuleBase
    {

        [Command("info")]
        [Alias("me")]
        [Summary("Get informations about an user")]
        public async Task InfoCommand([Summary("The optional user to get informations from")][Remainder]SocketGuildUser user = null)
        {
            if (user == null)
                user = (SocketGuildUser)Context.User;

            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithTitle(user.Username + " informations")
                .AddField("Global", "" +
                    "• ToString() : *" + user.ToString() + "*" +
                    "\n• CreatedAt : *" + user.CreatedAt.ToString("F", DateTimeFormatInfo.InvariantInfo) + "*" +
                    (user.Activity == null ? "" : "\n• Activity : *" + user.Activity + "*") +
                    "\n• Status : *" + user.Status + "*" +
                    "\n• GetDefaultAvatarUrl() : *[link](" + user.GetDefaultAvatarUrl() + ")*" +
                    "\n• GetAvatarUrl() : *[link](" + user.GetAvatarUrl() + ")*" +
                    "", false)
                .AddField("Guild", "" +
                    "\n• Guild : *" + user.Guild + "*" +
                    (String.IsNullOrEmpty(user.Nickname) ? "" : "\n• Nickname : *" + user.Nickname + "*") +
                    "\n• JoinedAt : *" + ((DateTimeOffset) user.JoinedAt).ToString("F", DateTimeFormatInfo.InvariantInfo) + "*" + 
                    "", false)
                .WithFooter("github.com/AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            await ReplyAsync("", false, embedMessage.Build());
        }

        [Command("github")]
        [Alias("avan0x", "owner")]
        [Summary("Give informations about the bot owner")]
        public async Task GitHubCommand()
        {
            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithTitle("AvaN0x - Clément RICATTE")
                .AddField("Github", "[github.com/AvaN0x](https://github.com/AvaN0x)", false)
                .AddField("WebSite", "[avan0x.github.io](https://avan0x.github.io/)", false)    // true - for inline
                .WithFooter("AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithThumbnailUrl("https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            await ReplyAsync("", false, embedMessage.Build());
            await Context.Message.DeleteAsync();
        }

        [Command("timer", RunMode = RunMode.Async)]
        [Summary("Setup a message that will be sent in X seconds")]
        public async Task TimerCommand([Summary("The duration in seconds")]int duration)
        {
            var date = DateTime.Now.AddSeconds(duration);
            await ReplyAsync(DateTime.Now.ToString("T") + " : Date set to " + date.ToString("T"));
            while (DateTime.Now <= date)
                await Task.Delay(1);
            await ReplyAsync(DateTime.Now.ToString("T") + " : " + Context.User.Mention + " : " + duration + " seconds have passed");
        }

        //[Command("test"]
        //[Summary("A command for tests")]
        //public async Task TestCommand()
        //{

        //}
    }
}