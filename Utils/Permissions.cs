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
		public static DPerm GetPermissions(this DiscordMember member) => member.Roles.Select(x => x.Permissions).Aggregate(member.Guild.EveryoneRole.Permissions,(a, b) => a | b);
		public static bool IsModerator(this DiscordMember member) => (member.GetPermissions() & AdministrativePermissions) != DPerm.None;
	}
}
