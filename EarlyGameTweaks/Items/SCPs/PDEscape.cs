using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp106;
using MEC;
using PlayerRoles;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.SCP1853)]
    public class PDEscape : CustomItem
    {
        public override uint Id { get; set; } = 800;
        public override string Name { get; set; } = "Ass im Ärmel";
        public override string Description { get; set; } = "Du wusstest schon seit längerem das dir dies irgendwann mal sehr viel Glück bringen wird.";
        public override float Weight { get; set; } = 0.5f;

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

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= OnUsingPDEscape;

            base.UnsubscribeEvents();
        }

        private void OnUsingPDEscape(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            ev.IsAllowed = false;

            Room room = Room.Get(RoomType.Pocket);

            if (room.Players.Contains(ev.Player))
            {
                ev.Player.EnableEffect(EffectType.Invisible, 15f, false);
                ev.Player.EnableEffect(EffectType.Poisoned, 1, 10f, false);
                ev.Player.EnableEffect(EffectType.SilentWalk, 255, 15f, false);

                foreach (Exiled.API.Features.Player larry in Exiled.API.Features.Player.List)
                {
                    if (larry.Role == RoleTypeId.Scp106)
                    {
                        ev.Player.Teleport(larry);
                    }
                }

                for (int i = 0; i <= 15; i++)
                {
                    Timing.CallDelayed(1f, () =>
                    {
                        ev.Player.PlaceBlood(ev.Player.Position + Vector3.down);
                    });
                }
            }

            ev.Item.Destroy();
        }
    }
}
