using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using NOTED.Helpers;
using NOTED.Models;
using System.Numerics;

namespace NOTED.Windows
{
    internal class NoteWindow : Window
    {
        private float _scale => ImGuiHelpers.GlobalScale;
        private Settings Settings => Plugin.Settings;

        public Note? Note = null;
        private ImGuiWindowFlags _baseFlags = ImGuiWindowFlags.NoScrollbar
                                            | ImGuiWindowFlags.NoCollapse
                                            | ImGuiWindowFlags.NoTitleBar
                                            | ImGuiWindowFlags.NoNav
                                            | ImGuiWindowFlags.NoScrollWithMouse;

        private static string PreviewText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\nFermentum iaculis eu non diam phasellus vestibulum lorem sed.\nSapien eget mi proin sed libero enim sed faucibus turpis.\nArcu bibendum at varius vel pharetra vel.\nScelerisque eleifend donec pretium vulputate sapien nec sagittis aliquam malesuada.\nAliquam malesuada bibendum arcu vitae elementum curabitur vitae.\nPotenti nullam ac tortor vitae purus faucibus ornare suspendisse.\nAenean sed adipiscing diam donec adipiscing tristique risus.\nAliquam sem fringilla ut morbi tincidunt augue interdum.\nRhoncus mattis rhoncus urna neque viverra justo nec ultrices.\nSagittis nisl rhoncus mattis rhoncus urna neque viverra justo.\nTortor at risus viverra adipiscing.\nHabitasse platea dictumst vestibulum rhoncus est pellentesque.\nPharetra vel turpis nunc eget lorem dolor sed viverra ipsum.\nDonec pretium vulputate sapien nec sagittis aliquam malesuada.\nDonec pretium vulputate sapien nec sagittis aliquam malesuada bibendum.\nSit amet facilisis magna etiam tempor orci.\nVitae turpis massa sed elementum tempus egestas sed sed.\nEu tincidunt tortor aliquam nulla facilisi cras fermentum odio.\nEtiam tempor orci eu lobortis elementum.";

        public NoteWindow(string name) : base(name)
        {
            Flags = _baseFlags;
            Size = new Vector2(400, 400);
            SizeCondition = ImGuiCond.FirstUseEver;
        }

        public override void PreDraw()
        {
            Vector4 bgColor = Settings.Locked ? Settings.LockedBackgroundColor : Settings.UnlockedBackgroundColor;
            ImGui.PushStyleColor(ImGuiCol.WindowBg, bgColor);

            Flags = _baseFlags;

            if (Settings.Locked)
            {
                Flags |= ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            }
        }

        public override void PostDraw()
        {
            ImGui.PopStyleColor();
        }

        public override void Draw()
        {
            string? text = Settings.Preview ? PreviewText : Note?.Text;
            if (text == null) { return; }

            ImGui.PushTextWrapPos(ImGui.GetWindowWidth());
            ImGui.TextWrapped(text);
            ImGui.PopTextWrapPos();

            if (ImGui.IsWindowHovered())
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    if (Settings.ShiftLeftClickToSend && ImGui.IsKeyDown(ImGuiKey.LeftShift))
                    {
                        ChatHelper.SendNoteTextToChat(text);
                    }

                    if (Settings.LeftClickToCopy)
                    {
                        if (!Settings.ShiftLeftClickToSend || !ImGui.IsKeyDown(ImGuiKey.LeftShift))
                        {
                            ImGui.SetClipboardText(text);
                        }
                    }
                }

                if (Settings.RightClickToEdit && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    Plugin.EditNote(Note);
                }
            }
        }
    }
}
