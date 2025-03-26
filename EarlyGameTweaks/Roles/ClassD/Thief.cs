using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Active;
using EarlyGameTweaks.API;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;

namespace EarlyGameTweaks.Roles.ClassD
{
    public class Thief : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 203;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "Class-D - Thief";
        public override string Description { get; set; } = "Du kannst mit deiner aktiven Fähigkeit Spieler ausrauben.";
        public override string CustomInfo { get; set; } = "Class-D Personnel";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
        public int Chance { get; set; } = 20;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;

        public override List<CustomAbility> CustomAbilities { get; set; } = new()
        {
            new Pickpocket
            {
                Name = "Pickpocket [Active]",
                Description = "Stelle dich nah an andere Spieler und gucke auf die um den betroffenen Spieler auszurauben."
            }
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new RoleSpawnPoint { Role = RoleTypeId.ClassD }
            }
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Lantern.ToString(),
            ItemType.Coin.ToString()
        };
    }
}
