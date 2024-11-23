using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace AYOTweaks.Windows;

public class MainWindow : Window, IDisposable
{
    private string memImagePath;
    private Plugin Plugin;
    private Configuration configuration;
    public MainWindow(Plugin plugin, string memImgPath)
        : base("AYO##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(620, 550),
        };

        memImagePath = memImgPath;
        Plugin = plugin;
        configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Text($"Settings");
        
        var suppressSnap = configuration.DisableSnap;
        if (ImGui.Checkbox("Disable sleep snap", ref suppressSnap))
        {
            configuration.DisableSnap = suppressSnap;
            configuration.Save();
        }
        
        ImGui.Spacing();
        
        var memImage = Plugin.TextureProvider.GetFromFile(memImagePath).GetWrapOrDefault();
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
