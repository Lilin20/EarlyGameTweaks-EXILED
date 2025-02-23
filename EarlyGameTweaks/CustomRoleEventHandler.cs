using System.Collections.Generic;
using EarlyGameTweaks.API;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;

namespace EarlyGameTweaks
{
    public class CustomRoleEventHandler
    {
        private readonly EarlyGameTweaks Plugin;
        public CustomRoleEventHandler(EarlyGameTweaks plugin) => Plugin = plugin;

        public void OnRoundStarted()
        {
            List<ICustomRole>.Enumerator dClassRoles = new();
            List<ICustomRole>.Enumerator scientistRoles = new();
            List<ICustomRole>.Enumerator guardRoles = new();
            List<ICustomRole>.Enumerator scpRoles = new();
            Log.Debug($"Found Roles: {Plugin.Roles}");
            foreach (KeyValuePair<StartTeam, List<ICustomRole>> kvp in Plugin.Roles)
            {
                Log.Debug($"Setting enumerator for {kvp.Key} - {kvp.Value.Count}");
                switch (kvp.Key)
                {
                    case StartTeam.ClassD:
                        Log.Debug("Class d funny");
                        dClassRoles = kvp.Value.GetEnumerator();
                        break;
                    case StartTeam.Scientist:
                        scientistRoles = kvp.Value.GetEnumerator();
                        break;
                    case StartTeam.Guard:
                        guardRoles = kvp.Value.GetEnumerator();
                        break;
                    case StartTeam.Scp:
                        scpRoles = kvp.Value.GetEnumerator();
                        break;
                }
            }

            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                Log.Debug($"Trying to give {player.Nickname} a role | {player.Role.Type}");
                CustomRole? role = null;
                switch (player.Role.Type)
                {
                    case RoleTypeId.FacilityGuard:
                        role = CustomRoleMethods.GetCustomRole(ref guardRoles);
                        break;
                    case RoleTypeId.Scientist:
                        role = CustomRoleMethods.GetCustomRole(ref scientistRoles);
                        break;
                    case RoleTypeId.ClassD:
                        role = CustomRoleMethods.GetCustomRole(ref dClassRoles);
                        break;
                    case { } when player.Role.Side == Side.Scp:
                        role = CustomRoleMethods.GetCustomRole(ref scpRoles);
                        break;
                }

                role?.AddRole(player);
            }

            guardRoles.Dispose();
            scientistRoles.Dispose();
            dClassRoles.Dispose();
            scpRoles.Dispose();
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.Players.Count == 0)
            {
                Log.Warn(
                    $"{nameof(OnRespawningTeam)}: The respawn list is empty ?!? -- {ev.NextKnownTeam} / {ev.MaximumRespawnAmount}");

                foreach (Player player in Player.Get(RoleTypeId.Spectator))
                    ev.Players.Add(player);
                ev.MaximumRespawnAmount = ev.Players.Count;
            }

            List<ICustomRole>.Enumerator roles = new();
            switch (ev.NextKnownTeam)
            {
                case (Faction)SpawnableFaction.ChaosWave or (Faction)SpawnableFaction.ChaosMiniWave:
                    {
                        if (Plugin.Roles.TryGetValue(StartTeam.Chaos, out List<ICustomRole> role))
                            roles = role.GetEnumerator();
                        break;
                    }
                case (Faction)SpawnableFaction.NtfWave or (Faction)SpawnableFaction.NtfMiniWave:
                    {
                        if (Plugin.Roles.TryGetValue(StartTeam.Ntf, out List<ICustomRole> pluginRole))
                            roles = pluginRole.GetEnumerator();
                        break;
                    }
            }

            foreach (Player player in ev.Players)
            {
                CustomRole? role = CustomRoleMethods.GetCustomRole(ref roles);

                role?.AddRole(player);
            }

            roles.Dispose();
        }

        public void FinishingRecall(FinishingRecallEventArgs ev)
        {
            Log.Debug("VVUP Custom Roles: FinishingRecall: Selecting random zombie role.");
            if (this.Plugin.Roles.ContainsKey(StartTeam.Scp) && ev.Target != null)
            {
                Log.Debug(string.Format("VVUP Custom Roles: {0}: List count {1}", "FinishingRecall", this.Plugin.Roles[StartTeam.Scp].Count));
                List<ICustomRole>.Enumerator enumerator = this.Plugin.Roles[StartTeam.Scp].GetEnumerator();
                CustomRole customRole = CustomRoleMethods.GetCustomRole(ref enumerator, false, true);
                Log.Debug("VVUP Custom Roles: Got custom role " + ((customRole != null) ? customRole.Name : null));
                if (customRole != null)
                {
                    int count = customRole.TrackedPlayers.Count;
                    Log.Debug(string.Format("VVUP Custom Roles: Active count for role {0} is {1}", customRole.Name, count));
                    if ((long)count < (long)((ulong)customRole.SpawnProperties.Limit))
                    {
                        if (Extensions.GetCustomRoles(ev.Target).Count == 0)
                        {
                            customRole.AddRole(ev.Target);
                        }
                    }
                    else
                    {
                        Log.Debug("VVUP Custom Roles: Role " + customRole.Name + " has reached its spawn limit. Not Spawning");
                    }
                }
                enumerator.Dispose();
            }
        }
    }
}
