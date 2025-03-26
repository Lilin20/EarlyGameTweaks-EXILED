using CustomPlayerEffects;
using Exiled.API.Features;
using Mirror;
using UnityEngine;

namespace EarlyGameTweaks
{
    public static class PlayerExtensions
    {
        public static void ODestroyNetworkIdentity(this Player player, NetworkIdentity networkIdentity)
        {
            player.Connection.Send(new ObjectDestroyMessage
            {
                netId = networkIdentity.netId
            });
        }

        public static bool IsStaying(Exiled.API.Features.Player player)
        {
            return player.Velocity == Vector3.zero;
        }

        public static void SetIntensityMovementBoost(Exiled.API.Features.Player player, byte intensity)
        {
            player.ChangeEffectIntensity<MovementBoost>(intensity);
        }
    }
}
