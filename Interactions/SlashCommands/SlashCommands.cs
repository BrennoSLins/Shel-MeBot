using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Shel_MeBotDB
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly PlayerService _service;
        private readonly InteractionService _interactions;
        private readonly WeaponService _wservice;
        private readonly ItemService _iservice;
        private readonly Bot _bot;

        public SlashCommands(PlayerService service, InteractionService interactions, WeaponService wservice, ItemService iservice, Bot bot)
        {
            _service = service;
            _interactions = interactions;
            _wservice = wservice;
            _iservice = iservice;
            _bot = bot;
        }

        //-------------------------------Console Commands---------------------------

        [SlashCommand("pcontrol", "Implanta o menu de controle de jogador.")]
        public async Task Butt()
        {
            var button = new ComponentBuilder()
                .WithButton("➕ Adicionar", "add", ButtonStyle.Success, row: 0)
                .WithButton("✏️ Editar", "editbutt", ButtonStyle.Primary, row: 0)
                .WithButton("🗑️ Excluir", "delselect", ButtonStyle.Danger, row: 0)
                .WithButton("🐦‍🔥 Transform", "transbutt", ButtonStyle.Secondary, row: 0)

                //Inventory/skill section
                .WithButton("➕ Adicionar item/arma", $"inventory:add", ButtonStyle.Success, row: 1)
                .WithButton("🗑️ ​Remover item/arma", $"inventory:remove", ButtonStyle.Danger, row: 1)
                .WithButton("➕ Adicionar skill", $"skills:add", ButtonStyle.Success, row: 1)
                .WithButton("🗑️ ​Remover skill", $"skills:remove", ButtonStyle.Danger, row: 1)

                //Quick Recovery Section
                .WithButton("❤️💯Recuperar HP", "qkrecov:hp", ButtonStyle.Danger, row: 2)
                .WithButton("💙💯Recuperar MP", "qkrecov:mp", ButtonStyle.Primary, row: 2)
                .WithButton("🧠💯Recuperar Mind", "qkrecov:mind", ButtonStyle.Success, row: 2)
                .WithButton("💯💯Recuperar Tudo", "qkrecov:all", ButtonStyle.Secondary, row: 2);


            await RespondAsync(components: button.Build());

        }

        [SlashCommand("wcontrol", "Implanta o menu de controle de armas.")]
        public async Task WButt()
        {
            var button = new ComponentBuilder()
                .WithButton("➕ Adicionar", "wadd", ButtonStyle.Success, row: 0)
                .WithButton("✏️ Editar", "weditbutt", ButtonStyle.Primary, row: 0)
                .WithButton("🗑️ Excluir", "wdelselect", ButtonStyle.Danger, row: 0)


                //Weapon Change Section
                .WithButton("⚔️ Normal", "wchange:1", ButtonStyle.Primary, row: 1)
                .WithButton("🔥 Shikai", "wchange:2", ButtonStyle.Success, row: 1)
                .WithButton("💥 Bankai", "wchange:3", ButtonStyle.Danger, row: 1);

               

            await RespondAsync(components: button.Build());

        }

        [SlashCommand("icontrol", "Implanta o menu de controle de itens.")]
        public async Task iButt()
        {
            var button = new ComponentBuilder()
                .WithButton("➕ Adicionar", "itemadd", ButtonStyle.Success, row: 0)
                .WithButton("✏️ Editar", "itemedit", ButtonStyle.Primary, row: 0)
                .WithButton("🗑️ Excluir", "itemdelete", ButtonStyle.Danger, row: 0)
                .WithButton("👁️ Ver item", "itemsee", ButtonStyle.Secondary, row:0);


            await RespondAsync(components: button.Build());

        }

        [SlashCommand("update", "Atualiza todos os jogadores, armas e itens")]
        public async Task _MassUpdate()
        {
            await DeferAsync();

            var players = await PlayerRepo.Load();
            var weapons = await WeaponRepo.Load();
            var items = await ItemRepo.Load();
            
            foreach (var player in players)
            {
                await _service.UpdatePlayer(player.Name);
            }
            foreach (var weapon in weapons)
            {
                await _wservice.UpdateWeapon(weapon.Name);
            }
            foreach (var item in items)
            {
                await _iservice.UpdateItem(item.Name);
            }
            await FollowupAsync("Atualizados", ephemeral: true);
        }

        //---------------------------Player Commands Section----------------------------------------

        [SlashCommand("pshow", "Implanta a carta do jogador. (Atualiza constantemente)")]
        public async Task _ShowPlayer([Autocomplete(typeof(PlayerAutocompleteHandler))] string Jogador)
        {
            Embed? embed = await _service.ShowEmbed(Jogador);
            await RespondAsync(embed: embed);

            IUserMessage sentemb = await GetOriginalResponseAsync();

            await _service.SetMessageID(Jogador, sentemb);
            await _service.UpdatePlayer(Jogador);
        }

        [SlashCommand("psee", "Mostra a carta do jogador. (Não atualiza)")]
        public async Task _SeePlayer([Autocomplete(typeof(PlayerAutocompleteHandler))] string Jogador)
        {
            Embed? embed = await _service.ShowEmbed(Jogador);
            await RespondAsync(embed: embed);
        }

        [SlashCommand("pregistrados", "Mostra todos os jogadores registrados")]
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

        [SlashCommand("money", "Altera o valor de dinheiro de um jogador")]
        public async Task money([Autocomplete(typeof(PlayerAutocompleteHandler))]string Jogador, int amount, [Autocomplete(typeof(CoinAutocompleteHandler))]string moeda)
        {
            await _service.CoinChange(Jogador, amount, moeda);
            await _service.UpdatePlayer(Jogador);

            await RespondAsync($"{Jogador} recebeu {amount} {moeda.ToLower()}s", ephemeral: true);

        }

        [SlashCommand("wallet", "Mostra a carteira do jogador")]
        public async Task wallet([Autocomplete(typeof(PlayerAutocompleteHandler))]string Jogador)
        {
            Embed? embed = await _service.SeeWallet(Jogador);

            var context = Context.User;

            await RespondAsync($"{context.Username}", embed: embed);

        }

        //---------------------------------Weapon Commands Section------------------------------

        [SlashCommand("wshow", "Implanta a carta da arma. (Atualiza constantemente)")]
        public async Task _ShowWeapon([Autocomplete(typeof(WeaponAutocompleteHandler))]string weapon)
        {
            Embed? embed = await _wservice.ShowWeaponEmbed(weapon);
            if (embed == null)
            {
                await RespondAsync("Arma não encontrada", ephemeral: true);
                return;
            }
            await RespondAsync(embed: embed);

            IUserMessage sentemb = await GetOriginalResponseAsync();

            await _wservice.SetWeaponMessageID(weapon, sentemb);
            await _wservice.UpdateWeapon(weapon);
        }

        [SlashCommand("wsee", "Mostra a carta da arma. (Não atualiza)")]
        public async Task _SeeWeapon([Autocomplete(typeof(WeaponAutocompleteHandler))]string weapon)
        {
            Embed? embed = await _wservice.ShowWeaponEmbed(weapon);
            if (embed == null)
            {
                await RespondAsync("Arma não encontrada", ephemeral: true);
                return;
            }
            await RespondAsync(embed: embed);
        }

        [SlashCommand("wregistrados", "Mostra todas as armas registrados")]
        public async Task _RegWeapon()
        {
            Embed? embed = await _wservice.RegisteredWeapons();
            await RespondAsync(embed: embed);
        }


        //---------------------------------Item Commands Section------------------------------

        [SlashCommand("iregistrados", "Mostra todos os itens registrados")]
        public async Task _RegItem()
        {
            Embed? embed = await _iservice.RegisteredItems();
            await RespondAsync(embed: embed);
        }


        //--------------------------------Quick Change Commands---------------------------

        [SlashCommand("sethp", "Define o HP do jogador")]
        public async Task _SetHP([Autocomplete(typeof(PlayerAutocompleteHandler))] string Jogador, int HP)
        {
            var usuario = Context.User;
            Console.WriteLine($"Comando sethp executado por: {usuario.Username}");
            Console.WriteLine($"{Jogador}jogador, {HP}hp");

            int erchek = await _service.HPSet(Jogador, HP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.", ephemeral: true);
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} agora tem {HP} de HP.", ephemeral: true);
        }

        [SlashCommand("setmp", "Define o MP do jogador")]
        public async Task _SetMP([Autocomplete(typeof(PlayerAutocompleteHandler))]string Jogador, int MP)
        {
            var usuario = Context.User;
            Console.WriteLine($"Comando setmp executado por: {usuario.Username}");
            Console.WriteLine($"{Jogador}jogador, {MP}mp");

            int erchek = await _service.MPSet(Jogador, MP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.", ephemeral: true);
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} agora tem {MP} de MP.", ephemeral: true);
        }

        [SlashCommand("setmind", "Define o Mind do jogador")]
        public async Task _SetMind([Autocomplete(typeof(PlayerAutocompleteHandler))]string Jogador, int Mind)
        {
            var usuario = Context.User;
            Console.WriteLine($"Comando setmind executado por: {usuario.Username}");
            Console.WriteLine($"{Jogador}jogador, {Mind}mind");

            int erchek = await _service.MindSet(Jogador, Mind);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.", ephemeral: true);
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} agora tem {Mind} de Mind.", ephemeral: true);

        }

        [SlashCommand("hp", "Altera o HP do jogador")]
        public async Task _HPChange([Autocomplete(typeof(PlayerAutocompleteHandler))]string Jogador, int HP)
        {
            var usuario = Context.User;
            Console.WriteLine($"Comando hp executado por: {usuario.Username}");
            Console.WriteLine($"{Jogador}jogador, {HP}hp");

            int erchek = await _service.HPChange(Jogador, HP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.", ephemeral: true);
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} recebeu {HP} de HP.", ephemeral: true);

        }

        [SlashCommand("mp", "Altera o MP do jogador")]
        public async Task _MPChange([Autocomplete(typeof(PlayerAutocompleteHandler))]string Jogador, int MP)
        {
            var usuario = Context.User;
            Console.WriteLine($"Comando mp executado por: {usuario.Username}");
            Console.WriteLine($"{Jogador}jogador, {MP}mp");

            int erchek = await _service.MPChange(Jogador, MP);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.", ephemeral: true);
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} recebeu {MP} de MP.", ephemeral: true);
        }

        [SlashCommand("mind", "Altera o Mind do jogador")]
        public async Task _MindChange([Autocomplete(typeof(PlayerAutocompleteHandler))]string Jogador, int Mind)
        {
            Console.WriteLine($"{Jogador}jogador, {Mind}mind");
            var usuario = Context.User;
            Console.WriteLine($"Comando mind executado por: {usuario.Username}");

            int erchek = await _service.MindChange(Jogador, Mind);
            if (erchek == -404)
            {
                await RespondAsync("Jogador não encontrado.", ephemeral: true);
                return;
            }
            await _service.UpdatePlayer(Jogador);
            await RespondAsync($"{Jogador} recebeu {Mind} de Mind.", ephemeral: true);
        }

        [SlashCommand("test", "test")]
        public async Task _test([Autocomplete(typeof(PlayerAutocompleteHandler))]string jogador, string skillinp)
        {
            Embed? embed = await _service.ShowSkill(jogador, skillinp);

            var button = new ComponentBuilder()
              .WithButton("➕ Aceitar", "skillaccept", ButtonStyle.Success, row: 0)
              .WithButton("❌ Recusar", "skilldeny", ButtonStyle.Danger, row: 0);

            await _bot.SendPlayerRequest($"O jogador {jogador} solicitou o uso de uma skill", embed: embed, components: button.Build());




        }

        [SlashCommand("test2", "test2")]
        public async Task test4([Autocomplete(typeof(PlayerAutocompleteHandler))]string jogador, string skill)
        {
            int useskill = await _service.UseSkill(jogador, skill);
            

            switch (useskill)
            {
                case 0:
                    await RespondAsync("Você não tem recurso o suficiente", ephemeral: true);
                    break;
                case 1:
                                       
    
                    await _bot.SendSitAtual($"{jogador} usou sua habilidade {skill}");
                    await _service.UpdatePlayer(jogador);
                    await RespondAsync("Skill utilizada", ephemeral: true);
                    break;
                    
            }
        }
                
        //----------------------------------Auto Complete Handlers---------------------------------
        
        public class PlayerAutocompleteHandler : AutocompleteHandler
        {
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
                IInteractionContext context,
                IAutocompleteInteraction autocompleteInteraction,
                IParameterInfo parameter,
                IServiceProvider services)
            {
                var players = await PlayerRepo.Load();

                var input = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

                var suggestions = players
                    .Where(p => p.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                    .Take(25)
                    .Select(p => new AutocompleteResult(p.Name, p.Name));

                return AutocompletionResult.FromSuccess(suggestions);
            }
        }

        public class WeaponAutocompleteHandler : AutocompleteHandler
        {
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
                IInteractionContext context,
                IAutocompleteInteraction autocompleteInteraction,
                IParameterInfo parameter,
                IServiceProvider services)
            {
                var weapons = await WeaponRepo.Load();

                var input = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

                var suggestions = weapons
                    .Where(p => p.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                    .Take(25)
                    .Select(p => new AutocompleteResult(p.Name, p.Name));

                return AutocompletionResult.FromSuccess(suggestions);
            }
        }

        public class ItemAutocompleteHandler : AutocompleteHandler
        {
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
                IInteractionContext context,
                IAutocompleteInteraction autocompleteInteraction,
                IParameterInfo parameter,
                IServiceProvider services)
            {
                var items = await ItemRepo.Load();

                var input = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

                var suggestions = items
                    .Where(p => p.Name.Contains(input, StringComparison.OrdinalIgnoreCase))
                    .Take(25)
                    .Select(p => new AutocompleteResult(p.Name, p.Name));

                return AutocompletionResult.FromSuccess(suggestions);
            }
        }

        public class CoinAutocompleteHandler : AutocompleteHandler
        {
            public override async Task<AutocompletionResult> GenerateSuggestionsAsync(
                IInteractionContext context,
                IAutocompleteInteraction autocompleteInteraction,
                IParameterInfo parameter,
                IServiceProvider services)
            {
                var moedas = typeof(Coins)
                .GetProperties()
                .Select(p => p.Name);

                var input = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

                var suggestions = moedas
                    .Where(m => m.Contains(input, StringComparison.OrdinalIgnoreCase))
                    .Take(25)
                    .Select(m => new AutocompleteResult(m, m));

                return AutocompletionResult.FromSuccess(suggestions);
            }
        }
    }

}
