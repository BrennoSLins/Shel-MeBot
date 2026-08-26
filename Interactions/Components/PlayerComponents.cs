using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Markup;
using System.Xml.Linq;
using static Shel_MeBotDB.PlayerModals;

namespace Shel_MeBotDB
{
    public class PlayerComponents : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly PlayerService _service;
        private readonly InteractionService _interactions;
        private readonly PlayerModals _modals;
        private readonly Bot _bot;
        private readonly WeaponService _wservice;

        public PlayerComponents(PlayerService service, InteractionService interactions, PlayerModals modals, Bot bot, WeaponService wservice)
        {
            _service = service;
            _interactions = interactions;
            _modals = modals;
            _bot = bot;
            _wservice = wservice;
        }

        //--------------------------------Player management section-----------------------------

        [ComponentInteraction("add")]
        public async Task _add()
        {
            await RespondWithModalAsync<CreatePlayerModal>("create_player");
        }

        //Edit button interactions

        [ComponentInteraction("editbutt")]
        public async Task _editt()
        {

            var button = new ComponentBuilder()
               .WithButton("✏️ Renomear/Alterar atributos", $"edit:rename", ButtonStyle.Primary)
               .WithButton("​📝​ Atribuir informações extras", $"edit:info", ButtonStyle.Primary)
               .WithButton("🖼 Alterar imagem", $"edit:altpic", ButtonStyle.Primary);


            await RespondAsync(components: button.Build(), ephemeral: true);


        }

        [ComponentInteraction("edit:*")]
        public async Task _edittomodal(string acao)
        {
            switch (acao)
            {
                case "rename":

                    await RespondWithModalAsync<EditPlayerModal>("medit");
                    break;
            

                case "info":

                    var button = new ComponentBuilder()
                    .WithButton("➕ Adicionar informações", $"editinfo:add", ButtonStyle.Success)
                    .WithButton("🗑️ ​Remover informações", $"editinfo:remove", ButtonStyle.Danger);
                    

                    await RespondAsync(components: button.Build(), ephemeral: true);
                    break;

                case "altpic":
                    await RespondWithModalAsync<AltPicModal>("maltpic");
                    break;
            }
        }

        [ComponentInteraction("editinfo:*")]
        public async Task _editinfo(string acao)
        {
            switch (acao)
            {
                case "add":

                    await RespondWithModalAsync<AddInfoModal>("maddinfo");
                    break;

                case "remove":

                    var players = await PlayerRepo.Load();

                    Console.WriteLine("FUCKYOU");
                    var menu = new SelectMenuBuilder()
                .WithCustomId($"editinfo:{acao}:p")
                .WithPlaceholder("Escolha um jogador");

                    foreach (Player player in players)
                    {
                        menu.AddOption($"{player.Name}", $"{player.Name}");
                    }

                    var components = new ComponentBuilder()
                .WithSelectMenu(menu);

                    await RespondAsync("Escolha um jogador:", components: components.Build(), ephemeral: true);
                    break;
            }
        }

        [ComponentInteraction("editinfo:remove:p")]
        public async Task _editinfo(string[] values)
        {
            string jogador = values[0];
            var players = await PlayerRepo.Load();
            Player? player = await _service.GetPlayer(jogador);

            await _modals.ModListInput(jogador, "", "", "", "");


            var menu = new SelectMenuBuilder()
        .WithCustomId($"removeinfo:def")
        .WithPlaceholder("Escolha a informação");

            foreach (string info in player.Info)
            {
                menu.AddOption($"{info}", $"{info}");
            }

            var components = new ComponentBuilder()
        .WithSelectMenu(menu);

            await RespondAsync("Escolha a informação:", components: components.Build(), ephemeral: true);

        }

        [ComponentInteraction("removeinfo:def")]
        public async Task _removeinfodef(string[] values)
        {
            await DeferAsync();

            string info = values[0];
            List<string> modlist = await _modals.ModListLoad();


            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name.Equals(modlist[0], StringComparison.OrdinalIgnoreCase));

            if (player.Info.Contains(info))
            {
                player.Info.Remove(info);
            }

            await PlayerRepo.Save(players);
            await _service.UpdatePlayer(player.Name);
            await _modals.ModListRemove();

