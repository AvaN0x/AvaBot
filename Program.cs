using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
	public class Program
	{
		private DiscordSocketClient _client;
		private IConfiguration _config;

		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
			_client = new DiscordSocketClient();

			_client.Log += Log;

			_client.Ready += ReadyAsync;

			_client.MessageReceived += MessageReceivedAsync;

			var _builder = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile(path: "config.json");
			_config = _builder.Build();

			await _client.LoginAsync(TokenType.Bot, _config["Token"]);
			await _client.StartAsync();

			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private Task ReadyAsync()
		{
			Console.WriteLine($"Connected as {_client.CurrentUser}");
			return Task.CompletedTask;
		}

		private async Task MessageReceivedAsync(SocketMessage message)
		{
			// The bot should never respond to itself.
			if (message.Author.Id == _client.CurrentUser.Id)
				return;
			if (message.Author.IsBot)
			{
				if (message.Author.Id == 503720029456695306) // Is dadbot
					await message.Channel.SendMessageAsync("Shut up dad !");
				return;
			}

			var msg = message.Content.ToLower();

			if ((msg.Contains("mod") || msg.Contains("maud") || msg.Contains("meaud") || msg.Contains("m0d")
					|| msg.Contains("mot") || msg.Contains("maut") || msg.Contains("meaut") || msg.Contains("m0t")
					|| msg.Contains("mox") || msg.Contains("maux") || msg.Contains("meaux") || msg.Contains("m0x")
					|| msg.Contains("mos") || msg.Contains("maus") || msg.Contains("meaus") || msg.Contains("m0s"))
					&&
					(msg.Contains("pac") || msg.Contains("paq") || msg.Contains("pak"))
				)
			{
				await message.Channel.SendMessageAsync("Non ta gueule ! " + message.Author.Mention + "\nRaison : je veux pas jouer à ce modpack !");
				await message.AddReactionAsync(new Emoji("❌"));
			}

			if (msg.Contains("cheh"))
			{
				await message.DeleteAsync();
				await message.Channel.SendMessageAsync("Non toi cheh ! " + message.Author.Mention);
			}
		}
	}
}
