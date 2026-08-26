using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Numerics;
using System.Text;
using Shel_MeBotDB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.IO;

namespace Shel_MeBotDB
{
    public class WeaponModals : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly WeaponService _service;
        private readonly InteractionService _interactions;
       

        public WeaponModals(WeaponService service, InteractionService interactions)
        {
            _service = service;
            _interactions = interactions;
            
        }

        public class StapleWeaponModal : IModal
        {
            public string Title => "Informações da arma";

            [InputLabel("Nome")]
            [ModalTextInput("name")]
            [RequiredInput(false)]
            public string Name { get; set; }

            [InputLabel("Descrição")]
            [ModalTextInput("desc", TextInputStyle.Paragraph,
                placeholder: "Digite a descrição da arma...",
                maxLength: 4000)]
            [RequiredInput(false)]
            public string Desc { get; set; }

            [InputLabel("Imagem(normal)")]
            [ModalTextInput("img")]
            [RequiredInput(false)]
            public string Img { get; set; }

            [InputLabel("Imagem(shikai)")]
            [ModalTextInput("shikai")]
            [RequiredInput(false)]
            public string ImgShikai { get; set; }

            [InputLabel("Imagem(bankai)")]
            [ModalTextInput("bankai")]
            [RequiredInput(false)]
            public string ImgBankai { get; set; }


        }

        

        //Modal interaction handlers
        //Create weapon handler

        [ModalInteraction("create_weapon")]
        public async Task _createWeapon(StapleWeaponModal modal)
        {

            await _service.CreateWeapon(modal.Name, modal.Desc, modal.Img, modal.ImgShikai, modal.ImgBankai);
                                   
            await RespondAsync($"Arma {modal.Name} criada com sucesso", ephemeral: true);


        }

        //Edit weapon handlers


        [ModalInteraction("edit_weapon")]
        public async Task _editPlayer(StapleWeaponModal modal)
        {
            Console.WriteLine("Got here");
            await ModListInput($"Name:{modal.Name}", $"Desc:{modal.Desc}", $"Img:{modal.Img}", $"Shikai:{modal.ImgShikai}", $"Bankai:{modal.ImgBankai}");

            List<Weapon> weapons = await WeaponRepo.Load();

            var menu = new SelectMenuBuilder();
            menu.WithCustomId($"weditselect")
                .WithPlaceholder("Escolha uma arma");

            foreach (Weapon weapon in weapons)
            {
                menu.AddOption($"{weapon.Name}", $"{weapon.Name}");
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(menu);

            await RespondAsync("Escolhe a arma que será editada:", components: components.Build(), ephemeral: true);

        }

       

                   
        //Modal data transfer functions

        public async Task ModListInput(string? one, string? two, string? three, string? four, string? five)
        {

            Jmodal jmodal = new Jmodal();         
            
            jmodal.Jlist = new List<string>();

            if (one != "")
            {
                jmodal.Jlist.Add(one);
            }

            if (two != "") {
                jmodal.Jlist.Add(two);
            }

            if (three != "") {
                jmodal.Jlist.Add(three);
            }

            if (four != "") {
                jmodal.Jlist.Add(four);
            }

            if (five != "") {
                jmodal.Jlist.Add(five);
            }

            string json = JsonSerializer.Serialize(jmodal, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync("Jlist.json", json);
        }

        public async Task<List<string>> ModListLoad()
        {
            string json = await File.ReadAllTextAsync("Jlist.json");
            Jmodal jmodal = JsonSerializer.Deserialize<Jmodal>(json)!;
            return jmodal.Jlist;
        }

        public async Task ModListRemove()
        {
            string json = await File.ReadAllTextAsync("Jlist.json");
            Jmodal jmodal = JsonSerializer.Deserialize<Jmodal>(json)!;
            jmodal.Jlist.Clear();
            json = JsonSerializer.Serialize(jmodal, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync("Jlist.json", json);
        }
    }
}
