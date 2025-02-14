using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Active;
using EarlyGameTweaks.API;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;

namespace EarlyGameTweaks.Roles.ChaosInsurgency
{
    public class Breacher : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 101;
        public override int MaxHealth { get; set; } = 200;
        public override string Name { get; set; } = "Chaos Insurgency - Breacher";
        public override string Description { get; set; } = "Ein Breacher mit der berüchtigten Breach Shotgun namens Kerberos-12";
        public override string CustomInfo { get; set; } = "Chaos - Breacher";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosConscript;
        public int Chance { get; set; } = 30;
        public override bool DisplayCustomItemMessages { get; set; } = false;
        public StartTeam StartTeam { get; set; } = StartTeam.Chaos;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints =
            [
                new()
                {
                    Role = RoleTypeId.ChaosConscript,
                }
            ]
        };
        public override List<CustomAbility> CustomAbilities { get; set; } = new()
        {
            new ChargeAbility
            {
                Name = "Charge [Active]",
                Description = "Test",
            }
        };
        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardChaosInsurgency.ToString(),
            ItemType.GunCrossvec.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.ArmorHeavy.ToString(),
            ItemType.GrenadeFlash.ToString(),
            "Kerberos-12",
            "Cluster Granate",
            ItemType.Ammo9x19.ToString(),
            ItemType.Ammo9x19.ToString(),
            ItemType.Ammo9x19.ToString(),
            ItemType.Ammo9x19.ToString(),
        };


    }
}
