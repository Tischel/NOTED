using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using NOTED.Windows;
using System;
using System.Reflection;

namespace NOTED
{
    public class Plugin : IDalamudPlugin
    {
        public static ClientState ClientState { get; private set; } = null!;
        public static CommandManager CommandManager { get; private set; } = null!;
        public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        public static Framework Framework { get; private set; } = null!;
        public static GameGui GameGui { get; private set; } = null!;
        public static UiBuilder UiBuilder { get; private set; } = null!;
        public static DataManager DataManager { get; private set; } = null!;
        public static KeyState KeyState { get; private set; } = null!;

        public static string AssemblyLocation { get; private set; } = "";
        public string Name => "NOTED";

        public static string Version { get; private set; } = "";

        public static Settings Settings { get; private set; } = null!;

        private static WindowSystem _windowSystem = null!;
        private static SettingsWindow _settingsWindow = null!;

        public Plugin(
            ClientState clientState,
            CommandManager commandManager,
            DalamudPluginInterface pluginInterface,
            Framework framwork,
            DataManager dataManager,
            GameGui gameGui,
            KeyState keyState
        )
        {
            ClientState = clientState;
            CommandManager = commandManager;
            PluginInterface = pluginInterface;
            Framework = framwork;
            DataManager = dataManager;
            GameGui = gameGui;
            UiBuilder = pluginInterface.UiBuilder;
            KeyState = keyState;

            if (pluginInterface.AssemblyLocation.DirectoryName != null)
            {
                AssemblyLocation = pluginInterface.AssemblyLocation.DirectoryName + "\\";
            }
            else
            {
                AssemblyLocation = Assembly.GetExecutingAssembly().Location;
            }

            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.1.0.0";

            Framework.Update += Update;
            UiBuilder.Draw += Draw;
            UiBuilder.OpenConfigUi += OpenConfigUi;

            CommandManager.AddHandler(
                "/noted",
                new CommandInfo(PluginCommand)
                {
                    HelpMessage = "Opens the NOTED configuration window.",
                    ShowInHelp = true
                }
            );

            CommandManager.AddHandler(
                "/noted toggle",
                new CommandInfo(PluginCommand)
                {
                    HelpMessage = "Toggles the current note on/off.",
                    ShowInHelp = true
                }
            );

            Settings = Settings.Load();

            CreateWindows();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void PluginCommand(string command, string arguments)
        {
            _settingsWindow.IsOpen = !_settingsWindow.IsOpen;
        }

        private void CreateWindows()
        {
            _settingsWindow = new SettingsWindow("NOTED v" + Version);

            _windowSystem = new WindowSystem("NOTED_Windows");
            _windowSystem.AddWindow(_settingsWindow);
        }

        private unsafe void Update(Framework framework)
        {

        }

        private unsafe void Draw()
        {
            if (Settings == null || ClientState.LocalPlayer == null) return;
            _windowSystem?.Draw();
        }

        private void OpenConfigUi()
        {
            _settingsWindow.IsOpen = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            Settings.Save(Settings);
            
            _windowSystem.RemoveAllWindows();

            CommandManager.RemoveHandler("/noted");
            CommandManager.RemoveHandler("/noted toggle");

            Framework.Update -= Update;
            UiBuilder.Draw -= Draw;
            UiBuilder.OpenConfigUi -= OpenConfigUi;
        }
    }
}
