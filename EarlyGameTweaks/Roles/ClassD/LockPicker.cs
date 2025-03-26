using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Active;
using EarlyGameTweaks.API;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;

namespace EarlyGameTweaks.Roles.ClassD
{
    public class Lockpicker : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 201;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "Class-D - Lockpicker";
        public override string Description { get; set; } = "Du bist ein Class-D mit der Fähigkeit Türen zu knacken.";
        public override string CustomInfo { get; set; } = "Class-D - Lockpicker";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
        public int Chance { get; set; } = 20;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;

        public override List<CustomAbility> CustomAbilities { get; set; } = new()
        {
            new DoorPicking
            {
                Name = "Lockpicking Ability [Active]",
                Description = "Allows you to open any door for a short period of time, but limited by some external factors",
            }
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new RoleSpawnPoint
                {
                    Role = RoleTypeId.ClassD,
                }
            }
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Lantern.ToString(),
            ItemType.Coin.ToString(),
        };
    }
}
