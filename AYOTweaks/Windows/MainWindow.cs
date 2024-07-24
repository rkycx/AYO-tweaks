using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AYOTweaks.Windows;

public class MainWindow : Window, IDisposable
{
    private string MemImagePath;
    private Plugin Plugin;
    private Configuration Configuration;
    public MainWindow(Plugin plugin, string memImagePath)
        : base("AYO##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(620, 550),
            MaximumSize = new Vector2(620, 550)
            //MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        MemImagePath = memImagePath;
        Plugin = plugin;
        Configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text($"Settings");
        
        var supressSnap = Configuration.disableSnap;
        if (ImGui.Checkbox("Disable sleep snap", ref supressSnap))
        {
            Configuration.disableSnap = supressSnap;
            Configuration.Save();
        }
        
        ImGui.Spacing();
        
        var memImage = Plugin.TextureProvider.GetFromFile(MemImagePath).GetWrapOrDefault();
        if (memImage != null)
        {
            ImGui.Image(memImage.ImGuiHandle, new Vector2(memImage.Width, memImage.Height));
        }
        else
        {
            ImGui.Text("Image not found.");
        }
    }
}
