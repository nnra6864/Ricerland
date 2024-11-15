// Decompiled with JetBrains decompiler
// Type: UnityEngine.UIElements.Layout.LayoutDataStore
// Assembly: UnityEngine.UIElementsModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DBB227D7-B33D-42B5-9537-0BACBF985830
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.24f1/Editor/Data/Managed/UnityEngine/UnityEngine.UIElementsModule.dll
// XML documentation location: /home/nnra/Unity/Hub/Editor/6000.0.24f1/Editor/Data/Managed/UnityEngine/UnityEngine.UIElementsModule.xml

using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

#nullable disable
namespace UnityEngine.UIElements.Layout
{
  internal struct LayoutDataStore : IDisposable
  {
    private const int k_ChunkSize = 32768;
    private readonly Allocator m_Allocator;
    [NativeDisableUnsafePtrRestriction]
    private unsafe LayoutDataStore.Data* m_Data;

    public unsafe bool IsValid => IntPtr.Zero != (IntPtr) this.m_Data;

    public unsafe int Capacity => this.m_Data->Capacity;

    public unsafe LayoutDataStore(
      ComponentType[] components,
      int initialCapacity,
      Allocator allocator)
    {
      Assert.IsTrue(components.Length != 0, "LayoutDataStore requires at least one component size.");
      Assert.IsTrue(components[0].Size >= 4, string.Format("{0} requires a minimum element size of {1} to alias", (object) nameof (LayoutDataStore), (object) 4));
      this.m_Allocator = allocator;
      this.m_Data = (LayoutDataStore.Data*) UnsafeUtility.Malloc((long) UnsafeUtility.SizeOf<LayoutDataStore.Data>(), UnsafeUtility.AlignOf<LayoutDataStore.Data>(), this.m_Allocator);
      UnsafeUtility.MemClear((void*) this.m_Data, (long) UnsafeUtility.SizeOf<LayoutDataStore.Data>());
      this.m_Data->ComponentCount = components.Length;
      this.m_Data->Components = (LayoutDataStore.ComponentDataStore*) UnsafeUtility.Malloc((long) (UnsafeUtility.SizeOf<LayoutDataStore.ComponentDataStore>() * components.Length), UnsafeUtility.AlignOf<LayoutDataStore.ComponentDataStore>(), allocator);
      for (int index = 0; index < components.Length; ++index)
        this.m_Data->Components[index] = new LayoutDataStore.ComponentDataStore(components[index].Size, allocator);
      this.ResizeCapacity(initialCapacity);
      this.m_Data->NextFreeIndex = 0;
    }

    public unsafe void Dispose()
    {
      for (int index = 0; index < this.m_Data->ComponentCount; ++index)
        this.m_Data->Components[index].Dispose();
      UnsafeUtility.Free((void*) this.m_Data->Versions, this.m_Allocator);
      UnsafeUtility.Free((void*) this.m_Data->Components, this.m_Allocator);
      UnsafeUtility.Free((void*) this.m_Data, this.m_Allocator);
      this.m_Data = (LayoutDataStore.Data*) null;
    }

