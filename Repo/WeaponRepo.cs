using Shel_MeBotDB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.IO;

namespace Shel_MeBotDB
{
    public class WeaponRepo
    {
        public static async Task<List<Weapon>> Load()
        {
            if (!File.Exists("Weaponlist.json"))
            {
                return new List<Weapon>();
            }

            string json = await File.ReadAllTextAsync("Weaponlist.json");


            return JsonSerializer.Deserialize<List<Weapon>>(json);
        }

        public static async Task Save(List<Weapon> weapons)
        {
            string json = JsonSerializer.Serialize(weapons, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync("Weaponlist.json", json);
        }
    }
}
