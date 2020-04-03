using DSharpPlus;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

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

		public static ConcurrentDictionary<Type, CancellableTask> RegisteredTimers { get; set; } = new ConcurrentDictionary<Type, CancellableTask>();


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

		internal void Register(Type t)
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
				if (typeof(ISchedule).IsAssignableFrom(t))
				{
					var cts = new CancellationTokenSource();
					RegisteredTimers.TryAdd(t,
						new CancellableTask(GenerateNewTimer(
							((ISchedule)system).Timeout,
							((ISchedule)system).RunContinuous(),
							cts
						), cts));

					RegisteredTimers[t].Task.Start();
					system.Log("Timer Created");
				}
				system.Log("System Loaded");
				RegisteredSystems.AddOrUpdate(t, system, (key, old) => system);
			}
		}

		internal static void Unregister(Type t)
		{
			if (RegisteredSystems.ContainsKey(t))
			{
				ISystem system;
				RegisteredSystems.TryGetValue(t, out system);
				system.Warn("System is disabling...");
				system.Deactivate();
				if (typeof(ISchedule).IsAssignableFrom(t))
				{
					RegisteredTimers[t].Cts.Cancel();
					RegisteredTimers.TryRemove(t, out _);
					Logger.Log("SMC", "A Timer was destroyed");
				}
				RegisteredSystems.TryRemove(t, out system);
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
		internal static void Register(this DiscordClient client, Type t)
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
				system.Execute();
				if (system.Status == true)
				{
					system.ApplyToClient(client);
					system.Log("System applied to client");
					if (typeof(ISchedule).IsAssignableFrom(t))
					{
						var cts = new CancellationTokenSource();
						SMC.RegisteredTimers.TryAdd(t,
							new CancellableTask(SMC.GenerateNewTimer(
								((ISchedule)system).Timeout,
								((ISchedule)system).RunContinuous(),
								cts
							), cts));

						SMC.RegisteredTimers[t].Task.Start();
						Logger.Log(system, "Timer Created");
					}

					SMC.RegisteredSystems.AddOrUpdate(t, system, (key, old) => system);
					system.Log("System Loaded");
				}
				else
				{
					Logger.Warn("SMC", $"A system was unloaded.");
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