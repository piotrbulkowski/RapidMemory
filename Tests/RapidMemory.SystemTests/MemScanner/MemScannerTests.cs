using System;
using Xunit;

namespace RapidMemory.Scanner.SystemTests.MemScanner
{
    public sealed class MemScannerTests : MemScannerTestBase
    {
        [Fact]
        public void Given_ValidGameProcess_Ctor_ShouldCreateMemScanner()
        {
            var memScanner = new RapidMemory.MemScanner(GameProcess);
            
            Assert.NotNull(memScanner);
        }
        //[Fact]
        public void Given_ValidPointers_FindPattern_ShouldReturnValidAdresses()
        {
            throw new NotImplementedException();
            var memScanner = new RapidMemory.MemScanner(GameProcess);
            
            //var basePtr = memScanner.FindPattern(AobPatterns.ScanBasePtr);
            //var agentBasePtr = memScanner.FindPattern(AobPatterns.ScanAgentBasePtr);
            //var myIdPtr = memScanner.FindPattern(AobPatterns.PlayerAgentIdPtr);
            
            Assert.NotNull(memScanner);
        }
    }
}
