using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerStatsSystem;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.GunRevolver)]
    public class RussianRoulette : CustomWeapon
    {
        public override uint Id { get; set; } = 302;
        public override float Damage { get; set; } = 0.1f;
        public override string Name { get; set; } = "Russisches Roulette";
        public override string Description { get; set; } = "...";
        public override byte ClipSize { get; set; } = 6;
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
            }
        };

        protected override void SubscribeEvents()
        {
            Player.Shot += OnShotDD;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.Shot -= OnShotDD;
            base.UnsubscribeEvents();
        }

        private void OnShotDD(ShotEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (ev.Target == null)
                return;

            ev.CanHurt = false;

            float random = UnityEngine.Random.value;
            if (ev.Target.Role.Team != PlayerRoles.Team.SCPs)
            {
                if (random <= 0.5f)
                {
                    ev.Target.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Unknown));
                    ev.Player.AddItem(ItemType.Coin);
                }
                else
                {
                    ev.Player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Unknown));
                }
            }
            else if (ev.Target.Role.Team == PlayerRoles.Team.SCPs)
            {
                if (random <= 0.05)
                {
                    ev.Target.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Unknown));
                    ev.Player.AddItem(ItemType.Coin);
                }
                else
                {
                    ev.Player.Hurt(new UniversalDamageHandler(-1f, DeathTranslations.Unknown));
                }
            }
        }
    }
}