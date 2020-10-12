using sisbase.Attributes;
using sisbase.Configuration;
using sisbase.Utils;
using sisbase.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sisbase.Systems {
    public class SystemManager {
        public SisbaseBot SisbaseInstance { get; set; }
        public Dictionary<Type, BaseSystem> Systems { get; } = new Dictionary<Type, BaseSystem>();
        public Dictionary<Type, BaseSystem> UnloadedSystems { get; } = new Dictionary<Type, BaseSystem>();
        public Dictionary<Type, Timer> RegisteredTimers { get; } = new Dictionary<Type, Timer>();
        public SystemManager(SisbaseBot sisbaseInstance) => SisbaseInstance = sisbaseInstance;

        public Task<bool> TryRegisterSystem<T>() where T : BaseSystem => TryRegisterType(typeof(T));

        private async Task<bool> TryRegisterType(Type type) {
            var System = (BaseSystem)Activator.CreateInstance(type);
            System.SisbaseInstance = SisbaseInstance;
            if (System is ClientSystem clientSystem) {
                clientSystem.Client = SisbaseInstance.Client;
            }

            if (!await System.CheckPreconditions()) {
                await System.Deactivate();
                UnloadedSystems.Add(type, System);
                Logger.Warn("SMC v3", $"{type.ToCustomName()} - Preconditions Failed. System was disabled.");
                return false;
            }
            else {
                await System.Activate();
                if (System is ClientSystem _clientSystem){
                    await _clientSystem.ApplyToClient(SisbaseInstance.Client);
                }
                if(System is IScheduledSystem scheduledSystem) {
                    RegisteredTimers.TryAdd(type, GenerateTimer(scheduledSystem));
                }
                Systems.Add(type, System);
                Logger.Warn("SMC v3", $"{System.Name} - System Loaded.");
                return true;
            }
        }

        internal Timer GenerateTimer(IScheduledSystem system) =>
            new Timer(new TimerCallback(x => system.RunContinuous()), null, TimeSpan.FromSeconds(1), system.Timeout);

        public T GetOrDefault<T>() where T : BaseSystem => (T) Systems.GetValueOrDefault(typeof(T));

        public async Task RegisterAssembly(Assembly assembly) {
            var systemTypes = assembly.GetTypes().Where(x => x.GetTypeInfo().IsCandidate()).ToList();
            foreach(var type in systemTypes) {
                if (Systems.TryGetValue(type, out var _)) continue;
                if (UnloadedSystems.TryGetValue(type, out var _)) continue;
                if (IsDisabledOnConfig(type)) {
                    Logger.Warn("SMC v3", $"System [{type.ToCustomName()}] was disabled on systems.json");
                    continue;
                }
                await TryRegisterType(type);
            }
        }

        private bool IsDisabledOnConfig(Type type) {
            if (SisbaseInstance.SystemCfg.Systems.ContainsKey(type.ToCustomName())) {
                return !SisbaseInstance.SystemCfg.Systems[type.ToCustomName()].Enabled;
            } else {
                return false;
            }
        }
    }
}
