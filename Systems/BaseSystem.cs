using sisbase.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems {
    public abstract class BaseSystem {
        public SisbaseBot SisbaseInstance;
        public string Name;
        public string Description;
        public bool Status = true;
        public abstract Task Activate();
        public abstract Task Deactivate();
        public virtual async Task<bool> CheckPreconditions() { return true; }
        internal SystemConfigData ToConfigData() => new SystemConfigData {
            Name = Name,
            Enabled = true
        };
    }
}
