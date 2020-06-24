using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

			await _client.SetGameAsync("github.com/AvaN0x", "", ActivityType.Watching);

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

			// Is dadbot
			if (message.Author.IsBot)
			{
				if (message.Author.Id == 503720029456695306)
					await message.Channel.SendMessageAsync("Shut up dad !");
				return;
			}

			var msg = message.Content.ToLower();

			{
				// modpack case
				if (new Regex("(m(o|0|au|eau)(d|t|s|x|e de)?).{0,5}(p(a|4)(c|q|k))").IsMatch(msg))
				{
					await message.Channel.SendMessageAsync("Non ta gueule ! " + message.Author.Mention + "\nRaison : je veux pas jouer à ce modpack !");
					await message.AddReactionAsync(new Emoji("❌"));
					return;
				}
			}
			{	
				// "cheh" case
				if (new Regex("(ch([eéè]+|ai)h+)").IsMatch(msg))
				{
					await message.Channel.SendMessageAsync("Non toi cheh ! " + message.Author.Mention);
					await message.AddReactionAsync(new Emoji("❌"));
					return;
				}
			}
			{	
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
		}
	}
}
