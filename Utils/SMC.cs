using DSharpPlus;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace sisbase.Utils
{
	/// <summary>
	/// The System Management Controller
	/// Now with 100% less PPBUS_G3H
	/// </summary>
	public class SMC
	{
		/// <summary>
		/// All of the current registerred systems on the SMC
		/// </summary>
		public static ConcurrentDictionary<Type, ISystem> RegisteredSystems { get; set; } = new ConcurrentDictionary<Type, ISystem>();

		/// <summary>
		/// All of the current registered timers on the SMC
		/// </summary>
		public static ConcurrentDictionary<Type, Timer> RegisteredTimers { get; set; } = new ConcurrentDictionary<Type, Timer>();

		internal static List<Assembly> RegisteredAssemblies { get; set; } = new List<Assembly>();

		internal Dictionary<Type, bool> RegisterSystems(Assembly assembly)
		{
			var response = new Dictionary<Type, bool>();
			if (!RegisteredAssemblies.Contains(assembly)) RegisteredAssemblies.Add(assembly);
			var Ts = assembly.ExportedTypes.Where(T => T.GetTypeInfo().IsSystemCandidate());
			foreach (var T in Ts)
			{
				if (RegisteredSystems.ContainsKey(T)) continue;
				if (T.GetInterfaces().Contains(typeof(IClientSystem)))
				{
					response.Add(T, SisbaseBot.Instance.Client.Register(T));
				}
				else
				{
					response.Add(T, Register(T));
				}
			}
			return response;
		}

		internal Dictionary<Assembly, Dictionary<string, bool>> Reload()
		{
			var data = new Dictionary<Assembly, Dictionary<string, bool>>();
			foreach (var asm in RegisteredAssemblies)
			{
				var systems = RegisterSystems(asm);
				var nDict = systems.Select(x =>
				new KeyValuePair<string, bool>(
					x.Key.Name,
					x.Value)).ToDictionary(x => x.Key, x => x.Value);

				data.Add(asm, nDict);
			}
			return data;
		}

		internal bool Register(Type t)
		{
			if (RegisteredSystems.ContainsKey(t))
			{
				RegisteredSystems.TryGetValue(t, out var system);
				system.Warn("This system is already registered");
				return true;
			}
			else
			{
				var system = (ISystem)Activator.CreateInstance(t);

				system.Activate();
				system.Log("System Started");
				system.Execute();
				if (system.Status == true)
				{
					if (typeof(ISchedule).IsAssignableFrom(t))
					{
						RegisteredTimers.TryAdd(t, CreateNewTimer(
							((ISchedule)system).Timeout,
							((ISchedule)system).RunContinuous
							));
						system.Log("Timer started");
					}
					RegisteredSystems.AddOrUpdate(t, system, (key, old) => system);
					system.Log("System Loaded");
					return true;
				}
				else
				{
					Logger.Warn("SMC", $"A system was unloaded.");
					return false;
				}
			}
		}

		internal static bool Unregister(Type t)
		{
			if (RegisteredSystems.ContainsKey(t))
			{
				ISystem system;
				RegisteredSystems.TryGetValue(t, out system);
				system.Warn("System is disabling...");
				if (typeof(ISchedule).IsAssignableFrom(t))
				{
					RegisteredTimers[t].Dispose();
					RegisteredTimers.TryRemove(t, out _);
					system.Warn("Timer stopped");
				}
				system.Deactivate();
				RegisteredSystems.TryRemove(t, out system);
				Logger.Log("SMC", $"A System was disabled : {system.Name}");
				return true;
			}
			else
			{
				Logger.Warn("SMC", "An unregistered system has attemped unregistering.");
				return false;
			}
		}

		internal static Timer CreateNewTimer(TimeSpan timeout, Action action) =>
			new Timer(new TimerCallback(x => action()), null, TimeSpan.FromSeconds(1), timeout);
	}

	/// <summary>
	/// Provides extension methods for the <see cref="SMC"/>
	/// </summary>
	public static class SMCExtensions
	{
		internal static bool Register(this DiscordClient client, Type t)
		{
			if (SMC.RegisteredSystems.ContainsKey(t))
			{
				SMC.RegisteredSystems.TryGetValue(t, out var system);
				system.Warn("This system is already registered");
				return true;
			}
			else
			{
				var system = (IClientSystem)Activator.CreateInstance(t);

				system.Activate();
				system.Log("System Started");
				system.Execute();
				if (system.Status == true)
				{
					system.ApplyToClient(client);
					system.Log("System applied to client");
					if (typeof(ISchedule).IsAssignableFrom(t))
					{
						SMC.RegisteredTimers.TryAdd(t, SMC.CreateNewTimer(
							((ISchedule)system).Timeout,
							((ISchedule)system).RunContinuous
							));
						system.Log("Timer started");
					}
					SMC.RegisteredSystems.AddOrUpdate(t, system, (key, old) => system);
					system.Log("System Loaded");
					return true;
				}
				else
				{
					Logger.Warn("SMC", $"A system was unloaded.");
					return false;
				}
			}
		}

		internal static bool IsSystemCandidate(this TypeInfo ti)
		{
			// check if compiler-generated
			if (ti.GetCustomAttribute<CompilerGeneratedAttribute>(false) != null)
				return false;

			// check if derives from the required base class
			var tmodule = typeof(ISystem);
			var timodule = tmodule.GetTypeInfo();
			if (!timodule.IsAssignableFrom(ti))
				return false;

			// check if anonymous
			if (ti.IsGenericType && ti.Name.Contains("AnonymousType") && (ti.Name.StartsWith("<>") || ti.Name.StartsWith("VB$")) && (ti.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic)
				return false;

			// check if abstract, static, or not a class
			if (!ti.IsClass || ti.IsAbstract)
				return false;

			// check if delegate type
			var tdelegate = typeof(Delegate).GetTypeInfo();
			if (tdelegate.IsAssignableFrom(ti))
				return false;

			// qualifies if any method or type qualifies
			return true;
		}
	}
}