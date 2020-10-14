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

        public Task<bool> TryUnregisterSystem<T>() where T : BaseSystem => TryUnregisterType(typeof(T));

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
                if (UnloadedSystems.ContainsKey(type)) UnloadedSystems.Remove(type);
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

        internal async Task<bool> TryUnregisterType(Type type) {
            if (!Systems.ContainsKey(type)) return false;
            if (UnloadedSystems.ContainsKey(type)) return true;
            var system = Systems[type];
            if(system is IScheduledSystem) {
                if (RegisteredTimers.ContainsKey(type)) {
                    RegisteredTimers[type].Dispose();
                    RegisteredTimers.Remove(type);
                }
            }
            await system.Deactivate();
            Systems.Remove(type);
            UnloadedSystems.Add(type, system);
            Logger.Log("SMC v3", $"{type.ToCustomName()} was unregistered.");
            return true;
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
            UpdateConfig();
        }

        private bool IsDisabledOnConfig(Type type) {
            if (SisbaseInstance.SystemCfg.Systems.ContainsKey(type.ToCustomName())) {
                return !SisbaseInstance.SystemCfg.Systems[type.ToCustomName()].Enabled;
            } else {
                return false;
            }
        }

        private SystemConfig GenerateConfig(string Path) {
            var config = new SystemConfig {
                Path = Path,
                Systems = SisbaseInstance.SystemCfg.Systems
            };
            var allSystems = Systems.Concat(UnloadedSystems);
            foreach (var sys in allSystems.Distinct()) {
                var data = sys.Value.ToConfigData();
                if (IsDisabledOnConfig(sys.Key)) data.Enabled = false;
                if (!config.Systems.ContainsKey(sys.Key.ToCustomName())) config.Systems.Add(sys.Key.ToCustomName(), data);
                else config.Systems[sys.Key.ToCustomName()] = data;
            }
            return config;
        }

        internal void UpdateConfig() {
            var cfg = GenerateConfig(SisbaseInstance.SystemCfg.Path);
            SisbaseInstance.SystemCfg.Systems = cfg.Systems;
            cfg.Update();
        }
    }
}
