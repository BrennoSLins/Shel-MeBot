using Discord;
using Discord.WebSocket;
using Shel_MeBot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Windows.Input;
using System.Text.Json;
using System.IO;

public class Bot
{
    public DiscordSocketClient _client;
    public Service _service;
    public Commands _commands;

    
    
    public async Task StartAsync()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents =
        GatewayIntents.Guilds |
        GatewayIntents.GuildMessages |
        GatewayIntents.MessageContent
        });

        _client.Log += Log;
        _client.Ready += Ready;


        _service = new Service(this);
        _commands = new Commands(_service);

        _client.MessageReceived += _commands.DCommands;
                      

        Console.WriteLine("Conectando...");

        string json = File.ReadAllText("config.json");
        Config config = JsonSerializer.Deserialize<Config>(json)!;

        
        await _client.LoginAsync(TokenType.Bot, config.Token);
        await _client.StartAsync();


        await Task.Delay(-1);
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg);
        return Task.CompletedTask;
    }

    private Task Ready()
    {
        Console.WriteLine($"Bot online! ({_client.CurrentUser.Username})");
        return Task.CompletedTask;
    }

    


}