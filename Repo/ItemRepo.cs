using Shel_MeBotDB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.IO;

namespace Shel_MeBotDB;

internal class ItemRepo
{

        public static async Task<List<Item>> Load()
        {
            if (!File.Exists("Itemlist.json"))
            {
                return new List<Item>();
            }

            string json = await File.ReadAllTextAsync("Itemlist.json");


            return JsonSerializer.Deserialize<List<Item>>(json);
        }

        public static async Task Save(List<Item> items)
        {
            string json = JsonSerializer.Serialize(items, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync("Itemlist.json", json);
        }
}