    public unsafe bool Exists(in LayoutHandle handle)
    {
      return (long) (uint) handle.Index < (long) this.m_Data->Capacity && this.m_Data->Versions[handle.Index] == handle.Version;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal readonly unsafe void* GetComponentDataPtr(int index, int componentIndex)
    {
      return (void*) this.m_Data->Components[componentIndex].GetComponentDataPtr(index);
    }

    private unsafe LayoutHandle Allocate(byte** data, int count)
    {
      int nextFreeIndex1 = this.m_Data->NextFreeIndex;
      int nextFreeIndex2 = LayoutDataStore.GetNextFreeIndex(this.m_Data->Components, nextFreeIndex1);
      if (nextFreeIndex2 == -1)
      {
        this.IncreaseCapacity();
        nextFreeIndex2 = LayoutDataStore.GetNextFreeIndex(this.m_Data->Components, nextFreeIndex1);
      }
      int version = this.m_Data->Versions[nextFreeIndex1];
      this.m_Data->NextFreeIndex = nextFreeIndex2;
      Debug.Assert(this.m_Data->ComponentCount == count, "All components must be initialized");
      Debug.Assert((IntPtr) data != IntPtr.Zero);
      for (int index = 0; index < count; ++index)
      {
        Debug.Assert((IntPtr) data[index] != IntPtr.Zero);
        UnsafeUtility.MemCpy((void*) this.m_Data->Components[index].GetComponentDataPtr(nextFreeIndex1), (void*) data[index], (long) this.m_Data->Components[index].Size);
      }
      return new LayoutHandle(nextFreeIndex1, version);
    }

    public unsafe void Free(in LayoutHandle handle)
    {
      if (!this.Exists(in handle))
        throw new InvalidOperationException(string.Format("Failed to Free handle with Index={0} Version={1}", (object) handle.Index, (object) handle.Version));
      int* numPtr = this.m_Data->Versions + handle.Index;
      *numPtr = *numPtr + 1;
      LayoutDataStore.SetNextFreeIndex(this.m_Data->Components, handle.Index, this.m_Data->NextFreeIndex);
      this.m_Data->NextFreeIndex = handle.Index;
    }

    private static unsafe void SetNextFreeIndex(
      LayoutDataStore.ComponentDataStore* ptr,
      int index,
      int value)
    {
      *(int*) ptr->GetComponentDataPtr(index) = value;
    }

    private static unsafe int GetNextFreeIndex(LayoutDataStore.ComponentDataStore* ptr, int index)
    {
      return *(int*) ptr->GetComponentDataPtr(index);
    }

    private unsafe void IncreaseCapacity()
    {
      this.ResizeCapacity((int) ((double) this.m_Data->Capacity * 1.5));
    }

    private unsafe void ResizeCapacity(int capacity)
    {
      Assert.IsTrue(capacity > 0);
      this.m_Data->Versions = (int*) LayoutDataStore.ResizeArray((void*) this.m_Data->Versions, (long) this.m_Data->Capacity, (long) capacity, 4L, 4, this.m_Allocator);
      for (int index = 0; index < this.m_Data->ComponentCount; ++index)
        this.m_Data->Components[index].ResizeCapacity(capacity);
      for (int index = this.m_Data->Capacity > 0 ? this.m_Data->Capacity - 1 : 0; index < capacity; ++index)
      {
        this.m_Data->Versions[index] = 1;
        LayoutDataStore.SetNextFreeIndex(this.m_Data->Components, index, index + 1);
      }
      LayoutDataStore.SetNextFreeIndex(this.m_Data->Components, capacity - 1, -1);
      this.m_Data->Capacity = capacity;
    }

    private static unsafe void* ResizeArray(
      void* fromPtr,
      long fromCount,
      long toCount,
      long size,
      int align,
      Allocator allocator)
    {
      Assert.IsTrue(toCount > 0L);
      void* destination = UnsafeUtility.Malloc(size * toCount, align, allocator);
      Assert.IsTrue((IntPtr) destination != IntPtr.Zero);
      if (fromCount <= 0L)
        return destination;
      long size1 = (toCount < fromCount ? toCount : fromCount) * size;
      UnsafeUtility.MemCpy(destination, fromPtr, size1);
      UnsafeUtility.Free(fromPtr, allocator);
      return destination;
    }

    public unsafe LayoutHandle Allocate<T0>(in T0 component0) where T0 : unmanaged
    {
      fixed (T0* objPtr = &component0)
      {
        // ISSUE: untyped stack allocation
        byte** data = (byte**) __untypedstackalloc(checked (new IntPtr(1) * sizeof (byte*)));
        *data = (byte*) objPtr;
        return this.Allocate(data, 1);
      }
    }

    public unsafe LayoutHandle Allocate<T0, T1, T2>(
      in T0 component0,
      in T1 component1,
      in T2 component2)
      where T0 : unmanaged
      where T1 : unmanaged
      where T2 : unmanaged
    {
      fixed (T0* objPtr1 = &component0)
        fixed (T1* objPtr2 = &component1)
          fixed (T2* objPtr3 = &component2)
          {
            // ISSUE: untyped stack allocation
            byte** data = (byte**) __untypedstackalloc(checked (new IntPtr(3) * sizeof (byte*)));
            *data = (byte*) objPtr1;
            data[1] = (byte*) objPtr2;
            data[2] = (byte*) objPtr3;
            return this.Allocate(data, 3);
          }
    }

    public unsafe LayoutHandle Allocate<T0, T1, T2, T3>(
      in T0 component0,
      in T1 component1,
      in T2 component2,
      in T3 component3)
      where T0 : unmanaged
      where T1 : unmanaged
      where T2 : unmanaged
      where T3 : unmanaged
    {
      fixed (T0* objPtr1 = &component0)
        fixed (T1* objPtr2 = &component1)
          fixed (T2* objPtr3 = &component2)
            fixed (T3* objPtr4 = &component3)
            {
              // ISSUE: untyped stack allocation
              byte** data = (byte**) __untypedstackalloc(checked (new IntPtr(4) * sizeof (byte*)));
              *data = (byte*) objPtr1;
              data[1] = (byte*) objPtr2;
              data[2] = (byte*) objPtr3;
              data[3] = (byte*) objPtr4;
              return this.Allocate(data, 4);
            }
    }

    public unsafe LayoutHandle Allocate<T0, T1, T2, T3, T4>(
      in T0 component0,
      in T1 component1,
      in T2 component2,
      in T3 component3,
      in T4 component4)
      where T0 : unmanaged
      where T1 : unmanaged
      where T2 : unmanaged
      where T3 : unmanaged
      where T4 : unmanaged
    {
      fixed (T0* objPtr1 = &component0)
        fixed (T1* objPtr2 = &component1)
          fixed (T2* objPtr3 = &component2)
            fixed (T3* objPtr4 = &component3)
              fixed (T4* objPtr5 = &component4)
              {
                // ISSUE: untyped stack allocation
                byte** data = (byte**) __untypedstackalloc(checked (new IntPtr(5) * sizeof (byte*)));
                *data = (byte*) objPtr1;
                data[1] = (byte*) objPtr2;
                data[2] = (byte*) objPtr3;
                data[3] = (byte*) objPtr4;
                data[4] = (byte*) objPtr5;
                return this.Allocate(data, 5);
              }
    }

    public unsafe LayoutHandle Allocate<T0, T1, T2, T3, T4, T5>(
      in T0 component0,
      in T1 component1,
      in T2 component2,
      in T3 component3,
      in T4 component4,
      in T5 component5)
      where T0 : unmanaged
      where T1 : unmanaged
      where T2 : unmanaged
      where T3 : unmanaged
      where T4 : unmanaged
      where T5 : unmanaged
    {
      fixed (T0* objPtr1 = &component0)
        fixed (T1* objPtr2 = &component1)
          fixed (T2* objPtr3 = &component2)
            fixed (T3* objPtr4 = &component3)
              fixed (T4* objPtr5 = &component4)
                fixed (T5* objPtr6 = &component5)
                {
                  // ISSUE: untyped stack allocation
                  byte** data = (byte**) __untypedstackalloc(checked (new IntPtr(6) * sizeof (byte*)));
                  *data = (byte*) objPtr1;
                  data[1] = (byte*) objPtr2;
                  data[2] = (byte*) objPtr3;
                  data[3] = (byte*) objPtr4;
                  data[4] = (byte*) objPtr5;
                  data[5] = (byte*) objPtr6;
                  return this.Allocate(data, 6);
                }
    }

    public unsafe LayoutHandle Allocate<T0, T1, T2, T3, T4, T5, T6>(
      in T0 component0,
      in T1 component1,
      in T2 component2,
      in T3 component3,
      in T4 component4,
      in T5 component5,
      in T6 component6)
      where T0 : unmanaged
      where T1 : unmanaged
      where T2 : unmanaged
      where T3 : unmanaged
      where T4 : unmanaged
      where T5 : unmanaged
      where T6 : unmanaged
    {
      fixed (T0* objPtr1 = &component0)
        fixed (T1* objPtr2 = &component1)
          fixed (T2* objPtr3 = &component2)
            fixed (T3* objPtr4 = &component3)
              fixed (T4* objPtr5 = &component4)
                fixed (T5* objPtr6 = &component5)
                  fixed (T6* objPtr7 = &component6)
                  {
                    // ISSUE: untyped stack allocation
                    byte** data = (byte**) __untypedstackalloc(checked (new IntPtr(7) * sizeof (byte*)));
                    *data = (byte*) objPtr1;
                    data[1] = (byte*) objPtr2;
                    data[2] = (byte*) objPtr3;
                    data[3] = (byte*) objPtr4;
                    data[4] = (byte*) objPtr5;
                    data[5] = (byte*) objPtr6;
                    data[6] = (byte*) objPtr7;
                    return this.Allocate(data, 7);
                  }
    }

    private struct Chunk
    {
      [NativeDisableUnsafePtrRestriction]
      public unsafe byte* Buffer;
    }

    private struct ComponentDataStore : IDisposable
    {
      public Allocator Allocator;
      public int Size;
      public int ComponentCountPerChunk;
      public int ChunkCount;
      [NativeDisableUnsafePtrRestriction]
      private unsafe LayoutDataStore.Chunk* m_Chunks;

      public unsafe ComponentDataStore(int size, Allocator allocator)
      {
        this.Allocator = allocator;
        this.Size = size;
        this.ComponentCountPerChunk = 32768 / size;
        this.ChunkCount = 0;
        this.m_Chunks = (LayoutDataStore.Chunk*) null;
      }

      public unsafe void Dispose()
      {
        if (IntPtr.Zero == (IntPtr) this.m_Chunks)
          return;
        for (int index = 0; index < this.ChunkCount; ++index)
          UnsafeUtility.Free((void*) this.m_Chunks[index].Buffer, this.Allocator);
        UnsafeUtility.Free((void*) this.m_Chunks, this.Allocator);
        this.ChunkCount = 0;
        this.m_Chunks = (LayoutDataStore.Chunk*) null;
      }

      public unsafe byte* GetComponentDataPtr(int index)
      {
        return this.m_Chunks[index / this.ComponentCountPerChunk].Buffer + index % this.ComponentCountPerChunk * this.Size;
      }

      public unsafe void EnsureCapacity(int capacity)
      {
        int toCount = capacity / this.ComponentCountPerChunk + 1;
        if (toCount <= this.ChunkCount)
          return;
        this.m_Chunks = (LayoutDataStore.Chunk*) LayoutDataStore.ResizeArray((void*) this.m_Chunks, (long) this.ChunkCount, (long) toCount, (long) UnsafeUtility.SizeOf<LayoutDataStore.Chunk>(), UnsafeUtility.AlignOf<LayoutDataStore.Chunk>(), this.Allocator);
        for (int chunkCount = this.ChunkCount; chunkCount < toCount; ++chunkCount)
          *(this.m_Chunks + chunkCount) = new LayoutDataStore.Chunk()
          {
            Buffer = (byte*) UnsafeUtility.Malloc(32768L, 4, this.Allocator)
          };
        this.ChunkCount = toCount;
      }

      public unsafe void ResizeCapacity(int capacity)
      {
        int toCount = capacity / this.ComponentCountPerChunk + 1;
        if (toCount > this.ChunkCount)
        {
          this.m_Chunks = (LayoutDataStore.Chunk*) LayoutDataStore.ResizeArray((void*) this.m_Chunks, (long) this.ChunkCount, (long) toCount, (long) UnsafeUtility.SizeOf<LayoutDataStore.Chunk>(), UnsafeUtility.AlignOf<LayoutDataStore.Chunk>(), this.Allocator);
          for (int chunkCount = this.ChunkCount; chunkCount < toCount; ++chunkCount)
            *(this.m_Chunks + chunkCount) = new LayoutDataStore.Chunk()
            {
              Buffer = (byte*) UnsafeUtility.Malloc(32768L, 4, this.Allocator)
            };
        }
        else if (toCount < this.ChunkCount)
        {
          for (int index = this.ChunkCount - 1; index >= toCount; --index)
            UnsafeUtility.Free((void*) this.m_Chunks[index].Buffer, this.Allocator);
          this.m_Chunks = (LayoutDataStore.Chunk*) LayoutDataStore.ResizeArray((void*) this.m_Chunks, (long) this.ChunkCount, (long) toCount, (long) UnsafeUtility.SizeOf<LayoutDataStore.Chunk>(), UnsafeUtility.AlignOf<LayoutDataStore.Chunk>(), this.Allocator);
        }
        this.ChunkCount = toCount;
      }
    }

    private struct Data
    {
      public int Capacity;
      public int NextFreeIndex;
      public int ComponentCount;
      [NativeDisableUnsafePtrRestriction]
      public unsafe int* Versions;
      [NativeDisableUnsafePtrRestriction]
      public unsafe LayoutDataStore.ComponentDataStore* Components;
    }
  }
}
