using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace EarlyGameTweaks.Roles.SCP
{
    public class ZombieJuggernaut : CustomRole, ICustomRole
    {
        private const uint RoleId = 500;
        private const int DefaultMaxHealth = 1000;
        private const int JuggernautMaxHealth = 2000;
        private const float SpawnDelay = 2f;
        private const int SlownessDuration = 25;
        private static readonly Vector3 Scale = new Vector3(1.15f, 1.15f, 1.15f);

        public override uint Id { get; set; } = RoleId;
        public override int MaxHealth { get; set; } = DefaultMaxHealth;
        public override string Name { get; set; } = "SCP-049-2 - Juggernaut";
        public override string Description { get; set; } = "Eine etwas mehr gepanzerte Art eines Zombies.";
        public override string CustomInfo { get; set; } = "SCP-049-2 - Juggernaut";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        public int Chance { get; set; } = 20;
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new()
        {
            new ScaleAbility
            {
                Name = "Juggernaut Scale [Passive]",
                Description = "Beefier type beat",
                ScaleForPlayers = Scale,
            },
            new EffectEnabler
            {
                Name = "Juggernaut Slowness [Passive]",
                Description = "Slow ahh type of guy",
                EffectsToApply =
                {
                    { EffectType.Slowness, SlownessDuration }
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
            Exiled.Events.Handlers.Player.Dying += HandleDying;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= HandleChangingRole;
            Exiled.Events.Handlers.Player.Dying -= HandleDying;
            base.UnsubscribeEvents();
        }

        private void HandleChangingRole(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(SpawnDelay, () =>
            {
                if (!Check(ev.Player)) return;

                ev.Player.MaxHealth = JuggernautMaxHealth;
                ev.Player.Health = JuggernautMaxHealth;
                ev.Player.EnableEffect(EffectType.Slowness, SlownessDuration);
            });
        }

        private void HandleDying(DyingEventArgs ev)
        {
            if (!Check(ev.Player)) return;

            ev.Player.Vaporize();
        }
    }
}
