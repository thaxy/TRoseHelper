using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TRoseHelper.Interaction.MemoryEditing
{
    public class MemoryHandler
    {
        #region Win32
        [StructLayout(LayoutKind.Sequential)]
        struct MEMORY_BASIC_INFORMATION
        {
            public readonly IntPtr BaseAddress;
            private readonly IntPtr AllocationBase;
            private readonly uint AllocationProtect;
            internal readonly IntPtr RegionSize;
            public readonly uint State;
            public readonly uint Protect;
            public readonly uint Type;
        }
        [DllImport("kernel32.dll")]
        private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);
        [DllImport("kernel32.dll")]
        private static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] lpBuffer, UInt32 nSize, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);
        #endregion
        public static Process Process { get; set; }

        public static bool IsReady(string processName)
        {
            bool isReady = true;
            if (Process == null || Process.HasExited || Process.ProcessName != processName)
            {
                isReady = false;
                Process = Process.GetProcessesByName(processName).FirstOrDefault();
            }
            return isReady;
        }
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
                pointerBytes = ReadMemory((IntPtr)BitConverter.ToInt32(pointerBytes, 0) + offset, (uint)Marshal.SizeOf(address.Type));
            }

            object result; //TODO: Clean this shit up
            if (address.Type == typeof(float))
            {
                result = BitConverter.ToSingle(pointerBytes, 0);
            }
            else if (address.Type == typeof (short))
            {
                result = BitConverter.ToUInt16(pointerBytes, 0);
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

        private static ProcessModule GetProcessModuleByName(string name)
        {
            return Process.Modules.Cast<ProcessModule>().FirstOrDefault(module => module.ModuleName == name);
        }
        private static IEnumerable<MemoryPage> ReadMemoryPages(long startAddress = 0, long stopAddress = 0x7fffffff)
        {
            List<MemoryPage> memoryPages = new List<MemoryPage>();
            do
            {
                MEMORY_BASIC_INFORMATION m;
                VirtualQueryEx(Process.Handle, (IntPtr)startAddress, out m, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (m.State == 0x1000 && m.Protect == 0x04 && m.Type == 0x20000) // MEM_COMMIT && PAGE_READWRITE && MEM_PRIVATE
                {
                    IntPtr baseAddress = m.BaseAddress;
                    uint to = (uint)baseAddress + (uint)m.RegionSize;
                    byte[] content = ReadMemory(baseAddress, (uint)m.RegionSize);
                    memoryPages.Add(new MemoryPage(baseAddress, to, content));
                }
                startAddress += (uint)m.RegionSize;
            } while (startAddress <= stopAddress);
            return memoryPages;
        }
        private static IEnumerable<int> IndexOfSequence(byte[] buffer, byte[] pattern)
        {
            List<int> positions = new List<int>();
            int i = Array.IndexOf(buffer, pattern[0], 0);
            while (i >= 0 && i <= buffer.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual(pattern))
                {
                    positions.Add(i);
                }
                i = Array.IndexOf(buffer, pattern[0], i + pattern.Length);
            }
            return positions;
        }
    }
}
