using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using FFXIVClientStructs.FFXIV.Component.GUI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NOTED.Helpers
{
    public static class ChatHelper
    {
        public static unsafe bool IsInputTextActive()
        {
            IntPtr ptr = *(IntPtr*)((IntPtr)AtkStage.GetSingleton() + 0x28) + 0x188E;
            return ptr != IntPtr.Zero && *(bool*)ptr;
        }

        public static unsafe void SendNoteTextToChat(string text)
        {
            try
            {
                string[] lines = text.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
                IntPtr ptr = Marshal.AllocHGlobal(Macro.kSize);
                Macro macro = new Macro(ptr, string.Empty, lines);
                Marshal.StructureToPtr(macro, ptr, false);

                RaptureShellModule.Instance->ExecuteMacro((FFXIVClientStructs.FFXIV.Client.UI.Misc.RaptureMacroModule.Macro*)ptr);

                Marshal.FreeHGlobal(ptr);
            }
            catch (Exception e)
            {
                PluginLog.Log(e.Message);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x688)]
    public readonly struct Macro : IDisposable
    {
        public const int kMaxLineCount = 15;
        public const int kUtf8StringSize = 0x68;
        public const int kSize = 0x688;

        public readonly uint IconId;
        public readonly uint Unk;
        public readonly MacroString Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = kMaxLineCount)]
        public readonly MacroString[] Lines;

        public unsafe Macro(IntPtr ptr, string name, IReadOnlyList<string> lines)
        {
            IconId = 66001;
            Unk = 1;
            Name = new MacroString(ptr + 0x8, name);
            Lines = new MacroString[kMaxLineCount];

            for (int i = 0; i < kMaxLineCount; i++)
            {
                string text = (lines.Count > i) ? lines[i] : string.Empty;
                Lines[i] = new MacroString(ptr + 0x8 + (MacroString.kSize * (i + 1)), text);
            }
        }

        public void Dispose()
        {
            Name.Dispose();

            for (int i = 0; i < kMaxLineCount; i++)
            {
                Lines[i].Dispose();
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 0x68)]
    public readonly struct MacroString : IDisposable
    {
        public const int kSize = 0x68;

        public readonly IntPtr StringPtr;
        public readonly ulong BufSize;
        public readonly ulong BufUsed;
        public readonly ulong StringLength;
        public readonly byte IsEmpty;
        public readonly byte IsUsingInlineBuffer;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x40)]
        public readonly byte[] InlineBuffer;

        public MacroString(IntPtr ptr, string text) : this(ptr, Encoding.UTF8.GetBytes(text)) { }

        public MacroString(IntPtr ptr, byte[] text)
        {
            BufSize = 0x40;
            BufUsed = (ulong)text.Length + 1;
            InlineBuffer = new byte[BufSize];

            if (BufUsed > BufSize)
            {
                StringPtr = Marshal.AllocHGlobal(text.Length + 1);
                BufSize = BufUsed;
                Marshal.Copy(text, 0, StringPtr, text.Length);
                Marshal.WriteByte(StringPtr, text.Length, 0);
                IsUsingInlineBuffer = 0;
            }
            else
            {
                StringPtr = ptr + 0x22;
                text.CopyTo(InlineBuffer, 0);
                IsUsingInlineBuffer = 1;
            }

            IsEmpty = (byte)((BufUsed == 1) ? 1 : 0);
            StringLength = 0;
        }

        public void Dispose()
        {
            if (IsUsingInlineBuffer == 0)
            {
                Marshal.FreeHGlobal(StringPtr);
            }
        }
    }
}
