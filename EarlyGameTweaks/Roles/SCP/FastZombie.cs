using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace EarlyGameTweaks.Roles.SCP
{
    public class FastZombie : CustomRole, ICustomRole
    {
        private const uint RoleId = 14;
        private const int Health = 500;
        private const string RoleName = "SCP-049-2 - Fast Zombie";
        private const string RoleDescription = "A smaller and faster type of zombie.";
        private const string RoleCustomInfo = "SCP-049-2 - Fast Zombie";
        private static readonly Vector3 Scale = new Vector3(0.75f, 0.75f, 0.75f);

        public override uint Id { get; set; } = RoleId;
        public override int MaxHealth { get; set; } = Health;
        public override string Name { get; set; } = RoleName;
        public override string Description { get; set; } = RoleDescription;
        public override string CustomInfo { get; set; } = RoleCustomInfo;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        public int Chance { get; set; } = 50;
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ScaleAbility
            {
                Name = "Fast Zombie Scale [Passive]",
                Description = "Smaller size",
                ScaleForPlayers = Scale,
            },
            new EffectEnabler
            {
                Name = "Fast Zombie Speed [Passive]",
                Description = "Increased movement speed",
                EffectsToApply =
                {
                    {EffectType.MovementBoost, 1}
                }
            }
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole += HandleChangingRole;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= HandleChangingRole;
            base.UnsubscribeEvents();
        }

        private void HandleChangingRole(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                if (!Check(ev.Player)) return;

                SetPlayerHealth(ev.Player);
            });
        }

        private static void SetPlayerHealth(Exiled.API.Features.Player player)
        {
            player.MaxHealth = Health;
            player.Health = Health;
        }
    }
}
