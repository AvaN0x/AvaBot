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
    public class RandomCommands : ModuleBase
    {

        [Command("info")]
        [Alias("me")]
        public async Task InfoCommand(SocketGuildUser user = null)
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
        [Alias("avan0x")]
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
        public async Task TestCommand(int duree)
        {
            var date = DateTime.Now.AddSeconds(duree);
            await ReplyAsync(DateTime.Now.ToString("T") + " : Date set to " + date.ToString("T"));
            while (DateTime.Now <= date)
                await Task.Delay(1);
            await ReplyAsync(DateTime.Now.ToString("T") + " : " + Context.User.Mention + " ça devrait faire " + duree + " secondes");
        }

        //[Command("test")]
        //public async Task TestCommand()
        //{
        //    //foreach (var m in ((SocketGuild)Context.Guild).Users)
        //    //    await m.SendMessageAsync("Quoi de pire qu'un ping? Un mp.");

        //    //await ((SocketGuild)Context.Guild).GetUser(261255005162438656).SendMessageAsync("GF1 sacam");
        //    //await Context.User.SendMessageAsync("Quoi de pire qu'un ping? Un mp.");
            
        //    //var emote = (Emote)Context.Guild.Emotes.FirstOrDefault(e => e.Name.ToLower() == Context.User.Username.ToLower());
        //    //if (emote != null)
        //    //    await Context.Message.AddReactionAsync(emote);


        //    //var emote = Emote.Parse("<:boulax:727532649215819806>");
        //    //await Context.Message.AddReactionAsync(emote);

        //}

    }
}