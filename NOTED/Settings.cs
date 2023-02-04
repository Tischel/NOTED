using Dalamud.Logging;
using Newtonsoft.Json;
using NOTED.Helpers;
using NOTED.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Numerics;

namespace NOTED
{
    public class Settings
    {
        public Dictionary<uint, Duty> Duties = new Dictionary<uint, Duty>();

        public bool Hidden = false;
        public bool HideInCombat = false;
        public bool Locked = false;
        public bool Preview = false;
        public bool LeftClickToCopy = true;
        public bool RightClickToEdit = true;
        public bool ShiftLeftClickToSend = true;
        public Vector4 LockedBackgroundColor = new Vector4(0f, 0f, 0f, 0.25f);
        public Vector4 UnlockedBackgroundColor = new Vector4(0f, 0f, 0f, 0.75f);

        public bool KeybindPassthrough = false;
        public KeyBind HideKeybind = new KeyBind((int)Keys.H, true, false, true);
        public KeyBind NextNoteKeybind = new KeyBind((int)Keys.Right, true, false, true);
        public KeyBind PreviousNoteKeybind = new KeyBind((int)Keys.Left, true, false, true);

        public KeyBind[] GetKeybinds()
        {
            KeyBind[] keybinds = { HideKeybind, NextNoteKeybind, PreviousNoteKeybind };
            return keybinds;
        }

        public void ValidateKeyBind(KeyBind prioritizedKeybind)
        {
            KeyBind[] keybinds = GetKeybinds();

            foreach (KeyBind keybind in keybinds)
            {
                if (prioritizedKeybind != keybind && prioritizedKeybind.Equals(keybind))
                {
                    keybind.Reset();
                }
            }
        }

        #region load / save
        private static string JsonPath = Path.Combine(Plugin.PluginInterface.GetPluginConfigDirectory(), "Settings.json");
        public static Settings Load()
        {
            string path = JsonPath;
            Settings? settings = null;

            try
            {
                if (File.Exists(path))
                {
                    string jsonString = File.ReadAllText(path);
                    settings = JsonConvert.DeserializeObject<Settings>(jsonString);
                }
            }
            catch (Exception e)
            {
                PluginLog.Error("Error reading settings file: " + e.Message);
            }

            if (settings == null)
            {
                settings = new Settings();
                Save(settings);
            }
            else
            {
                foreach (Duty duty in settings.Duties.Values)
                {
                    foreach (Note note in duty.Notes)
                    {
                        note.Jobs.Update();
                    }
                }
            }

            return settings;
        }

        public static void Save(Settings settings)
        {
            try
            {
                JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    TypeNameHandling = TypeNameHandling.Objects
                };
                string jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented, serializerSettings);

                File.WriteAllText(JsonPath, jsonString);
            }
            catch (Exception e)
            {
                PluginLog.Error("Error saving settings file: " + e.Message);
            }
        }
        #endregion
    }
}
