using System;
using System.Collections.Generic;
using RapidMemory.Definitions;
using RapidMemory.Native.Enums;
using RapidMemory.SignatureScanner.Definitions;

namespace RapidMemory;

public interface IMemScanner : IDisposable
{
    void Read<T>(nuint memoryAddress, out T value) where T : unmanaged;
    void ReadBytes(nint memoryAddress, out byte[] value, int length);
    void Write<T>(nuint memoryAddress, ref T item) where T : unmanaged;
    IEnumerable<PatternScanResult> FindPatterns(IReadOnlyList<BytePattern> bytePatterns);
    PatternScanResult FindPattern(BytePattern bytePattern);
    MemPageProtection ChangePageProtection(nuint memoryAddress, long size, MemPageProtection newProtection);
}