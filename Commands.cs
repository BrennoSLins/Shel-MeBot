using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Shel_MeBot
{
    public class Commands
    {
        private readonly Service _service;

        public Commands(Service service)
        {
            _service = service;
        }

        public async Task DCommands(SocketMessage message)
        {
            try
            {

                if (message.Author.IsBot)
                    return;

                List<ulong> authorizedIDs = new List<ulong>();
                ulong roboticosId = 1531440613356994590;
                ulong labId = 1533303054017888406;

                authorizedIDs.Add(roboticosId);
                authorizedIDs.Add(labId);


                string input = (message.Content);
                string[] comando = input.Split(' '); //!hp Player ParI ParII ParIII

                

                if (message.Author is SocketGuildUser user && comando[0].StartsWith("!"))
                {
                    
                    if (!authorizedIDs.Contains(user.Roles.FirstOrDefault(r => authorizedIDs.Contains(r.Id))?.Id ?? 0))
                    {
                        await message.Channel.SendMessageAsync($"Você não tem o molho.");
                        return;
                    }
                                       
                    
                    
                }

                    switch (comando[0].ToLower())
                {
                    case "!hp":
                        {
                            await _service.HPChange(comando[1], int.Parse(comando[2]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"{comando[1]} recebeu {comando[2]} de HP");
                            await message.DeleteAsync();
                            break;

                        }
                    case "!mp":
                        {
                            await _service.MPChange(comando[1], int.Parse(comando[2]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"{comando[1]} recebeu {comando[2]} de MP");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!mind":
                        {
                            await _service.MindChange(comando[1], int.Parse(comando[2]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"{comando[1]} recebeu {comando[2]} de Mind");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!create":
                        {
                            await _service.CreatePlayer(comando[1], int.Parse(comando[2]), int.Parse(comando[3]), int.Parse(comando[4]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Jogador {comando[1]} criado com sucesso.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!delete":
                        {
                            await _service.DeletePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Jogador {comando[1]} retornou ao vazio.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!show":
                        {
                            Embed? embed = await _service.ShowPlayer(comando[1]);
                            IUserMessage sentemb = await message.Channel.SendMessageAsync(embed: embed);
                            await _service.SetMessageID(comando[1], sentemb);
                            await _service.UpdatePlayer(comando[1]);
                            await message.DeleteAsync();
                            break;
                        }
                    case "!maxhp":
                        {
                            await _service.MaxHPSet(comando[1], int.Parse(comando[2]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Jogador {comando[1]} agora tem {comando[2]} de vida máxima.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!maxmp":
                        {
                            await _service.MaxMPSet(comando[1], int.Parse(comando[2]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Jogador {comando[1]} agora tem {comando[2]} de mana máxima.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!maxmind":
                        {
                            await _service.MaxMindSet(comando[1], int.Parse(comando[2]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Jogador {comando[1]} agora tem {comando[2]} de mind máxima.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!sethp":
                        {
                            await _service.HPSet(comando[1], int.Parse(comando[2]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Jogador {comando[1]} agora tem {comando[2]} de vida.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!setmp":
                        {
                            await _service.MPSet(comando[1], int.Parse(comando[2]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Jogador {comando[1]} agora tem {comando[2]} de mana.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!setmind":
                        {
                            await _service.MindSet(comando[1], int.Parse(comando[2]));
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Jogador {comando[1]} agora tem {comando[2]} de mind.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!addinfo":
                        {
                            string mensagem = string.Join(" ", comando.Skip(2));
                            await _service.AddInfo(comando[1], mensagem);
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Informação adicionada ao jogador {comando[1]}: {mensagem}");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!deleteinfo":
                        {
                            await _service.RemoveInfo(comando[1]);
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Informação do jogador {comando[1]} removida.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!see":
                        {
                            Embed? embed = await _service.ShowPlayer(comando[1]);
                            await message.Channel.SendMessageAsync(embed: embed);
                            await message.DeleteAsync();
                            break;
                        }
                    case "!addpic":
                        {
                            await _service.AddPlayerPic(comando[1], comando[2]);
                            await _service.UpdatePlayer(comando[1]);
                            await message.Channel.SendMessageAsync($"Foto vinculada ao jogador {comando[1]}.");
                            await message.DeleteAsync();
                            break;
                        }
                    case "!help":
                        {
                            await message.Channel.SendMessageAsync("Modelo de como usar comandos: \r\n\r\nManipulação de personagem:\r\n\r\n▪️ !create NomeJogador MaxHP MaxMP MaxMind\r\n▪️" +
                                " !delete NomeJogador\r\n▪️ !addpic NomeJogador LinkImagem\r\n▪️ !addinfo NomeJogador Texto\r\n▪️ !deleteinfo NomeJogador\r\n\r\nAlterar valores de hp/mp/mind" +
                                ":\r\n(para alterar mp ou mind, só alterar depois do prefixo \" ! \")\r\n\r\n▪️ !hp NomeJogador Valor (Acrescenta/diminui pelo valor)\r\n▪️ !sethp NomeJogador Valor (Dita o recurso atual)\r\n▪️ " +
                                "!maxhp NomeJogador Valor (Dita o recurso máximo)\n\nRepresentar personagem:\r\n\r\n▪️!show NomeJogador (usar no canal de status, implementa uma carta do jogador que atualiza constantemente)\r\n▪️!see NomeJogador (mostra uma carta de status única, não atualiza)\r\n" +
                                "▪️ !registrados (Mostra a quantidade de jogadores registrados no bot) ");
                            break;
                        }
                    case "!registrados":
                        {
                            List<string> registeredPlayers = await _service.RegisteredPlayers();
                            int playercount = 0;
                            foreach (string player in registeredPlayers)
                            {
                                await message.Channel.SendMessageAsync($"- {player}");
                                playercount++;
                                
                            }

                            await message.Channel.SendMessageAsync($"{playercount} jogadores.");
                            await message.DeleteAsync();
                            break;
                        }
                    


                }
            } catch (Exception ex)
            {
                await message.Channel.SendMessageAsync("Erro, comando inválido seu dumb ahh.");
            }

        }
        
            public static async Task Error(IMessageChannel channel)
            {
                await channel.SendMessageAsync("Erro, you stupid bum.");
            }
        





    }
        

        
    
}
