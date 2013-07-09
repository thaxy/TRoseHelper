using System;
using System.Runtime.InteropServices;
using TRoseHelper.Interaction.MemoryEditing;

namespace TRoseHelper.Interaction
{
    public class KeyHandler
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        public enum VirtualKeys
        {
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B
        }

        public static void SendKey(VirtualKeys key)
        {
            SendMessage(MemoryHandler.Process.MainWindowHandle, 260, (uint)key, 0);
        }
    }
}
