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

        public async Task ToggleMuteForAllUsersAsync(ulong serverId, string channelName, bool toggle = false)
        {
            SocketGuild guild = client.GetGuild(serverId);
            Console.WriteLine();
            Console.WriteLine("Task: Mute All");
            Console.WriteLine($"Guild Name: {guild.Name}");
            Console.WriteLine($"Guild Channels: {guild.Channels.Count}");

            var channel = guild.VoiceChannels.FirstOrDefault(p => p.Name.Equals(channelName));

            foreach (var user in channel.Users)
            {
                Console.WriteLine($"User name: {user.Id}");
                await user.ModifyAsync(p => p.Mute = toggle);
            }
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
            foreach (var user in from.Users)
            {
                await user.ModifyAsync(x => x.Channel = to);
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
