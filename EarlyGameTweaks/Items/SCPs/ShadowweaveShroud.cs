using System.Collections.Generic;
using System.Linq;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerHandler = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.SCP268)]
    public class ShadowweaveShroud : CustomItem
    {
        public override uint Id { get; set; } = 802;
        public override string Name { get; set; } = "Shadowweave Shroud";
        public override string Description { get; set; } = "Tarnt dich. Bezieht seine Energie aus nahen Quellen. Du bist für max. 1 Minute unsichtbar.";
        public override float Weight { get; set; } = 0.1f;
        private CoroutineHandle _coroutine;
        public bool shroudEnd = false;
        public bool shroudStart = false;
        public int shroudTime = 0;
        public int roomTime = 0;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.Inside173Armory,
                }
            },
        };

        protected override void SubscribeEvents()
        {
            PlayerHandler.UsedItem += OnShroudUse;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            PlayerHandler.UsedItem -= OnShroudUse;

            base.UnsubscribeEvents();
        }

        private void OnShroudUse(UsedItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            shroudEnd = false;
            shroudStart = true;
            shroudTime = 0;
            roomTime = 0;

            ev.Player.ReferenceHub.playerEffectsController.DisableEffect<Invisible>();

            Timing.CallDelayed(0.5f, () =>
            {
                ev.Player.EnableEffect(EffectType.Invisible, 45f, true);
                ev.Player.EnableEffect(EffectType.Slowness, 15, 45f, false);
            });

            Timing.CallDelayed(1f, () =>
            {
                _coroutine = Timing.RunCoroutine(ShadowweaveCoroutine(ev.Player));
            });
        }

        public IEnumerator<float> ShadowweaveCoroutine(Player player)
        {
            Log.Info("Coroutine start...");

            List<Room> usedRooms = new List<Room>();

            while (shroudEnd == false)
            {
                yield return Timing.WaitForSeconds(0.5f);

                // Check if Invisible is active. If no, kill all effects and coroutine.

                Room room = player.CurrentRoom;

                if (!player.ActiveEffects.ToList().Contains(player.GetEffect(EffectType.Invisible)))
                {
                    player.DisableAllEffects();
                    Timing.KillCoroutines(_coroutine);
                }

                if (!usedRooms.Contains(room))
                {
                    room.TurnOffLights(0.5f);
                    usedRooms.Add(room);
                }

                if (roomTime >= 20)
                {
                    if (usedRooms != null && usedRooms.Count > 0)
                    {
                        usedRooms.RemoveAt(0);
                        roomTime = 0;
                    }
                }

                roomTime++;
                shroudTime++;

                if (shroudTime >= 90)
                {
                    Log.Info("Coroutine stop...");
                    shroudEnd = true;
                    shroudStart = false;
                    Timing.KillCoroutines(_coroutine);
                }
            }
        }
    }
}
