using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TRoseHelper
{
    public class Player
    {
        public int Health { get; set; }
        public int ActionId { get; set; }
        public int TargetId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }

        public void SetTarget(int id)
        {            
            MemoryHandler.WriteMemory((IntPtr)0x007A24EC, new int[] { 0x14 }, id);            
        }
    }    
}
