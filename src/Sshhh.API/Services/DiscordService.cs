using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Sshhh.Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sshhh.API.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient client;

        public DiscordConfiguration Configuration { get; set; }


        public DiscordService()
        {
            client = new DiscordSocketClient();
            client.Log += LogAsync;
            client.Ready += ReadyAsync;
        }

        public async Task Configure(DiscordConfiguration configuration)
        {
            
            if (configuration == null && string.IsNullOrEmpty(configuration.Token))
            {
                throw new Exception("Invalid Configuration");
            }
            Configuration = configuration;

            try
            {
                await client.LoginAsync(TokenType.Bot, Configuration.Token,true);
            }
            catch (Exception e)
            {

                throw new Exception("Invalid Configuration", e);
            }
        }


        public ConnectionState GetConnectionStateAsync()
        {
            return client.ConnectionState;
        }

        public async Task MainAsync()
        {

            if (string.IsNullOrEmpty(Configuration.Token))
            {
                throw new Exception("Invalid Discord Token. Please update the token on the Configuration Page");
            }

            if (client.ConnectionState.Equals(ConnectionState.Disconnected))
            {
                await client.LoginAsync(TokenType.Bot, Configuration.Token);
                await client.StartAsync();
            }
        }

        public async Task ToggleMuteForAllUsersAsync(string channelName, bool toggle = true)
        {
            SocketGuild guild = client.GetGuild(Configuration.DefaultServerId);
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
                    if ((!user.IsMuted && toggle) || (user.IsMuted && !toggle))
                    {
                        tasks.Add(user.ModifyAsync(p => p.Mute = toggle));
                        names.Add(user.Mention);
                    }
                }

                while (tasks.Any())
                {
                    Task finished = await Task.WhenAny(tasks);
                    tasks.Remove(finished);
                }

                if (names.Any())
                {
                    string message = $"I just {(toggle ? "muted" : "unmuted")} the following users on {channelName}:\n {string.Join("\n", names)}\n I'm not spamming guys...blame EXD!";
                    await LogToChannel(message);
                }
            }
        }

        public async Task FlushChannelLogs()
        {
            SocketGuild guild = client.GetGuild(Configuration.DefaultServerId);
            Console.WriteLine();
            Console.WriteLine("Task: Clear Logs");
            Console.WriteLine($"Guild Name: {guild.Name}");
            var channel = guild.GetTextChannel(Configuration.DefaultLogChannel);
            var text = await channel.GetMessagesAsync(100).FlattenAsync();

            if (text.Any())
            {
                await channel.DeleteMessagesAsync(text);

                string message = $"Okay okay okay! Relax, I just deleted previous messages. Don't hate me. blame EXD";
                await LogToChannel(message);
            }
        }

        private async Task LogToChannel(string message)
        {
            SocketGuild guild = client.GetGuild(Configuration.DefaultServerId);
            Console.WriteLine();
            Console.WriteLine("Task: Log to Channel");
            Console.WriteLine($"Guild Name: {guild.Name}");
            var log = guild.GetTextChannel(Configuration.DefaultLogChannel);
            await log.SendMessageAsync(message);
        }

        public async Task MoveUsers(string fromChannel, string toChannel)
        {
            SocketGuild guild = client.GetGuild(Configuration.DefaultServerId);
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

                if (names.Any())
                {
                    string message = $"I just moved the following users from {fromChannel} to {toChannel}:\n {string.Join("\n", names)} \n I'm not spamming....blame EXD!";
                    await LogToChannel(message);
                }
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
