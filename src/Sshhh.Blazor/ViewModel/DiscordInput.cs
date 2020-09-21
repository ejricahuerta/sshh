using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sshhh.Blazor.ViewModel
{
    public class DiscordInput
    {
        [Required]
        public ulong ServerId { get; set; }
        [Required]
        public string From { get; set; }
        public string To { get; set; }
        public bool Mute { get; set; }
    }
}
