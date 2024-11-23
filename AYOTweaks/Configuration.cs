using Dalamud.Configuration;
using System;

namespace AYOTweaks;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public bool DisableSnap { get; set; } = true;
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
