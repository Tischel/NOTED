using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Bindings.ImGui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NOTED.Helpers
{
    internal static class DrawHelper
    {
        public static void SetTooltip(string message)
        {
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip(message);
            }
        }

        public static void Tab(float count = 1)
        {
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 20 * count * ImGuiHelpers.GlobalScale);
        }

        public static (bool, bool) DrawConfirmationModal(string title, IEnumerable<string> textLines)
        {
            bool didConfirm = false;
            bool didClose = false;

            ImGui.OpenPopup(title + " ##NOTED");

            Vector2 center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));

            bool p_open = true; // i've no idea what this is used for

            if (ImGui.BeginPopupModal(title + " ##NOTED", ref p_open, ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove))
            {
                float width = 300;
                float height = Math.Min((ImGui.CalcTextSize(" ").Y + 5) * textLines.Count(), 240);

                ImGui.BeginChild("confirmation_modal_message", new Vector2(width, height), false);
                foreach (string text in textLines)
                {
                    ImGui.Text(text);
                }
                ImGui.EndChild();

                ImGui.NewLine();

                if (ImGui.Button("OK", new Vector2(width / 2f - 5, 24)))
                {
                    ImGui.CloseCurrentPopup();
                    didConfirm = true;
                    didClose = true;
                }

                ImGui.SetItemDefaultFocus();
                ImGui.SameLine();
                if (ImGui.Button("Cancel", new Vector2(width / 2f - 5, 24)))
                {
                    ImGui.CloseCurrentPopup();
                    didClose = true;
                }

                ImGui.EndPopup();
            }
            // close button on nav
            else
            {
                didClose = true;
            }

            return (didConfirm, didClose);
        }
    }
}
