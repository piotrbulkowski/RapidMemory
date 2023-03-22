using RapidMemory.Main.Domain;
using Xunit;

namespace RapidMemory.Scanner.SystemTests.MemScanner
{
    public sealed class MemScannerMemScannerTests : MemScannerTestBase
    {
        //[Fact]
        public void Given_ValidGameProcess_Ctor_ShouldCreateMemScanner()
        {
            var memScanner = new Scanner.MemScanner(GameProcess);
            
            Assert.NotNull(memScanner);
        }
        [Fact]
        public void Given_ValidPointers_AobScan_ShouldReturnValidAdresses()
        {
            var memScanner = new Scanner.MemScanner(GameProcess);
            
            var basePtr = memScanner.FindPattern(AobPatterns.ScanBasePtr);
            var agentBasePtr = memScanner.FindPattern(AobPatterns.ScanAgentBasePtr);
            // maxAgents = agentBasePtr + 0x8
            var myIdPtr = memScanner.FindPattern(AobPatterns.PlayerAgentIdPtr);
            
            Assert.NotNull(memScanner);
        }
    }
}