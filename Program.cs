using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AvaBot
{
    internal class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;

        private static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            // create and build configuration to access the json file
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json");

            _config = _builder.Build();
        }

        public async Task MainAsync()
        {
            // call ConfigureServices to create the ServiceCollection/Provider for passing around the services
            using var services = ConfigureServices();
            // get the client from GetRequiredService<T>
            var client = services.GetRequiredService<DiscordSocketClient>();
            _client = client;

            // setup logging and the ready event
            client.Log += Utils.LogAsync;
            client.Ready += ReadyAsync;
            services.GetRequiredService<CommandService>().Log += Utils.LogAsync;

            // login and start the bot
            await client.LoginAsync(TokenType.Bot, _config["Token"]);
            await client.StartAsync();

            // get the CommandHandler and call the InitializeAsync method to start using it
            await services.GetRequiredService<CommandHandler>().InitializeAsync();

            // set the game activity
            await _client.SetGameAsync("github.com/AvaN0x", "", ActivityType.Watching);

            Utils.Init();

            // let the bot run until the window is closed
            await Task.Delay(-1);
        }

        // log when the user is ready
        private Task ReadyAsync()
        {
            Utils.LogAsync("Connected as " + _client.CurrentUser + " on " + _client.Guilds.Count + " servers");
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            // setup everything that the service will contain
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}