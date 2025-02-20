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
using PlayerRoles;

namespace EarlyGameTweaks.Roles.SCP
{
    public class MedicZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 25;
        public override uint Id { get; set; } = 501;
        public override int MaxHealth { get; set; } = 450;
        public override string Name { get; set; } = "<color=#FF0000>SCP-049-2 - Medic</color>";
        public override string Description { get; set; } = "Ein Zombie das eine Chemiekalie freilässt um andere SCPs zu heilen.";
        public override string CustomInfo { get; set; } = "SCP-049-2 - Medic";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;

        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new EffectEnabler()
            {
                Name = "Move Speed Reduction [Passive]",
                Description = "Slows you down",
                EffectsToApply = new Dictionary<EffectType, byte>()
                {
                    {EffectType.Slowness, 30},
                },
            },
            new HealingMist()
            {
                Name = "Healing Mist [Active]",
                Description = "Emits an invisible mist that can heal other SCPs",
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };
    }
}
