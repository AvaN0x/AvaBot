using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace AvaBot.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    [Summary("📋 Help Commands")]
    //[Group("help")]
    //[Alias("h")]
    public class HelpCommands : ModuleBase
    {
        private readonly CommandService _commands;
        private readonly IConfiguration _config;

        public HelpCommands(CommandService commands, IConfiguration config)
        {
            _commands = commands;
            _config = config;
        }

        [Command("help")]
        [Alias("h")]
        [Summary("Show every commands available")]
        public async Task HelpCommand()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle("Commands help")
                .WithDescription("The actual prefix is `" + _config["Prefix"] + "`." +
                    "\nFor more informations about a command, use `" + _config["Prefix"] + "help [command]`.")
                .WithFooter("github.com/AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            //TODO : Don't add multiple times the same command
            foreach (var module in _commands.Modules.Where(m => !m.Name.ToLower().Contains("help")))
                embedBuilder.AddField(module.Summary ?? module.Name,
                    string.Join(", ", module.Commands.Select(c => "`" + c.Name + "`")));
            await ReplyAsync("", false, embedBuilder.Build());
        }

        [Command("help")]
        [Alias("h")]
        [Summary("Show informations about a command")]
        public async Task HelpCommand([Summary("The command to explain")] string commandName)
        {
            var commands = _commands.Commands.Where(c => c.Name == commandName.ToLower());
            if (commands.Count() == 0)
            {
                EmbedBuilder embedMessage = new EmbedBuilder()
                    .WithDescription("There are no commands with that name.")
                    .WithColor(255, 0, 0);
                var errorMessage = await ReplyAsync("", false, embedMessage.Build());
                await Task.Delay(5000); // 5 seconds
                await errorMessage.DeleteAsync();
                return;
            }

            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithFooter("github.com/AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
                .WithColor(255, 241, 185);

            foreach (var cmd in commands)
            {
                embedBuilder.AddField("Command " + cmd.Name, "" +
                    string.Join("\n", cmd.Aliases.Select(a => "`" + _config["Prefix"] + a + string.Join("", cmd.Parameters.Select(p => " [" + p.Name + "]")) + "`")) +
                    "\n__Summary :__ \n" +
                    "*" + (cmd.Summary ?? "No description available") + "*\n" +
                    (cmd.Parameters.Count() > 0
                        ? "__Parameters :__ " + string.Concat(cmd.Parameters.Select(p => "\n[" + p.Name + "] : *" + (p.Summary ?? "No description available") + "*")) + "\n"
                        : "") +
                    "");
            }
            await ReplyAsync("", false, embedBuilder.Build());
        }
    }
}