using System;
using System.Collections.Generic;
using System.Linq;
using EarlyGameTweaks.Abilities.Active;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using UnityEngine;
using UserSettings.ServerSpecific;
using PlayerRoles;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Scp939;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Scp1344;
using UnityEngine.AI;
using MEC;
using static PlayerList;
using PlayerStatsSystem;
using Exiled.API.Features.DamageHandlers;
using MapGeneration;

namespace EarlyGameTweaks
{
    public class EventHandlers
    {
        public static List<GameObject> BlacklistedObjects = new();
        private readonly EarlyGameTweaks Plugin;
        private readonly Config _config;
        public CoroutineHandle pathFinderCoroutine = new CoroutineHandle();

        public void OnShot(ShotEventArgs ev)
        {
            if (ev.Player.ActiveEffects.ToList().Contains(ev.Player.GetEffect(EffectType.Invisible)))
            {
                ev.Player.DisableEffect(EffectType.Invisible);
            }

            if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit raycastHit,
                   5, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                return;

            if (raycastHit.collider is null)
                return;

            if (!Exiled.API.Features.Camera.TryGet(raycastHit.collider.gameObject.GetComponentInParent<Scp079Camera>(), out Exiled.API.Features.Camera hit))
                return;

            Log.Info(hit.Room);
            if (hit.IsBeingUsed)
            {
                foreach (Exiled.API.Features.Player playercam in Exiled.API.Features.Player.List)
                {
                    if (playercam.Role is Exiled.API.Features.Roles.Scp079Role scp079)
                    {
                        float random = UnityEngine.Random.value;
                        if (random <= 0.1f)
                        {
                            scp079.LoseSignal(5);
                        }
                    }
                }
            }
        }

        public void OnVerify(VerifiedEventArgs ev)
        {
            ServerSpecificSettingsSync.DefinedSettings = SettingHandlers.LilinsAIOMenu();
            ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
        }

        public void OnRageStart(AddingTargetEventArgs ev)
        {
            ev.Target.EnableEffect(EffectType.MovementBoost, 30, 5, false);
        }

        public void OnWearingGlasses(ChangedStatusEventArgs ev)
        {
            if (ev.Scp1344Status == InventorySystem.Items.Usables.Scp1344.Scp1344Status.Active)
            {
                ev.Player.EnableEffect(EffectType.FogControl, 5);
            }
        }

        public void OnMapGeneration()
        {
            //navMeshCoroutine = Timing.RunCoroutine(OptimizeNavMeshCoroutine());
        }

        public void OnRoundStartSendHint()
        {
            Room roomPeanut = Room.Get(RoomType.Lcz173);
            roomPeanut.Color = new Color(1f, 0f, 0f);

            Door door = Door.Get(DoorType.Scp173Gate);
            door.DoorLockType = DoorLockType.Isolation;
            door.AllowsScp106 = false;

            Room[] allRooms = Room.List.ToArray();
            List<RoomType> forbiddenRoomTypes = new List<RoomType>
            {
                RoomType.Hcz079,
                RoomType.Hcz106,
                RoomType.HczHid,
                RoomType.Hcz096,
                RoomType.Hcz939,
                RoomType.HczTestRoom,
                RoomType.Hcz049,
                RoomType.EzCollapsedTunnel,
                RoomType.EzGateA,
                RoomType.EzGateB,
                RoomType.Lcz173,
                RoomType.HczTesla,
                RoomType.EzShelter,
                RoomType.Pocket,
            };

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Radio", onIntialCreation: (p) =>
            {
                // This created speaker will be in 3D space.
                Speaker speaker = p.AddSpeaker("Main", isSpatial: true, minDistance: 1f, maxDistance: 10f);

                speaker.transform.localPosition = new Vector3(62.363f, 995.667f, -38.315f);
                speaker.transform.position = new Vector3(62.363f, 995.667f, -38.315f);
            });

            audioPlayer.SetSpeakerPosition("Main", new Vector3(62.363f, 995.667f, -38.315f));

            // As example we will add clip
            audioPlayer.AddClip("alarmSound", loop: true, volume: 1, destroyOnEnd: false);

            foreach (Exiled.API.Features.Player player in Exiled.API.Features.Player.List)
            {
                if (player.Role == RoleTypeId.FacilityGuard)
                {
                    Room randomRoom = allRooms[UnityEngine.Random.Range(0, allRooms.Length)];
                    while (forbiddenRoomTypes.Contains(randomRoom.Type))
                    {
                        randomRoom = allRooms[UnityEngine.Random.Range(0, allRooms.Length)];
                    }
                    Log.Info($"Guard {player.CustomName} spawned in {randomRoom.Type}");
                    player.Teleport(randomRoom.Position + Vector3.up);
                }
            }
        }

