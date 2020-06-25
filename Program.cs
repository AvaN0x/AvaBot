using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
		private Settings settings;

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

			settings = new Settings();

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
			var actualGuild = settings.Get(((SocketGuildChannel)message.Channel).Guild.Id);
			var ui = ((SocketGuildChannel)message.Channel).Guild.Owner == message.Author;
			// commands
			{
				if (msg.StartsWith("//" + "embed"))
				{
					EmbedBuilder embedMessage = new EmbedBuilder();

					embedMessage.WithTitle("AvaN0x - Clément RICATTE")
						.AddField("Github", "[github.com/AvaN0x](https://github.com/AvaN0x)", false)
						.AddField("WebSite", "[avan0x.github.io](https://avan0x.github.io/)", false)    // true - for inline
						.WithFooter("AvaN0x", "https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
						.WithThumbnailUrl("https://avatars3.githubusercontent.com/u/27494805?s=460&v=4")
						.WithColor(255, 241, 185);
					
					await message.Channel.SendMessageAsync("", false, embedMessage.Build());
					await message.DeleteAsync();
				}
			}
			if (message.Author == ((SocketGuildChannel)message.Channel).Guild.Owner)
			{
				if (msg.StartsWith("//" + "true"))
				{
					actualGuild.all = true;
					settings.SaveSettings();
					await message.Channel.SendMessageAsync("all value : " + actualGuild.all);
				}
				if (msg.StartsWith("//" + "false"))
				{
					actualGuild.all = false;
					settings.SaveSettings();
					await message.Channel.SendMessageAsync("all value : " + actualGuild.all);
				}
				if (msg.StartsWith("//" + "all"))
				{
					await message.Channel.SendMessageAsync("all value : " + actualGuild.all);
				}
			}


			

			if (actualGuild.all)
			{
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
}
