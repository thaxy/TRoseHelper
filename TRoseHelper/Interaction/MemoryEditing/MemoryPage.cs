using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TRoseHelper
{
    public class MemoryPage
    {
        public IntPtr From { get; set; }
        public uint To { get; set; }
        public byte[] Content { get; set; }
    }
}
