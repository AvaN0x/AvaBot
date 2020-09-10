using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AvaBot.Modules
{
    [Summary("⏱ Reminder Commands")]
    public class ReminderCommands : ModuleBase
    {
        private readonly IConfiguration _config;

        public ReminderCommands(IConfiguration config)
        {
            _config = config;
        }

        [Command("reminder")]
        [Summary("Make the bot remind you/them something")]
        public async Task RappelCommand(
                                      [Summary("The role to ping when reminder hits")] SocketRole role,
                                      [Summary("The date to execute the remind. Format dd/mm/yyyy-hh:mm")] string dateString,
                                      [Remainder][Summary("The subject of the remind")] string subject
            ) => await Reminder(role, dateString, subject);

        [Command("reminder")]
        [Summary("Make the bot remind him/her something")]
        public async Task RappelCommand(
                                    [Summary("The user to ping when reminder hits")] SocketGuildUser user,
                                    [Summary("The date to execute the remind. Format dd/mm/yyyy-hh:mm")] string dateString,
                                    [Remainder][Summary("The subject of the remind")] string subject
            ) => await Reminder(user, dateString, subject);

        [Command("reminder")]
        [Summary("Make the bot remind you something")]
        public async Task RappelCommand(
                                    [Summary("The date to execute the remind. Format dd/mm/yyyy-hh:mm")] string dateString,
                                    [Remainder][Summary("The subject of the remind")] string subject
            ) => await Reminder((SocketGuildUser)Context.User, dateString, subject);

        public async Task Reminder(IMentionable mention, string dateString, string subject)
        {
            var date = dateString.Split('-')[0].Split('/');
            var time = dateString.Split('-')[1].Split(':');
            var dateTime = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]), int.Parse(time[0]), int.Parse(time[1]), 5);

            if ((dateTime - DateTime.Now).Ticks > 0)
            {
                var message = await ReplyAsync($"I'll remind {mention} to {subject} at {dateTime}");
                await Task.Delay(5000); // 5 seconds
                await message.DeleteAsync();
                while (DateTime.Now <= dateTime)
                    await Task.Delay(10000);
                await ReplyAsync($"Hey {mention.Mention}, you need to {subject} !");
            }
            else
                await ReplyAsync("You can't wait for a past date !");
        }
    }
}