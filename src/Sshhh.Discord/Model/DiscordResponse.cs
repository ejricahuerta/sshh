using System;
using System.Collections.Generic;
using System.Text;

namespace Sshhh.Discord.Model
{
    public class DiscordResponse
    {
        public ResponseType ResponseType { get; set; }
        public string ResponseName
        {
            get
            {
                return nameof(ResponseType);
            }
        }
        public string ResponseMessage { get; set; }
    }
    public enum ResponseType
    {
        Success, Error
    }
}
