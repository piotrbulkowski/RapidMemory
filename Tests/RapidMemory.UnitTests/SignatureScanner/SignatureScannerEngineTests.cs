using RapidMemory.Definitions;
using Xunit;

namespace RapidMemory.Scanner.UnitTests.SignatureScanner;

public sealed class SignatureScannerEngineTests
{
    [Fact]
    public void BuildFullMatchTable_Should_ReturnValidTable()
    {
        var signatureScanner = new SignatureScannerEngineTestWrapper();
        var bytePattern = new BytePattern("50 6A 0F 6A 00 FF 35", 0x7);
        ushort[] expectedResults = {0, 1, 2, 3, 4, 5};
        var expectedResultSpan = new ReadOnlySpan<ushort>(expectedResults);

        var result = SignatureScannerEngineTestWrapper.BuildFullMatchTableWrapped(bytePattern);
        
        Assert.False(result.IsEmpty);
        Assert.True(result.Length == 6);
        var isSpanEqual = expectedResultSpan.SequenceEqual(result);
        Assert.True(isSpanEqual);
    }
}