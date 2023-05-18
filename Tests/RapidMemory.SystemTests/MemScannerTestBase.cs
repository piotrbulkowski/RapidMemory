using System;
using System.Diagnostics;
using System.Linq;

namespace RapidMemory.Scanner.SystemTests;

public abstract class MemScannerTestBase : IDisposable
{
    protected readonly Process DestinationProcess;

    protected MemScannerTestBase()
    {
        const string ProcessName = "RapidMemory.SystemTests.ExternalApp";

        var systemProcesses = Process.GetProcesses();
        DestinationProcess = systemProcesses.FirstOrDefault(p => p.ProcessName.Equals(ProcessName));
        if (DestinationProcess is null)
        {
            StartTestableApplication();
        }
    }


    private void StartTestableApplication()
    {

    }

    public void Dispose()
    {
        DestinationProcess?.Dispose();
        GC.SuppressFinalize(this);
    }
}