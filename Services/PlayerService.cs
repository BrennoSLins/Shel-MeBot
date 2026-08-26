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
using System.Diagnostics.CodeAnalysis;

namespace Shel_MeBotDB
{
    public class PlayerService
    {
        private Bot _bot;
        

        public PlayerService(Bot bot)
        {
            _bot = bot;
           
        }

        public async Task<int> HPChange(string pname, int value)
        {
            try
            {
                
                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));


                if (player != null)
                {
                    player.HP += value;
                    await PlayerRepo.Save(players);
                    return player.HP;
                }
                else
                {
                    Console.WriteLine("Player not found");
                    return -404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HPChange: {ex.Message}");
                return -404;
            }

        }

        public async Task<int> MPChange(string pname, int value)
        {
            try
            {
                
                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MP += value;
                    await PlayerRepo.Save(players);
                    return player.MP;
                }
                else
                {
                    Console.WriteLine("Who is this neguinho?");
                    return -404;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MPChange: {ex.Message}");
                return -404;
            }
        }

        public async Task<int> MindChange(string pname, int value)
        {
            try
            {
                pname = pname.ToLower();
                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.Mind += value;
                    await PlayerRepo.Save(players);
                    return player.Mind;
                }
                else
                {
                    Console.WriteLine("Who is this neguinho?");
                    return -404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MindChange: {ex.Message}");
                return -404;
            }



        }

