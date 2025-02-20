using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Active;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace EarlyGameTweaks.Roles.ChaosInsurgency
{
    public class Berserker : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 100;
        public override int MaxHealth { get; set; } = 200;
        public override string Name { get; set; } = "Chaos Insurgency - Berserker";
        public override string Description { get; set; } = "Experte des Nahkampfes mit tendenz zu aggressiven Verhalten.";
        public override string CustomInfo { get; set; } = "Chaos - Berserker";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosConscript;
        public int Chance { get; set; } = 25;
        public override bool DisplayCustomItemMessages { get; set; } = false;
        public override bool KeepRoleOnChangingRole { get; set; } = false;
        public override bool KeepRoleOnDeath { get; set; } = false;
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
            new BerserkerFury
            {
                Name = "Berserker's Fury [Active]",
                Description = "Test",
            },
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
                },
                RestrictUsingItems = false,
                RestrictPickingUpItems = true,
                RestrictDroppingItems = false,
            }
        };
        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardChaosInsurgency.ToString(),
            ItemType.Medkit.ToString(),
            ItemType.ArmorHeavy.ToString(),
            ItemType.GrenadeFlash.ToString(),
            "Berserker's Rod of Destruction",
        };
    }
}