            await FollowupAsync($"Jogador {player.Name} editado com sucesso", ephemeral: true);

        }

        [ComponentInteraction("editselect")]
        public async Task _componenttofunc(string[] values)
        {
            string jogador = values[0];

            List<string> modallist = await _modals.ModListLoad();

            await _service.EditPlayer(modallist, jogador);
            await _service.UpdatePlayer(jogador);
            await _modals.ModListRemove();

            await RespondAsync($"Jogador {jogador} editado com sucesso", ephemeral: true);

        }

        [ComponentInteraction("addinfoselect")]
        public async Task _addinfoselect(string[] values)
        {
            string jogador = values[0];
            List<string> modallist = await _modals.ModListLoad();

            await _service.AddInfo(modallist, jogador);
            await _service.UpdatePlayer(jogador);
            await _modals.ModListRemove();

            await RespondAsync($"Informações adicionados ao jogador {jogador}.", ephemeral: true);
        }

        //Edit inventory button interactions

        [ComponentInteraction("inventory:*")]
        public async Task _editinventory(string acao)
        {
            switch (acao)
            {
                case "add":
                    List<Weapon> weapons = await WeaponRepo.Load();
                    List<Item> items = await ItemRepo.Load();

                    var menu = new SelectMenuBuilder()
                .WithCustomId($"inventoryadd")
                .WithPlaceholder("Escolha um item/arma");

                menu.AddOption($"-----ARMAS-----", $"dummyw");

                foreach (Weapon weapon in weapons)
                    {
                        menu.AddOption($"{weapon.Name}", $"W:{weapon.Name}");
                    }

                menu.AddOption($"-----ITEMS-----", $"dummyi");

                foreach (Item item in items)
                    {
                        menu.AddOption($"{item.Name}", $"I:{item.Name}");
                    }
                    
                    var components = new ComponentBuilder()
                    .WithSelectMenu(menu);

                    await RespondAsync("Escolha qual item/arma adicionar:", components: components.Build(), ephemeral: true);

                    break;

                case "remove":
                    List<Player> players = await PlayerRepo.Load();

                    var rmenu = new SelectMenuBuilder()
                .WithCustomId($"inventoryremove")
                .WithPlaceholder("Escolha um jogador");

                
                    foreach (Player player in players)
                    {
                        rmenu.AddOption($"{player.Name}", $"{player.Name}");
                    }

                    
                    var rcomponents = new ComponentBuilder()
                    .WithSelectMenu(rmenu);

                    await RespondAsync("Escolha um jogador:", components: rcomponents.Build(), ephemeral: true);

                    break;

                case "see":
                    List<Player> splayers = await PlayerRepo.Load();

                    var smenu = new SelectMenuBuilder()
                .WithCustomId($"inventorysee")
                .WithPlaceholder("Escolha um jogador");


                    foreach (Player player in splayers)
                    {
                        smenu.AddOption($"{player.Name}", $"{player.Name}");
                    }


                    var scomponents = new ComponentBuilder()
                    .WithSelectMenu(smenu);

                    await RespondAsync("Escolha um jogador:", components: scomponents.Build(), ephemeral: true);

                    break;

            }
                
        }

        [ComponentInteraction("inventoryadd")]
        public async Task _inventoryadd(string[] values)
        {
           string added = values[0];
           await _modals.ModListInput(added, "", "", "", "");
            var players = await PlayerRepo.Load();

            var menu = new SelectMenuBuilder()
        .WithCustomId("inventoryadd:player")
        .WithPlaceholder("Escolha o jogador que receberá o item");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
        .WithSelectMenu(menu);

            await RespondAsync($"Escolha o jogador que receberá {added}:", components: components.Build(), ephemeral: true);


        }

        [ComponentInteraction("inventoryadd:player")]
        public async Task _inventoryaddfinal(string[] values)
        {
            await DeferAsync();
            Console.WriteLine("got here: inventoryaddfinal");
            string jogador = values[0];
            var players = await PlayerRepo.Load();
            Player? player = players.FirstOrDefault(x => x.Name == jogador);

            Console.WriteLine($"Values:{jogador} jogador:{player.Name}");
            
            string GetValue(string value)
            {
                int index = value.IndexOf(':');

                if (index == -1)
                    return value;

                return value[(index + 1)..].Trim();
            }

            
            List<string> modlist = await _modals.ModListLoad();

            if (modlist[0].StartsWith("W:"))
            {
                string addweapon = GetValue(modlist[0]);
                
                Weapon weapon = await _wservice.GetWeapon(addweapon);
                Weapon? existingWeapon = player.Weapons.FirstOrDefault(w => w.Name == modlist[0]);

                if (existingWeapon != null)
                {
                    existingWeapon.Amount++;
                }
                else
                {
                    player.Weapons.Add(weapon);
                }
                                               
                                                
                await PlayerRepo.Save(players);
                await _service.UpdatePlayer(player.Name);
                await _modals.ModListRemove();

                await FollowupAsync($"{player.Name} agora possui {weapon.Name}", ephemeral: true);
            }

            if (modlist[0].StartsWith("I:"))
            {
                string additem = GetValue(modlist[0]);
                var items = await ItemRepo.Load();
                Item item = items.FirstOrDefault(p => p.Name == additem);
                Item? existingItem = player.Items.FirstOrDefault(w => w.Name == additem);

                if (existingItem != null)
                {
                    existingItem.Amount++;
                }
                else
                {
                    player.Items.Add(item);
                }

                                
                await PlayerRepo.Save(players);
                await _service.UpdatePlayer(player.Name);
                await _modals.ModListRemove();

                await FollowupAsync($"{player.Name} agora possui {item.Name}", ephemeral: true);
            }


        }

        [ComponentInteraction("inventoryremove")]
        public async Task _inventoryremove(string[] values)
        {
            string jogador = values[0];
            Player player = await _service.GetPlayer(jogador);
            await _modals.ModListInput(jogador, "", "", "", "");

            var menu = new SelectMenuBuilder()
        .WithCustomId("inventoryremove:final")
        .WithPlaceholder("Escolha o item que será removido");

            menu.AddOption($"-----ARMAS-----", $"dummyw");

            foreach (Weapon weapon in player.Weapons)
            {
                menu.AddOption($"{weapon.Name}", $"W:{weapon.Name}");
            }

            menu.AddOption($"-----ITEMS-----", $"dummyi");

            foreach (Item item in player.Items)
            {
                menu.AddOption($"{item.Name}", $"I:{item.Name}");
            }

            var components = new ComponentBuilder()
            .WithSelectMenu(menu);

            await RespondAsync("Escolha qual item/arma remover:", components: components.Build(), ephemeral: true);

            
        }

        [ComponentInteraction("inventoryremove:final")]
        public async Task _inventoryremovefinal(string[] values)
        {
            await DeferAsync();

            string removed = values[0];
            var players = await PlayerRepo.Load();
            var modlist = await _modals.ModListLoad();
            Console.WriteLine($"{modlist[0]}");
            Player? player = players.FirstOrDefault(x => x.Name == modlist[0]);
            Console.WriteLine($"{player.Name}");

            string GetValue(string value)
            {
                int index = value.IndexOf(':');

                if (index == -1)
                    return value;

                return value[(index + 1)..].Trim();
            }

                        

            if (removed.StartsWith("W:"))
            {
                string addweapon = GetValue(removed);
                Weapon? weapon = player.Weapons.FirstOrDefault(p => p.Name == addweapon);
                
                
                if (weapon != null)
                {
                    Console.WriteLine($"{weapon.Name}");
                    player.Weapons.Remove(weapon);
                }
                                
                await PlayerRepo.Save(players);
                await _service.UpdatePlayer(player.Name);
                await _modals.ModListRemove();

                await FollowupAsync($"{player.Name} perdeu {addweapon}", ephemeral: true);
            }

            if (removed.StartsWith("I:"))
            {
                string additem = GetValue(removed);
                Item? item = player.Items.FirstOrDefault(p => p.Name == additem);
               
                if (item != null)
                {
                    player.Items.Remove(item);
                }

                
                await PlayerRepo.Save(players);
                await _service.UpdatePlayer(player.Name);
                await _modals.ModListRemove();

                await FollowupAsync($"{player.Name} perdeu {additem}", ephemeral: true);
            }


        }

        [ComponentInteraction("inventorysee")]
        public async Task _inventorysee(string[] values)
        {
            await DeferAsync();

            string jogador = values[0];
            var players = await PlayerRepo.Load();
            Player? player = players.FirstOrDefault(p => p.Name == jogador);

            Embed? embed = await _service.ShowInventory(jogador);

            await FollowupAsync(embed: embed, ephemeral: true);


        }

        //Skill buttons

        [ComponentInteraction("skills:*")]
        public async Task _skills(string acao)
        {
            switch (acao)
            {
                case "add":
                    var players = await PlayerRepo.Load();

                    var menu = new SelectMenuBuilder()
                    .WithCustomId($"skills:{acao}:select")
                    .WithPlaceholder("Selecione o jogador");

                    foreach (Player player in players)
                    {
                        menu.AddOption($"{player.Name}", $"{player.Name}");
                    }
                    
                    var components = new ComponentBuilder()
                    .WithSelectMenu(menu);

                    await RespondAsync("Escolha um jogador:", components: components.Build(), ephemeral: true);
                    break;

                case "remove":
                    var rplayers = await PlayerRepo.Load();

                    var rmenu = new SelectMenuBuilder()
                    .WithCustomId($"skills:{acao}:select")
                    .WithPlaceholder("Selecione o jogador");

                    Console.WriteLine($"skills:{acao}:select");

                    foreach (Player player in rplayers)
                    {
                        rmenu.AddOption($"{player.Name}", $"{player.Name}");
                    }

                    var rcomponents = new ComponentBuilder()
                    .WithSelectMenu(rmenu);

                    await RespondAsync("Escolha um jogador:", components: rcomponents.Build(), ephemeral: true);
                    break;
            }
                
        }

        [ComponentInteraction("skills:add:select")]
        public async Task _skilladdselect(string[] values)
        {
            string jogador = values[0];
            await _modals.ModListInput(jogador, "", "", "", "");
            await RespondWithModalAsync<CreateSkillModal>("create_skill");

        }

        [ComponentInteraction("skills:remove:select")]
        public async Task _skillremoveselect(string[] values)
        {
            string jogador = values[0];
            await _modals.ModListInput(jogador, "", "", "", "");

            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name == jogador);

            var rmenu = new SelectMenuBuilder()
                    .WithCustomId($"skillsremovefinal")
                    .WithPlaceholder("Selecione a skill");

            foreach (Skill skill in player.Skills)
            {
                rmenu.AddOption($"{skill.Name}", $"{skill.Name}");
            }

            var rcomponents = new ComponentBuilder()
            .WithSelectMenu(rmenu);

            await RespondAsync("Escolha uma skill:", components: rcomponents.Build(), ephemeral: true);


        }

        [ComponentInteraction("skillsremovefinal")]
        public async Task _skillfinalremov(string[] values)
        {
            var modlist = await _modals.ModListLoad();
            string chosenskill = values[0];
            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name == modlist[0]);

            var deleteskill = player.Skills.FirstOrDefault(p => p.Name == chosenskill);

            player.Skills.Remove(deleteskill);
            await PlayerRepo.Save(players);
            await _modals.ModListRemove();

            await RespondAsync($"Skill {chosenskill} removida de {modlist[0]}");

        }



        //Delete button interactions

        [ComponentInteraction("delselect")]
        public async Task _delete()
        {
            var players = await PlayerRepo.Load();

            var menu = new SelectMenuBuilder()
        .WithCustomId("del_player")
        .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
        .WithSelectMenu(menu);

            await RespondAsync("Escolha um jogador para editar:", components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction("del_player")]
        public async Task _delplayer(string[] values)
        {
            string jogador = values[0];
            await _service.DeletePlayer(jogador);
            await RespondAsync($"Jogador {jogador} deletado com sucesso.", ephemeral: true);
        }

        //Alt pic button interactions

        [ComponentInteraction("altpicselect")]
        public async Task _altpicselect(string[] values)
        {
            string jogador = values[0];
            List<string> modallist = await _modals.ModListLoad();

            await DeferAsync();
            await _service.AltPlayerPic(modallist, jogador);
            await _service.UpdatePlayer(jogador);
            await _modals.ModListRemove();

            await FollowupAsync($"Imagem do jogador {jogador} alterada com sucesso.", ephemeral: true);
        }

        [ComponentInteraction("close")]
        public async Task _close()
        {
            await RespondAsync("Close button clicked!", ephemeral: true);
        }

        //--------------------------Quick Change Section--------------------------

        [ComponentInteraction("qkchanger:*:*:*")]
        public async Task _qkchange(string acao, string jogador, string valor)
        {
            switch (acao)
            {
                case "hp":
                    await DeferAsync();
                    Player player = await _service.GetPlayer(jogador);
                    await _service.HPChange(jogador, int.Parse(valor));
                    await _service.UpdatePlayer(jogador);
                    await FollowupAsync($"{jogador} recebeu {valor} de HP. {player.HP} / {player.MaxHP}", ephemeral: true);
                    break;
                case "mp":
                    await DeferAsync();
                    Player mplayer = await _service.GetPlayer(jogador);
                    await _service.MPChange(jogador, int.Parse(valor));
                    await _service.UpdatePlayer(jogador);
                    await FollowupAsync($"{jogador} recebeu {valor} de MP. {mplayer.MP} / {mplayer.MaxMP}", ephemeral: true);
                    break;
                case "mind":
                    await DeferAsync();
                    Player miplayer = await _service.GetPlayer(jogador);
                    await _service.MindChange(jogador, int.Parse(valor));
                    await _service.UpdatePlayer(jogador);
                    await FollowupAsync($"{jogador} recebeu {valor} de Mind. {miplayer.Mind} / {miplayer.MaxMind}", ephemeral: true);
                    break;
            }
        }



        [ComponentInteraction("qkchanger:hp")]
        public async Task _qkchangehp(string[] values)
        {
            string jogador = values[0];
            Player player = await _service.GetPlayer(jogador);

            var button = new ComponentBuilder()
               .WithButton("<<", $"qkchanger:hp:{jogador}:-2", ButtonStyle.Primary)
               .WithButton("​<", $"qkchanger:hp:{jogador}:-1", ButtonStyle.Primary)
               .WithButton(">", $"qkchanger:hp:{jogador}:1", ButtonStyle.Primary)
               .WithButton(">>", $"qkchanger:hp:{jogador}:2", ButtonStyle.Primary);


            await RespondAsync($"Jogador: {jogador} (HP antes da troca: {player.HP})", components: button.Build(), ephemeral: true);


        }

        [ComponentInteraction("qkchanger:mp")]
        public async Task _qkchangemp(string[] values)
        {
            string jogador = values[0];
            Player player = await _service.GetPlayer(jogador);

            var button = new ComponentBuilder()
               .WithButton("<<", $"qkchanger:mp:{jogador}:-2", ButtonStyle.Primary)
               .WithButton("​<", $"qkchanger:mp:{jogador}:-1", ButtonStyle.Primary)
               .WithButton(">", $"qkchanger:mp:{jogador}:1", ButtonStyle.Primary)
               .WithButton(">>", $"qkchanger:mp:{jogador}:2", ButtonStyle.Primary);


            await RespondAsync($"Jogador: {jogador} (MP antes da troca: {player.MP})", components: button.Build(), ephemeral: true);


        }

        [ComponentInteraction("qkchanger:mind")]
        public async Task _qkchangemind(string[] values)
        {
            string jogador = values[0];
            Player player = await _service.GetPlayer(jogador);

            var button = new ComponentBuilder()
               .WithButton("<<", $"qkchanger:mind:{jogador}:-2", ButtonStyle.Primary)
               .WithButton("​<", $"qkchanger:mind:{jogador}:-1", ButtonStyle.Primary)
               .WithButton(">", $"qkchanger:mind:{jogador}:1", ButtonStyle.Primary)
               .WithButton(">>", $"qkchanger:mind:{jogador}:2", ButtonStyle.Primary);


            await RespondAsync($"Jogador: {jogador} (Mind antes da troca: {player.Mind})", components: button.Build(), ephemeral: true);


        }

        [ComponentInteraction("qkchange:*")]
        public async Task _hpbutt(string acao)
        {
            var players = await PlayerRepo.Load();

            Console.WriteLine("FUCKYOU");
            var menu = new SelectMenuBuilder()
        .WithCustomId($"qkchanger:{acao}")
        .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
        .WithSelectMenu(menu);

            await RespondAsync("Escolha um jogador:", components: components.Build(), ephemeral: true);
        }

        //--------------------------Quick Recovery Section--------------------------

        [ComponentInteraction("qkrecov:hp")]
        public async Task _qkrecover()
        {
            var players = await PlayerRepo.Load();

            Console.WriteLine("FUCKYOU");
            var menu = new SelectMenuBuilder()
        .WithCustomId($"qkrecover:hp")
        .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
        .WithSelectMenu(menu);

            await RespondAsync("Escolha um jogador:", components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction("qkrecov:mp")]
        public async Task _qkrecovermp()
        {
            var players = await PlayerRepo.Load();

            Console.WriteLine("FUCKYOU");
            var menu = new SelectMenuBuilder()
        .WithCustomId($"qkrecover:mp")
        .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
        .WithSelectMenu(menu);

            await RespondAsync("Escolha um jogador:", components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction("qkrecov:mind")]
        public async Task _qkrecovermind()
        {
            var players = await PlayerRepo.Load();

            Console.WriteLine("FUCKYOU");
            var menu = new SelectMenuBuilder()
        .WithCustomId($"qkrecover:mind")
        .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
        .WithSelectMenu(menu);

            await RespondAsync("Escolha um jogador:", components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction("qkrecov:all")]
        public async Task _qkrecoverall()
        {
            var players = await PlayerRepo.Load();
                        
            var menu = new SelectMenuBuilder()
        .WithCustomId($"qkrecover:all")
        .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
        .WithSelectMenu(menu);

            await RespondAsync("Escolha um jogador:", components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction("qkrecover:all")]
        public async Task _qkrecoverallmind(string[] values)
        {
            string jogador = values[0];

            await DeferAsync();
            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name.Equals(jogador, StringComparison.OrdinalIgnoreCase));

            player.HP = player.MaxHP;
            player.MP = player.MaxMP;
            player.Mind = player.MaxMind;

            await PlayerRepo.Save(players);
            await _service.UpdatePlayer(jogador);

            await FollowupAsync($"Jogador {jogador} recuperou completamente!", ephemeral: true);

        }

        [ComponentInteraction("qkrecover:hp")]
        public async Task _qkrecoverfunchp(string[] values)
        {
            string jogador = values[0];
            Console.WriteLine($"qkrecoverfunchhp, {jogador}");
            await DeferAsync();
            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name.Equals(jogador, StringComparison.OrdinalIgnoreCase));

            player.HP = player.MaxHP;

            await PlayerRepo.Save(players);
            await _service.UpdatePlayer(jogador);

            await FollowupAsync($"Jogador {jogador} recuperou toda vida!", ephemeral: true);


        }

        [ComponentInteraction("qkrecover:mp")]
        public async Task _qkrecoverfuncmp(string[] values)
        {
            string jogador = values[0];

            await DeferAsync();
            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name.Equals(jogador, StringComparison.OrdinalIgnoreCase));

            player.MP = player.MaxMP;

            await PlayerRepo.Save(players);
            await _service.UpdatePlayer(jogador);

            await FollowupAsync($"Jogador {jogador} recuperou toda mana!", ephemeral: true);

        }

        [ComponentInteraction("qkrecover:mind")]
        public async Task _qkrecoverfuncmind(string[] values)
        {
            string jogador = values[0];

            await DeferAsync();
            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name.Equals(jogador, StringComparison.OrdinalIgnoreCase));

            player.Mind = player.MaxMind;

            await PlayerRepo.Save(players);
            await _service.UpdatePlayer(jogador);

            await FollowupAsync($"Jogador {jogador} recuperou todo mind!", ephemeral: true);

        }
        //--------------------------Transform Section--------------------------


        [ComponentInteraction("transbutt")]
        public async Task _qktransselect()
        {
            var players = await PlayerRepo.Load();

            Console.WriteLine("FUCKYOU");
            var menu = new SelectMenuBuilder()
            .WithCustomId($"transform")
            .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
            .WithSelectMenu(menu);

            await RespondAsync("Escolha um jogador:", components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction("transform")]
        public async Task _transform(string[] values)
        {
            string jogador = values[0];
            Player? player = await _service.GetPlayer(jogador);

            await DeferAsync();
            int tcheck = await _service.Transform(jogador);

            if (tcheck == 3)
            {
                await _service.UpdatePlayer(jogador);
                await FollowupAsync($"{jogador} não tem transformação.", ephemeral: true);
            }
            else if (tcheck == 1)
            {
                await _service.UpdatePlayer(jogador);
                await FollowupAsync($"{jogador} transformado!", ephemeral: true);
            }
            else if (tcheck == 0)
            {
                await _service.UpdatePlayer(jogador);
                await FollowupAsync($"{jogador} teve a transformação revertida!", ephemeral: true);
            }
        }

        
    }
}
