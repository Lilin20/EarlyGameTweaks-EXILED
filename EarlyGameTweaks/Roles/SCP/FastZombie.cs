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
        public override uint Id { get; set; } = 14;
        public override int MaxHealth { get; set; } = 500;
        public override string Name { get; set; } = "SCP-049-2 - Fast Zombie";
        public override string Description { get; set; } = "A smaller and faster type of zombie.";
        public override string CustomInfo { get; set; } = "SCP-049-2 - Fast Zombie";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        public int Chance { get; set; } = 50;
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ScaleAbility()
            {
                Name = "Fast Zombie Scale [Passive]",
                Description = "Smaller size",
                ScaleForPlayers = new Vector3(0.75f, 0.75f, 0.75f),
            },
            new EffectEnabler()
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
            Exiled.Events.Handlers.Player.ChangingRole += OnFastZombieSpawn;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnFastZombieSpawn;
            base.UnsubscribeEvents();
        }

        public void OnFastZombieSpawn(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                if (!Check(ev.Player)) return;

                ev.Player.MaxHealth = 500;
                ev.Player.Health = 500;
            });
        }
    }
}
