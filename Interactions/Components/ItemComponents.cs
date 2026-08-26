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
using static Shel_MeBotDB.ItemModals;
using static Shel_MeBotDB.WeaponModals;

namespace Shel_MeBotDB
{
    public class ItemComponents : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly ItemService _service;
        private readonly InteractionService _interactions;
        private readonly ItemModals _modals;
        private readonly PlayerService _pservice;

        public ItemComponents(ItemService service, InteractionService interactions, ItemModals modals, PlayerService pservice)
        {
            _service = service;
            _interactions = interactions;
            _modals = modals;
            _pservice = pservice;
        }


        //--------------------------------Item management section-----------------------------
        
        [ComponentInteraction("itemadd")]
        public async Task _itemadd()
        {
            await RespondWithModalAsync<StapleItemModal>("create_item");
        }

        [ComponentInteraction("itemedit")]
        public async Task _itemedit()
        {
            await RespondWithModalAsync<StapleItemModal>("edit_item");
        }

        [ComponentInteraction("itemsee")]
        public async Task _itemsee()
        {
            List<Item> items = await ItemRepo.Load();

            var menu = new SelectMenuBuilder()
            .WithCustomId($"itemsee:select")
            .WithPlaceholder("Escolha um item");
                                

            foreach (Item item in items)
            {
                menu.AddOption($"{item.Name}", $"{item.Name}");
            }

            var components = new ComponentBuilder()
            .WithSelectMenu(menu);

            await RespondAsync("Escolha qual item ver:", components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction("itemsee:select")]
        public async Task _itemseeselect(string[] values)
        {
            string selecteditem = values[0];
            
            Embed? embed = await _service.ShowItemEmbed(selecteditem);
            
            await RespondAsync(embed: embed, ephemeral: true);
        }

        [ComponentInteraction("edititemselect")]
        public async Task _itemeditselect(string[] values)
        {
            string selitem = values[0];

            List<string> modallist = await _modals.ModListLoad();

            await _service.EditItem(modallist, selitem);

            if (!modallist.Contains($"Name:"))
            {
                string newitem = modallist.ElementAtOrDefault(0) ?? "";
                await _service.UpdateItem(newitem);
            }
            else
            {
                await _service.UpdateItem(selitem);
            }
            await _modals.ModListRemove();

            await RespondAsync($"Item {selitem} editado com sucesso", ephemeral: true);
        }

        [ComponentInteraction("itemdelete")]
        public async Task _itemdelete()
        {
            List<Item> items = await ItemRepo.Load();

            var menu = new SelectMenuBuilder()
            .WithCustomId($"itemdelete:select")
            .WithPlaceholder("Escolha um item");


            foreach (Item item in items)
            {
                menu.AddOption($"{item.Name}", $"{item.Name}");
            }

            var components = new ComponentBuilder()
            .WithSelectMenu(menu);

            await RespondAsync("Escolha qual item deletar:", components: components.Build(), ephemeral: true);
        }

        [ComponentInteraction("itemdelete:select")]
        public async Task _itemdeleteselect(string[] values)
        {
            string selected = values[0];

            List<Item> items = await ItemRepo.Load();
            Item? removeitem = items.FirstOrDefault(p => p.Name == selected);

            if (removeitem != null)
            {
                items.Remove(removeitem);
                await RespondAsync($"Item {selected} removido.", ephemeral: true);
                await ItemRepo.Save(items);
                return;

            }

            await RespondAsync($"Item não encontrado", ephemeral: true);
            

        }
    }
}
