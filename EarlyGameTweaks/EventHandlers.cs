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
            if (ev.Player.ActiveEffects.Contains(ev.Player.GetEffect(EffectType.Invisible)))
                ev.Player.DisableEffect(EffectType.Invisible);

            if (!Physics.Raycast(ev.Player.CameraTransform.position, ev.Player.CameraTransform.forward, out RaycastHit hit, 5, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                return;

            if (hit.collider == null || !Exiled.API.Features.Camera.TryGet(hit.collider.gameObject.GetComponentInParent<Scp079Camera>(), out var camera))
                return;

            Log.Info(camera.Room);
            if (camera.IsBeingUsed)
            {
                foreach (var player in Exiled.API.Features.Player.List.Where(p => p.Role is Exiled.API.Features.Roles.Scp079Role scp079))
                {
                    if (UnityEngine.Random.value <= 0.1f)
                        ((Exiled.API.Features.Roles.Scp079Role)player.Role).LoseSignal(5);
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
        }s

        public void OnMapGeneration()
        {
            // Placeholder for future implementation
        }

        public void OnRoundStartSendHint()
        {
            HighlightRoom(RoomType.Lcz173, Color.red);
            ConfigureDoor(DoorType.Scp173Gate, DoorLockType.Isolation, allowsScp106: false);

            var allRooms = Room.List.ToArray();
            var forbiddenRoomTypes = GetForbiddenRoomTypes();

            foreach (var player in Exiled.API.Features.Player.List.Where(p => p.Role == RoleTypeId.FacilityGuard))
            {
                var randomRoom = GetRandomRoom(allRooms, forbiddenRoomTypes);
                Log.Info($"Guard {player.CustomName} spawned in {randomRoom.Type}");
                player.Teleport(randomRoom.Position + Vector3.up);
            }

            SetupAudioPlayer();
        }

        public void OnPickupArmor(PickingUpItemEventArgs ev)
        {
            if (ev.Pickup.Category != ItemCategory.Armor)
                return;

            if (ev.Player.Items.Any(item => item.IsArmor && CustomItem.Get(900).Check(item)))
            {
                ev.IsAllowed = false;
                ev.Pickup.Destroy();
                ev.Player.Heal(50);
            }
        }

        public void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (!Exiled.API.Features.Player.TryGet(hub, out var player) || hub == null || player == null)
                return;

            if (settingBase is SSKeybindSetting keybindSetting && keybindSetting.SyncIsPressed)
                HandleKeybindSetting(player, keybindSetting);
        }

        private void HandleKeybindSetting(Exiled.API.Features.Player player, SSKeybindSetting keybindSetting)
        {
            if (!ActiveAbility.AllActiveAbilities.TryGetValue(player, out var abilities))
                return;

            string response = string.Empty;
            var ability = abilities.FirstOrDefault(a => a.GetType() == GetAbilityType(keybindSetting.SettingId));

            if (ability != null && ability.CanUseAbility(player, out response))
            {
                ability.SelectAbility(player);
                ability.UseAbility(player);
                player.ShowHint(GetAbilityHint(keybindSetting.SettingId));
            }
            else
            {
                player.ShowHint(response);
            }
        }

        private Type GetAbilityType(int settingId) => settingId switch
        {
            10003 => typeof(DoorPicking),
            10004 => typeof(ZoneBlackout),
            10005 => typeof(ChargeAbility),
            10006 => typeof(BerserkerFury),
            10007 => typeof(HealingMist),
            10008 => typeof(Pickpocket),
            _ => null
        };

        private string GetAbilityHint(int settingId) => settingId switch
        {
            10003 => "Activated Door Picking, Interact with the door you want to pick.",
            _ => string.Empty
        };

        private void HighlightRoom(RoomType roomType, Color color)
        {
            var room = Room.Get(roomType);
            room.Color = color;
        }

        private void ConfigureDoor(DoorType doorType, DoorLockType lockType, bool allowsScp106)
        {
            var door = Door.Get(doorType);
            door.DoorLockType = lockType;
            door.AllowsScp106 = allowsScp106;
        }

        private Room GetRandomRoom(Room[] allRooms, List<RoomType> forbiddenRoomTypes)
        {
            Room randomRoom;
            do
            {
                randomRoom = allRooms[UnityEngine.Random.Range(0, allRooms.Length)];
            } while (forbiddenRoomTypes.Contains(randomRoom.Type));

            return randomRoom;
        }

        private List<RoomType> GetForbiddenRoomTypes() => new()
        {
            RoomType.Hcz079, RoomType.Hcz106, RoomType.HczHid, RoomType.Hcz096, RoomType.Hcz939,
            RoomType.HczTestRoom, RoomType.Hcz049, RoomType.EzCollapsedTunnel, RoomType.EzGateA,
            RoomType.EzGateB, RoomType.Lcz173, RoomType.HczTesla, RoomType.EzShelter, RoomType.Pocket
        };

        private void SetupAudioPlayer()
        {
            var audioPlayer = AudioPlayer.CreateOrGet("Radio", p =>
            {
                var speaker = p.AddSpeaker("Main", isSpatial: true, minDistance: 1f, maxDistance: 10f);
                speaker.transform.position = new Vector3(62.363f, 995.667f, -38.315f);
            });

            audioPlayer.SetSpeakerPosition("Main", new Vector3(62.363f, 995.667f, -38.315f));
            audioPlayer.AddClip("alarmSound", loop: true, volume: 1, destroyOnEnd: false);
        }
    }
}
