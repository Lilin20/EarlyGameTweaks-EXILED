using System.Collections.Generic;
using System.Text;
using EarlyGameTweaks.Roles.ChaosInsurgency;
using EarlyGameTweaks.Roles.ClassD;
using EarlyGameTweaks.Roles.NTF;
using EarlyGameTweaks.Roles.Scientist;
using Exiled.CustomRoles.API.Features;
using NorthwoodLib.Pools;
using UserSettings.ServerSpecific;

namespace EarlyGameTweaks
{
    public class SettingHandlers
    {
        public static ServerSpecificSettingBase[] LilinsAIOMenu() => SettingsHelper.GetSettings();
    }

    public class SettingsHelper
    {
        public static ServerSpecificSettingBase[] GetSettings()
        {
            List<ServerSpecificSettingBase> settings = [];
            StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();

            settings.Add(new SSGroupHeader("Lilin's AIO - Ability Keybinds"));
            settings.Add(new SSKeybindSetting(10003, "Lockpicking [Ability]", UnityEngine.KeyCode.B, true, "B"));
            settings.Add(new SSKeybindSetting(10004, "Zone Blackout [Ability]", UnityEngine.KeyCode.B, true, "B"));
            settings.Add(new SSKeybindSetting(10005, "Charge [Ability]", UnityEngine.KeyCode.B, true, "B"));
            settings.Add(new SSKeybindSetting(10006, "Berserker's Fury [Ability]", UnityEngine.KeyCode.B, true, "B"));
            settings.Add(new SSKeybindSetting(10007, "Healing Mist [Ability]", UnityEngine.KeyCode.B, true, "B"));
            settings.Add(new SSKeybindSetting(10008, "NOPE [Ability]", UnityEngine.KeyCode.B, true, "B"));

            return [.. settings];
        }
    }
}
