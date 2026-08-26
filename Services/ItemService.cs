using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.VisualBasic;
using Shel_MeBotDB;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Xml.Linq;

namespace Shel_MeBotDB
{
    public class ItemService
    {
        private Bot _bot;


        public ItemService(Bot bot)
        {
            _bot = bot;

        }

        public async Task CreateItem(string name, string desc, string? img, string emoji)
        {
            try
            {

                var items = await ItemRepo.Load();
                var item = items.FirstOrDefault(p => p.Name == name);

                if (item != null)
                {
                    Console.WriteLine("CreateItem: Item already exists.");
                    return;
                }

                Item newitem = new Item();

                newitem.Name = name;
                newitem.Description = desc;
                newitem.ImageUrl = img;
                newitem.Emoji = emoji;
                
                items.Add(newitem);

                await ItemRepo.Save(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateItem: {ex.Message}");
                return;
            }


        }
        public async Task<Embed?> RegisteredItems()
        {
            try
            {
                List<Item> items = await ItemRepo.Load();

                if (items.Count == 0)
                {
                    Console.WriteLine("Nenhum item registrado.");
                    return null;
                }

                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle("Itens Registrados")
                    .WithColor(Color.DarkRed);
                foreach (var item in items)
                {
                    embed.AddField(item.Name, "\u200B");
                }

                return embed.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisteredItems: {ex.Message}");
                return null;
            }
        }
        public async Task EditItem(List<string> Info, string wname)
        {
            try
            {

                var items = await ItemRepo.Load();
                var item = items.FirstOrDefault(p => p.Name == wname);

                if (item == null)
                {
                    Console.WriteLine("EditItem: Item dont exist.");
                    return;
                }

                string GetValue(string value)
                {
                    int index = value.IndexOf(':');

                    if (index == -1)
                        return value;

                    return value[(index + 1)..].Trim();
                }

                string newname = GetValue(Info.ElementAtOrDefault(0) ?? "");
                string newdesc = GetValue(Info.ElementAtOrDefault(1) ?? "");
                string newwimg = GetValue(Info.ElementAtOrDefault(2) ?? "");
                string newemoji = GetValue(Info.ElementAtOrDefault(3) ?? "");
                



                if (newname != "")
                {
                    item.Name = newname;
                }
                if (newdesc != "")
                {
                    item.Description = newdesc;
                }
                if (newwimg != "")
                {
                    item.ImageUrl = newwimg;
                }
                if (newemoji != "")
                {
                    item.Emoji = newemoji;
                }

                await ItemRepo.Save(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateItem: {ex.Message}");
                return;
            }




        }
    
        public async Task DeleteItem(string pname)
        {
            try
            {

                var items = await ItemRepo.Load();

                items.RemoveAll(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                await ItemRepo.Save(items);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteItem: {ex.Message}");
                return;
            }
        }

        public async Task<Item?> GetItem(string pname)
        {
            try
            {

                var items = await ItemRepo.Load();
                var item = items.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (item == null)
                {
                    Console.WriteLine("GetItem: Item not found");
                    return null;
                }

                if (item.Name == pname)
                {
                    return item;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetItem: {ex.Message}");
                return null;
            }
        }

        public async Task<Embed?> ShowItemEmbed(string wname)
        {
            try
            {
                var items = await ItemRepo.Load();
                var item = items.FirstOrDefault(p => p.Name.Equals(wname, StringComparison.OrdinalIgnoreCase));

                if (item == null)
                {
                    Console.WriteLine("GetItem: Item not found");
                    return null;
                }

                EmbedBuilder embed = new EmbedBuilder()
                .WithFooter(string.Join(Environment.NewLine, item.Description));

                embed.ImageUrl = item.ImageUrl;

                embed.Title = $"{item.Emoji} {item.Name}";

                embed.Color = Color.DarkRed;


                return embed.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ShowItem: {ex.Message}");
                return null;
            }
        }

        public async Task SetItemMessageID(string pname, IUserMessage messageId)
        {
            try
            {
                var items = await ItemRepo.Load();
                var item = items.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (item == null)
                {
                    Console.WriteLine("Item não existe: Task SetItemMessageID");
                    return;
                }

                if (item.MessageID != messageId.Id)
                {
                    item.MessageID = messageId.Id;
                }

                if (item.ChannelID != messageId.Channel.Id)
                {
                    item.ChannelID = messageId.Channel.Id;
                }


                await ItemRepo.Save(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetItemMessageID: {ex.Message}");
                return;
            }

        }

        public async Task UpdateItem(string pname)
        {
            try
            {
                var items = await ItemRepo.Load();
                var item = items.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));


                if (item == null)
                {
                    Console.WriteLine("Item not found.");
                    return;
                }

                pname = item.Name;

                ulong MessageId = item.MessageID;
                ulong ChannelId = item.ChannelID;

                IMessageChannel channel = await _bot.GetMessageChannelID(ChannelId);

                if (channel == null)
                    return;

                var msg = await channel.GetMessageAsync(MessageId) as IUserMessage;

                if (msg == null)
                    return;

                Embed? novoEmbed = await ShowItemEmbed(pname);

                if (novoEmbed == null)
                    return;

                await msg.ModifyAsync(x =>
                {
                    x.Embed = novoEmbed;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateItem: {ex.Message}");
                return;
            }


        }

        public async Task AltItemPic(List<string> modallist, string pname)
        {
            try
            {
                var items = await ItemRepo.Load();
                var item = items.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));


                await ItemRepo.Save(items);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AltItemPic: {ex.Message}");
                return;
            }
        }
    }
}
