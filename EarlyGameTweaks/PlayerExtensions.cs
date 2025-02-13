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
            if (player.Velocity.x != Vector3.zero.x)
            {
                return false;
            }
            else if (player.Velocity.y != Vector3.zero.y)
            {
                return false;
            }
            else if (player.Velocity.z != Vector3.zero.z)
            {
                return false;
            }

            return true;
        }

        public static void SetIntensityMovementBoost(Exiled.API.Features.Player player, byte itensity)
        {
            player.ChangeEffectIntensity<MovementBoost>(itensity);
        }
    }
}
