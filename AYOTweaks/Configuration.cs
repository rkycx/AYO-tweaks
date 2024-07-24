using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace AYOTweaks;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public bool disableSnap { get; set; } = true;
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
