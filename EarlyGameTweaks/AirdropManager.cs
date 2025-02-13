using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MapEditorReborn.API.Features;
using MEC;
using Respawning.Waves;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace EarlyGameTweaks.Abilities
{
    public class AirdropManager
    {
        static System.Random random = new System.Random();
        public List<ItemType> itemsToDrop = new List<ItemType>
        { 
            ItemType.Medkit,
            ItemType.Adrenaline,
            ItemType.Painkillers,
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
            ItemType.GunShotgun,
            ItemType.Ammo556x45,
            ItemType.Ammo762x39,
            ItemType.Ammo9x19,
            ItemType.Ammo762x39,
            ItemType.Ammo12gauge,
        };
        public enum ItemTypes
        {
            Medkit,
            Adrenaline,
            Painkillers,
            GunA7,
            GunAK,
            GunCOM15,
            GunCOM18,
            GunCom45,
            GunCrossvec,
            GunE11SR,
            GunFRMG0,
            GunFSP9,
            GunLogicer,
            GunRevolver,
            GunShotgun,
            Ammo556x45,
            Ammo762x39,
            Ammo9x19,
            Ammo12gauge
        }
        public static Dictionary<ItemType, int> weightedItems = new Dictionary<ItemType, int>
        {
            { ItemType.Medkit, 15 },
            { ItemType.Adrenaline, 10 },
            { ItemType.Painkillers, 15 },
            { ItemType.GunA7, 1 },
            { ItemType.GunAK, 1 },
            { ItemType.GunCOM15, 1 },
            { ItemType.GunCOM18, 1 },
            { ItemType.GunCom45, 1 },
            { ItemType.GunCrossvec, 1 },
            { ItemType.GunE11SR, 1 },
            { ItemType.GunFRMG0, 1 },
            { ItemType.GunFSP9, 1 },
            { ItemType.GunLogicer, 1 },
            { ItemType.GunRevolver, 1 },
            { ItemType.GunShotgun, 1 },
            { ItemType.Ammo556x45, 15 },
            { ItemType.Ammo762x39, 15 },
            { ItemType.Ammo9x19, 15 },
            { ItemType.Ammo12gauge, 15 }
        };
        public static ItemType GetWeightedRandomItem()
        {
            int totalWeight = weightedItems.Values.Sum();
            int randomNumber = random.Next(0, totalWeight);
            int cumulativeWeight = 0;

            foreach (var item in weightedItems)
            {
                cumulativeWeight += item.Value;
                if (randomNumber < cumulativeWeight)
                    return item.Key;
            }

            return weightedItems.Keys.First(); // Fallback
        }

        public void BuildDrop()
        {
            itemsToDrop.Clear();
            for (int i = 0; i < 20; i++)
            {
                var item = GetWeightedRandomItem();
                Log.Info($"Trying to add {item}");
                itemsToDrop.Add(item);
            }
        }

        public void HumeDrop()
        {
            Light light = Light.Create(Vector3.zero, Vector3.zero, Vector3.one, true, new Color(1f, 0, 0));
            light.Intensity = 50;
            light.Range = 50;
            light.ShadowStrength = 0;
            light.ShadowType = LightShadows.None;

            light.Position = new Vector3(126.5183f, 995.47f, -43.0551f);

            Respawn.PlayEffect(Respawn.TryGetWaveBase(SpawnableFaction.NtfWave, out var wave) ? wave : new NtfSpawnWave());

            Timing.CallDelayed(18f, () =>
            {
                var schematic = ObjectSpawner.SpawnSchematic(schematicName: "HumeCrate", position: new Vector3(126.5183f, 995.46f, -43.0551f), Quaternion.Euler(0, 0, 0), new Vector3(1, 1, 1), null);
                Timing.CallDelayed(40f, () =>
                {
                    schematic.Destroy();
                });
            });

            Timing.CallDelayed(20f, () =>
            {
                light.Destroy();
            });
        }
        public void DropToPlayer(Player player)
        {
            Light light = Light.Create(Vector3.zero, Vector3.zero, Vector3.one, true, new Color(0f, 0.8f, 0));
            light.Intensity = 50;
            light.Range = 50;
            light.ShadowStrength = 0;
            light.ShadowType = LightShadows.None;

            light.Position = new Vector3(126.5183f, 995.4606f, -43.0551f);




            Respawn.PlayEffect(Respawn.TryGetWaveBase(SpawnableFaction.NtfWave, out var wave) ? wave : new NtfSpawnWave());
            
            BuildDrop();
            Timing.CallDelayed(18f, () =>
            {
                foreach (ItemType item in itemsToDrop)
                {
                    var itemObj = Item.Create(item).CreatePickup(ItemOffset(new Vector3(126.5183f, 995.4606f, -43.0551f)));
                }
            });
            Timing.CallDelayed(20f, () =>
            {
                light.Destroy();
            });
        }

        private Vector3 ItemOffset(Vector3 position)
        {
            System.Random random = new System.Random();
            float x = position.x - 1 + ((float)random.NextDouble() * random.Next(0, 5));
            float y = position.y;
            float z = position.z - 1 + ((float)random.NextDouble() * random.Next(0, 5));
            return new Vector3(x, y, z);
        }
    }
}
