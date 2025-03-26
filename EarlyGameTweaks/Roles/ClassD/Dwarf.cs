using System.Collections.Generic;
using EarlyGameTweaks.Abilities.Passive;
using EarlyGameTweaks.API;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;

namespace EarlyGameTweaks.Roles.ClassD
{
    public class Dwarf : CustomRole, ICustomRole
    {
        public override uint Id { get; set; } = 200;
        public override int MaxHealth { get; set; } = 50;
        public override string Name { get; set; } = "Class-D - Dwarf";
        public override string Description { get; set; } = "Du bist kleiner - schwerer zu treffen aber dafür weniger HP.";
        public override string CustomInfo { get; set; } = "Class-D - Dwarf";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;

        public override List<CustomAbility> CustomAbilities { get; set; } = new()
        {
            new ScaleAbility
            {
                Name = "Dwarf [Passive]",
                Description = "Macht dich kleiner und schneller aber dafür weniger HP.",
                ScaleForPlayers = new Vector3(0.65f, 0.65f, 0.65f),
            },
            new RestrictedItems
            {
                Name = "Restricted Items [Passive]",
                Description = "Handles restricted items",
                RestrictedItemList = new List<ItemType>
                {
                    ItemType.GunAK,
                    ItemType.GunE11SR,
                    ItemType.GunFRMG0,
                    ItemType.GunLogicer,
                    ItemType.MicroHID,
                    ItemType.ParticleDisruptor,
                },
                RestrictUsingItems = false,
                RestrictPickingUpItems = true,
                RestrictDroppingItems = false,
            }
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
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
