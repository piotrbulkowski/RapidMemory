using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using RapidMemory.Definitions;
using RapidMemory.Exceptions;
using RapidMemory.Native;
using RapidMemory.Native.Enums;
using RapidMemory.SignatureScanner;
using RapidMemory.SignatureScanner.Definitions;

namespace RapidMemory
{
    public unsafe class MemScanner : IMemScanner
    {
        private readonly Process _gameProcess;
        private readonly SignatureScannerEngine _scannerEngine;
        private byte* _moduleDataPtr;
        private int _moduleMemorySize;
        private GCHandle? _gcHandle;
        
        private bool _isDisposed;

        public MemScanner(Process gameProcess, ProcessModule? targetModule = null)
        {
            if (gameProcess.Handle == IntPtr.Zero)
            {
                throw new InvalidProcessHandleException(gameProcess.Handle.ToString(), gameProcess.HasExited);
            }
            _gameProcess = gameProcess;

            AssignModuleData((targetModule ?? gameProcess.MainModule)!);
            if (Avx2.IsSupported)
            {
                _scannerEngine = new SignatureScannerAvxEngine();
                return;
            }
            if (Sse2.IsSupported)
            {
                _scannerEngine = new SignatureScannerSseEngine();
                return;
            }
            _scannerEngine = new SignatureScannerCompiledEngine();
        }

        private void AssignModuleData(ProcessModule module)
        {
            ReadBytes(module.BaseAddress, out var gameBaseModuleData, module.ModuleMemorySize);
            
            // pin gameBaseModuleData so GC won't corrupt it
            _gcHandle = GCHandle.Alloc(gameBaseModuleData, GCHandleType.Pinned);
            _moduleDataPtr = (byte*)_gcHandle.Value.AddrOfPinnedObject();
            _moduleMemorySize = gameBaseModuleData.Length;
            
            if (module is null || gameBaseModuleData.LongLength <= 0)
            {
                throw new InvalidProcessModuleException(_gameProcess.Id, gameBaseModuleData.LongLength);
            }
        }

        public void Read<T>(nuint memoryAddress, out T value) where T : unmanaged
        {
            var structSize = Unsafe.SizeOf<T>();
            var buffer = GC.AllocateUninitializedArray<byte>(structSize, false);

            fixed (byte* bufferPtr = buffer)
            {
                var isSuccess = Imports.ReadProcessMemory(_gameProcess.Handle, memoryAddress, (nuint)bufferPtr, (nuint)structSize, out _);
                if (!isSuccess)
                {
                    throw new MemoryOperationException($"ReadProcessMemory failed to read from {memoryAddress}, bytes: {structSize}");
                }
                value = Unsafe.Read<T>(bufferPtr);
            }
        }

        public void ReadBytes(nint memoryAddress, out byte[] value, int length)
        {
            value = GC.AllocateUninitializedArray<byte>(length, false);
            fixed (byte* bufferPtr = value)
            {
                var isSuccess = Imports.ReadProcessMemory(_gameProcess.Handle, memoryAddress, (nint)bufferPtr, (nuint) value.Length, out _);
                if (!isSuccess)
                {
                    throw new MemoryOperationException($"ReadProcessMemory failed to read from {memoryAddress}, bytes: {length}");
                }
            }
        }
        public void Write<T>(nuint memoryAddress, ref T item) where T : unmanaged
        {
            var itemSize = Unsafe.SizeOf<T>();
            var bytes = GC.AllocateUninitializedArray<byte>(itemSize, false);
            var arraySpan = new Span<byte>(bytes);
            MemoryMarshal.Write(arraySpan, ref item);

            fixed (byte* bytePtr = bytes)
            {
                var isSuccess = Imports.WriteProcessMemory(_gameProcess.Handle, memoryAddress, (nuint)bytePtr, (nuint)bytes.Length, out _);

                if (!isSuccess)
                {
                    throw new MemoryOperationException($"WriteProcessMemory failed to write {bytes.Length} bytes of memory to {memoryAddress}");
                }
                    
            }
        }
        
        public MemPageProtection ChangePageProtection(nuint memoryAddress, long size, MemPageProtection newProtection)
        {
            var isSuccess = Imports.VirtualProtectEx(_gameProcess.Handle, memoryAddress, (nuint) size, newProtection, out var oldPermissions);

            if (!isSuccess)
            {
                throw new MemoryOperationException($"Unable to change permissions for " +
                                                   $"the memory address: {memoryAddress} ," +
                                                   $"size: {size} ," +
                                                   $"new protection: {newProtection} ," +
                                                   $"old protection: {oldPermissions}");
            }

            return oldPermissions;
        }

        public IEnumerable<PatternScanResult> FindPatterns(IReadOnlyList<BytePattern> bytePatterns)
        {
            var results = new PatternScanResult[bytePatterns.Count];
            Parallel.ForEach(Partitioner.Create(0, bytePatterns.Count), tuple =>
            {
                for (var x = tuple.Item1; x < tuple.Item2; x++)
                    results[x] = _scannerEngine.FindPattern(_moduleDataPtr + bytePatterns[x].Offset, _moduleMemorySize, bytePatterns[x]);
            });
            return results;
        }

        public PatternScanResult FindPattern(BytePattern bytePattern)
        {
            return _scannerEngine
                .FindPattern(_moduleDataPtr, _moduleMemorySize - (int)bytePattern.Offset, bytePattern)
                .AddOffset(bytePattern.Offset);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            
            _gcHandle?.Free();
            _gameProcess.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
