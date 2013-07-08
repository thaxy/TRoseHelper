using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;

namespace TRoseHelper
{
    public static class MemoryHandler
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }
        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
        [DllImport("kernel32.dll")]
        static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

        public static Process Process { get; set; }

        public static List<byte[]> ScanMemory(int search, int bytesToRead)
        {
            List<byte[]> results = new List<byte[]>();
            foreach (MemoryPage memoryPage in ReadMemoryPages())
            {
                foreach (int position in IndexOfSequence(memoryPage.Content, BitConverter.GetBytes(search)))
                {
                    byte[] result = new byte[bytesToRead];
                    Buffer.BlockCopy(memoryPage.Content, position, result, 0, bytesToRead);
                    results.Add(result);
                }
            }
            return results;
        }
        public static byte[] ReadMemory(IntPtr address, uint bytesToRead)
        {
            byte[] buffer = new byte[bytesToRead];
            IntPtr lpNumberOfBytesRead;
            ReadProcessMemory(Process.Handle, address, buffer, bytesToRead, out lpNumberOfBytesRead);
            return buffer;
        }
        public static object ReadMemory(Address address)
        {
            IntPtr pointer = address.Pointer;
            ProcessModule processModule = GetProcessModuleByName(address.ModuleName);
            if (processModule != null)
            {
                pointer = (IntPtr)((uint)processModule.BaseAddress + (uint)address.Pointer);
            }
            byte[] pointerBytes = ReadMemory(pointer, 4);
            foreach (int offset in address.Offsets)
            {
                pointerBytes = ReadMemory((IntPtr)BitConverter.ToInt32(pointerBytes, 0) + offset, 4);
            }

            object result;
            if (address.Type == typeof(float))
            {
                result = BitConverter.ToSingle(pointerBytes, 0);
            }
            else
            {
                result = BitConverter.ToInt32(pointerBytes, 0);
            }
            return result;
        }
        public static void WriteMemory(Address address, int value)
        {
            byte[] byteValue = BitConverter.GetBytes(value);
            IntPtr pointer = address.Pointer;
            ProcessModule processModule = GetProcessModuleByName(address.ModuleName);
            if (processModule != null)
            {
                pointer = (IntPtr)((uint)processModule.BaseAddress + (uint)address.Pointer);
            }
            byte[] pointerBytes = ReadMemory(pointer, 4);
            for (int i = 0; i < address.Offsets.Length; i++) //not sure if offset.length > 1 works but should xD
            {
                int location = BitConverter.ToInt32(pointerBytes, 0) + address.Offsets[i];
                if (i == address.Offsets.Length - 1)
                {
                    WriteProcessMemory(Process.Handle, location, byteValue, byteValue.Length, 0);
                }
                else
                {
                    pointerBytes = ReadMemory((IntPtr)location, 4);
                }
            }
        }

        private static List<MemoryPage> ReadMemoryPages()
        {
            List<MemoryPage> memoryPages = new List<MemoryPage>();

            long startAddress = 0;
            long stopAddress = 0x7fffffff;
            int total = 0;
            do
            {
                MEMORY_BASIC_INFORMATION m;
                VirtualQueryEx(Process.Handle, (IntPtr)startAddress, out m, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (m.State == 0x1000 && m.Protect == 0x04 && m.Type == 0x20000) // MEM_COMMIT & PAGE_READWRITE & MEM_PRIVATE
                {
                    IntPtr baseAddress = m.BaseAddress;
                    uint to = (uint)m.BaseAddress + (uint)m.RegionSize;
                    byte[] content = ReadMemory(baseAddress, (uint)m.RegionSize);
                    memoryPages.Add(new MemoryPage { From = baseAddress, To = to, Content = content });
                    total += (int)m.RegionSize;
                }
                startAddress += (uint)m.RegionSize;
            } while (startAddress <= stopAddress);

            return memoryPages;
        }
        private static ProcessModule GetProcessModuleByName(string name)
        {
            ProcessModule found = null;
            foreach (ProcessModule module in Process.Modules)
            {
                if (module.ModuleName == name)
                {
                    found = module;
                    break;
                }
            }
            return found;
        }
        private static List<int> IndexOfSequence(byte[] buffer, byte[] pattern)
        {
            List<int> positions = new List<int>();
            int i = Array.IndexOf<byte>(buffer, pattern[0], 0);
            while (i >= 0 && i <= buffer.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual<byte>(pattern))
                    positions.Add(i);
                i = Array.IndexOf<byte>(buffer, pattern[0], i + pattern.Length);
            }
            return positions;
        }
    }
}
