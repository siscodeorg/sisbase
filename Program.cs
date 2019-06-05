using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using LA_RPbot.Discord;
using LA_RPbot.Discord.Utils;
using Newtonsoft.Json;
using static System.Reflection.Assembly;

namespace LA_RPbot.Discord
{
    public class Program
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension CommandsNext { get; set; }
        public static InteractivityExtension Interactivity { get; set; }
        public static Config Config { get; set; }

        public static List<Type> systems = new List<Type>();
        public static Program Instance { get; set; }
        static void Main(string[] args)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "/Config.json"))
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Directory.GetCurrentDirectory() + "/Config.json"));
                systems = GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IApplicableSystem))).ToList();
                Instance = new Program();
                
                Init().GetAwaiter().GetResult();
            }
            else
            {
                Config = TUI_cfg();
                File.WriteAllText(Directory.GetCurrentDirectory()+"/Config.json",JsonConvert.SerializeObject(Config, Formatting.Indented));
                systems = GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IApplicableSystem))).ToList();
                Instance = new Program();
                
                Init().GetAwaiter().GetResult();
            }
            
        }

        private Program()
        {
            Client = new DiscordClient(new DiscordConfiguration
            {
                Token = Config.Token,
                AutoReconnect = true,
                AutomaticGuildSync = true,
                UseInternalLogHandler = true,
                TokenType = TokenType.Bot
            });

            CommandsNext = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolver,
                EnableDefaultHelp = false,
                EnableDms = true
            });

            Interactivity = Client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehaviour = PaginationBehaviour.WrapAround,
                Timeout = TimeSpan.FromMinutes(10),
                PaginationDeletion = PaginationDeletion.DeleteEmojis
            });
            foreach (var system in systems)
            {
                if (!system.GetInterfaces().Contains(typeof(IApplyToInteractivity))) continue;
                var instance = (IApplyToInteractivity)Activator.CreateInstance(system);
                instance.ApplyToInteractivity(Interactivity);
                Console.WriteLine($"[System] {system.Name} Loaded");
            }
            CommandsNext.RegisterCommands(GetExecutingAssembly());
            Client.GuildDownloadCompleted += GuildFinished;
        }

        private static async Task GuildFinished(GuildDownloadCompletedEventArgs e)
        {
            if (Config.MasterId == 0)
            {
                if (Client.Guilds.Count > 1)
                {
                    var names = Client.Guilds.Aggregate("", (current, guild) => current + $"{guild.Value.Name}, ");

                    foreach (KeyValuePair<ulong, DiscordGuild> guild in Client.Guilds)
                    {
                        var ch = guild.Value.GetDefaultChannel();
                        var builder = new DiscordEmbedBuilder();
                        
                        builder
                            .AddField("List of Servers", names)
                            .WithDescription($"Bot is loaded on multiple servers, please use {Client.CurrentUser.Mention}setMaster on the MASTER guild.")
                            .WithAuthor("Error on determining MASTER server")
                            .WithColor(DiscordColor.Red);
                        await ch.SendMessageAsync(embed:builder);
                    }
                    
                }
                else
                {
                    var ch = Client.Guilds.Values.ToList()[0].GetDefaultChannel();
                    var builder = new DiscordEmbedBuilder();
                    builder
                        .WithAuthor("Guild set as MASTER guild")
                        .WithColor(DiscordColor.PhthaloGreen);
                    Config.MasterId = Client.Guilds.Values.ToList()[0].Id;
                    File.WriteAllText(Directory.GetCurrentDirectory()+"Config.json",JsonConvert.SerializeObject(Config, Formatting.Indented));
                }
            }
            
        }

        private static Config TUI_cfg()
        {
            Config c = new Config();
            Console.WriteLine("Please Input the TOKEN :");
            c.Token = Console.ReadLine();
            c.MasterId = 0;
            c.PuppetId = new List<ulong?>();
            c.Prefixes = new List<string>();
            Console.WriteLine("Configuration Completed.");
            return c;
        }

        private static async Task Init()
        {
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private async Task<int> PrefixResolver(DiscordMessage msg)
        {
            switch (msg.GetMentionPrefixLength(Client.CurrentUser))
            {
                case -1:
                    int x;
                    foreach (var prefix in Config.Prefixes)
                    {
                        x = msg.GetStringPrefixLength(prefix);
                        if (x != -1)
                            return x;
                    }

                    break;
                default:
                    return msg.GetMentionPrefixLength(Client.CurrentUser);
            }

            return -1;
        }
    }
}