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
        public static async Task Scan(SocketUserMessage message)
        {
            var msg = message.Content.ToLower();
            if (Program.settings.Get(((SocketGuildChannel)message.Channel).Guild.Id).chehScan)
            {
                // modpack case
                if (Program.settings.Get(((SocketGuildChannel)message.Channel).Guild.Id).modpackScan && 
                    new Regex("(m(o|0|au|eau)(d|t|s|x|e de)?).{0,5}(p(a|4)(c|q|k))").IsMatch(msg))
                {
                    await message.Channel.SendMessageAsync("Non ta gueule ! " + message.Author.Mention + "\nRaison : je veux pas jouer à ce modpack !");
                    await message.AddReactionAsync(new Emoji("❌"));
                    return;
                }

                // "cheh" case
                if (Program.settings.Get(((SocketGuildChannel)message.Channel).Guild.Id).chehScan &&
                     new Regex("(ch([eéè]+|ai)h+)").IsMatch(msg))
                {
                    await message.Channel.SendMessageAsync("Non toi cheh ! " + message.Author.Mention + "\nhttps://tenor.com/blYQK.gif");
                    await message.AddReactionAsync(new Emoji("❌"));
                    return;
                }

                // "gf1" case
                if (Program.settings.Get(((SocketGuildChannel)message.Channel).Guild.Id).gf1Scan &&
                    new Regex("(gf1|j.?ai.?faim)").IsMatch(msg))
                {
                    await message.Channel.SendMessageAsync("Moi aussi j'ai faim !");
                    return;
                }

                // -ine case
                if (Program.settings.Get(((SocketGuildChannel)message.Channel).Guild.Id).ineScan)
                {
                    var ineList = Regex.Matches(msg, "[a-zA-ZÀ-ÿ]+ine").Cast<Match>().Select(m => m.Value).ToList();
                    int maxPerMsg = 10;
                    if (ineList.Count() > 0)
                    {
                        for (int i = 0; i < (ineList.Count() > maxPerMsg ? maxPerMsg : ineList.Count()); i++)
                            await message.Channel.SendMessageAsync("Non ce n'est pas " + ineList[i] + " mais pain " + (new Regex("(s|x)$").IsMatch((ineList[i])[0..^3]) ? "aux" : "au") + " " + (ineList[i])[0..^3] + " !");
                        return;
                    }
                }
            }
        }
    }
}
