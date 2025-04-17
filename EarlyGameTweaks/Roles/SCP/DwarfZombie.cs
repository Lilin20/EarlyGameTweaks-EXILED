using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarlyGameTweaks.Abilities.Active;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;

namespace EarlyGameTweaks.Roles.SCP
{
    public class DwarfZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 25;
        public override uint Id { get; set; } = 502;
        public override int MaxHealth { get; set; } = 250;
        public override string Name { get; set; } = "<color=#FF0000>SCP-049-2 - Dwarf</color>";
        public override string Description { get; set; } = "Ein Zombie das eine Chemiekalie freilässt um andere SCPs zu heilen.";
        public override string CustomInfo { get; set; } = "SCP-049-2 - Dwarf";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;

        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ScaleAbility()
            {
                Name = "Midget [Passive]",
                Description = "You are smaller but faster.",
                ScaleForPlayers = new UnityEngine.Vector3(0.6f, 0.6f, 0.6f),
            },

            new EffectEnabler()
            {
                Name = "Speedboost [Passive]",
                Description = "You walk faster.",
                EffectsToApply =
                {
                    {EffectType.MovementBoost, 25}
                }
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnDwarfZombieSpawn;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnDwarfZombieSpawn;
            base.UnsubscribeEvents();
        }

        public void OnDwarfZombieSpawn(ChangingRoleEventArgs ev)
        {
            Timing.CallDelayed(2f, () =>
            {
                if (!Check(ev.Player)) return;

                ev.Player.MaxHealth = 250;
                ev.Player.Health = 250;
            });

        }
    }
}
