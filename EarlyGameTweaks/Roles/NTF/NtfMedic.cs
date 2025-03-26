using System.Collections.Generic;
using EarlyGameTweaks.API;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;

namespace EarlyGameTweaks.Roles.ChaosInsurgency
{
    public class NtfMedic : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 301;
        public override string Name { get; set; } = "NTF - Trauma Team";
        public override string Description { get; set; } = "Speziell ausgerüstet für den medizinischen Support.";
        public override string CustomInfo { get; set; } = "NTF - Trauma Team";
        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfPrivate;
        public override int MaxHealth { get; set; } = 100;
        public override bool DisplayCustomItemMessages { get; set; } = false;

        public int Chance { get; set; } = 50;
        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new() { Role = RoleTypeId.NtfPrivate }
            }
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardMTFPrivate.ToString(),
            ItemType.GunFSP9.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.ArmorLight.ToString(),
            "Trauma Paket",
            "MS9K - MedShot 9000",
            ItemType.Ammo9x19.ToString(),
            ItemType.Ammo9x19.ToString(),
            ItemType.Ammo9x19.ToString(),
            ItemType.Ammo9x19.ToString()
        };
    }
}
