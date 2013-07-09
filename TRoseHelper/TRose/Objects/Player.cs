using TRoseHelper.Interaction.MemoryEditing;

namespace TRoseHelper.TRose.Objects
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
            MemoryHandler.WriteMemory(Addresses.Player.Target, id);
        }
    }
}
