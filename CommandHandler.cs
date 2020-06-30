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

            if(Utils.GetSettings(((SocketGuildChannel)message.Channel).Guild.Id).admin_mute &&
                Utils.GetSettings(((SocketGuildChannel)message.Channel).Guild.Id).IsMuted(message.Author.Id))
            {
                await message.Author.SendMessageAsync("Your message go deleted on *" + ((SocketGuildChannel)message.Channel).Guild.Name + "* : " + "\n> " + message.Content);
                await message.DeleteAsync();
                if (((SocketGuildChannel)message.Channel).Guild.OwnerId != message.Author.Id) // so the owner can unmute himself
                    return;
            }


            // sets the argument position away from the prefix we set
            var argPos = 0;

            // get prefix from the configuration file
            string prefix = _config["Prefix"];

            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasStringPrefix(prefix, ref argPos)))
            {
                if (message.Channel is SocketGuildChannel channel)
                    await TextObservation.Scan(message);
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
                await Utils.LogAsync($"Command failed to execute for [{context.User}] <-> [{result.ErrorReason}]!", "Error");
                return;
            }
                

            // log success to the console and exit this method
            if (result.IsSuccess)
            {
                await Utils.LogAsync($"Command [{command.Value.Name}] executed for -> [{context.User}]");
                return;
            }


            // failure scenario, let's let the user know
            await Utils.LogAsync($"{context.User} something went wrong -> [{result}]!", "Error");
            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithDescription("This command does not exist.")
                .WithColor(255, 0, 0);
            await context.Channel.SendMessageAsync("", false, embedMessage.Build());
        }        
    }
}