using System;

namespace TRoseHelper.Interaction.MemoryEditing
{
    public class Address
    {
        public IntPtr Pointer { get; set; }
        public int[] Offsets { get; set; }
        public Type Type { get; set; }
        public string ModuleName { get; set; }

        public Address(IntPtr pointer, int[] offsets, Type type, string moduleName = "")
        {
            Pointer = pointer;
            Offsets = offsets;
            Type = type;
            ModuleName = moduleName;
        }
    }
}
