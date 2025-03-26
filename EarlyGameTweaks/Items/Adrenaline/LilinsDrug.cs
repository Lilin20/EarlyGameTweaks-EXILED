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

        private const float NegativeEffectChance = 0.25f;

        private static readonly Dictionary<EffectType, (int Intensity, float Duration)> NegativeEffects = new()
        {
            { EffectType.CardiacArrest, (1, 100f) },
            { EffectType.AmnesiaItems, (1, 100f) },
            { EffectType.Bleeding, (1, 100f) },
            { EffectType.Poisoned, (1, 100f) },
            { EffectType.Hemorrhage, (1, 100f) },
            { EffectType.SeveredHands, (1, 100f) },
        };

        private static readonly Dictionary<EffectType, (int Intensity, float Duration)> PositiveEffects = new()
        {
            { EffectType.Invisible, (1, 15f) },
            { EffectType.MovementBoost, (50, 15f) },
            { EffectType.Flashed, (1, 1f) },
            { EffectType.SilentWalk, (10, 15f) },
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
            if (!Check(ev.Player.CurrentItem) || ev.Item.Type != ItemType.Adrenaline)
                return;

            ev.IsAllowed = false;
            ev.Item.Destroy();

            if (UnityEngine.Random.value <= NegativeEffectChance)
            {
                ApplyEffects(ev.Player, NegativeEffects);
            }
            else
            {
                ApplyEffects(ev.Player, PositiveEffects);
                ev.Player.Health = 25;
                ev.Player.MaxHealth = 25;
                CreateRagdoll(ev.Player);
            }
        }

        private static void ApplyEffects(Player player, Dictionary<EffectType, (int Intensity, float Duration)> effects)
        {
            foreach (var (effect, (intensity, duration)) in effects)
            {
                player.EnableEffect(effect, intensity, duration, false);
            }
        }

        private static void CreateRagdoll(Player player)
        {
            Ragdoll.CreateAndSpawn(
                player.Role,
                player.DisplayNickname,
                "Es sieht aus wie eine leblose Hülle.",
                player.Position,
                player.ReferenceHub.PlayerCameraReference.rotation
            );
        }
    }
}
