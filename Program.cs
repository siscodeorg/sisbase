using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using sisbase.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Reflection.Assembly;

namespace sisbase
{
    public class Program
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension CommandsNext { get; set; }
        public static InteractivityExtension Interactivity { get; set; }
        public static Config Config { get; set; }

        public static List<Type> systems = new List<Type>();
        public static Program Instance { get; set; }

        private static void Main(string[] args)
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "/Config.json"))
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Directory.GetCurrentDirectory() + "/Config.json"));
                systems = GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(ISystem)) && x.IsClass ).ToList();
                Instance = new Program();

                Init().GetAwaiter().GetResult();
            }
            else
            {
                Config = TUI_cfg();
                File.WriteAllText(Directory.GetCurrentDirectory() + "/Config.json", JsonConvert.SerializeObject(Config, Formatting.Indented));
                systems = GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Contains(typeof(ISystem))).ToList();
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
                var instance = (ISystem)Activator.CreateInstance(system);
                instance.Activate();
                instance.Log("System started");
                instance.Execute();
                if (system.GetInterfaces().Contains(typeof(IApplyToClient)))
                {
                    ((IApplyToClient)instance).ApplyToClient(Client);
                    instance.Log("System applied to client");
                    Console.WriteLine($"[System] {system.Name} Loaded");
                }
            }
            CommandsNext.RegisterCommands(GetExecutingAssembly());
        }


        private static Config TUI_cfg()
        {
            var c = new Config();
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

#pragma warning disable CS1998 
        private async Task<int> PrefixResolver(DiscordMessage msg)
#pragma warning restore CS1998 
        {
            switch (msg.GetMentionPrefixLength(Client.CurrentUser))
            {
                case -1:
                    int x;
                    foreach (string prefix in Config.Prefixes)
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