using System;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Usables;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerStatsSystem;
using Utils.Networking;
using VoiceChat;

namespace EarlyGameTweaks
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem__UInt16__Boolean))]
    internal static class InventoryCmdDropItemPrefix
    {
        /// <summary>
        /// The logic may need to be re-worked as some players want to be able to throw items like the vase.
        /// </summary>
        [HarmonyPrefix]
        private static bool Prefix(Inventory __instance, ushort itemSerial, bool tryThrow)
        {
            try
            {

                if (!tryThrow)
                    return true;
                Player ply = Player.Get(__instance._hub);
                if (ply.Role != RoleTypeId.Scp3114)
                    return true;

                return FakeFiringExtensions.OnPlayerThrowItem(ply, itemSerial, tryThrow);
            }
            catch (Exception e)
            {
                Log.Debug($"An error has been caught at InventoryCmdDropItemPrefix. Exception: \n{e}");
                return true;

            }
        }
    }

    [HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.ServerReceivedStatus))]
    internal class UsableItemsServerReceivedStatusPostfix
    {
        [HarmonyPostfix]
        private static void Postfix(NetworkConnection conn, StatusMessage msg)
        {
            try
            {
                Log.Debug($"Fake Usable Interaction Triggered.");
                Player ply = Player.Get(conn.identity);
                if (ply is null || !(ply.ReferenceHub.inventory.CurInstance is UsableItem usableItem) ||
                    usableItem.ItemSerial != msg.ItemSerial)
                {
                    return;
                }

                if (ply.Role != RoleTypeId.Scp3114)
                    return;
                if (msg.Status == StatusMessage.StatusType.Cancel)
                {
                    Log.Debug("Sending Fake Usable Interaction.");
                    new StatusMessage(StatusMessage.StatusType.Start, msg.ItemSerial).SendToAuthenticated();
                    Timing.CallDelayed(usableItem.UseTime, () =>
                    {
                        ply.CurrentItem = null;
                        Log.Debug("Hiding Item for Fake Use.");
                    });
                }
            }
            catch (Exception e)
            {
                Log.Error("Scp3114Mods has caught an error at UsableItemsServerReceivedStatusPostfix.");
                Log.Debug($"Exception: \n{e}");
                return;
            }
        }
    }
}
