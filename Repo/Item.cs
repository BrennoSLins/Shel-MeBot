using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Shel_MeBotDB
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; } = 1;
        public string? ImageUrl { get; set; }
        public ulong MessageID { get; set; }
        public ulong ChannelID { get; set; }
        public string Emoji { get; set; }
       

    }
}
