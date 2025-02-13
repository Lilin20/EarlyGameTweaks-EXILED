using System.Collections.Generic;
using EarlyGameTweaks.Abilities;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;
using Light = Exiled.API.Features.Toys.Light;
using MapEditorReborn;
using MapEditorReborn.Events.Handlers;
using MapEditorReborn.API.Features.Serializable;
using MapEditorReborn.API.Features;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Medkit)]
    public class CustomMedkit : CustomItem
    {
        public override uint Id { get; set; } = 30;
        public override string Name { get; set; } = "Medkit Mk. 2";
        public override string Description { get; set; } = "Dieses Medkit sendet Nanobots aus. Heilt alle Spieler in einem 5 Meter Radius.";
        public override float Weight { get; set; } = 0.5f;
        public AirdropManager am = new AirdropManager();
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 4,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 30,
                    Location = SpawnLocationType.InsideLczWc,
                },
                new()
                {
                    Chance = 30,
                    Location = SpawnLocationType.InsideLczArmory,
                },
                new()
                {
                    Chance = 30,
                    Location = SpawnLocationType.InsideHczArmory,
                },
            },
        };

        protected override void SubscribeEvents()
        {
            Player.UsingItemCompleted += OnUsingItem;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= OnUsingItem;

            base.UnsubscribeEvents();
        }

        private void OnUsingItem(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            Log.Info("Trying to use Medkit Mk. 2...");

            float maxDistance = 10f;

            List<Exiled.API.Features.Player> nearbyPlayers = new List<Exiled.API.Features.Player>();

            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                if (player == ev.Player)
                    continue;

                float distance = Vector3.Distance(ev.Player.Position, player.Position);
                if (distance <= maxDistance)
                {
                    nearbyPlayers.Add(player);
                    Log.Info($"Adding {player}...");
                }
            }

            foreach (Exiled.API.Features.Player player in nearbyPlayers)
            {
                player.Heal(65f);
                Log.Info("Healing player...");
            }

            //Log.Info(am.itemsToDrop);
            //am.HumeDrop();

            
        }
    }
}