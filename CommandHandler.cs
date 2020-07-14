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
        private readonly IConfiguration _config;
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            // get services needed from the IServiceProvider
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            
            // set method used when we executed a command
            _commands.CommandExecuted += CommandExecutedAsync;

            // set method used when a message is recieved
            _client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage.Author.IsBot && rawMessage.Author.Id == 503720029456695306) // Is dadbot
                await rawMessage.Channel.SendMessageAsync("Shut up dad !");

            // only accept User messages and not bots
            if (!(rawMessage is SocketUserMessage message)) 
                return;
            // verify if the source of the message is not the client
            if (message.Source != MessageSource.User) 
                return;

            if (message.Channel is SocketGuildChannel channel)
            {
                var guild = channel.Guild;
                if (Utils.GetSettings(guild.Id).admin_mute &&
                    Utils.GetSettings(guild.Id).IsMuted(message.Author.Id))
                {
                    await message.Author.SendMessageAsync("Your message go deleted on *" + guild.Name + "* : " + "\n> " + message.Content);
                    await message.DeleteAsync();
                    await Utils.LogAsync("Message from " + message.Author + " got removed on `" + guild.Name + "`");
                    if (guild.OwnerId != message.Author.Id) // so the owner can unmute himself
                        return;
                }
            }


            // var for the arguments position
            var argPos = 0;
            // determine if the message has a valid prefix, and adjust argPos based on prefix
            if (!(message.HasStringPrefix(_config["Prefix"], ref argPos)))
            {
                if (message.Channel is SocketGuildChannel)
                {
                    await TextObservation.ScanContent(message);
                    await TextObservation.ScanUser(message);
                }
                return;
            }
           
            var context = new SocketCommandContext(_client, message);

            // execute command if one is found that matches
            await _commands.ExecuteAsync(context, argPos, _services); 
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // case if a command isn't found
            if (!command.IsSpecified)
            {
                await Utils.LogAsync($"Command failed to execute for [{context.User}] on [{context.Guild.Name}] <-> [{result.ErrorReason}]!", "Error");
                return;
            }
                

            // case if the command is a success
            if (result.IsSuccess)
            {
                await Utils.LogAsync($"Command [{command.Value.Name}] executed for [{context.User}] on [{context.Guild.Name}]");
                return;
            }


            // other cases
            await Utils.LogAsync($"{context.User} something went wrong : [{result}]!", "Error");
            EmbedBuilder embedMessage = new EmbedBuilder()
                .WithDescription("This command does not exist or you can't use it.")
                .WithColor(255, 0, 0);
            var errorMessage = await context.Channel.SendMessageAsync("", false, embedMessage.Build());
            await Task.Delay(5000); // 5 seconds
            await errorMessage.DeleteAsync();
        }        
    }
}