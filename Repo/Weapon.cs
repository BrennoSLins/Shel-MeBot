using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Shel_MeBotDB
{
    public class Weapon
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int State { get; set; }
        public string? WImg { get; set; }
        public string? ShImg { get; set; }
        public string? BaImg { get; set; }
        public ulong MessageID { get; set; }
        public ulong ChannelID { get; set; }
        public int Amount { get; set; } = 1;

        

    }
}
