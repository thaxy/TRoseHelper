using System;
using TRoseHelper.Interaction.MemoryEditing;

namespace TRoseHelper.TRose
{
    public class Addresses
    {
        public class Player
        {
            public static Address Health = new Address((IntPtr)0x007C6710, new[] { 0x904 }, typeof(int));
            public static Address Action = new Address((IntPtr)0x007C6710, new[] { 0x2e }, typeof(int));
            public static Address Target = new Address((IntPtr)0x007A24EC, new[] { 0x14 }, typeof(int));
            public static Address PositionX = new Address((IntPtr)0x002822E8, new[] { 0x9c, 0x180 }, typeof(float), "znzin.dll");
            public static Address PositionY = new Address((IntPtr)0x002822E8, new[] { 0x9c, 0x184 }, typeof(float), "znzin.dll");
        }
        public class Creep
        {
        }
    }
}
