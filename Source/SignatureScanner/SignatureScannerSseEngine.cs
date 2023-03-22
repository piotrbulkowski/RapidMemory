using RapidMemory.Definitions;
using RapidMemory.SignatureScanner.Definitions;

namespace RapidMemory.SignatureScanner
{
    internal unsafe class SignatureScannerSseEngine : SignatureScannerEngine
    {
        public override PatternScanResult FindPattern(byte* data, int dataLength, BytePattern pattern)
        {
            throw new System.NotImplementedException();
        }
    }
}
