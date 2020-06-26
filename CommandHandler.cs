using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Linq;

namespace AvaBot
{
    public class CommandHandler
    {
        // setup fields to be set later in the constructor
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            // juice up the fields with these services
            // since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            
            // take action when we execute a command
            _commands.CommandExecuted += CommandExecutedAsync;

            // take action when we receive a message (so we can process it, and see if it is a valid command)
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        // this class is where the magic starts, and takes actions upon receiving messages
        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage.Author.IsBot && rawMessage.Author.Id == 503720029456695306) // Is dadbot
                await rawMessage.Channel.SendMessageAsync("Shut up dad !");

            // ensures we don't process system/other bot messages
            if (!(rawMessage is SocketUserMessage message)) 
                return;
            
            if (message.Source != MessageSource.User) 
                return;

            // sets the argument position away from the prefix we set
            var argPos = 0;

            // get prefix from the configuration file
            string prefix = _config["Prefix"];

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasStringPrefix(prefix, ref argPos))) 
            {
                // TODO place this somewhere else
                var msg = message.Content.ToLower();
                if (Program.settings.Get(((SocketGuildChannel)message.Channel).Guild.Id).all)
                {
                    // modpack case
                    if (new Regex("(m(o|0|au|eau)(d|t|s|x|e de)?).{0,5}(p(a|4)(c|q|k))").IsMatch(msg))
                    {
                        await message.Channel.SendMessageAsync("Non ta gueule ! " + message.Author.Mention + "\nRaison : je veux pas jouer à ce modpack !");
                        await message.AddReactionAsync(new Emoji("❌"));
                        return;
                    }

                    // "cheh" case
                    if (new Regex("(ch([eéè]+|ai)h+)").IsMatch(msg))
                    {
                        await message.Channel.SendMessageAsync("Non toi cheh ! " + message.Author.Mention + "\nhttps://tenor.com/blYQK.gif");
                        await message.AddReactionAsync(new Emoji("❌"));
                        return;
                    }

                    // "gf1" case
                    if (new Regex("(gf1|j.?ai.?faim)").IsMatch(msg))
                    {
                        await message.Channel.SendMessageAsync("Moi aussi j'ai faim !");
                        return;
                    }

                    // -ine case
                    var ineList = Regex.Matches(msg, "[a-zA-ZÀ-ÿ]+ine").Cast<Match>().Select(m => m.Value).ToList();
                    int maxPerMsg = 10;
                    if (ineList.Count() > 0)
                    {
                        //await message.Channel.SendMessageAsync("||Debug : " + ineList.Count() + "||");
                        for (int i = 0; i < (ineList.Count() > maxPerMsg ? maxPerMsg : ineList.Count()); i++)
                            await message.Channel.SendMessageAsync("Non ce n'est pas " + ineList[i] + " mais pain " + (new Regex("(s|x)$").IsMatch((ineList[i])[0..^3]) ? "aux" : "au") + " " + (ineList[i])[0..^3] + " !");
                        return;
                    }
                }

                return;
            }
           
            var context = new SocketCommandContext(_client, message);

            // execute command if one is found that matches
            await _commands.ExecuteAsync(context, argPos, _services); 
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // if a command isn't found, log that info to console and exit this method
            if (!command.IsSpecified)
            {
                Console.WriteLine($"Command failed to execute for [{context.User}] <-> [{result.ErrorReason}]!");
                return;
            }
                

            // log success to the console and exit this method
            if (result.IsSuccess)
            {
                Console.WriteLine($"Command [{command.Value.Name}] executed for -> [{context.User}]");
                return;
            }


            // failure scenario, let's let the user know
            Console.WriteLine($"{context.User} something went wrong -> [{result}]!");
            await context.Channel.SendMessageAsync("This command does not exist.");
        }        
    }
}