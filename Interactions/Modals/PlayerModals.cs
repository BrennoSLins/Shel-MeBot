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
using System.Security.AccessControl;
using System.Text;
using System.Text;
using System.Text.Json;

namespace Shel_MeBotDB
{
    public class PlayerModals : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly PlayerService _service;
        private readonly InteractionService _interactions;
       

        public PlayerModals(PlayerService service, InteractionService interactions)
        {
            _service = service;
            _interactions = interactions;
            
        }

        public class CreatePlayerModal : IModal
        {
            public string Title => "Informações do Jogador";

            [InputLabel("Nome")]
            [ModalTextInput("nome")]

            public string Nome { get; set; }

            [InputLabel("HP")]
            [ModalTextInput("hp")]
            public int HP { get; set; }

            [InputLabel("MP")]
            [ModalTextInput("mp")]
            public int MP { get; set; }

            [InputLabel("Mind")]
            [ModalTextInput("mind")]

            public int Mind { get; set; }

            [InputLabel("Link da imagem")]
            [ModalTextInput("newimg")]
            [RequiredInput(false)]
            public string? NewImg { get; set; }


        }

        public class EditPlayerModal : IModal
        {
            public string Title => "Informações do Jogador";

            
            [InputLabel("Novo nome")]
            [ModalTextInput("newname")]
            [RequiredInput(false)]
            public string NewName { get; set; }

            [InputLabel("HP")]
            [ModalTextInput("hp")]
            [RequiredInput(false)]
            public int HP { get; set; }

            [InputLabel("MP")]
            [ModalTextInput("mp")]
            [RequiredInput(false)]
            public int MP { get; set; }

            [InputLabel("Mind")]
            [ModalTextInput("mind")]
            [RequiredInput(false)]
            public int Mind { get; set; }



        }

        public class AddInfoModal : IModal
        {
            public string Title => "Digite as informações extras";


            [InputLabel("Informação")]
            [ModalTextInput("info1")]
            public string info1 { get; set; }

            [InputLabel("Informação")]
            [ModalTextInput("info2")]
            [RequiredInput(false)]
            public string? info2 { get; set; }

            [InputLabel("Informação")]
            [ModalTextInput("info3")]
            [RequiredInput(false)]
            public string? info3 { get; set; }

            [InputLabel("Informação")]
            [ModalTextInput("info4")]
            [RequiredInput(false)]
            public string? info4 { get; set; }

            [InputLabel("Informação")]
            [ModalTextInput("info5")]
            [RequiredInput(false)]
            public string? info5 { get; set; }


        }

        public class AltPicModal : IModal
        {
            public string Title => "Mande o link da imagem.";

            [InputLabel("Foto do personagem")]
            [ModalTextInput("charpic")]
            public string MCharPic { get; set; }

            [InputLabel("Foto da transformação")]
            [ModalTextInput("transpic")]
            [RequiredInput(false)]
            public string MTransPic { get; set; }


        }

        public class CreateSkillModal : IModal
        {
            public string Title => "Criando skill para personagem.";

            [InputLabel("Nome da skill")]
            [ModalTextInput("skillname")]
            public string Name { get; set; }

            [InputLabel("Descrição da skill")]
            [ModalTextInput("skilldesc",
                TextInputStyle.Paragraph,
                placeholder: "Digite a descrição da habilidade...",
                maxLength: 4000)]
            public string Desc { get; set; }

            [InputLabel("Atributo usado")]
            [ModalTextInput("skillamount")]
            public int Amount { get; set; }

            [InputLabel("Recurso usado")]
            [ModalTextInput("skillresource", placeholder: "HP, MP, Mind")]
            public string Resource { get; set; }


        }


        //Modal interaction handlers
        //Create player handler

        [ModalInteraction("create_player")]
        public async Task _createPlayer(CreatePlayerModal modal)
        {

            await _service.CreatePlayer(modal.Nome, modal.HP, modal.MP, modal.Mind, modal.NewImg);
            await RespondAsync($"Jogador {modal.Nome} criado com sucesso!", ephemeral: true);


        }

        //Edit player handlers


        [ModalInteraction("medit")]
        public async Task _editPlayer(EditPlayerModal modal)
        {
            Console.WriteLine("Got here");
            await ModListInput(modal.NewName,(modal.HP.ToString()),(modal.MP.ToString()), (modal.Mind.ToString()), "");

            List<Player> players = await PlayerRepo.Load();

            var menu = new SelectMenuBuilder();
            menu.WithCustomId($"editselect")
                .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(menu);

            await RespondAsync("Escolhe o jogador que será editado:", components: components.Build(), ephemeral: true);

        }

        [ModalInteraction("maddinfo")]
        public async Task _maddInfo(AddInfoModal modal)
        {
            Console.WriteLine("Got here");
            await ModListInput(modal.info1, modal.info2, modal.info3, modal.info4, modal.info5);

            List<Player> players = await PlayerRepo.Load();

            var menu = new SelectMenuBuilder();
            menu.WithCustomId($"addinfoselect")
                .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(menu);

            await RespondAsync("Escolha o jogador:", components: components.Build(), ephemeral: true);


        }

        [ModalInteraction("maltpic")]
        public async Task _maltPic(AltPicModal modal)
        {
            await ModListInput(modal.MCharPic, modal.MTransPic, "", "", "");

            List<Player> players = await PlayerRepo.Load();

            var menu = new SelectMenuBuilder();
            menu.WithCustomId($"altpicselect")
                .WithPlaceholder("Escolha um jogador");

            foreach (Player player in players)
            {
                menu.AddOption($"{player.Name}", $"{player.Name}");
            }

            var components = new ComponentBuilder()
                .WithSelectMenu(menu);

            await RespondAsync("Escolha o jogador:", components: components.Build(), ephemeral: true);


        }

        //Skill modals

        [ModalInteraction("create_skill")]
        public async Task _createskill(CreateSkillModal modal)
        {
            ResourceType resourcetype;


            switch (modal.Resource.ToLower())
            {
                case "hp":
                    resourcetype = ResourceType.hp;
                    break;

                case "mp":
                    resourcetype = ResourceType.mp;
                    break;

                case "mind":
                    resourcetype = ResourceType.mind;
                    break;

                default:
                    await RespondAsync("Recurso inválido, escolha entre HP, MP ou Mind", ephemeral: true);
                    throw new ArgumentException("Recurso inválido.");
            }

            Skill newskill = await _service.CreateSkill(modal.Name, modal.Desc, modal.Amount, resourcetype);
            var modlist = await ModListLoad();
            var players = await PlayerRepo.Load();
            var player = players.FirstOrDefault(p => p.Name == modlist[0]);

            player.Skills.Add(newskill);
            await PlayerRepo.Save(players);
            await _service.UpdatePlayer(player.Name);

            await RespondAsync("Skill criada com sucesso", ephemeral: true);

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
