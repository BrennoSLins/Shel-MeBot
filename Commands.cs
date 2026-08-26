using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
using System.Text;

namespace Shel_MeBot_2
{
    public class Commands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Service _service;
        private readonly InteractionService _interactions;

        public Commands(Service service, InteractionService interactions)
        {
            _service = service;
            _interactions = interactions;
        }

        [SlashCommand("hp", "Altera o HP do jogador")]
        public async Task _HPChange(string Jogador, int HP)
        {
            int erchek = await _service.HPChange(Jogador, HP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.");
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} recebeu {HP} de HP.");

        }

        [SlashCommand("mp", "Altera o MP do jogador")]
        public async Task _MPChange(string Jogador, int MP)
        {
            int erchek = await _service.MPChange(Jogador, MP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.");
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} recebeu {MP} de MP.");
        }

        [SlashCommand("mind", "Altera o Mind do jogador")]
        public async Task _MindChange(string Jogador, int Mind)
        {
            int erchek = await _service.MindChange(Jogador, Mind);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.");
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} recebeu {Mind} de Mind.");
        }

        [SlashCommand("maxhp", "Altera a HP máxima do jogador")]
        public async Task _MaxHPChange(string Jogador, int MaxHP)
        {
            int erchek = await _service.MaxHPSet(Jogador, MaxHP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.");
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} recebeu {MaxHP} de HP máximo.");
        }

        [SlashCommand("maxmp", "Altera a MP máxima do jogador")]
        public async Task _MaxMPChange(string Jogador, int MaxMP)
        {
            int erchek = await _service.MaxMPSet(Jogador, MaxMP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.");
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} recebeu {MaxMP} de MP máximo.");
        }

        [SlashCommand("maxmind", "Altera o Mind máximo do jogador")]
        public async Task _MaxMindChange(string Jogador, int MaxMind)
        {
            int erchek = await _service.MaxMindSet(Jogador, MaxMind);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.");
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} recebeu {MaxMind} de Mind máximo.");

        }

        [SlashCommand("sethp", "Define o HP do jogador")]
        public async Task _SetHP(string Jogador, int HP)
        {
            int erchek = await _service.HPSet(Jogador, HP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.");
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} agora tem {HP} de HP.");
        }

        [SlashCommand("setmp", "Define o MP do jogador")]
        public async Task _SetMP(string Jogador, int MP)
        {
            int erchek = await _service.MPSet(Jogador, MP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.");
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} agora tem {MP} de MP.");
        }

        [SlashCommand("setmind", "Define o Mind do jogador")]
        public async Task _SetMind(string Jogador, int Mind)
        {
            int erchek = await _service.MindSet(Jogador, Mind);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.");
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} agora tem {Mind} de Mind.");

        }

        [SlashCommand("create", "Cria um novo jogador")]
        public async Task _CreatePlayer(string Nome, int MaxHP, int MaxMP, int MaxMind)
        {
            await _service.CreatePlayer(Nome, MaxHP, MaxMP, MaxMind);
            await RespondAsync($"Jogador {Nome} criado com sucesso.");
        }

        [SlashCommand("delete", "Deleta um jogador")]
        public async Task _DeletePlayer(string Jogador)
        {
            await _service.DeletePlayer(Jogador);
            await RespondAsync($"Jogador {Jogador} deletado com sucesso.");
        }

        [SlashCommand("addinfo", "Adiciona status ao jogador")]
        public async Task _AddInfo(string Jogador, string Info)
        {
            await _service.AddInfo(Jogador, Info);
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"Informação adicionada ao jogador {Jogador}.");
        }

        [SlashCommand("removeinfo", "Remove status do jogador")]
        public async Task _DeleteInfo(string Jogador)
        {
            await _service.RemoveInfo(Jogador);
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"Informação removida do jogador {Jogador}.");
        }

        [SlashCommand("addpic", "Define a imagem do jogador")]
        public async Task _AddPlayerPic(string Jogador, string Imagem)
        {
            await _service.AddPlayerPic(Jogador, Imagem);
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"Imagem definida para o jogador {Jogador}.");
        }

        [SlashCommand("show", "Implanta a carta do jogador. (Atualiza constantemente)")]
        public async Task _ShowPlayer(string Jogador)
        {
            Embed? embed = await _service.ShowEmbed(Jogador);
            await RespondAsync(embed: embed);

            IUserMessage sentemb = await GetOriginalResponseAsync();

            await _service.SetMessageID(Jogador, sentemb);
            await _service.UpdatePlayer(Jogador);
        }

        [SlashCommand("see", "Mostra a carta do jogador. (Não atualiza)")]
        public async Task _SeePlayer(string Jogador)
        {
            Embed? embed = await _service.ShowEmbed(Jogador);
            await RespondAsync(embed: embed);
        }

        [SlashCommand("registrados", "Mostra todos os jogadores registrados")]
        public async Task _RegPlayers()
        {
            Embed? embed = await _service.RegisteredPlayers();
            await RespondAsync(embed: embed);
        }

        [SlashCommand("help", "Lista todos os comandos")]
        public async Task Help()
        {
            var embed = new EmbedBuilder()
                .WithTitle("Comandos");

            foreach (var command in _interactions.SlashCommands)
            {
                embed.AddField(
                    $"/{command.Name}",
                    command.Description,
                    false);
            }

            await RespondAsync(embed: embed.Build(), ephemeral: true);

        }

        [SlashCommand("transform", "Transforma o jogador.")]
        public async Task _transform(string Jogador)
        {
            int ercheck = await _service.Transform(Jogador);
            if (ercheck == 1)
            {
                await _service.UpdatePlayer(Jogador);
                await RespondAsync($"Jogador {Jogador} transformado com sucesso.");
            }
            else if (ercheck == 0)
            {
                await _service.UpdatePlayer(Jogador);
                await RespondAsync($"Jogador {Jogador} cancelou a transformação.");
                return;
            }
            else if (ercheck == 3)
            {
                await RespondAsync("Jogador não tem transformação ou não existe.");
                return;
            }
        }

        [SlashCommand("addtpic", "Define a imagem de transformação do jogador")]
        public async Task _AddTPic(string Jogador, string Imagem)
        {
            await _service.AddTransPic(Jogador, Imagem);
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"Imagem de transformação definida para o jogador {Jogador}.");
        }

        [SlashCommand("rename", "Renomeia o jogador")]
        public async Task _RenamePlayer(string Jogador, string NovoNome)
        {
            await _service.RenamePlayer(Jogador, NovoNome);
            await _service.UpdatePlayer(NovoNome);
            await RespondAsync($"Jogador {Jogador} renomeado para {NovoNome}.");
        }

        [SlashCommand("update", "Atualiza as informações do jogador")]
        public async Task _UpdatePlayer()
        {
            await DeferAsync();

            var players = await Repo.Load();

            foreach (Player player in players)
            {
                await _service.UpdatePlayer(player.Name);
            }

           
            await FollowupAsync($"Informações dos jogadores atualizadas.");
        
        }
    }
}
