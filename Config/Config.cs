using System;
using System.Collections.Generic;
using System.Text;

namespace Shel_MeBotDB
{
    public class Config
    {
        public string Token { get; set; } = "";
        public ulong SitAtualId { get; set; }
        public ulong PlayerRequestId { get; set; }
    }

    public enum ResourceType
    {
        hp,
        mp,
        mind,
        ea

    }


}
