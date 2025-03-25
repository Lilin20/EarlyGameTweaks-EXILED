using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Adrenaline)]
    public class LilinsDrug : CustomItem
    {
        public override uint Id { get; set; } = 102;
        public override string Name { get; set; } = "SCP-395822 - Dermatodissoziations-Injektor";
        public override string Description { get; set; } = "SCP-395822, der Dermatodissoziations-Injektor, ist ein anomales Injektionsgerät, das bei Verabreichung eine sofortige Abtrennung der Hautschicht bewirkt, gefolgt von Unsichtbarkeit und anderen anomalen Effekten.";
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
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
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside049Armory,
                }
            },
        };

        protected override void SubscribeEvents()
        {
            Player.UsingItemCompleted += OnUsingLilinsDrug;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= OnUsingLilinsDrug;

            base.UnsubscribeEvents();
        }

        private void OnUsingLilinsDrug(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            if (ev.Item.Type == ItemType.Adrenaline)
            {
                ev.IsAllowed = false;
                ev.Item.Destroy();
            }

            float random = UnityEngine.Random.value;
            if (random <= 0.25f)
            {
                ev.Player.EnableEffect(EffectType.CardiacArrest, 1, 100f, false);
                ev.Player.EnableEffect(EffectType.AmnesiaItems, 1, 100f, false);
                ev.Player.EnableEffect(EffectType.Bleeding, 1, 100f, false);
                ev.Player.EnableEffect(EffectType.Poisoned, 1, 100f, false);
                ev.Player.EnableEffect(EffectType.Hemorrhage, 1, 100f, false);
                ev.Player.EnableEffect(EffectType.SeveredHands, 1, 100f, false);
            }
            else
            {
                ev.Player.EnableEffect(EffectType.Invisible, 1, 15f, false);
                ev.Player.EnableEffect(EffectType.MovementBoost, 50, 15f, false);
                ev.Player.EnableEffect(EffectType.Flashed, 1, 1f, false);
                ev.Player.EnableEffect(EffectType.SilentWalk, 10, 15f, false);
                ev.Player.Health = 25;
                ev.Player.MaxHealth = 25;
                Ragdoll ragdoll = Ragdoll.CreateAndSpawn(ev.Player.Role, ev.Player.DisplayNickname, "Es sieht aus wie eine leblose Hülle.", ev.Player.Position, ev.Player.ReferenceHub.PlayerCameraReference.rotation);
            }
        }
    }
}
