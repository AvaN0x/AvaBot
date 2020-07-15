using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AvaBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    [Summary("😁 Random Commands")]
    public class RandomCommands : ModuleBase
    {
        [Command("info")]
        [Alias("me")]
        [Summary("Get informations about an user")]
        public async Task InfoCommand([Summary("The optional user to get informations from")][Remainder] SocketGuildUser user = null)
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
                    "\n• JoinedAt : *" + ((DateTimeOffset)user.JoinedAt).ToString("F", DateTimeFormatInfo.InvariantInfo) + "*" +
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
        }

        [Command("timer", RunMode = RunMode.Async)]
        [Summary("Setup a message that will be sent in X seconds")]
        public async Task TimerCommand([Summary("The duration in seconds")] int duration)
        {
            var date = DateTime.Now.AddSeconds(duration);
            await ReplyAsync(DateTime.Now.ToString("T") + " : Date set to " + date.ToString("T"));
            while (DateTime.Now <= date)
                await Task.Delay(1);
            await ReplyAsync(DateTime.Now.ToString("T") + " : " + Context.User.Mention + " : " + duration + " seconds have passed");
        }

        [Command("say")]
        [Summary("Make the bot say whatever you want")]
        public async Task SayCommand([Remainder][Summary("The text to say")] string text)
        {
            await ReplyAsync(text);
            await Context.Message.DeleteAsync();
        }

        [Command("gif")]
        [Summary("A command to get a random gif")]
        public async Task GifCommand([Remainder][Summary("Tag to search")] string tag)
        {
            tag = tag.Replace(" ", "+");
            var httpClient = await new HttpClient().GetAsync("http://api.giphy.com/v1/gifs/random?api_key=9teQ63i5YhQvzlaMYhv4GwpCGtVrwqG3&tag=" + tag);
            var builder = new ConfigurationBuilder().AddJsonStream(httpClient.Content.ReadAsStreamAsync().Result)
                .Build();
            var gifLink = builder.GetSection("data").GetSection("images").GetSection("downsized").GetSection("url").Value;

            if (!string.IsNullOrWhiteSpace(gifLink))
            {
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithImageUrl(gifLink)
                    .WithColor(255, 241, 185);
                await ReplyAsync("", false, embedMessage.Build());
            }
            else
            {
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription("No gif found")
                    .WithColor(255, 0, 0);
                var errorMessage = await ReplyAsync("", false, embedMessage.Build());
                await Task.Delay(5000); // 5 seconds
                await errorMessage.DeleteAsync();
            }
        }

        [Command("catgif")]
        [Alias("cat")]
        [Summary("Cats, cats everywhere")]
        public async Task CatGifCommand()
        {
            await GifCommand("cat");
        }

        //[Command("test")]
        //[Summary("A command for tests")]
        //public async Task TestCommand()
        //{
        //}
    }
}