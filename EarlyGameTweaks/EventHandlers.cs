using System;
using System.Collections.Generic;
using System.Linq;
using EarlyGameTweaks.Abilities.Active;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using MEC;
using PlayerRoles.PlayableScps.Scp079.Cameras;
using PluginAPI.Events;
using UnityEngine;
using UserSettings.ServerSpecific;
using HintServiceMeow;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.UI.Utilities;
using Exiled.API.Features.Roles;
using PlayerRoles;
using EarlyGameTweaks.API;
using Exiled.Events.EventArgs.Server;
using CustomPlayerEffects;

namespace EarlyGameTweaks
{
    public class EventHandlers
    {
        public static List<GameObject> BlacklistedObjects = new();
        private readonly EarlyGameTweaks Plugin;
        private readonly Config _config;

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

        public void OnRoundStartSendHint()
        {
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
                RoomType.Lcz173
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

            foreach (Player player in Exiled.API.Features.Player.List)
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
                foreach (Item item in ev.Player.Items)
                {
                    if (item.IsArmor)
                    {
                        if (CustomItem.Get(667).Check(item))
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
                if ((ssKeybindSetting.SettingId == 10003 || ssKeybindSetting.SettingId == 10004 || ssKeybindSetting.SettingId == 10005 || ssKeybindSetting.SettingId == 10006) && ActiveAbility.AllActiveAbilities.TryGetValue(player, out var abilities))
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
                }
            }
        }
    }
}
