using System.Collections.Generic;
using EarlyGameTweaks.API;
using EarlyGameTweaks.Roles.ChaosInsurgency;
using EarlyGameTweaks.Roles.ClassD;
using EarlyGameTweaks.Roles.NTF;
using EarlyGameTweaks.Roles.Scientist;
using EarlyGameTweaks.Roles.SCP;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using HarmonyLib;
using MEC;
using UserSettings.ServerSpecific;

namespace EarlyGameTweaks
{
    public class EarlyGameTweaks : Plugin<Config>
    {
        public static EarlyGameTweaks Instance;
        public override PluginPriority Priority { get; } = PluginPriority.Medium;
        public EventHandlers EventHandlers;
        public Harmony harmony = new Harmony("test.patch.gigachad");

        private readonly List<ICustomRole> customRoles = new()
        {
            new Lockpicker(),
            new Dwarf(),
            new DeltaAgent(),
            new Breacher(),
            new Pacifist(),
            new ChaosMedic(),
            new NtfMedic(),
            new LuckyMan(),
            new Sniper(),
            new Berserker(),
            new SpecialAgentGuard(),
            new ZombieJuggernaut(),
            new MedicZombie(),
            new Thief()
        };

        public List<ExplosionGrenadeProjectile> GrenadeProjectiles { get; set; } = new();
        public List<ExplosionGrenadeProjectile> AlertGrenadeProjectiles { get; set; } = new();
        public Dictionary<StartTeam, List<ICustomRole>> Roles { get; } = new();
        public CustomRoleEventHandler CustomRoleEventHandler;

        public override void OnEnabled()
        {
            Instance = this;
            EventHandlers = new EventHandlers();
            RegisterEvents();
            LoadAudioClips();
            RegisterCustomRoles();
            InitializeRoleDictionary();
            CustomAbility.RegisterAbilities();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();
            UnregisterCustomRoles();
            CustomAbility.UnregisterAbilities();
            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Shot += EventHandlers.OnShot;
            Exiled.Events.Handlers.Player.Verified += EventHandlers.OnVerify;
            Exiled.Events.Handlers.Player.PickingUpItem += EventHandlers.OnPickupArmor;
            Exiled.Events.Handlers.Scp096.AddingTarget += EventHandlers.OnRageStart;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += EventHandlers.OnSettingValueReceived;

            CustomRoleEventHandler = new CustomRoleEventHandler(this);
            Exiled.Events.Handlers.Server.RoundStarted += CustomRoleEventHandler.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundStarted += EventHandlers.OnRoundStartSendHint;
            Exiled.Events.Handlers.Server.RespawningTeam += CustomRoleEventHandler.OnRespawningTeam;
            Exiled.Events.Handlers.Map.Generated += EventHandlers.OnMapGeneration;
            Exiled.Events.Handlers.Scp049.FinishingRecall += CustomRoleEventHandler.FinishingRecall;
        }

        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Shot -= EventHandlers.OnShot;
            Exiled.Events.Handlers.Player.Verified -= EventHandlers.OnVerify;
            Exiled.Events.Handlers.Player.PickingUpItem -= EventHandlers.OnPickupArmor;
            Exiled.Events.Handlers.Scp096.AddingTarget -= EventHandlers.OnRageStart;
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= EventHandlers.OnSettingValueReceived;

            Exiled.Events.Handlers.Server.RoundStarted -= CustomRoleEventHandler.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundStarted -= EventHandlers.OnRoundStartSendHint;
            Exiled.Events.Handlers.Server.RespawningTeam -= CustomRoleEventHandler.OnRespawningTeam;
            Exiled.Events.Handlers.Map.Generated -= EventHandlers.OnMapGeneration;
            Exiled.Events.Handlers.Scp049.FinishingRecall -= CustomRoleEventHandler.FinishingRecall;
        }

        private void LoadAudioClips()
        {
            AudioClipStorage.LoadClip("C:\\Users\\Administrator\\AppData\\Roaming\\EXILED\\Audio\\test.ogg", "test");
            AudioClipStorage.LoadClip("C:\\Users\\Administrator\\AppData\\Roaming\\EXILED\\Audio\\alarm.ogg", "alarmSound");
            AudioClipStorage.LoadClip("C:\\Users\\Administrator\\AppData\\Roaming\\EXILED\\Audio\\berserker2.ogg", "berserker2");
        }

        private void RegisterCustomRoles()
        {
            foreach (var role in customRoles)
                role.Register();
        }

        private void UnregisterCustomRoles()
        {
            foreach (var role in customRoles)
                role.Unregister();
        }

        private void InitializeRoleDictionary()
        {
            foreach (CustomRole role in CustomRole.Registered)
            {
                if (role is ICustomRole custom)
                {
                    Log.Debug($"Adding {role.Name} to dictionary...");
                    StartTeam team = DetermineStartTeam(custom.StartTeam);

                    if (!Roles.ContainsKey(team))
                        Roles.Add(team, new());

                    for (int i = 0; i < role.SpawnProperties.Limit; i++)
                        Roles[team].Add(custom);

                    Log.Debug($"Roles {team} now has {Roles[team].Count} elements.");
                }
            }
        }

        private StartTeam DetermineStartTeam(StartTeam startTeam)
        {
            if (startTeam.HasFlag(StartTeam.Chaos)) return StartTeam.Chaos;
            if (startTeam.HasFlag(StartTeam.Guard)) return StartTeam.Guard;
            if (startTeam.HasFlag(StartTeam.Ntf)) return StartTeam.Ntf;
            if (startTeam.HasFlag(StartTeam.Scientist)) return StartTeam.Scientist;
            if (startTeam.HasFlag(StartTeam.ClassD)) return StartTeam.ClassD;
            if (startTeam.HasFlag(StartTeam.Scp)) return StartTeam.Scp;
            return StartTeam.Other;
        }
    }
}
