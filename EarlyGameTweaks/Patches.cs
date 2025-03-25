using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore;
using HarmonyLib;

namespace EarlyGameTweaks
{
    [HarmonyPatch(typeof(DummyUtils))]
    [HarmonyPatch("DummyGroup", MethodType.Setter)]
    public static class DummyGroupPatch
    {
        public static void Postfix(ref UserGroup value)
        {
            value.HiddenByDefault = true;
        }
    }
}
