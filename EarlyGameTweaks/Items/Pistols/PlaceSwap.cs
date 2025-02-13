using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GunCOM15)]
    public class PlaceSwap : CustomWeapon
    {
        public override uint Id { get; set; } = 1233;
        public override float Damage { get; set; } = 1;
        public override string Name { get; set; } = "Entity Swapper";
        public override string Description { get; set; } = "Tausche die Position mit deinem Ziel.";
        public override byte ClipSize { get; set; } = 5;
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 35,
                    Location = SpawnLocationType.InsideGr18,
                },
                new()
                {
                    Chance = 40,
                    Location = SpawnLocationType.Inside173Gate,
                },
            },
        };

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            ev.IsAllowed = false;
        }
        protected override void OnShot(ShotEventArgs ev)
        {
            ev.CanHurt = false;

            if (!Check(ev.Player.CurrentItem))
                return;

            if (ev.Target == null)
                return;

            if (ev.Target == ev.Player)
                return;

            Vector3 targetPosition = ev.Target.Position;
            Vector3 shooterPosition = ev.Player.Position;

            Timing.CallDelayed(0.25f, () =>
            {
                ev.Target.Position = shooterPosition;
                ev.Player.Position = targetPosition;
            });
            
        }
    }
}