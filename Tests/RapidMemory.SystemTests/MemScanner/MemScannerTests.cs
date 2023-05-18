using System;
using Xunit;

namespace RapidMemory.Scanner.SystemTests.MemScanner
{
    public sealed class MemScannerTests : MemScannerTestBase
    {
        [Fact]
        public void Given_ValidProcess_Ctor_ShouldCreateMemScanner()
        {
            var memScanner = new RapidMemory.MemScanner(DestinationProcess);
            
            Assert.NotNull(memScanner);
        }
        //[Fact]
        public void Given_ValidPointers_FindPattern_ShouldReturnValidAdresses()
        {
            throw new NotImplementedException();
            var memScanner = new RapidMemory.MemScanner(DestinationProcess);
            
            //var basePtr = memScanner.FindPattern(AobPatterns.ScanBasePtr);
            //var agentBasePtr = memScanner.FindPattern(AobPatterns.ScanAgentBasePtr);
            //var myIdPtr = memScanner.FindPattern(AobPatterns.PlayerAgentIdPtr);
            
            Assert.NotNull(memScanner);
        }
    }
}
