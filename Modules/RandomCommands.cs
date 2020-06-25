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
    public class RandomCommands : ModuleBase
    {
        [Command("createdat")]
        public async Task CreatedAtCommand()
            => await ReplyAsync(Context.User + " where created on " + Context.User.CreatedAt);

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

    }
}