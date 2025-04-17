using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Permissions.Features;
using MapEditorReborn.Commands.ModifyingCommands.Rotation;
using MEC;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.Painkillers)]
    public class AfterImage : CustomItem
    {
        public override uint Id { get; set; } = 1738;
        public override string Name { get; set; } = "Mimic Tabletten";
        public override string Description { get; set; } = "Erzeugt Trugbilder von dir in einem Kreis.";
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


        public bool DidPlayerEscape = false;
        public Exiled.API.Features.Player recentUser = null;

        protected override void SubscribeEvents()
        {
            Player.UsingItemCompleted += OnUsingInjection;
            //Player.Hurt += OnHittingDummy;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= OnUsingInjection;
            //Player.Hurt -= OnHittingDummy;

            base.UnsubscribeEvents();
        }

        private void OnHittingDummy(HurtEventArgs ev)
        {
            if (ev.Player is Npc npc)
            {
                ev.Attacker.EnableEffect(EffectType.Blinded, 255, 5f);
                ev.Attacker.EnableEffect(EffectType.Deafened, 255, 5f, false);
                ev.Attacker.EnableEffect(EffectType.Ensnared, 5f, false);

                DidPlayerEscape = true;

                Log.Info("Enemy hit clone. Giving boosted benefits.");
                recentUser.EnableEffect(EffectType.MovementBoost, 50, 10f);
                recentUser.EnableEffect(EffectType.Ghostly, 10f);
            }
        }

        private void OnUsingInjection(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;

            recentUser = ev.Player;

            int npcCount = 5;
            float radius = 2.0f; // Radius des Kreises
            float playerHor = 0f;
            float playerVert = 0f;
            Vector3 playerPos = ev.Player.Position;

            int swapIndex = UnityEngine.Random.Range(0, npcCount); // Zufälliger Platz für den Spieler
            List<Npc> spawnedNpcs = new List<Npc>();

            for (int i = 0; i < npcCount; i++)
            {
                float angle = i * (360f / npcCount);
                float radian = angle * Mathf.Deg2Rad;

                Vector3 spawnPos = new Vector3(
                    playerPos.x + Mathf.Cos(radian) * radius,
                    playerPos.y,
                    playerPos.z + Mathf.Sin(radian) * radius
                );

                var dummy = Npc.Spawn(ev.Player.CustomName, ev.Player.Role, spawnPos);
                dummy.Health = 9999;

                

                dummy.Heal(9999);

                spawnedNpcs.Add(dummy);
            }

            // Sobald alle NPCs gespawnt sind, teleportiere den Spieler und entferne den Dummy an dieser Position
            Timing.CallDelayed(0.5f, () =>
            {
                Vector3 newPlayerPos = spawnedNpcs[swapIndex].Position;
                ev.Player.Position = newPlayerPos;
                
                spawnedNpcs[swapIndex].Destroy(); // Entferne den NPC, der durch den Spieler ersetzt wird
                spawnedNpcs.RemoveAt(swapIndex);

                foreach(var npc in spawnedNpcs)
                {
                    npc.EnableEffect(EffectType.Invisible, 2f);
                    npc.BadgeHidden = true;
                }

                ev.Player.EnableEffect(EffectType.Invisible, 2f);
                ev.Player.EnableEffect(EffectType.Ensnared, 2f);

                if (ev.Player.Role is FpcRole playerFpc)
                {
                    Log.Info("Getting player rotation...");
                    var playerMouseLook = playerFpc.FirstPersonController.FpcModule.MouseLook;
                    playerHor = playerMouseLook.CurrentHorizontal;
                    playerVert = playerMouseLook.CurrentVertical;
                }

                foreach (Npc npc in spawnedNpcs)
                {
                    if (npc.Role is FpcRole fpcRole)
                    {
                        var mouseLook = fpcRole.FirstPersonController.FpcModule.MouseLook;
                        mouseLook.CurrentVertical = playerVert;
                        mouseLook.CurrentHorizontal = playerHor;
                    }
                }
            });

            // Entfernt die verbleibenden NPCs nach 10 Sekunden
            Timing.CallDelayed(10f, () =>
            {
                if (!DidPlayerEscape)
                {
                    Log.Info("Player survived the waiting time. Giving nerfed benefits.");
                    ev.Player.EnableEffect(EffectType.MovementBoost, 20, 10f);
                    ev.Player.EnableEffect(EffectType.Ghostly, 10f);
                }

                foreach (var npc in spawnedNpcs)
                {
                    npc.Destroy();
                }

                DidPlayerEscape = false;
            });
        }
    }
}
