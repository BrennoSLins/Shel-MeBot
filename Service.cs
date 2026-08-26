using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.VisualBasic;
using Shel_MeBot_2;
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

namespace Shel_MeBot_2
{
    public class Service
    {
        private Bot _bot;

        public Service(Bot bot)
        {
            _bot = bot;
        }

        public async Task<int> HPChange(string pname, int value)
        {
            try
            {
                
                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));


                if (player != null)
                {
                    player.HP += value;
                    await Repo.Save(players);
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
                
                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MP += value;
                    await Repo.Save(players);
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
                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.Mind += value;
                    await Repo.Save(players);
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


        public async Task CreatePlayer(string pname, int php, int pmp, int pmind)
           {
               try
               {

                   var players = await Repo.Load();
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

                   players.Add(newplayer);


                   await Repo.Save(players);
               }
               catch (Exception ex)
               {
                   Console.WriteLine($"Error in CreatePlayer: {ex.Message}");
                   return;
               }


           }
  
        public async Task DeletePlayer(string pname)
        {
            try
            {

                var players = await Repo.Load();

                players.RemoveAll(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                await Repo.Save(players);
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

                var players = await Repo.Load();
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
                var players = await Repo.Load();
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



                await Repo.Save(players);
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
                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MaxHP = value;
                    await Repo.Save(players);
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

                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MaxMP = value;
                    await Repo.Save(players);
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
                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MaxMind = value;
                    await Repo.Save(players);
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
                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.HP = value;
                    await Repo.Save(players);
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

                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.MP = value;
                    await Repo.Save(players);
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
                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.Mind = value;
                    await Repo.Save(players);
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

        public async Task AddInfo(string pname, string info)
        {
            try
            {
                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player == null)
                {
                    Console.WriteLine("Player não encontrado");
                    return;
                }


                player.Info.Add(info);

                await Repo.Save(players);
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

                List<Player> players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player == null)
                {
                    Console.WriteLine("Player não encontrado");
                    return;
                }

                player.Info.Clear();

                await Repo.Save(players);
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
                var players = await Repo.Load();
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

        public async Task AddPlayerPic(string pname, string ppic)
        {
            try
            {
                var players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.CharPic = ppic;
                }

                await Repo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddPlayerPic: {ex.Message}");
                return;
            }
        }

        public async Task<Embed?> RegisteredPlayers()
        {
            try
            {
                List<Player> players = await Repo.Load();

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
                var players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));
                if (player != null && (player.TransPic == null || player.TransPic == ""))
                {
                    return 3;
                }

                else if (player != null && !player.isTransformed)
                {
                    player.isTransformed = true;
                    await Repo.Save(players);
                    return 1;
                }
                else if (player != null && player.isTransformed)
                {
                    player.isTransformed = false;
                    await Repo.Save(players);
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

        public async Task AddTransPic(string pname, string ppic)
        {
            try
            {
                var players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));

                if (player != null)
                {
                    player.TransPic = ppic;
                }

                await Repo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddTransPic: {ex.Message}");
                return;
            }
        }

        public async Task RenamePlayer(string pname, string nname)
        {
            try
            {
                var players = await Repo.Load();
                var player = players.FirstOrDefault(p => p.Name.Equals(pname, StringComparison.OrdinalIgnoreCase));
                if (player != null)
                {
                    player.Name = nname;
                }
                else
                {
                    return;
                }
                await Repo.Save(players);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RenamePlayer: {ex.Message}");
                return;
            }
        }
    }
}
