using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Shel_MeBot_2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Windows.Input;

public class Bot
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly Commands _commands;

    public Bot(
        DiscordSocketClient client,
        InteractionService interactions,
        IServiceProvider services)
    {
        _client = client;
        _interactions = interactions;
        _services = services;
    }


    public async Task StartAsync()
    {

        _client.Log += Log;
        _client.Ready += Ready;
        _client.InteractionCreated += HandleInteraction;


        Console.WriteLine("Conectando...");

        string json = File.ReadAllText("D:\\Work\\Projects\\Shel MeBot 2\\Shel MeBot 2\\Config\\config.json");
        Config config = JsonSerializer.Deserialize<Config>(json)!;


        await _client.LoginAsync(TokenType.Bot, config.Token);

        await _interactions.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

        await _client.StartAsync();






        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }

    private async Task Ready()
    {
        Console.WriteLine($"Bot online! ({_client.CurrentUser.Username})");
        //await _interactions.RegisterCommandsGloballyAsync();
        //Console.WriteLine($"{DateTime.Now:HH:mm:ss}Comandos registrados globalmente!");
        await _interactions.RegisterCommandsToGuildAsync(543844510317150259);
        await _interactions.RegisterCommandsToGuildAsync(1449270736727576598);

    }

    private async Task HandleInteraction(SocketInteraction interaction)
    {
        {
            try
            {

                {
                    Console.WriteLine($"Handle Interaction arrived! {interaction.Type}");
                    var context = new SocketInteractionContext(_client, interaction);

                    await _interactions.ExecuteCommandAsync(context, _services);
                    Console.WriteLine("Execute command async finished!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }

    public async Task<IMessageChannel> GetMessageChannelID(ulong channelID)
    {

        var channel = _client.GetChannel(channelID) as IMessageChannel;

        if (channel == null)
            return null;
        return channel;
    }
}

    