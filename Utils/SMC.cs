using DSharpPlus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace sisbase.Utils
{
    /// <summary>
    /// The System Management Controller
    /// Now with 100% less PPBUS_G3H
    /// </summary>
    public class SMC
    {
        public static ConcurrentDictionary<Type,ISystem> RegisteredSystems { get; set; } = new ConcurrentDictionary<Type, ISystem>();
        public static void Register<T>() where T : IStaticSystem
        {
            if (RegisteredSystems.ContainsKey(typeof(T)))
            {
                ISystem system;
                RegisteredSystems.TryGetValue(typeof(T), out system);
                system.Warn("This system is already registered");
            }
            else
            {
                var system = Activator.CreateInstance<T>();
                system.Activate();
                system.Execute();
                system.Log("System Activated");
                RegisteredSystems.AddOrUpdate(typeof(T), system, (key,old) =>  system );
            }
        }
        public static void Unregister<T>() where T : IStaticSystem
        {
            if (RegisteredSystems.ContainsKey(typeof(T)))
            {
                ISystem system;
                RegisteredSystems.TryGetValue(typeof(T), out system);
                system.Warn("System is disabling...");
                system.Deactivate();
                RegisteredSystems.TryRemove(typeof(T), out system);
                Logger.Log("SMC", $"A System was disabled : {system.Name}");
            }
            else
            {
                Logger.Warn("SMC", "An unregistered system has attemped unregistering.");
            }
        }
    }
    public static class SMCExtensions
    {
        public static void Register<T>(this DiscordClient client) where T : IClientSystem
        {
            if (SMC.RegisteredSystems.ContainsKey(typeof(T)))
            {
                ISystem system;
                SMC.RegisteredSystems.TryGetValue(typeof(T), out system);
                system.Warn("This system is already registered");

            }
            else
            {
                var system = Activator.CreateInstance<T>();
                system.Activate();
                system.ApplyToClient(client);
                system.Execute();
                system.Log("System Activated");
                SMC.RegisteredSystems.AddOrUpdate(typeof(T), system, (key, old) => system);
            }
        }
        public static void Unregister<T>() where T : IClientSystem
        {
            if (SMC.RegisteredSystems.ContainsKey(typeof(T)))
            {
                ISystem system;
                SMC.RegisteredSystems.TryGetValue(typeof(T), out system);
                system.Warn("System is disabling...");
                system.Deactivate();
                SMC.RegisteredSystems.TryRemove(typeof(T), out system);
                Logger.Log("SMC", $"A System was disabled : {system.Name}");
            }
            else
            {
                Logger.Warn("SMC", "An unregistered system has attemped unregistering.");
            }
        }
    }
}
