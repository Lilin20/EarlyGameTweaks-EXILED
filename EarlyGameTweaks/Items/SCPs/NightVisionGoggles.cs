using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace EarlyGameTweaks.Items
{
    [CustomItem(ItemType.SCP268)]
    public class NightVisionGoggles : GogglesItem
    {
        public override uint Id { get; set; } = 801;
        public override string Name { get; set; } = "Night Hawk Mk. 2";
        public override string Description { get; set; } = "Ermöglicht es dir bei völliger dunkelheit zu sehen.";
        public override float Weight { get; set; } = 0.5f;
        private static Dictionary<int, Light> _playerLights = new Dictionary<int, Light>();
        public override SpawnProperties SpawnProperties { get; set; }


        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected void OnConnected(VerifiedEventArgs e)
        {
            foreach (Light light in _playerLights.Values)
            {
                e.Player.ODestroyNetworkIdentity(light.AdminToyBase.netIdentity);
            }
        }
        protected override void RemoveGoggles(Player player, bool showMessage = true)
        {
            base.RemoveGoggles(player, showMessage);
            if (_playerLights.TryGetValue(player.Id, out var lights))
            {
                lights.Destroy();
                _playerLights.Remove(player.Id);
            }
        }
        protected override void EquipGoggles(Player player, bool showMessage = true)
        {
            base.EquipGoggles(player, showMessage);

            Light light = Light.Create(Vector3.zero, Vector3.zero, Vector3.one, true, new Color(0.2f, 1, 0.2f));
            light.Intensity = 50;
            light.Range = 50;
            light.ShadowStrength = 0;
            light.ShadowType = LightShadows.None;
            light.AdminToyBase.NetworkMovementSmoothing = 60;
            _playerLights.Add(player.Id, light);

            foreach (Player processingPlayer in Player.List)
            {
                if (processingPlayer != player)
                {

                    processingPlayer.ODestroyNetworkIdentity(light.AdminToyBase.netIdentity);

                }
            }
            Timing.CallDelayed(1.5f, () =>
            {
                light.Base.transform.parent = player.Transform;
                light.Position = player.Position;
            });
        }
    }
}
