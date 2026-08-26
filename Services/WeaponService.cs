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
    public class WeaponService
    {
        private Bot _bot;
        

        public WeaponService(Bot bot)
        {
            _bot = bot;
           
        }

        
        public async Task CreateWeapon(string wname, string desc, string? img, string simg, string bimg)
           {
               try
               {

                   var weapons = await WeaponRepo.Load();
                   var weapon = weapons.FirstOrDefault(p => p.Name == wname);

                   if (weapon != null)
                   {
                       Console.WriteLine("CreateWeapon: Weapon already exists.");
                       return;
                   }

                   Weapon newweapon = new Weapon();
                   
                    newweapon.Name = wname;
                    newweapon.Description = desc;
                    newweapon.State = 1;
                    newweapon.WImg = img;
                    newweapon.ShImg = simg;
                    newweapon.BaImg = bimg;

                   weapons.Add(newweapon);

                   await WeaponRepo.Save(weapons);
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"Error in CreateWeapon: {ex.Message}");
                   return;
               }


        }

        public async Task EditWeapon(List<string> Info, string wname)
        {
            try
            {

                var weapons = await WeaponRepo.Load();
                var weapon = weapons.FirstOrDefault(p => p.Name == wname);

                if (weapon == null)
                {
                    Console.WriteLine("EditWeapon: Player dont exist.");
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
                string newshikai = GetValue(Info.ElementAtOrDefault(3) ?? "");
                string newbankai = GetValue(Info.ElementAtOrDefault(4) ?? "");



                if (newname != "")
                {
                    weapon.Name = newname;
                }
                if (newdesc != "")
                {
                    weapon.Description = newdesc;
                }
                if (newwimg != "")
                {
                    weapon.WImg = newwimg;
                }
                if (newshikai != "")
                {
                    weapon.ShImg = newshikai;
                }
                if (newbankai != "")
                {
                    weapon.BaImg = newbankai;
                }


                

                await WeaponRepo.Save(weapons);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreatePlayer: {ex.Message}");
                return;
            }


        }

        public async Task DeleteWeapon(string pname)
        {
            try
            {

                var weapons = await WeaponRepo.Load();

                weapons.RemoveAll(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                await WeaponRepo.Save(weapons);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeletWeapon: {ex.Message}");
                return;
            }
        }

        public async Task<Weapon?> GetWeapon(string pname)
        {
            try
            {

                var weapons = await WeaponRepo.Load();
                var weapon = weapons.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (weapon == null)
                {
                    Console.WriteLine("GetWeapon: Player not found");
                    return null;
                }

                if (weapon.Name == pname)
                {
                    return weapon;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPlayer: {ex.Message}");
                return null;
            }
        }

        public async Task<Embed?> ShowWeaponEmbed(string wname)
        {
            try
            {
                var weapons = await WeaponRepo.Load();
                var weapon = weapons.FirstOrDefault(p => p.Name.Equals(wname, StringComparison.OrdinalIgnoreCase));

                if (weapon == null)
                {
                    Console.WriteLine("ShowEmbed: Weapon not found");
                    return null;
                }

                EmbedBuilder embed = new EmbedBuilder()
                .WithFooter(string.Join(Environment.NewLine, weapon.Description));

                if (weapon.State == 1)
                {
                    embed.ImageUrl = weapon.WImg;
                }
                if (weapon.State == 2)
                {
                    embed.ImageUrl = weapon.ShImg;
                }
                if (weapon.State == 3)
                {
                    embed.ImageUrl = weapon.BaImg;
                }

                embed.Title = weapon.Name;

                embed.Color = Color.DarkRed;

                               
                return embed.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ShowWeapon: {ex.Message}");
                return null;
            }
        }

        public async Task SetWeaponMessageID(string pname, IUserMessage messageId)
        {
            try
            {
                var weapons = await WeaponRepo.Load();
                var weapon = weapons.FirstOrDefault(p => p.Name == pname);

                if (weapon == null)
                {
                    Console.WriteLine("Arma não existe: Task SetWeaponMessageID");
                    return;
                }

                if (weapon.MessageID != messageId.Id)
                {
                   weapon.MessageID = messageId.Id;
                }

                if (weapon.ChannelID != messageId.Channel.Id)
                {
                    weapon.ChannelID = messageId.Channel.Id;
                }


                await WeaponRepo.Save(weapons);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetWeaponMessageID: {ex.Message}");
                return;
            }

        }

                       

        public async Task UpdateWeapon(string pname)
        {
            try
            {
                var weapons = await WeaponRepo.Load();
                var weapon = weapons.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));
                

                if (weapon == null)
                {
                    Console.WriteLine("Weapon not found.");
                    return;
                }

                pname = weapon.Name;

                ulong MessageId = weapon.MessageID;
                ulong ChannelId = weapon.ChannelID;

                IMessageChannel channel = await _bot.GetMessageChannelID(ChannelId);

                if (channel == null)
                    return;

                var msg = await channel.GetMessageAsync(MessageId) as IUserMessage;

                if (msg == null)
                    return;

                Embed? novoEmbed = await ShowWeaponEmbed(pname);

                if (novoEmbed == null)
                    return;

                await msg.ModifyAsync(x =>
                {
                    x.Embed = novoEmbed;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateWeapon: {ex.Message}");
                return;
            }


        }

        public async Task AltWeaponPic(List<string> modallist, string pname)
        {
            try
            {
                var weapons = await WeaponRepo.Load();
                var weapon = weapons.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                
                await WeaponRepo.Save(weapons);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AltWeaponPic: {ex.Message}");
                return;
            }
        }

        public async Task<Embed?> RegisteredWeapons()
        {
            try
            {
                List<Weapon> weapons = await WeaponRepo.Load();

                if (weapons.Count == 0)
                {
                    Console.WriteLine("Nenhuma arma registrada.");
                    return null;
                }

                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle("Armas Registradas")
                    .WithColor(Color.DarkRed);
                foreach (var weapon in weapons)
                {
                    embed.AddField(weapon.Name, "\u200B");
                }

                return embed.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WeaponPlayers: {ex.Message}");
                return null;
            }


        }

              

        
    }
}
