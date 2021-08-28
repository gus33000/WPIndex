using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WPIndex
{
    public class Delta
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DELTA_INPUT
        {
            public IntPtr lpcStart;
            public UIntPtr uSize;
            [MarshalAs(UnmanagedType.Bool)]
            public bool Editable;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DELTA_OUTPUT
        {
            public IntPtr lpcStart;
            public UIntPtr uSize;
        }

        [Flags]
        private enum DELTA_FLAG_TYPE : long
        {
            None = 0,
            AllowPa19 = 1
        }

        [DllImport("msdelta.dll", SetLastError = true)]
        private static extern bool ApplyDeltaB(DELTA_FLAG_TYPE ApplyFlags, DELTA_INPUT Source, DELTA_INPUT Delta, out DELTA_OUTPUT lpTarget);

        [DllImport("msdelta.dll", SetLastError = true)]
        private static extern bool DeltaFree(IntPtr lpMemory);

        public static byte[] ApplyDelta(byte[] Orig, byte[] Delta)
        {
            DELTA_INPUT hOrig = new DELTA_INPUT() { lpcStart = IntPtr.Zero, uSize = new UIntPtr((uint)Orig.Length), Editable = true },
                hDelta = new DELTA_INPUT() { lpcStart = IntPtr.Zero, uSize = new UIntPtr((uint)Delta.Length), Editable = true };
            DELTA_OUTPUT hResult = new DELTA_OUTPUT() { lpcStart = IntPtr.Zero, uSize = UIntPtr.Zero };

            try
            {
                hOrig.lpcStart = Marshal.AllocHGlobal(Orig.Length);
                Marshal.Copy(Orig, 0, hOrig.lpcStart, Orig.Length);

                hDelta.lpcStart = Marshal.AllocHGlobal(Delta.Length);
                Marshal.Copy(Delta, 0, hDelta.lpcStart, Delta.Length);

                if (!ApplyDeltaB(DELTA_FLAG_TYPE.AllowPa19, hOrig, hDelta, out hResult))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                byte[] Result = new byte[hResult.uSize.ToUInt32()];
                Marshal.Copy(hResult.lpcStart, Result, 0, Result.Length);
                return Result;
            }
            finally
            {
                if (hOrig.lpcStart != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hOrig.lpcStart);
                }

                if (hDelta.lpcStart != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hDelta.lpcStart);
                }

                if (hResult.lpcStart != IntPtr.Zero)
                {
                    DeltaFree(hResult.lpcStart);
                }
            }
        }
    }
}