        public async Task CreatePlayer(string pname, int php, int pmp, int pmind, string? newimg)
           {
               try
               {

                   var players = await PlayerRepo.Load();
                   var player = players.FirstOrDefault(p => p.Name == pname);

                   if (player != null)
                   {
                       Console.WriteLine("CreatePlayer: Player already exists.");
                       return;
                   }

                   Player newplayer = new Player();
                   newplayer.Name = pname;

                   newplayer.HP = php;
                   newplayer.MaxHP = php;

                   newplayer.MP = pmp;
                   newplayer.MaxMP = pmp;

                   newplayer.Mind = pmind;
                   newplayer.MaxMind = pmind;

                   newplayer.CharPic = newimg;

                   players.Add(newplayer);


                   await PlayerRepo.Save(players);
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"Error in CreatePlayer: {ex.Message}");
                   return;
               }


           }

        public async Task EditPlayer(List<string> Info, string pname)
        {
            try
            {

                var players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name == pname);

                if (player == null)
                {
                    Console.WriteLine("EditPlayer: Player dont exist.");
                    return;
                }

                string newname = Info.ElementAtOrDefault(0) ?? "";
                int newhp = int.Parse(Info.ElementAtOrDefault(1) ?? "0");
                int newmp = int.Parse(Info.ElementAtOrDefault(2) ?? "0");
                int newmind = int.Parse(Info.ElementAtOrDefault(3) ?? "0");

                if (newname != "")
                    player.Name = newname;

                if (newhp != 0)
                {
                    player.HP = newhp;
                    player.MaxHP = newhp;
                }
                    
                if (newmp != 0)
                {
                    player.MP = newmp;
                    player.MaxMP = newmp;
                }

                if (newmind != 0)
                {
                    player.Mind = newmind;
                    player.MaxMind = newmind;
                }

                         
                                                
                await PlayerRepo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditPlayer: {ex.Message}");
                return;
            }


        }

        public async Task DeletePlayer(string pname)
        {
            try
            {

                var players = await PlayerRepo.Load();

                players.RemoveAll(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                await PlayerRepo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeletPlayer: {ex.Message}");
                return;
            }
        }

        public async Task<Player?> GetPlayer(string pname)
        {
            try
            {

                var players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player == null)
                {
                    Console.WriteLine("GetPlayer: Player not found");
                    return null;
                }

                if (player.Name == pname)
                {
                    return player;
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

        public async Task<Embed?> ShowEmbed(string pname)
        {
            try
            {
                var player = await GetPlayer(pname);
                var weapons = await WeaponRepo.Load();
                

                if (player == null)
                {
                    Console.WriteLine("ShowEmbed: Player not found");
                    return null;
                }

                EmbedBuilder embed = new EmbedBuilder()
                .WithFooter(string.Join(Environment.NewLine, player.Info));

                embed.ImageUrl = player.isTransformed ? player.TransPic : player.CharPic;
                                
                embed.Title = player.Name;

                embed.Color = Color.DarkRed;

                embed.AddField("❤️ HP", $"{player.HP} / {player.MaxHP}", false);
                               
                embed.AddField("🔵 MP", $"{player.MP} / {player.MaxMP}", false);
                
               
                if (player.Name == "Zagreus")
                {
                    embed.AddField("🟢  E.A", $"{player.Mind} / {player.MaxMind}", false);
                }
                else
                {
                    embed.AddField("🧠 Mind", $"{player.Mind} / {player.MaxMind}", false);
                }

                return embed.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ShowPlayer: {ex.Message}");
                return null;
            }
        }

        public async Task SetMessageID(string pname, IUserMessage messageId)
        {
            try
            {
                var players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name == pname);

                if (player == null)
                {
                    Console.WriteLine("Jogador não existe: Task SetMessageID");
                    return;
                }

                if (player.MessageID != messageId.Id)
                {
                    player.MessageID = messageId.Id;
                }

                if (player.ChannelID != messageId.Channel.Id)
                {
                    player.ChannelID = messageId.Channel.Id;
                }



                await PlayerRepo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SetMessageID: {ex.Message}");
                return;
            }

        }

        public async Task<int> MaxHPSet(string pname, int value)
        {
            try
            {
                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MaxHP = value;
                    await PlayerRepo.Save(players);
                    return player.MaxHP;
                }
                else
                {
                    Console.WriteLine("Player not found");
                    return -404;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MaxHPSet: {ex.Message}");
                return -404;
            }

        }

        public async Task<int> MaxMPSet(string pname, int value)
        {
            try
            {

                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MaxMP = value;
                    await PlayerRepo.Save(players);
                    return player.MaxMP;
                }
                else
                {
                    Console.WriteLine("Who is this neguinho?");
                    return -404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MaxMPSet: {ex.Message}");
                return -404;
            }

        }

        public async Task<int> MaxMindSet(string pname, int value)
        {
            try
            {
                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MaxMind = value;
                    await PlayerRepo.Save(players);
                    return player.MaxMind;
                }
                else
                {
                    Console.WriteLine("Who is this neguinho?");
                    return -404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MaxMindSet: {ex.Message}");
                return -404;
            }


        }

        public async Task<int> HPSet(string pname, int value)
        {
            try
            {
                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.HP = value;
                    await PlayerRepo.Save(players);
                    return player.HP;
                }
                else
                {
                    Console.WriteLine("Player not found");
                    return -404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreatePlayer: {ex.Message}");
                return -404;
            }

        }

        public async Task<int> MPSet(string pname, int value)
        {
            try
            {

                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MP = value;
                    await PlayerRepo.Save(players);
                    return player.MP;
                }
                else
                {
                    Console.WriteLine("Player not found");
                    return -404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MPSet: {ex.Message}");
                return -404;
            }

        }

        public async Task<int> MindSet(string pname, int value)
        {
            try
            {
                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.Mind = value;
                    await PlayerRepo.Save(players);
                    return player.Mind;
                }
                else
                {
                    Console.WriteLine("Player not found");
                    return -404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MindSet: {ex.Message}");
                return -404;
            }
        }

        public async Task AddInfo(List<string> Info, string pname)
        {
            try
            {
                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player == null)
                {
                    Console.WriteLine("Player não encontrado");
                    return;
                }

                string info1 = Info.ElementAtOrDefault(0) ?? "";
                string info2 = Info.ElementAtOrDefault(1) ?? "";
                string info3 = Info.ElementAtOrDefault(2) ?? "";
                string info4 = Info.ElementAtOrDefault(3) ?? "";
                string info5 = Info.ElementAtOrDefault(4) ?? "";

                

                if (!player.Info.Contains(info1))
                    player.Info.Add(info1);

                if (info2 != "" && !player.Info.Contains(info2))
                    player.Info.Add(info2);
                                   
                if (info3 != "" && !player.Info.Contains(info3))
                    player.Info.Add(info3);

                if (info4 != "" && !player.Info.Contains(info4))
                    player.Info.Add(info4);

                if (info5 != "" && !player.Info.Contains(info5))
                    player.Info.Add(info5);

                await PlayerRepo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddInfo: {ex.Message}");
                return;
            }
        }

        public async Task RemoveInfo(string pname)
        {
            try
            {

                List<Player> players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player == null)
                {
                    Console.WriteLine("Player não encontrado");
                    return;
                }

                player.Info.Clear();

                await PlayerRepo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RemoveInfo: {ex.Message}");
                return;
            }
        }

        public async Task UpdatePlayer(string pname)
        {
            try
            {
                var players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));
                

                if (player == null)
                {
                    Console.WriteLine("Player not found.");
                    return;
                }

                pname = player.Name;

                ulong MessageId = player.MessageID;
                ulong ChannelId = player.ChannelID;

                IMessageChannel channel = await _bot.GetMessageChannelID(ChannelId);

                if (channel == null)
                    return;

                var msg = await channel.GetMessageAsync(MessageId) as IUserMessage;

                if (msg == null)
                    return;

                Embed? novoEmbed = await ShowEmbed(pname);

                if (novoEmbed == null)
                    return;

                await msg.ModifyAsync(x =>
                {
                    x.Embed = novoEmbed;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdatePlayer: {ex.Message}");
                return;
            }


        }

        public async Task AltPlayerPic(List<string> modallist, string pname)
        {
            try
            {
                var players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                string? fuck1 = modallist.ElementAtOrDefault(0);
                string? fuck2 = modallist.ElementAtOrDefault(1);

                             

                if (fuck1 != null )
                {
                    player.CharPic = fuck1;
                }

                if (fuck2 != null )
                {
                    player.TransPic = fuck2;
                }
                                

                await PlayerRepo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AltPlayerPic: {ex.Message}");
                return;
            }
        }

        public async Task<Embed?> RegisteredPlayers()
        {
            try
            {
                List<Player> players = await PlayerRepo.Load();

                if (players.Count == 0)
                {
                    Console.WriteLine("Nenhum jogador registrado.");
                    return null;
                }

                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle("Jogadores Registrados")
                    .WithColor(Color.DarkRed);
                foreach (var player in players)
                {
                    embed.AddField(player.Name, "\u200B");
                }

                return embed.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisteredPlayers: {ex.Message}");
                return null;
            }


        }

        public async Task<int> Transform(string pname)
        {
            try
            {
                var players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));
                if (player != null && (player.TransPic == null || player.TransPic == ""))
                {
                    return 3;
                }

                else if (player != null && !player.isTransformed)
                {
                    player.isTransformed = true;
                    
                    await PlayerRepo.Save(players);
                    return 1;
                }
                else if (player != null && player.isTransformed)
                {
                    player.isTransformed = false;
                    await PlayerRepo.Save(players);
                    return 0;
                }
                
                return 3;

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Transform: {ex.Message}");
                return 3;
            }
        }

        public async Task RenamePlayer(string pname, string nname)
        {
            try
            {
                var players = await PlayerRepo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));
                if (player != null)
                {
                    player.Name = nname;
                }
                else
                {
                    return;
                }
                await PlayerRepo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RenamePlayer: {ex.Message}");
                return;
            }
        }

        public async Task<Embed?> ShowInventory(string pname)
        {
            try
            {
                var player = await GetPlayer(pname);
                
                if (player == null)
                {
                    Console.WriteLine("ShowInventory: Player not found");
                    return null;
                }

                EmbedBuilder embed = new EmbedBuilder();
                                
                embed.Title = $"Inventário de {player.Name}:";

                embed.Color = Color.DarkRed;

                embed.AddField("-----ARMAS-----", "\u200B", false);
                foreach (Weapon weapon in player.Weapons)
                {
                    embed.AddField(weapon.Name, $"Quantidade: {weapon.Amount}", false);
                }

                
                embed.AddField("-----ITENS-----", "\u200B", false);
                foreach (Item item in player.Items)
                {
                    embed.AddField($"{item.Emoji} {item.Name}", $"Quantidade: {item.Amount}" , false);
                }
                                

                return embed.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ShowInventory: {ex.Message}");
                return null;
            }
        }

        public async Task<int> CoinChange(string pname, int amount, string moeda)
        {
            var players = await PlayerRepo.Load();
            Player? player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

            if (player == null)
            {
                Console.WriteLine("CoinChange: Player not found");
                return 1;
            }


            switch (moeda.ToLower())
            {
                case "bronze":

                    player.Wallet.Bronze += amount;
                    await PlayerRepo.Save(players);
                    Console.WriteLine($"{player.Wallet.Bronze}");
                    return 0;
                    

                case "grandebronze":

                    player.Wallet.GrandeBronze += amount;
                    await PlayerRepo.Save(players);
                    return 0;
                    
                case "prata":

                    player.Wallet.Prata += amount;
                    await PlayerRepo.Save(players);
                    return 0;
                    

                case "ouro":

                    player.Wallet.Ouro += amount;
                    await PlayerRepo.Save(players);
                    return 0;
                    
            }


            return 1;

        }

        public async Task<Embed?> SeeWallet(string pname)
        {
            var players = await PlayerRepo.Load();
            Player? player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

            if (player == null)
            {
                Console.WriteLine("SeeWallet: Player not found");
                return null;
            }

            EmbedBuilder embed = new EmbedBuilder();
                

            embed.Title = $"Carteira de {player.Name}";

            embed.AddField("🟡 Ouro", $"{player.Wallet.Ouro}", false);
            embed.AddField("⚪ Prata", $"{player.Wallet.Prata}", false);
            embed.AddField("🟤 Grande Bronze", $"{player.Wallet.GrandeBronze}", false);
            embed.AddField("🟠 Bronze", $"{player.Wallet.Bronze}", false);

            return embed.Build();

        }

        public async Task<Skill> CreateSkill(string name, string desc, int amount, ResourceType resource)
        {
            Skill newskill = new Skill();

            newskill.Name = name;
            newskill.Description = desc;
            newskill.Amount = amount;
            newskill.Resource = resource;

            return newskill;
        }

        public async Task<int> UseSkill(string pname, string pskill)
        {
            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name == pname);
            Skill? skill = player.Skills.FirstOrDefault(p => p.Name.Equals(pskill, StringComparison.OrdinalIgnoreCase));

            switch (skill.Resource)
            {
                case ResourceType.hp: //hp
                    if (player.HP >= skill.Amount)
                    {
                        player.HP += skill.Amount;
                        await PlayerRepo.Save(players);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                                                            
                case ResourceType.mp: //mp
                    if (player.MP >= skill.Amount)
                    {
                        player.MP += skill.Amount;
                        await PlayerRepo.Save(players);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                case ResourceType.mind: //mind
                    if (player.Mind >= skill.Amount)
                    {
                        player.Mind += skill.Amount;
                        await PlayerRepo.Save(players);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                case ResourceType.ea: //mind
                    if (player.Mind >= skill.Amount)
                    {
                        player.Mind += skill.Amount;
                        await PlayerRepo.Save(players);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                default:
                    return 3;

            }

            
        }

        public async Task<Embed?> ShowSkill(string pname, string pskill)
        {
            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name == pname);
            Skill? skill = player.Skills.FirstOrDefault(p => p.Name.Equals(pskill, StringComparison.OrdinalIgnoreCase));

            var embed = new EmbedBuilder()
    .WithTitle($"{skill.Name}:")
    .WithDescription($"{skill.Description}")
    .AddField(
        $"[{skill.Amount} de {skill.Resource} ao uso]",
        "\u200B",
        inline: false
    )
    .WithColor(Color.DarkGrey);

            return embed.Build();
        }
    }
}
