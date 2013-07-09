using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TRoseHelper
{
    public class Creep
    {
        public int Id { get; set; }
        public int Health { get; set; }
        public int MaximumHealth { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public int LastTargetId { get; set; }

        public double GetDistance()
        {
            double a = ObjectHandler.Player.PositionX - PositionX;
            double b = ObjectHandler.Player.PositionY - PositionY;
            return Math.Sqrt((a * a) + (b * b));
        }

        public override string ToString()
        {
            return "Id: " + Id +
                    "\r\nHP: " + Health + "/" + MaximumHealth +
                    "\r\nX: " + PositionX +
                    "\r\nY: " + PositionY;
        }
    }
}
