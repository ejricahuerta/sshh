using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sshhh.Discord
{
    public class DiscordConfiguration
    {
        public ulong  DefaultServerId { get; set; }
        public ulong  DefaultLogChannel { get; set; }
        public string Token { get; set; }
    }
}
