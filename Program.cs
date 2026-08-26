using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Shel_MeBot_2;

var provider = DI.Services();

var bot = provider.GetRequiredService<Bot>();

await bot.StartAsync();






