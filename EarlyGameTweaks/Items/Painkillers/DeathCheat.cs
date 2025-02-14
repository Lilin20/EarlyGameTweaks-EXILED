using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using Player_H = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Painkillers)]
    public class DeathCheat : CustomItem
    {
        public override uint Id { get; set; } = 104;
        public override string Name { get; set; } = "Death Cheat";
        public override string Description { get; set; } = "Belebt dich nach einem zufälligen Interval wieder nachdem du gestorben bist. Keine Garantie.";
        public override float Weight { get; set; } = 0.5f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            LockerSpawnPoints = new List<LockerSpawnPoint>
            {
                new()
                {
                    Chance = 45,
                    Zone = ZoneType.LightContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 45,
                    Zone = ZoneType.HeavyContainment,
                    UseChamber = false,
                    Type = LockerType.Misc,
                },
                new()
                {
                    Chance = 45,
                    Zone = ZoneType.Entrance,
                    UseChamber = false,
                    Type = LockerType.Misc,
                }
            }
        };
        public Player oldPlayer = null;

        protected override void SubscribeEvents()
        {
            Player_H.UsingItemCompleted += OnUsingDeathCheat;
            Player_H.Dying += OnPlayerDeath;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player_H.UsingItemCompleted -= OnUsingDeathCheat;
            Player_H.Dying -= OnPlayerDeath;

            base.UnsubscribeEvents();
        }

        private void OnUsingDeathCheat(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            oldPlayer = ev.Player;
        }

        private void OnPlayerDeath(DyingEventArgs ev)
        {
            float random = UnityEngine.Random.value;
            if (random <= 0.45f)
            {
                if (oldPlayer != null)
                {
                    oldPlayer = ev.Player;
                    Role oldRole = oldPlayer.Role;

                    System.Random randomTime = new System.Random();
                    float randomValue = randomTime.Next(30, 121);

                    Timing.CallDelayed(randomValue, () =>
                    {
                        if (oldPlayer.Role is Exiled.API.Features.Roles.SpectatorRole)
                        {
                            oldPlayer.Role.Set(oldRole, RoleSpawnFlags.None);
                        }
                    });
                }
                oldPlayer = null;
            }
        }
    }
}
