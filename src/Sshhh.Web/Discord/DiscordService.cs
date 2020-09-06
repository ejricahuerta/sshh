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

            if (client.ConnectionState.Equals(ConnectionState.Disconnected))
            {
                await client.LoginAsync(TokenType.Bot, discordToken);
                await client.StartAsync();
            }
        }

        public async Task ToggleMuteForAllUsersAsync(ulong serverId, string channelName, bool toggle = true)
        {
            SocketGuild guild = client.GetGuild(serverId);
            Console.WriteLine();
            Console.WriteLine("Task: Mute All");
            Console.WriteLine($"Guild Name: {guild.Name}");

            var channel = guild.VoiceChannels.FirstOrDefault(p => p.Name.Equals(channelName));

            if (channel.Users.Any())
            {

                var names = new List<string>();

                var tasks = new List<Task>();
                foreach (var user in channel.Users)
                {
                    Console.WriteLine($"User name: {user.Id}");
                    tasks.Add(user.ModifyAsync(p => p.Mute = toggle));
                    names.Add(user.Mention);
                }

                while (tasks.Any())
                {
                    Task finished = await Task.WhenAny(tasks);
                    tasks.Remove(finished);
                }

                string message = $"I just {(toggle ? "muted" : "unmuted")} the following users on {channelName}:\n {string.Join("\n", names)}\n I'm not spamming guys...blame EXD!";
                await LogToChannel(serverId, this.logChannel, message);
            }
        }

        public async Task FlushChannelLogs(ulong serverId)
        {
            SocketGuild guild = client.GetGuild(serverId);
            Console.WriteLine();
            Console.WriteLine("Task: Clear Logs");
            Console.WriteLine($"Guild Name: {guild.Name}");
            var channel = guild.GetTextChannel(this.logChannel);
            var text = await channel.GetMessagesAsync(100).FlattenAsync();

            await channel.DeleteMessagesAsync(text);

            string message = $"Okay okay okay! Relax, I just deleted previous messages. Don't hate me. blame EXD";
            await LogToChannel(serverId, this.logChannel, message);
        }

        private async Task LogToChannel(ulong serverId, ulong channelId, string message)
        {
            SocketGuild guild = client.GetGuild(serverId);
            Console.WriteLine();
            Console.WriteLine("Task: Log to Channel");
            Console.WriteLine($"Guild Name: {guild.Name}");
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


            if (from.Users.Any())
            {

                var names = new List<string>();
                var tasks = new List<Task>();
                foreach (var user in from.Users)
                {
                    tasks.Add(user.ModifyAsync(x => x.Channel = to));
                    names.Add(user.Mention);
                }
                while (tasks.Any())
                {
                    Task finished = await Task.WhenAny(tasks);
                    tasks.Remove(finished);
                }

                string message = $"I just moved the following users from {fromChannel} to {toChannel}:\n {string.Join("\n", names)} \n I'm not spamming....blame EXD!";
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
