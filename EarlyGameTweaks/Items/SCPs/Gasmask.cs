using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.SCP268)]
    public class Gasmask : GogglesItem
    {
        public override uint Id { get; set; } = 803;
        public override string Name { get; set; } = "SCP-1499";
        public override string Description { get; set; } = "yup";
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            LockerSpawnPoints = new List<LockerSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Zone = ZoneType.LightContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 25,
                    Zone = ZoneType.HeavyContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 25,
                    Zone = ZoneType.Entrance,
                    UseChamber = false,
                    Type = LockerType.Misc,
                }
            },
        };
        private CoroutineHandle damageCoroutine;

        // Variable für die Speicherung der ursprünglichen Position
        private Vector3? originalPosition = null;

        private float currentDamage = 0f;
        private float maxDamage = 10f;
        private float damageIncrement = 0.2f;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void EquipGoggles(Player player, bool showMessage = true)
        {
            base.EquipGoggles(player, showMessage);

            if (originalPosition == null)
            {
                originalPosition = player.Position;

                Room pocketRoom = Room.Get(RoomType.Pocket);

                Vector3 roomPosition = pocketRoom.Position;
                roomPosition.y = -1991.400f;

                player.Teleport(roomPosition);
                damageCoroutine = Timing.RunCoroutine(ApplyDamage(player));
            }
        }

        protected override void RemoveGoggles(Player player, bool showMessage = true)
        {
            base.RemoveGoggles(player, showMessage);

            if (originalPosition != null)
            {
                player.Teleport(originalPosition.Value);

                if (damageCoroutine.IsRunning)
                {
                    Timing.KillCoroutines(damageCoroutine);
                }

                currentDamage = 0f;

                originalPosition = null;
            }
        }

        private IEnumerator<float> ApplyDamage(Player player)
        {
            while (true)
            {
                player.Health -= currentDamage;

                currentDamage = Mathf.Min(currentDamage + damageIncrement, maxDamage);

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}