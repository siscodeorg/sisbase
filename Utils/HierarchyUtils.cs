using DSharpPlus.Entities;
using System.Linq;

namespace sisbase.Utils {
    public static class HierarchyUtils {
        public static DiscordRole GetHighestRole(this DiscordMember m) => m.Roles.OrderByDescending(x => x.Position).FirstOrDefault();
        public static bool IsAbove(this DiscordMember m, DiscordRole r) => m.Hierarchy > r.Position;
        public static bool IsAbove(this DiscordRole left, DiscordRole right) => left.Position > right.Position;
    }
}
