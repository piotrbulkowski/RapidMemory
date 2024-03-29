﻿using RapidMemory.Definitions;
using RapidMemory.SignatureScanner;
using RapidMemory.SignatureScanner.Definitions;

namespace RapidMemory.Scanner.UnitTests.SignatureScanner;

public sealed class SignatureScannerEngineTestWrapper : SignatureScannerEngine
{
    public static ReadOnlySpan<ushort> BuildFullMatchTableWrapped(in BytePattern pattern)
    {
        return BuildFullMatchTable(pattern);
    }

    public unsafe PatternScanResult FindPatternSimpleWrapped(byte* data, int dataLength, BytePattern pattern)
    {
        return FindPatternSimple(data, dataLength, pattern);
    }

    public override unsafe PatternScanResult FindPattern(byte* data, int dataLength, BytePattern pattern)
    {
        throw new NotImplementedException();
    }
}