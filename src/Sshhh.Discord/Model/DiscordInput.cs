using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Sshhh.Discord.Model
{
    public class DiscordInput
    {
        [Required]
        public string From { get; set; }
        public string To { get; set; }
        public bool Mute { get; set; }
    }
}
