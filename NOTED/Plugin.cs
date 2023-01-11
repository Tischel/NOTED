using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using NOTED.Models;
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
        private static JobsDataWindow _jobsDataWindow = null!;
        private static NoteWindow _noteWindow = null!;

        private static ushort _prevTerritoryID = 0;
        private static Duty? _activeDuty = null;
        private static bool _forceHide = false;
        private static Note? _prevNote = null;

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

            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";

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
            if (arguments == "toggle")
            {
                _forceHide = !_forceHide;
            } 
            else
            {
                _settingsWindow.IsOpen = !_settingsWindow.IsOpen;
            }
            
        }

        private void CreateWindows()
        {
            _settingsWindow = new SettingsWindow("NOTED v" + Version);
            _jobsDataWindow = new JobsDataWindow("Jobs");
            _noteWindow = new NoteWindow("Note");

            _windowSystem = new WindowSystem("NOTED_Windows");
            _windowSystem.AddWindow(_settingsWindow);
            _windowSystem.AddWindow(_jobsDataWindow);
            _windowSystem.AddWindow(_noteWindow);

            _noteWindow.IsOpen = true;
        }

        public static void ShowJobsDataWindow(Note note)
        {
            _jobsDataWindow.WindowName = "Configure \"" + note.Title + "\" jobs";
            _jobsDataWindow.Note = note;
            _jobsDataWindow.IsOpen = true;
        }

        public static void EditNote(Note? note)
        {
            if (note == null) { return; }

            if (Settings.Duties.TryGetValue(ClientState.TerritoryType, out Duty? duty) && duty != null)
            {
                if (duty.Notes.Contains(note))
                {
                    _settingsWindow.SelectedDuty = duty;
                    _settingsWindow.SelectedNote = note;
                    _settingsWindow.NeedsFocus = true;
                    _settingsWindow.IsOpen = true;
                }
            }
        }

        private unsafe void Draw()
        {
            if (Settings == null || ClientState.LocalPlayer == null) return;

            // detect territory change
            ushort territory = ClientState.TerritoryType;
            if (territory != _prevTerritoryID)
            {
                _activeDuty = null;
                _noteWindow.Note = null;
                _prevNote = null;
                _forceHide = false;
                _prevTerritoryID = territory;

                if (Settings.Duties.TryGetValue(territory, out Duty? duty) && duty != null)
                {
                    _activeDuty = duty;
                }
            }

            // set note
            if (_activeDuty != null)
            {
                Note? activeNote = _activeDuty.GetActiveNote();

                // auto copy?
                if (activeNote != null && activeNote != _prevNote && activeNote.AutoCopy)
                {
                    ImGui.SetClipboardText(activeNote.Text);
                }

                _prevNote = activeNote;
                _noteWindow.Note = activeNote;
            }

            _noteWindow.IsOpen = Settings.Preview || (_noteWindow.Note != null && !_forceHide);
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

            UiBuilder.Draw -= Draw;
            UiBuilder.OpenConfigUi -= OpenConfigUi;
        }
    }
}
