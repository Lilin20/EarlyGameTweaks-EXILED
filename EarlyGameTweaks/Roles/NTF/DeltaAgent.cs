using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Active;
using EarlyGameTweaks.API;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;

namespace EarlyGameTweaks.Roles.NTF
{
    public class DeltaAgent : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 300;
        public override int MaxHealth { get; set; } = 200;
        public override string Name { get; set; } = "NTF - Zeta 9 Unit";
        public override string Description { get; set; } = "Spezialisiert auf Einsätze in Tunneln und instabilen Strukturen. Deine Aufgabe: SCPs sichern, Gefahren neutralisieren – im Dunkeln.";
        public override string CustomInfo { get; set; } = "NTF - Zeta 9 Unit";
        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfCaptain;
        public override bool DisplayCustomItemMessages { get; set; } = false;
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ZoneBlackout()
            {
                Name = "Zone Blackout [Active]",
                Description = "Schaltet in der Zone in der du dich befindest die Lichter aus."
            }
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints =
            [
                new()
                {
                    Role = RoleTypeId.NtfCaptain,
                }
            ]
        };
        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardMTFCaptain.ToString(),
            ItemType.GunFRMG0.ToString(),
            ItemType.Adrenaline.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.GrenadeHE.ToString(),
            ItemType.ArmorHeavy.ToString(),
            ItemType.Radio.ToString(),
            "Night Hawk Mk. 2",
            ItemType.Ammo556x45.ToString(),
            ItemType.Ammo556x45.ToString(),
            ItemType.Ammo556x45.ToString(),
            ItemType.Ammo556x45.ToString(),
            ItemType.Ammo556x45.ToString(),
            ItemType.Ammo556x45.ToString(),
            ItemType.Ammo556x45.ToString(),
            ItemType.Ammo556x45.ToString(),
        };
    }
}
