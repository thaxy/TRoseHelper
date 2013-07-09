using System;

namespace TRoseHelper.Interaction.MemoryEditing
{
    public class MemoryPage
    {
        public IntPtr From { get; set; }
        public uint To { get; set; }
        public byte[] Content { get; set; }

        public MemoryPage(IntPtr from, uint to, byte[] content)
        {
            From = from;
            To = to;
            Content = content;
        }
    }
}
