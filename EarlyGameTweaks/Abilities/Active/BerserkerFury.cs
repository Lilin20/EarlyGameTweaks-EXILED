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

        protected override void AbilityUsed(Player player)
        {
            player.ShowHint(UsingAbility, 5f);
            PlayersWithFuryDoorAbility.Add(player);

            AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Player {player.Nickname}", onIntialCreation: (p) =>
            {
                // Attach created audio player to player.
                p.transform.parent = player.GameObject.transform;

                // This created speaker will be in 3D space.
                Speaker speaker = p.AddSpeaker("Main", isSpatial: true, minDistance: 1f, maxDistance: 40f);

                // Attach created speaker to player.
                speaker.transform.parent = player.GameObject.transform;

                // Set local positino to zero to make sure that speaker is in player.
                speaker.transform.localPosition = Vector3.zero;
            });

            // As example we will add clip
            audioPlayer.AddClip("berserker2", volume: 1.25f);

            Light light = Light.Create(Vector3.zero, Vector3.zero, Vector3.one, true, new Color(0.8f, 0f, 0f));
            light.Intensity = 25;
            light.Range = 25;
            light.ShadowType = LightShadows.Soft;
            light.AdminToyBase.NetworkMovementSmoothing = 60;
            _playerLights.Add(player.Id, light);

            Timing.CallDelayed(0.5f, () =>
            {
                light.Base.transform.parent = player.Transform;
                light.Position = player.Position;
            });

            player.EnableEffect(EffectType.Invigorated, Duration, false);
            player.EnableEffect(EffectType.Vitality, Duration, false);
            player.EnableEffect(EffectType.DamageReduction, 40 , Duration, false);
            player.EnableEffect(EffectType.Deafened, Duration, false);
            player.EnableEffect(EffectType.AmnesiaItems, Duration, false);
            player.EnableEffect(EffectType.MovementBoost, 20, Duration, false);

            Timing.CallDelayed(Duration, () =>
            {
                player.EnableEffect(EffectType.Concussed, 10f, false);
                player.EnableEffect(EffectType.Exhausted, 10f, false);
                player.EnableEffect(EffectType.Blinded, 80, 10f, false);
                player.EnableEffect(EffectType.Slowness, 40, 10f, false);

                if (_playerLights.TryGetValue(player.Id, out var lights))
                {
                    lights.Destroy();
                    _playerLights.Remove(player.Id);
                    audioPlayer.Destroy();
                }
            });
        }

        protected override void AbilityRemoved(Player player)
        {
            if (_playerLights.TryGetValue(player.Id, out var lights))
            {
                lights.Destroy();
                _playerLights.Remove(player.Id);
            }
        }
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }
    }
}
