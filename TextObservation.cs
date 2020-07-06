using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AvaBot
{
    static class TextObservation
    {
        public static async Task ScanContent(SocketUserMessage message)
        {
            var settings = Utils.GetSettings(((SocketGuildChannel)message.Channel).Guild.Id);
            var msg = message.Content.ToLower();
            // modpack case
            if (settings.modpackScan && 
                new Regex("(m(o|0|au|eau)(d|t|s|x|e de)?).{0,5}(p(a|4)(c|q|k))").IsMatch(msg))
            {
                await message.Channel.SendMessageAsync("Non ta gueule ! " + message.Author.Mention + "\nRaison : je veux pas jouer à ce modpack !");
                await message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            // "cheh" case
            if (settings.chehScan &&
                    new Regex("(ch([eéè]+|ai)h+)").IsMatch(msg))
            {
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithImageUrl("https://media.tenor.com/images/db5d206d665edc6b77c088da7bba097b/tenor.gif")
                    .WithColor(255, 241, 185);

                await message.Channel.SendMessageAsync("Non toi cheh ! " + message.Author.Mention, false, embedMessage.Build());
                await message.AddReactionAsync(new Emoji("❌"));
                return;
            }

            // "gf1" case
            if (settings.gf1Scan &&
                new Regex("(gf1|j.?ai.?faim)").IsMatch(msg))
            {
                await message.Channel.SendMessageAsync("Moi aussi j'ai faim !");
                return;
            }

            // -ine case
            if (settings.ineScan)
            {
                var ineList = Regex.Matches(msg, "[a-zA-ZÀ-ÿ]+(ine)$").Cast<Match>().Select(m => m.Value).ToList();
                int maxPerMsg = 10;
                if (ineList.Count() > 0)
                {
                    for (int i = 0; i < (ineList.Count() > maxPerMsg ? maxPerMsg : ineList.Count()); i++)
                        await message.Channel.SendMessageAsync("Non ce n'est pas " + ineList[i] + " mais pain " + (new Regex("(s|x)$").IsMatch((ineList[i])[0..^3]) ? "aux" : "au") + " " + (ineList[i])[0..^3] + " !");
                    return;
                }
            }
        }

        public static async Task ScanUser(SocketUserMessage message)
        {
            var settings = Utils.GetSettings(((SocketGuildChannel)message.Channel).Guild.Id);

            // modpack case
            if (settings.reactionToUsername)
            {
                var username = message.Author.Username.ToLower().Replace(" ", "");
                var emote = (Emote)((SocketGuildChannel)message.Channel).Guild.Emotes.FirstOrDefault(e => e.Name.ToLower() == username);
                if (emote != null)
                    await message.AddReactionAsync(emote);
                return;
            }

        }

    }
}
