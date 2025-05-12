using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using Exiled.API.Features;
using Exiled.API.Structs;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles;

namespace EarlyGameTweaks
{
    public static class FakeFiringExtensions
    {
        public static void FakeFireAutomatic(this Firearm firearm)
        {
            ModuleBase[] modules = firearm.Modules;
            foreach (ModuleBase module in modules)
            {
                Log.Info(module.name);
                if (module is AutomaticActionModule aam)
                {
                    aam.PlayFireAnims(true);
                }  
            }
        }

        private static void _processDryFiring(Firearm firearm, Player sender)
        {
            Log.Debug("Fake dry firing gun.");
            // Dry Fire

            ModuleBase[] modules = firearm.Modules;
            foreach (ModuleBase module in modules)
            {
                Log.Info(module.name);
                if (module is AutomaticActionModule aam)
                {
                    aam.PlayFire(4);
                    aam.PlayFireAnims(false);
                }
            }
        }

        public static bool OnPlayerThrowItem(Player ply, ushort itemSerial, bool tryThrow)
        {
            if (ply.Role == RoleTypeId.Scp3114)
            {
                if (!ply.ReferenceHub.inventory.UserInventory.Items.ContainsKey(itemSerial))
                    return true;
                var item = ply.ReferenceHub.inventory.UserInventory.Items[itemSerial];
                if (item is Firearm firearm)
                {
                    _processDryFiring(firearm, ply);
                    return false;
                }
            }

            return true;
        }
    }
}