        public void OnPickupArmor(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.Category == ItemCategory.Armor)
            {
                foreach (Exiled.API.Features.Items.Item item in ev.Player.Items)
                {
                    if (item.IsArmor)
                    {
                        if (CustomItem.Get(900).Check(item))
                        {
                            ev.IsAllowed = false;
                            ev.Pickup.Destroy();
                            ev.Player.Heal(50);
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        public void OnLunging(LungingEventArgs ev)
        {
            if (ev.Player.Role is Scp939Role scp)
            {
               
            }
        }

        public void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (!Exiled.API.Features.Player.TryGet(hub, out Exiled.API.Features.Player player))
                return;

            if (hub == null)
                return;

            if (player == null)
                return;

            if (settingBase is SSKeybindSetting ssKeybindSetting && ssKeybindSetting.SyncIsPressed)
            {
                if ((ssKeybindSetting.SettingId == 10003 || ssKeybindSetting.SettingId == 10004 || ssKeybindSetting.SettingId == 10005 || ssKeybindSetting.SettingId == 10006 || ssKeybindSetting.SettingId == 10007 || ssKeybindSetting.SettingId == 10008) && ActiveAbility.AllActiveAbilities.TryGetValue(player, out var abilities))
                {
                    string response = String.Empty;
                    if (ssKeybindSetting.SettingId == 10003)
                    {
                        var doorPickingAbility = abilities.FirstOrDefault(abilities => abilities.GetType() == typeof(DoorPicking));
                        if (doorPickingAbility != null && doorPickingAbility.CanUseAbility(player, out response))
                        {
                            doorPickingAbility.SelectAbility(player);
                            doorPickingAbility.UseAbility(player);
                            player.ShowHint("Activated Door Picking, Interact with the door you want to pick.");
                        }
                        else
                        {
                            player.ShowHint(response);
                        }
                    }
                    else if (ssKeybindSetting.SettingId == 10004)
                    {
                        var blackoutAbility = abilities.FirstOrDefault(abilities => abilities.GetType() == typeof(ZoneBlackout));
                        if (blackoutAbility != null && blackoutAbility.CanUseAbility(player, out response))
                        {
                            blackoutAbility.SelectAbility(player);
                            blackoutAbility.UseAbility(player);
                        }
                        else
                        {
                            player.ShowHint(response);
                        }
                    }
                    else if (ssKeybindSetting.SettingId == 10005)
                    {
                        var chargeAbility = abilities.FirstOrDefault(abilities => abilities.GetType() == typeof(ChargeAbility));
                        if (chargeAbility != null && chargeAbility.CanUseAbility(player, out response))
                        {
                            chargeAbility.SelectAbility(player);
                            chargeAbility.UseAbility(player);
                        }
                        else
                        {
                            player.ShowHint(response);
                        }
                    }
                    else if (ssKeybindSetting.SettingId == 10006)
                    {
                        var furyAbility = abilities.FirstOrDefault(abilities => abilities.GetType() == typeof(BerserkerFury));
                        if (furyAbility != null && furyAbility.CanUseAbility(player, out response))
                        {
                            furyAbility.SelectAbility(player);
                            furyAbility.UseAbility(player);
                        }
                        else
                        {
                            player.ShowHint(response);
                        }
                    }
                    else if (ssKeybindSetting.SettingId == 10007)
                    {
                        var zombieHealingAbility = abilities.FirstOrDefault(abilities => abilities.GetType() == typeof(HealingMist));
                        if (zombieHealingAbility != null && zombieHealingAbility.CanUseAbility(player, out response))
                        {
                            zombieHealingAbility.SelectAbility(player);
                            zombieHealingAbility.UseAbility(player);
                        }
                        else
                        {
                            player.ShowHint(response);
                        }
                    }
                    else if (ssKeybindSetting.SettingId == 10008)
                    {
                        var pickpocketAbility = abilities.FirstOrDefault(abilities => abilities.GetType() == typeof(Pickpocket));
                        if (pickpocketAbility != null && pickpocketAbility.CanUseAbility(player, out response))
                        {
                            pickpocketAbility.SelectAbility(player);
                            pickpocketAbility.UseAbility(player);
                        }
                        else
                        {
                            player.ShowHint(response);
                        }
                    }
                }
            }
        }

    }
}
