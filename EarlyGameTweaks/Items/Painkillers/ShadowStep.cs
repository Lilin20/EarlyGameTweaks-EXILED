using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Adrenaline)]
    public class ShadowStep : CustomItem
    {
        public override uint Id { get; set; } = 105;
        public override string Name { get; set; } = "Shadow Step";
        public override string Description { get; set; } = "Gibt dir die Möglichkeit die Umgebung zu erkunden ohne erkannt zu werden.";
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
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
            }
        };

        protected override void SubscribeEvents()
        {
            Player.UsingItemCompleted += OnUsingInjection;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= OnUsingInjection;

            base.UnsubscribeEvents();
        }

        private void OnUsingInjection(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Vector3 oldPos = ev.Player.Position;
            ev.Player.EnableEffect(EffectType.Ghostly);
            ev.Player.EnableEffect(EffectType.SilentWalk, 100);
            ev.Player.EnableEffect(EffectType.FogControl, 0);
            ev.Player.EnableEffect(EffectType.Invisible);
            ev.Player.Handcuff();

            Timing.CallDelayed(15f, () =>
            {
                ev.Player.Teleport(oldPos);
                ev.Player.DisableAllEffects();
                ev.Player.RemoveHandcuffs();
            });
        }
    }
}
