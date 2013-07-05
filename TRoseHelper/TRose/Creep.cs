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

        public double GetDistance(Player player)
        {
            double a = player.PositionX - PositionX;
            double b = player.PositionY - PositionY;
            return Math.Sqrt((a * a) + (b * b));
        }

        public override string ToString()
        {
            return "Id: " + Id + "\r\nHP: " + Health + "/" + MaximumHealth + "\r\nX: " + PositionX + "\r\nY: " + PositionY;
        }
    }
}
