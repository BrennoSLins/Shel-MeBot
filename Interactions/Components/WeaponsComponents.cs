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
using static Shel_MeBotDB.WeaponModals;


namespace Shel_MeBotDB
{
    public class WeaponsComponents : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly WeaponService _service;
        private readonly InteractionService _interactions;
        private readonly WeaponModals _modals;
        private readonly PlayerService _pservice;

        public WeaponsComponents(WeaponService service, InteractionService interactions, WeaponModals modals, PlayerService pservice)
        {
            _service = service;
            _interactions = interactions;
            _modals = modals;
            _pservice = pservice;
        }

        //--------------------------------Weapon management section-----------------------------
        [ComponentInteraction("wadd")]
        public async Task _add()
        {
            await RespondWithModalAsync<StapleWeaponModal>("create_weapon");
        }
                

        [ComponentInteraction("weditbutt")]
        public async Task _editt()
        {
            await RespondWithModalAsync<StapleWeaponModal>("edit_weapon");

        }

        [ComponentInteraction("weditselect")]
        public async Task _weditselect(string[] values)
        {
            string weapon = values[0];

            List<string> modallist = await _modals.ModListLoad();

            await _service.EditWeapon(modallist, weapon);

            if (!modallist.Contains($"Name:"))
            {
                string newweapon = modallist.ElementAtOrDefault(0) ?? "";
                await _service.UpdateWeapon(newweapon);
            }
            else
            {
                await _service.UpdateWeapon(weapon);
            }
            await _modals.ModListRemove();

            await RespondAsync($"Arma {weapon} editado com sucesso", ephemeral: true);

        }

        [ComponentInteraction("wdelselect")]
        public async Task _wdelselect()
        {
            List<Weapon> weapons = await WeaponRepo.Load();

            var menu = new SelectMenuBuilder();
            menu.WithCustomId($"wdelmenu")
                .WithPlaceholder("Escolha uma arma");

            foreach (Weapon weapon in weapons)
            {
                menu.AddOption($"{weapon.Name}", $"{weapon.Name}");
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(menu);

            await RespondAsync("Escolhe a arma que será apagada:", components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction("wdelmenu")]
        public async Task _wdelmenu(string[] values)
        {
            string arma = values[0];

            await _service.DeleteWeapon(arma);
            await RespondAsync($"Arma {arma} deletad com sucesso.", ephemeral: true);
        }

        //-------------------------Weapon State Section---------------------

        [ComponentInteraction("wchange:*")]
        public async Task _wchange(string state)
        {
            switch (state)
            {
                case "1":

                    List<Weapon> weapons = await WeaponRepo.Load();

                    var menu = new SelectMenuBuilder();
                    menu.WithCustomId($"wchange:{state}:select")
                        .WithPlaceholder("Escolha uma arma");

                    foreach (Weapon weapon in weapons)
                    {
                        menu.AddOption($"{weapon.Name}", $"{weapon.Name}");
                    }

                    var components = new ComponentBuilder()
                        .WithSelectMenu(menu);

                    await RespondAsync("Escolhe a arma que será transformada:", components: components.Build(), ephemeral: true);
                    break;

                case "2":

                    List<Weapon> shweapons = await WeaponRepo.Load();

                    var shmenu = new SelectMenuBuilder();
                    shmenu.WithCustomId($"wchange:{state}:select")
                        .WithPlaceholder("Escolha uma arma");

                    foreach (Weapon weapon in shweapons)
                    {
                        shmenu.AddOption($"{weapon.Name}", $"{weapon.Name}");
                    }

                    var shcomponents = new ComponentBuilder()
                        .WithSelectMenu(shmenu);

                    await RespondAsync("Escolhe a arma que será transformada:", components: shcomponents.Build(), ephemeral: true);
                    break;

                case "3":
                    List<Weapon> bweapons = await WeaponRepo.Load();

                    var bmenu = new SelectMenuBuilder();
                    bmenu.WithCustomId($"wchange:{state}:select")
                        .WithPlaceholder("Escolha uma arma");

                    foreach (Weapon weapon in bweapons)
                    {
                        bmenu.AddOption($"{weapon.Name}", $"{weapon.Name}");
                    }

                    var bcomponents = new ComponentBuilder()
                        .WithSelectMenu(bmenu);

                    await RespondAsync("Escolhe a arma que será transformada:", components: bcomponents.Build(), ephemeral: true);
                    break;
            }


        }
        [ComponentInteraction("wchange:1:select")]
        public async Task _wchange1select(string[] values)
        {
            Console.WriteLine("got here: wchange:1select");
            string weaponvalue = values[0];
            await DeferAsync();

            var weapons = await WeaponRepo.Load();
            var weapon = weapons.FirstOrDefault(p => p.Name.Equals(weaponvalue, StringComparison.OrdinalIgnoreCase));

            weapon.State = 1;
            await WeaponRepo.Save(weapons);
            await _service.UpdateWeapon(weapon.Name);

            await FollowupAsync($"{weapon.Name} agora está em modo normal", ephemeral: true);


        }

        [ComponentInteraction("wchange:2:select")]
        public async Task _wchange2select(string[] values)
        {
            Console.WriteLine("got here: wchange:2select");
            string weaponvalue = values[0];
            await DeferAsync();
            var weapons = await WeaponRepo.Load();
            var weapon = weapons.FirstOrDefault(p => p.Name.Equals(weaponvalue, StringComparison.OrdinalIgnoreCase));
            weapon.State = 2;
            await WeaponRepo.Save(weapons);
            await _service.UpdateWeapon(weapon.Name);

            await FollowupAsync($"{weapon.Name} agora está em modo shikai", ephemeral: true);


        }

        [ComponentInteraction("wchange:3:select")]
        public async Task _wchange3select(string[] values)
        {
            Console.WriteLine("got here: wchange:3select");
            string weaponvalue = values[0];
            await DeferAsync();

            var weapons = await WeaponRepo.Load();
            var weapon = weapons.FirstOrDefault(p => p.Name.Equals(weaponvalue, StringComparison.OrdinalIgnoreCase));


            weapon.State = 3;
            await WeaponRepo.Save(weapons);
            await _service.UpdateWeapon(weapon.Name);
            await _pservice.UpdatePlayer("Foda");
            

            await FollowupAsync($"{weapon.Name} agora está em modo bankai", ephemeral: true);
        }
    } 
}