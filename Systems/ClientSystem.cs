using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Systems {
    public abstract class ClientSystem : BaseSystem {
        public DiscordClient Client;
        public virtual async Task ApplyToClient(DiscordClient client) { }
    }
}
