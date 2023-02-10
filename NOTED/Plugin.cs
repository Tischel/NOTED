using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using ImGuiNET;
using Lumina.Data.Parsing.Uld;
using NOTED.Helpers;
using NOTED.Models;
using NOTED.Windows;
using System;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace NOTED
{
    public class Plugin : IDalamudPlugin
    {
        public static ClientState ClientState { get; private set; } = null!;
        public static CommandManager CommandManager { get; private set; } = null!;
        public static Condition Condition { get; private set; } = null!;
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
            Condition condition,
            DalamudPluginInterface pluginInterface,
            Framework framwork,
            DataManager dataManager,
            GameGui gameGui,
            KeyState keyState
        )
        {
            ClientState = clientState;
            CommandManager = commandManager;
            Condition = condition;
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

            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.1.1.0";

            UiBuilder.Draw += Draw;
            UiBuilder.OpenConfigUi += OpenConfigUi;
            Framework.Update += Update;

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

            KeyboardHelper.Initialize();

            SetupKeybinds();
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

        private void SetupKeybinds()
        {
            Settings.HideKeybind.SetAction(() =>
            {
                Settings.Hidden = !Settings.Hidden;
            });

            Settings.NextNoteKeybind.SetAction(() =>
            {
                NextNote();
            });

            Settings.PreviousNoteKeybind.SetAction(() =>
            {
                PreviousNote();
            });
        }

        private void NextNote()
        {
            if (_activeDuty == null || _activeDuty.Notes.Count <= 1) { return; }

            int count = 0;

            do
            {
                Note firstNote = _activeDuty.Notes[0];
                _activeDuty.Notes.Remove(firstNote);
                _activeDuty.Notes.Add(firstNote);
                count++;
            }
            while (count < _activeDuty.Notes.Count && !_activeDuty.Notes[0].Enabled);
        }

        private void PreviousNote()
        {
            if (_activeDuty == null || _activeDuty.Notes.Count <= 1) { return; }

            int count = 0;

            do
            {
                Note lastNote = _activeDuty.Notes[_activeDuty.Notes.Count - 1];
                _activeDuty.Notes.Remove(lastNote);
                _activeDuty.Notes.Insert(0, lastNote);
                count++;
            }
            while (count < _activeDuty.Notes.Count && !_activeDuty.Notes[0].Enabled);
        }

        private void Update(Framework framework)
        {
            if (Settings == null || ClientState.LocalPlayer == null) return;

            KeyboardHelper.Instance?.Update();

            KeyBind[] keybinds = Settings.GetKeybinds();
            foreach (KeyBind keybind in keybinds)
            {
                keybind.Update();
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

            _noteWindow.IsOpen = IsNoteWindowOpened();
            _windowSystem?.Draw();
        }

        private bool IsNoteWindowOpened()
        {
            if (Settings.Preview)
            {
                return true;
            }

            if (Settings.Hidden || Settings.HideInCombat && Plugin.Condition[ConditionFlag.InCombat])
            {
                return false;
            }

            return (_noteWindow.Note != null && !_forceHide);
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

            KeyboardHelper.Instance?.Dispose();

            _windowSystem.RemoveAllWindows();

            CommandManager.RemoveHandler("/noted");
            CommandManager.RemoveHandler("/noted toggle");

            Framework.Update -= Update;
            UiBuilder.Draw -= Draw;
            UiBuilder.OpenConfigUi -= OpenConfigUi;
        }
    }
}
