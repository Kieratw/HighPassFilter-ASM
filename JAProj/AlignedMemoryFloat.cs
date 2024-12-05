using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public unsafe class AlignedMemoryFloat : IDisposable
{
    public IntPtr UnmanagedPointer { get; private set; }
    public float* AlignedPointer { get; private set; }
    public int Length { get; private set; }

    public AlignedMemoryFloat(int length)
    {
        Length = length;
        int byteSize = length * sizeof(float);
        IntPtr ptr = Marshal.AllocHGlobal(byteSize + 31);

        long alignedAddress = ((long)ptr + 31) & ~31;

        UnmanagedPointer = ptr;
        AlignedPointer = (float*)alignedAddress;

        // Opcjonalnie wyzeruj pamięć
        Unsafe.InitBlockUnaligned(AlignedPointer, 0, (uint)byteSize);
    }

    public void Dispose()
    {
        if (UnmanagedPointer != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(UnmanagedPointer);
            UnmanagedPointer = IntPtr.Zero;
        }
    }
}