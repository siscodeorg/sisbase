using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems.Attributes {
    public class RequiresAttribute : CheckBaseAttribute {
        private List<Type> Systems = new List<Type>();
        private SisbaseBot Instance;
        public RequiresAttribute(params Type[] systems) {
            foreach(var type in systems) {
                if (!typeof(BaseSystem).IsAssignableFrom(type)) throw new ArgumentException($"{type.Name} is not a valid system.", type.Name);
                Systems.Add(type);
            }
        }
        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) {
            Instance = (SisbaseBot) ctx.CommandsNext.Services.GetService(typeof(SisbaseBot));
            foreach(var type in Systems) {
                if (!Instance.SystemManager.Systems.ContainsKey(type)) return false;
                if (Instance.SystemManager.UnloadedSystems.ContainsKey(type)) return false;
            }
            return true;
        }
    }
}
