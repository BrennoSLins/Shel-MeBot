using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Shel_MeBot
{
    public class Player
    {
        public string? Name { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }
        public int Mind { get; set; }
        public int MaxMind { get; set; }
        public List<string> Info { get; set; } = new();
        public ulong MessageID { get; set; }
        public ulong ChannelID { get; set; }
        public string? CharPic { get; set; }
       

    }
}
