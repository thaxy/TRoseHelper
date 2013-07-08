using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TRoseHelper
{
    public static class ObjectHandler
    {
        public static Player Player { get; set; }
        public static List<Creep> Creeps { get; set; }

        public static Player UpdatePlayer()
        {
            Player player = new Player();

            player.Health = Convert.ToInt32(MemoryHandler.ReadMemory((IntPtr)0x007C6710, new[] { 0x904 }, typeof(int)));
            player.ActionId = Convert.ToInt32(MemoryHandler.ReadMemory((IntPtr)0x007C6710, new[] { 0x2e }, typeof(int)));
            player.TargetId = Convert.ToInt32(MemoryHandler.ReadMemory((IntPtr)0x007A24EC, new int[] { 0x14 }, typeof(int)));
            player.PositionX = Convert.ToSingle(MemoryHandler.ReadMemory((IntPtr)0x002822E8, new int[] { 0x9c, 0x180 }, typeof(float), "znzin.dll"));
            player.PositionY = Convert.ToSingle(MemoryHandler.ReadMemory((IntPtr)0x002822E8, new int[] { 0x9c, 0x184 }, typeof(float), "znzin.dll"));

            Player = player;
            return player;
        }
        public static List<Creep> UpdateCreeps()
        {
            List<Creep> creeps = new List<Creep>();

            foreach (byte[] memoryRegion in MemoryHandler.ScanMemory(7596148, 200))
            {
                byte[] mobId = new byte[4];
                byte[] mobHealth = new byte[4];
                byte[] mobMaximumHealth = new byte[4];
                byte[] mobPositionX = new byte[4];
                byte[] mobPositionY = new byte[4];

                Array.Copy(memoryRegion, 0x18, mobId, 0, 2);
                Array.Copy(memoryRegion, 0x98, mobHealth, 0, 4);
                Array.Copy(memoryRegion, 0xA0, mobMaximumHealth, 0, 4);
                Array.Copy(memoryRegion, 0x80, mobPositionX, 0, 4);
                Array.Copy(memoryRegion, 0x84, mobPositionY, 0, 4);

                creeps.Add(new Creep
                {
                    Id = BitConverter.ToInt32(mobId, 0),
                    Health = BitConverter.ToInt32(mobHealth, 0),
                    MaximumHealth = BitConverter.ToInt32(mobMaximumHealth, 0),
                    PositionX = BitConverter.ToSingle(mobPositionX, 0) / 100,
                    PositionY = BitConverter.ToSingle(mobPositionY, 0) / 100
                });
            }

            Creeps = creeps;
            return creeps;
        }

        public static Creep GetCreepById(int id)
        {
            return Creeps.FirstOrDefault(creep => creep.Id == id);
        }
    }
}
