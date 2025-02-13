using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;

namespace EarlyGameTweaks.Roles.Scientist
{
    public class Pacifist : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 50;
        public override int MaxHealth { get; set; } = 150;
        public override string Name { get; set; } = "Scientist - Pazifist";
        public override string Description { get; set; } = "Ein simpler Wissenschaftler. Du kannst keine Waffen aufheben.";
        public override string CustomInfo { get; set; } = "Scientist - Pazifist";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;
        public int Chance { get; set; } = 25;
        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new RestrictedItems()
            {
                Name = "Restricted Items [Passive]",
                Description = "Handles restricted items",
                RestrictedItemList =
                {
                    ItemType.GunA7,
                    ItemType.GunAK,
                    ItemType.GunCOM15,
                    ItemType.GunCOM18,
                    ItemType.GunCom45,
                    ItemType.GunCrossvec,
                    ItemType.GunE11SR,
                    ItemType.GunFRMG0,
                    ItemType.GunFSP9,
                    ItemType.GunLogicer,
                    ItemType.GunRevolver,
                    ItemType.GunRevolver,
                    ItemType.GrenadeFlash,
                    ItemType.GrenadeHE,
                    ItemType.MicroHID,
                    ItemType.ParticleDisruptor,
                    ItemType.Jailbird,
                },
                RestrictUsingItems = false,
                RestrictPickingUpItems = true,
                RestrictDroppingItems = false,
            }
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints =
            [
                new()
                {
                    Role = RoleTypeId.Scientist,
                }
            ]
        };
        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Lantern.ToString(),
            ItemType.KeycardScientist.ToString(),
        };
    }
}
