using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NOTED.Helpers;
using NOTED.Models;
using System;
using System.Numerics;

namespace NOTED.Windows
{
    public class JobsDataWindow : Window
    {
        private float _scale => ImGuiHelpers.GlobalScale;
        private Settings Settings => Plugin.Settings;

        public Note? Note = null;

        private bool _changed = false;


        public JobsDataWindow(string name) : base(name)
        {
            Flags = ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoScrollWithMouse;

            Size = new Vector2(710, 360);
        }

        public override void OnOpen()
        {
            _changed = false;
        }

        public override void OnClose()
        {
            if (_changed)
            {
                Settings.Save(Settings);
            }
        }

        public override void Draw()
        {
            if (Note == null) { return; }

            JobsData data = Note.Jobs;
            Vector2 cursorPos = ImGui.GetCursorPos() + new Vector2(14 * _scale);
            Vector2 originalPos = cursorPos;
            float maxY = 0;

            JobRoles[] roles = (JobRoles[])Enum.GetValues(typeof(JobRoles));

            foreach (JobRoles role in roles)
            {
                if (role == JobRoles.Unknown) { continue; }
                if (!data.Map.ContainsKey(role)) { continue; }

                bool roleValue = data.GetRoleEnabled(role);
                string roleName = JobsHelper.RoleNames[role];

                ImGui.SetCursorPos(cursorPos);
                if (ImGui.Checkbox(roleName, ref roleValue))
                {
                    data.SetRoleEnabled(role, roleValue);
                    _changed = true;
                }

                cursorPos.Y += 40 * _scale;
                int jobCount = data.Map[role].Count;

                for (int i = 0; i < jobCount; i++)
                {
                    maxY = Math.Max(cursorPos.Y, maxY);
                    uint jobId = JobsHelper.JobsByRole[role][i];
                    bool jobValue = data.Map[role][i];
                    string jobName = JobsHelper.JobNames[jobId];

                    ImGui.SetCursorPos(cursorPos);
                    if (ImGui.Checkbox(jobName, ref jobValue))
                    {
                        data.SetJobEnabled(role, i, jobValue);
                        _changed = true;
                    }

                    cursorPos.Y += 30 * _scale;
                }

                cursorPos.X += 100 * _scale;
                cursorPos.Y = originalPos.Y;
            }

            ImGui.SetCursorPos(new Vector2(originalPos.X, maxY + 30));
        }
    }
}
