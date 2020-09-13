using DSharpPlus.Entities;
using System.Linq;

namespace sisbase.Utils {
    /// <summary>
    /// Provide functions for comparing role higherachies, for permission validation and similar issues based on the position
    /// of the role instead of the <see cref="Permissions"/> provided by them.
    /// </summary>
    public static class HierarchyUtils {
        /// <summary>
        /// Get the <paramref name="member"/>'s highest role
        /// </summary>
        /// <param name="member">The member</param>
        /// <returns>The <see cref="DiscordRole"/> if found, otherwise <see cref="DiscordGuild.EveryoneRole"/>.</returns>
        public static DiscordRole GetHighestRole(this DiscordMember member) => member.Roles.OrderByDescending(x => x.Position).FirstOrDefault();
        /// <summary>
        /// Check if <paramref name="member"/>'s highest role is higher (and not equal) then <paramref name="role"/>.<br></br>
        /// Useful for knowing if <paramref name="role"/> can be granted/revoked by <paramref name="member"/>.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="role">The role.</param>
        /// <returns> true if <paramref name="member"/> is above <paramref name="role"/>, otherwise false.</returns>
        public static bool IsAbove(this DiscordMember member, DiscordRole role) => member.Hierarchy > role.Position;
        /// <summary>
        /// Check if <paramref name="role"/>'s position is higher (and not equal) then <paramref name="other"/>'s
        /// </summary>
        /// <param name="role">The role to be checked.</param>
        /// <param name="other">The role it will be checked against.</param>
        /// <returns> true if the <paramref name="role"/> is above <paramref name="other"/>, otherwise false</returns>
        public static bool IsAbove(this DiscordRole role, DiscordRole other) => role.Position > other.Position;
    }
}
