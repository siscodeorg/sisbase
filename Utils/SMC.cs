using DSharpPlus;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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
				system.Log("System Started");
				system.Execute();
				system.Log("System Loaded");
				RegisteredSystems.AddOrUpdate(typeof(T), system, (key, old) => system);
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

		internal void RegisterSystems(Assembly assembly)
		{
			var Ts = assembly.ExportedTypes.Where(T => T.GetTypeInfo().IsSystemCandidate());
			foreach (var T in Ts)
			{
				if (T.GetInterfaces().Contains(typeof(IClientSystem)))
				{
					SisbaseBot.Instance.Client.Register(T);
				}
				else
				{
					Register(T);
				}
			}
		}

		private void Register(Type t)
		{
			if (RegisteredSystems.ContainsKey(t))
			{
				RegisteredSystems.TryGetValue(t, out var system);
				system.Warn("This system is already registered");
			}
			else
			{
				var system = (ISystem)Activator.CreateInstance(t);

				system.Activate();
				system.Log("System Started");
				system.Execute();
				system.Log("System Loaded");
				RegisteredSystems.AddOrUpdate(t, system, (key, old) => system);
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
				system.Log("System Started");
				system.Execute();
				system.ApplyToClient(client);
				system.Log("System applied to client");
				system.Log("System Loaded");
				SMC.RegisteredSystems.AddOrUpdate(typeof(T), system, (key, old) => system);
			}
		}

		public static void Register(this DiscordClient client, Type t)
		{
			if (SMC.RegisteredSystems.ContainsKey(t))
			{
				SMC.RegisteredSystems.TryGetValue(t, out var system);
				system.Warn("This system is already registered");
			}
			else
			{
				var system = (IClientSystem)Activator.CreateInstance(t);

				system.Activate();
				system.Log("System Started");
				system.ApplyToClient(client);
				system.Log("System applied to client");
				system.Execute();

				SMC.RegisteredSystems.AddOrUpdate(t, system, (key, old) => system);
				system.Log("System Loaded");
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