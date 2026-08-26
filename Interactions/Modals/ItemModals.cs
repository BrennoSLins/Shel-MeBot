using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.VisualBasic;
using Shel_MeBotDB;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text;
using System.Text.Json;
using static Shel_MeBotDB.WeaponModals;

namespace Shel_MeBotDB
{
    public class ItemModals : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ItemService _service;
        private readonly InteractionService _interactions;


        public ItemModals(ItemService service, InteractionService interactions)
        {
            _service = service;
            _interactions = interactions;

        }

        public class StapleItemModal : IModal
        {
            public string Title => "Informações do item";

            [InputLabel("Nome")]
            [ModalTextInput("name")]
            [RequiredInput(false)]
            public string Name { get; set; }

            [InputLabel("Descrição")]
            [ModalTextInput("desc", TextInputStyle.Paragraph,
                placeholder: "Digite a descrição do item...",
                maxLength: 4000)]
            [RequiredInput(false)]
            public string Desc { get; set; }

            [InputLabel("Imagem")]
            [ModalTextInput("img")]
            [RequiredInput(false)]
            public string Img { get; set; }

            [InputLabel("Emoji")]
            [ModalTextInput("emoji")]
            [RequiredInput(false)]
            public string Emoji { get; set; }


        }

        [ModalInteraction("create_item")]
        public async Task _createWeapon(StapleItemModal modal)
        {

            if (modal.Name == "")
            {
                await RespondAsync($"Selecione pelo menos um nome filha da puta", ephemeral: true);
            }

            await _service.CreateItem(modal.Name, modal.Desc, modal.Img, modal.Emoji);

            await RespondAsync($"Item {modal.Name} criado com sucesso", ephemeral: true);

        }

        [ModalInteraction("edit_item")]
        public async Task _editWeapon(StapleItemModal modal)
        {

            await ModListInput($"Name:{modal.Name}", $"Desc:{modal.Desc}", $"Img:{modal.Img}", $"Emoji:{modal.Emoji}", $"Bankai:");

            List<Item> items = await ItemRepo.Load();

            var menu = new SelectMenuBuilder();
            menu.WithCustomId($"edititemselect")
                .WithPlaceholder("Escolha um item");

            foreach (Item item in items)
            {
                menu.AddOption($"{item.Name}", $"{item.Name}");
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(menu);

            await RespondAsync("Escolhe o item que será editado:", components: components.Build(), ephemeral: true);


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

            if (two != "")
            {
                jmodal.Jlist.Add(two);
            }

            if (three != "")
            {
                jmodal.Jlist.Add(three);
            }

            if (four != "")
            {
                jmodal.Jlist.Add(four);
            }

            if (five != "")
            {
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
