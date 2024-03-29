﻿using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AvaBot
{
    internal static class TextObservation
    {
        public static async Task ScanContent(SocketUserMessage message)
        {
            var settings = Utils.GetSettings(((SocketGuildChannel)message.Channel).Guild.Id);
            var msg = message.Content.ToLower();

            // "cheh" case
            // TODO delete that when I'm changing the GuildSetting class
            if (settings.chehScan &&
                    new Regex("((c([_*~]*)(h|(\\|-\\|))([&eéè_*~]+)(h|(\\|-\\|))+)|(^c([_*~]+)(h|(\\|-\\|))[eéè_*~]*$))").IsMatch(msg))
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

            // react to user message case
            if (settings.reactToUserScan)
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