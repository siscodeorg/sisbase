using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPerm = DSharpPlus.Permissions;
namespace sisbase.Utils
{
	public static class Permissions
	{
		public const DPerm AdministrativePermissions = DPerm.KickMembers | DPerm.BanMembers | DPerm.Administrator | DPerm.ManageChannels
				| DPerm.ManageGuild | DPerm.ManageMessages | DPerm.ManageRoles
				| DPerm.ManageWebhooks;
		/// <summary>
		/// Gets all the global permissions of a <see cref="DiscordMember"/> including <seealso cref="DiscordGuild.EveryoneRole"/>
		/// </summary>
		/// <param name="member">The member of the </param>
		/// <returns></returns>
		public static DPerm GetPermissions(this DiscordMember member) => member.Roles.Select(x => x.Permissions).Aggregate(member.Guild.EveryoneRole.Permissions,(a, b) => a | b);
		/// <summary>
		/// Checks if a <see cref="DiscordMember"/> has any permission from <see cref="AdministrativePermissions"/>
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public static bool IsModerator(this DiscordMember member) => (member.GetPermissions() & AdministrativePermissions) != DPerm.None;
	}
}
