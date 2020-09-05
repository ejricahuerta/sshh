using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sshhh.Web.Discord
{
    public class DiscordService
    {
        private readonly DiscordSocketClient client;

        private string discordToken;

        private ulong logChannel = 751614757500354581;

        public DiscordService(IOptions<DiscordConfiguration> options)
        {
            client = new DiscordSocketClient();

            client.Log += LogAsync;
            client.Ready += ReadyAsync;
            discordToken = options.Value.Token;

        }

        public async Task MainAsync(string token)
        {
            if (!string.IsNullOrEmpty(token) && !token.Equals(discordToken))
            {
                discordToken = token;
            }

            // Tokens should be considered secret data, and never hard-coded.
            await client.LoginAsync(TokenType.Bot, discordToken);
            await client.StartAsync();
        }

        public async Task ToggleMuteForAllUsersAsync(ulong serverId, string channelName, bool toggle = true)
        {
            SocketGuild guild = client.GetGuild(serverId);
            Console.WriteLine();
            Console.WriteLine("Task: Mute All");
            Console.WriteLine($"Guild Name: {guild.Name}");
            Console.WriteLine($"Guild Channels: {guild.Channels.Count}");

            var channel = guild.VoiceChannels.FirstOrDefault(p => p.Name.Equals(channelName));

            if (channel.Users.Any())
            {

                var names = new List<string>();
                foreach (var user in channel.Users)
                {
                    Console.WriteLine($"User name: {user.Id}");
                    await user.ModifyAsync(p => p.Mute = toggle);
                    names.Add(user.Username);
                }
                string message = $"I just {(toggle ? "muted" : "unmuted")} the following users on {channelName}:\n {string.Join(',', names)} ";
                await LogToChannel(serverId, this.logChannel, message);
            }
        }

        private async Task LogToChannel(ulong serverId, ulong channelId, string message)
        {
            SocketGuild guild = client.GetGuild(serverId);
            Console.WriteLine();
            Console.WriteLine("Task: Log");
            Console.WriteLine($"Guild Name: {guild.Name}");
            Console.WriteLine($"Guild Channels: {guild.Channels.Count}");
            var log = guild.GetTextChannel(channelId);
            await log.SendMessageAsync(message);
        }

        public async Task MoveUsers(ulong serverId, string fromChannel, string toChannel)
        {
            SocketGuild guild = client.GetGuild(serverId);
            Console.WriteLine();
            Console.WriteLine("Task: Mute Users");
            Console.WriteLine($"Guild Name: {guild.Name}");
            Console.WriteLine($"Guild Channels: {guild.Channels.Count}");
            var from = guild.VoiceChannels.FirstOrDefault(p => p.Name.Equals(fromChannel));
            var to = guild.VoiceChannels.FirstOrDefault(p => p.Name.Equals(toChannel));

            var names = new List<string>();

            if (from.Users.Any())
            {

                foreach (var user in from.Users)
                {
                    await user.ModifyAsync(x => x.Channel = to);
                    names.Add(user.Username);
                }
                string message = $"I just moved the following users from {fromChannel} to {toChannel}:\n {string.Join(',', names)} ";
                await LogToChannel(serverId, this.logChannel, message);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }


    }
}
