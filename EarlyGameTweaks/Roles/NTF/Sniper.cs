using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;

namespace EarlyGameTweaks.Roles.ChaosInsurgency
{
    public class Sniper : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 302;
        public override string Name { get; set; } = "NTF - Scharfschütze";
        public override string Description { get; set; } = "Speziell ausgerüstet für den Fernkampf.";
        public override string CustomInfo { get; set; } = "NTF - Scharfschütze";
        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfCaptain;
        public override int MaxHealth { get; set; } = 100;
        public override bool DisplayCustomItemMessages { get; set; } = false;

        public int Chance { get; set; } = 25;
        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new() { Role = RoleTypeId.NtfCaptain }
            }
        };

        public override List<CustomAbility> CustomAbilities { get; set; } = new()
        {
            new RestrictedItems
            {
                Name = "Restricted Items [Passive]",
                Description = "Handles restricted items",
                RestrictedItemList = new List<ItemType>
                {
                    ItemType.GunAK,
                    ItemType.GunCOM18,
                    ItemType.GunE11SR,
                    ItemType.GunFRMG0,
                    ItemType.GunLogicer,
                    ItemType.MicroHID,
                    ItemType.ParticleDisruptor,
                    ItemType.GunCrossvec,
                    ItemType.GunFSP9,
                },
                RestrictUsingItems = false,
                RestrictPickingUpItems = true,
                RestrictDroppingItems = false,
            },
            new EffectEnabler
            {
                Name = "Adleraugen [Passive]",
                Description = "Du kannst weiter sehen als andere Spieler.",
                EffectsToApply = new Dictionary<EffectType, byte>
                {
                    { EffectType.FogControl, 255 }
                }
            }
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardMTFCaptain.ToString(),
            "RangeTec - .308 Lapua",
            ItemType.GunCOM15.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.ArmorLight.ToString(),
            ItemType.Ammo556x45.ToString(),
            ItemType.Ammo556x45.ToString(),
            ItemType.Ammo9x19.ToString(),
            ItemType.Ammo9x19.ToString(),
        };
    }
}
