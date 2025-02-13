using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp106;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.SCP1853)]
    public class PDEscape : CustomItem
    {
        public override uint Id { get; set; } = 1111;
        public override string Name { get; set; } = "Ass im Ärmel";
        public override string Description { get; set; } = "Du wusstest schon seit längerem das dir dies irgendwann mal sehr viel Glück bringen wird.";
        public override float Weight { get; set; } = 0.5f;

        public Vector3 oldPlayerPos { get; set; }

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 4,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside914,
                }
            },
            LockerSpawnPoints = new List<LockerSpawnPoint>
            {
                new()
                {
                    Chance = 35,
                    Zone = ZoneType.LightContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 35,
                    Zone = ZoneType.HeavyContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 35,
                    Zone = ZoneType.Entrance,
                    UseChamber = false,
                    Type = LockerType.Misc,
                }
            },
        };

        protected override void SubscribeEvents()
        {
            Player.UsingItemCompleted += OnUsingPDEscape;
            Exiled.Events.Handlers.Scp106.Attacking += OnAttacking;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= OnUsingPDEscape;
            Exiled.Events.Handlers.Scp106.Attacking -= OnAttacking;

            base.UnsubscribeEvents();
        }

        private void OnAttacking(AttackingEventArgs ev)
        {
            oldPlayerPos = ev.Player.Position;
        }

        private void OnUsingPDEscape(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Room room = Room.Get(RoomType.Pocket);

            if (room.Players.Contains(ev.Player))
            {
                ev.Player.Teleport(oldPlayerPos + Vector3.up);
                ev.Player.DropItems();
            }
        }
    }
}
