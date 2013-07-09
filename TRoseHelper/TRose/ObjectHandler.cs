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
            player.Health = Convert.ToInt32(MemoryHandler.ReadMemory(AddressList.Player.Health));
            player.ActionId = Convert.ToInt32(MemoryHandler.ReadMemory(AddressList.Player.Action));
            player.TargetId = Convert.ToInt32(MemoryHandler.ReadMemory(AddressList.Player.Target));
            player.PositionX = Convert.ToSingle(MemoryHandler.ReadMemory(AddressList.Player.PositionX));
            player.PositionY = Convert.ToSingle(MemoryHandler.ReadMemory(AddressList.Player.PositionY));

            Player = player;
            return player;
        }
        public static List<Creep> UpdateCreeps()
        {
            List<Creep> creeps = new List<Creep>();

            foreach (byte[] memoryRegion in MemoryHandler.ScanMemory(7596148, 200))
            {
                byte[] mobId = new byte[2];
                byte[] mobHealth = new byte[4];
                byte[] mobMaximumHealth = new byte[4];
                byte[] mobPositionX = new byte[4];
                byte[] mobPositionY = new byte[4];
                byte[] mobLasttargetId = new byte[2];

                Array.Copy(memoryRegion, 0x18, mobId, 0, 2);
                Array.Copy(memoryRegion, 0x98, mobHealth, 0, 4);
                Array.Copy(memoryRegion, 0xa0, mobMaximumHealth, 0, 4);
                Array.Copy(memoryRegion, 0x0c, mobPositionX, 0, 4);
                Array.Copy(memoryRegion, 0x10, mobPositionY, 0, 4);
                Array.Copy(memoryRegion, 0x54, mobLasttargetId, 0, 2);

                creeps.Add(new Creep
                {
                    Id = BitConverter.ToInt16(mobId, 0),
                    Health = BitConverter.ToInt32(mobHealth, 0),
                    MaximumHealth = BitConverter.ToInt32(mobMaximumHealth, 0),
                    PositionX = BitConverter.ToSingle(mobPositionX, 0) / 100,
                    PositionY = BitConverter.ToSingle(mobPositionY, 0) / 100,
                    LastTargetId = BitConverter.ToInt16(mobLasttargetId, 0)
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
