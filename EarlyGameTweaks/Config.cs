using Exiled.API.Interfaces;

namespace EarlyGameTweaks
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public bool EnableBroadcast { get; set; } = false;
        public string BroadcastMessage { get; set; } = "Gained Speed Boost for 2 minutes!";
        public ushort BroadcastDuration { get; set; } = 5;
        public int MovementBoostDuration { get; set; } = 120;
        public byte MovementBoostValue { get; set; } = 20;
        public int DamageReductionDuration { get; set; } = 120;
        public byte DamageReductionValue { get; set; } = 100;
    }
}
