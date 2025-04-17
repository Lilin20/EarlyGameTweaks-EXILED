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
        public override uint Id { get; set; } = 500;
        public override int MaxHealth { get; set; } = 1000;
        public override string Name { get; set; } = "<color=#FF0000>SCP-049-2 - Juggernaut</color>";
        public override string Description { get; set; } = "Eine etwas mehr gepanzerte Art eines Zombies.";
        public override string CustomInfo { get; set; } = "SCP-049-2 - Juggernaut";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        public int Chance { get; set; } = 25;
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ScaleAbility()
            {
                Name = "Juggernaut Scale [Passive]",
                Description = "Beefier type beat",
                ScaleForPlayers = new Vector3(1.15f, 1.15f, 1.15f),
            },
            new EffectEnabler()
            {
                Name = "Juggernaut Slowness [Passive]",
                Description = "Slow ahh type of guy",
                EffectsToApply =
                {
                    {EffectType.Slowness, 25}
                }
            }
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnJuggerZombieSpawn;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnJuggerZombieSpawn;
            base.UnsubscribeEvents();
        }

        public void OnJuggerZombieSpawn(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                if (!Check(ev.Player)) return;

                ev.Player.MaxHealth = 2000;
                ev.Player.Health = 2000;
                ev.Player.EnableEffect(EffectType.Slowness, 25);
            });
            
        }

        public void OnJuggerDeath(DyingEventArgs ev)
        {
            if (!Check(ev.Player)) return;

            ev.Player.Vaporize();
        }
    }
}
