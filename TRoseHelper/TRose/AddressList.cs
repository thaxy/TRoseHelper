using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TRoseHelper
{
    public static class AddressList
    {
        public static class Player
        {
            public static Address Health = new Address((IntPtr)0x007C6710, new int[] { 0x904 }, typeof(int));
            public static Address Action = new Address((IntPtr)0x007C6710, new int[] { 0x2e }, typeof(int));
            public static Address Target = new Address((IntPtr)0x007A24EC, new int[] { 0x14 }, typeof(int));
            public static Address PositionX = new Address((IntPtr)0x002822E8, new int[] { 0x9c, 0x180 }, typeof(float), "znzin.dll");
            public static Address PositionY = new Address((IntPtr)0x002822E8, new int[] { 0x9c, 0x184 }, typeof(float), "znzin.dll");
        }
        public static class Creep
        {
        }
    }
}
