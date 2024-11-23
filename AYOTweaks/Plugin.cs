using AYOTweaks.Windows;
using System.IO;
using System.Runtime.InteropServices;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Client.Game.Character;

namespace AYOTweaks;
public unsafe sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("AYOTweaks");
    private MainWindow MainWindow { get; init; }
    
    
    //:B: meme stuff
    
    //Sleep stuff
    private delegate byte ShouldSnap(Character* a1, SnapPosition* a2);
    private byte ShouldSnapDetour(Character* a1, SnapPosition* a2) => (byte) (Configuration.DisableSnap ? 0 : ShouldSnapHook!.Original(a1, a2));
    [Signature("E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? 4C 8D 74 24", DetourName = nameof(ShouldSnapDetour))]
    private Hook<ShouldSnap>? ShouldSnapHook { get; init; } = null;
    [StructLayout(LayoutKind.Explicit, Size = 0x38)]
    public struct SnapPosition
    {
        [FieldOffset(0x00)]
        public Vector3 PositionA;

        [FieldOffset(0x10)]
        public float RotationA;
        
        [FieldOffset(0x20)] public Vector3 PositionB;

        [FieldOffset(0x30)]
        public float RotationB;
    }
    
    //Plogon
    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        
        //Sleep
        GameInteropProvider.InitializeFromAttributes(this);
        ShouldSnapHook?.Enable();

        var memImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "mem.png");
        
        MainWindow = new MainWindow(this, memImagePath);
        WindowSystem.AddWindow(MainWindow);
        
        CommandManager.AddHandler("/ayo", new CommandInfo(OnCommand)
        {
            HelpMessage = "Open settings menu"
        });
        CommandManager.AddHandler("/asleep", new CommandInfo(OnCommand)
        {
            HelpMessage = "Illegal sleep"
        });
        CommandManager.AddHandler("/asit", new CommandInfo(OnCommand)
        {
            HelpMessage = "Illegal sit"
        });
        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        
        
    }
    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        
        MainWindow.Dispose();

        ShouldSnapHook?.Disable();
        ShouldSnapHook?.Dispose();
        
        CommandManager.RemoveHandler("/ayo");
        CommandManager.RemoveHandler("/asleep");
        CommandManager.RemoveHandler("/asit");
    }
    private void OnCommand(string command, string args)
    {
        switch (command)
        {
            case "/ayo":
                ToggleMainUI();
                break;
            case "/asit":
                ExecuteEmote(96);
                break;
            case "/asleep":
                ExecuteEmote(88);
                break;
        }
    }
    private unsafe void ExecuteEmote(ushort emoteId)
    {
        var playEmoteOption = new EmoteController.PlayEmoteOption
        {
            TargetId = TargetSystem.Instance()->GetTargetObjectId(),
            Flags = 1
        };
        AgentEmote.Instance()->ExecuteEmote(emoteId, &playEmoteOption);
    }
    
    private void DrawUI() => WindowSystem.Draw();
    public void ToggleMainUI() => MainWindow.Toggle();
}
