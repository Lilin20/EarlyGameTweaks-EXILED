using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.SCP268)]
    public class SCP714 : GogglesItem
    {
        public override uint Id { get; set; } = 805;
        public override string Name { get; set; } = "SCP-714";
        public override string Description { get; set; } = "Ein normaler Jade Ring.";
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

        private CoroutineHandle staminaDrainCoroutine;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.UnsubscribeEvents();
        }

        private void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            if (PlayerHasGoggles(ev.Player))
            {
                if (ev.DamageHandler.Type == DamageType.Poison ||
                    ev.DamageHandler.Type == DamageType.Scp049 ||
                    ev.DamageHandler.Type == DamageType.Scp939)
                {
                    ev.IsAllowed = false; // Schaden blocken
                }
            }
        }

        protected override void EquipGoggles(Player player, bool showMessage = true)
        {
            base.EquipGoggles(player, showMessage);

            staminaDrainCoroutine = Timing.RunCoroutine(StaminaDrain(player));
            player.EnableEffect(EffectType.Exhausted, 30);
        }

        protected override void RemoveGoggles(Player player, bool showMessage = true)
        {
            base.RemoveGoggles(player, showMessage);

            Timing.KillCoroutines(staminaDrainCoroutine);

            player.DisableEffect(EffectType.Exhausted);
        }

        private IEnumerator<float> StaminaDrain(Player player)
        {
            while (player.IsAlive)
            {
                yield return Timing.WaitForSeconds(0.25f); // alle 0.5 Sekunden drainen
                if (player.Stamina <= 0f)
                {
                    player.Hurt(1);
                }
            }
        }
    }
}