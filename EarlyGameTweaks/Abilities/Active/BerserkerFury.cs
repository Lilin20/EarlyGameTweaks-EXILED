using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Toys;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace EarlyGameTweaks.Abilities.Active
{
    [CustomAbility]
    public class BerserkerFury : ActiveAbility
    {
        public override string Name { get; set; } = "Berserker's Fury";
        public override string Description { get; set; } = "Lasse deiner Wut freien lauf. Gewährt dir viele Boni.";
        public override float Duration { get; set; } = 15f;
        public override float Cooldown { get; set; } = 45f;
        private static Dictionary<int, Light> _playerLights = new Dictionary<int, Light>();
        public string UsingAbility { get; set; } = "The rage consumes me once more...";
        public List<Player> PlayersWithFuryDoorAbility = new List<Player>();

        private static void CleanupPlayerResources(Player player, AudioPlayer audioPlayer)
        {
            if (_playerLights.TryGetValue(player.Id, out var light))
            {
                light.Destroy();
                _playerLights.Remove(player.Id);
            }

            audioPlayer?.Destroy();
        }

        protected override void AbilityUsed(Player player)
        {
            player.ShowHint(UsingAbility, 5f);
            PlayersWithFuryDoorAbility.Add(player);

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {player.Nickname}", onIntialCreation: (p) =>
            {
                // Attach created audio player to player.
                p.transform.parent = player.GameObject.transform;

                // Create and attach speaker to player.
                Speaker speaker = p.AddSpeaker("Main", isSpatial: true, minDistance: 1f, maxDistance: 40f);
                speaker.transform.parent = player.GameObject.transform;
                speaker.transform.localPosition = Vector3.zero;
            });

            audioPlayer.AddClip("berserker2", volume: 1.25f);

            Light light = Light.Create(Vector3.zero, Vector3.zero, Vector3.one, true, new Color(0.8f, 0f, 0f))
            {
                Intensity = 25,
                Range = 25,
                ShadowType = LightShadows.Soft,
                AdminToyBase = { NetworkMovementSmoothing = 60 }
            };
            _playerLights.Add(player.Id, light);

            Timing.CallDelayed(0.5f, () =>
            {
                light.Base.transform.parent = player.Transform;
                light.Position = player.Position;
            });

            ApplyEffects(player, Duration);

            Timing.CallDelayed(Duration, () =>
            {
                ApplyPostEffects(player);
                CleanupPlayerResources(player, audioPlayer);
            });
        }

        private static void ApplyEffects(Player player, float duration)
        {
            player.EnableEffect(EffectType.Invigorated, duration, false);
            player.EnableEffect(EffectType.Vitality, duration, false);
            player.EnableEffect(EffectType.DamageReduction, 40, duration, false);
            player.EnableEffect(EffectType.Deafened, duration, false);
            player.EnableEffect(EffectType.AmnesiaItems, duration, false);
            player.EnableEffect(EffectType.MovementBoost, 20, duration, false);
        }

        private static void ApplyPostEffects(Player player)
        {
            player.EnableEffect(EffectType.Concussed, 10f, false);
            player.EnableEffect(EffectType.Exhausted, 10f, false);
            player.EnableEffect(EffectType.Blinded, 80, 10f, false);
            player.EnableEffect(EffectType.Slowness, 40, 10f, false);
        }

        protected override void AbilityRemoved(Player player)
        {
            CleanupPlayerResources(player, null);
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }
    }
}
