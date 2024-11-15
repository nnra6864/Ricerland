// Decompiled with JetBrains decompiler
// Type: System.Threading.ThreadPoolWorkQueue
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: F724BEAC-1C70-4825-AD74-571377C52F70
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.24f1/Editor/Data/MonoBleedingEdge/lib/mono/unityjit-linux/mscorlib.dll

using System.Runtime.ConstrainedExecution;
using System.Security;

#nullable disable
namespace System.Threading
{
  internal sealed class ThreadPoolWorkQueue
  {
    internal volatile ThreadPoolWorkQueue.QueueSegment queueHead;
    internal volatile ThreadPoolWorkQueue.QueueSegment queueTail;
    internal static ThreadPoolWorkQueue.SparseArray<ThreadPoolWorkQueue.WorkStealingQueue> allThreadQueues = new ThreadPoolWorkQueue.SparseArray<ThreadPoolWorkQueue.WorkStealingQueue>(16);
    private volatile int numOutstandingThreadRequests;

    public ThreadPoolWorkQueue()
    {
      this.queueTail = this.queueHead = new ThreadPoolWorkQueue.QueueSegment();
    }

    [SecurityCritical]
    public ThreadPoolWorkQueueThreadLocals EnsureCurrentThreadHasQueue()
    {
      if (ThreadPoolWorkQueueThreadLocals.threadLocals == null)
        ThreadPoolWorkQueueThreadLocals.threadLocals = new ThreadPoolWorkQueueThreadLocals(this);
      return ThreadPoolWorkQueueThreadLocals.threadLocals;
    }

    [SecurityCritical]
    internal void EnsureThreadRequested()
    {
      int num;
      for (int comparand = this.numOutstandingThreadRequests; comparand < ThreadPoolGlobals.processorCount; comparand = num)
      {
        num = Interlocked.CompareExchange(ref this.numOutstandingThreadRequests, comparand + 1, comparand);
        if (num == comparand)
        {
          ThreadPool.RequestWorkerThread();
          break;
        }
      }
    }

    [SecurityCritical]
    internal void MarkThreadRequestSatisfied()
    {
      int num;
      for (int comparand = this.numOutstandingThreadRequests; comparand > 0; comparand = num)
      {
        num = Interlocked.CompareExchange(ref this.numOutstandingThreadRequests, comparand - 1, comparand);
        if (num == comparand)
          break;
      }
    }

    [SecurityCritical]
    public void Enqueue(IThreadPoolWorkItem callback, bool forceGlobal)
    {
      ThreadPoolWorkQueueThreadLocals queueThreadLocals = (ThreadPoolWorkQueueThreadLocals) null;
      if (!forceGlobal)
        queueThreadLocals = ThreadPoolWorkQueueThreadLocals.threadLocals;
      if (queueThreadLocals != null)
      {
        queueThreadLocals.workStealingQueue.LocalPush(callback);
      }
      else
      {
        ThreadPoolWorkQueue.QueueSegment queueHead = this.queueHead;
        while (!queueHead.TryEnqueue(callback))
        {
          Interlocked.CompareExchange<ThreadPoolWorkQueue.QueueSegment>(ref queueHead.Next, new ThreadPoolWorkQueue.QueueSegment(), (ThreadPoolWorkQueue.QueueSegment) null);
          for (; queueHead.Next != null; queueHead = this.queueHead)
            Interlocked.CompareExchange<ThreadPoolWorkQueue.QueueSegment>(ref this.queueHead, queueHead.Next, queueHead);
        }
      }
      ThreadPool.NotifyWorkItemQueued();
      this.EnsureThreadRequested();
    }

    [SecurityCritical]
    internal bool LocalFindAndPop(IThreadPoolWorkItem callback)
    {
      ThreadPoolWorkQueueThreadLocals threadLocals = ThreadPoolWorkQueueThreadLocals.threadLocals;
      return threadLocals != null && threadLocals.workStealingQueue.LocalFindAndPop(callback);
    }

    [SecurityCritical]
    public void Dequeue(
      ThreadPoolWorkQueueThreadLocals tl,
      out IThreadPoolWorkItem callback,
      out bool missedSteal)
    {
      callback = (IThreadPoolWorkItem) null;
      missedSteal = false;
      ThreadPoolWorkQueue.WorkStealingQueue workStealingQueue1 = tl.workStealingQueue;
      workStealingQueue1.LocalPop(out callback);
      if (callback == null)
      {
        for (ThreadPoolWorkQueue.QueueSegment queueTail = this.queueTail; !queueTail.TryDequeue(out callback) && queueTail.Next != null && queueTail.IsUsedUp(); queueTail = this.queueTail)
          Interlocked.CompareExchange<ThreadPoolWorkQueue.QueueSegment>(ref this.queueTail, queueTail.Next, queueTail);
      }
      if (callback != null)
        return;
      ThreadPoolWorkQueue.WorkStealingQueue[] current = ThreadPoolWorkQueue.allThreadQueues.Current;
      int num = tl.random.Next(current.Length);
      for (int length = current.Length; length > 0; --length)
      {
        ThreadPoolWorkQueue.WorkStealingQueue workStealingQueue2 = Volatile.Read<ThreadPoolWorkQueue.WorkStealingQueue>(ref current[num % current.Length]);
        if (workStealingQueue2 != null && workStealingQueue2 != workStealingQueue1 && workStealingQueue2.TrySteal(out callback, ref missedSteal))
          break;
        ++num;
      }
    }

    [SecurityCritical]
    internal static bool Dispatch()
    {
      ThreadPoolWorkQueue workQueue = ThreadPoolGlobals.workQueue;
      int tickCount = Environment.TickCount;
      workQueue.MarkThreadRequestSatisfied();
      bool flag1 = true;
      IThreadPoolWorkItem callback = (IThreadPoolWorkItem) null;
      try
      {
        ThreadPoolWorkQueueThreadLocals tl = workQueue.EnsureCurrentThreadHasQueue();
        while (Environment.TickCount - tickCount < 30)
        {
          try
          {
          }
          finally
          {
            bool missedSteal = false;
            workQueue.Dequeue(tl, out callback, out missedSteal);
            if (callback == null)
              flag1 = missedSteal;
            else
              workQueue.EnsureThreadRequested();
          }
          if (callback == null)
            return true;
          if (ThreadPoolGlobals.enableWorkerTracking)
          {
            bool flag2 = false;
            try
            {
              try
              {
              }
              finally
              {
                ThreadPool.ReportThreadStatus(true);
                flag2 = true;
              }
              callback.ExecuteWorkItem();
              callback = (IThreadPoolWorkItem) null;
            }
            finally
            {
              if (flag2)
                ThreadPool.ReportThreadStatus(false);
            }
          }
          else
          {
            callback.ExecuteWorkItem();
            callback = (IThreadPoolWorkItem) null;
          }
          if (!ThreadPool.NotifyWorkItemComplete())
            return false;
        }
        return true;
      }
      catch (ThreadAbortException ex)
      {
        callback?.MarkAborted(ex);
        flag1 = false;
      }
      finally
      {
        if (flag1)
          workQueue.EnsureThreadRequested();
      }
      return true;
    }

    internal class SparseArray<T> where T : class
    {
      private volatile T[] m_array;

      internal SparseArray(int initialSize) => this.m_array = new T[initialSize];

      internal T[] Current => this.m_array;

      internal int Add(T e)
      {
label_0:
        T[] array = this.m_array;
        lock (array)
        {
          for (int index = 0; index < array.Length; ++index)
          {
            if ((object) array[index] == null)
            {
              Volatile.Write<T>(ref array[index], e);
              return index;
            }
            if (index == array.Length - 1 && array == this.m_array)
            {
              T[] destinationArray = new T[array.Length * 2];
              Array.Copy((Array) array, (Array) destinationArray, index + 1);
              destinationArray[index + 1] = e;
              this.m_array = destinationArray;
              return index + 1;
            }
          }
          goto label_0;
        }
      }

      internal void Remove(T e)
      {
        lock (this.m_array)
        {
          for (int index = 0; index < this.m_array.Length; ++index)
          {
            if ((object) this.m_array[index] == (object) e)
            {
              Volatile.Write<T>(ref this.m_array[index], default (T));
              break;
            }
          }
        }
      }
    }

    internal class WorkStealingQueue
    {
      private const int INITIAL_SIZE = 32;
      internal volatile IThreadPoolWorkItem[] m_array = new IThreadPoolWorkItem[32];
      private volatile int m_mask = 31;
      private const int START_INDEX = 0;
      private volatile int m_headIndex;
      private volatile int m_tailIndex;
      private SpinLock m_foreignLock = new SpinLock(false);

      public void LocalPush(IThreadPoolWorkItem obj)
      {
        int num1 = this.m_tailIndex;
        if (num1 == int.MaxValue)
        {
          bool lockTaken = false;
          try
          {
            this.m_foreignLock.Enter(ref lockTaken);
            if (this.m_tailIndex == int.MaxValue)
            {
              this.m_headIndex &= this.m_mask;
              this.m_tailIndex = num1 = this.m_tailIndex & this.m_mask;
            }
          }
          finally
          {
            if (lockTaken)
              this.m_foreignLock.Exit(true);
          }
        }
        if (num1 < this.m_headIndex + this.m_mask)
        {
          Volatile.Write<IThreadPoolWorkItem>(ref this.m_array[num1 & this.m_mask], obj);
          this.m_tailIndex = num1 + 1;
        }
        else
        {
          bool lockTaken = false;
          try
          {
            this.m_foreignLock.Enter(ref lockTaken);
            int headIndex = this.m_headIndex;
            int num2 = this.m_tailIndex - this.m_headIndex;
            if (num2 >= this.m_mask)
            {
              IThreadPoolWorkItem[] threadPoolWorkItemArray = new IThreadPoolWorkItem[this.m_array.Length << 1];
              for (int index = 0; index < this.m_array.Length; ++index)
                threadPoolWorkItemArray[index] = this.m_array[index + headIndex & this.m_mask];
              this.m_array = threadPoolWorkItemArray;
              this.m_headIndex = 0;
              this.m_tailIndex = num1 = num2;
              this.m_mask = this.m_mask << 1 | 1;
            }
            Volatile.Write<IThreadPoolWorkItem>(ref this.m_array[num1 & this.m_mask], obj);
            this.m_tailIndex = num1 + 1;
          }
          finally
          {
            if (lockTaken)
              this.m_foreignLock.Exit(false);
          }
        }
      }

      public bool LocalFindAndPop(IThreadPoolWorkItem obj)
      {
        if (this.m_array[this.m_tailIndex - 1 & this.m_mask] == obj)
          return this.LocalPop(out IThreadPoolWorkItem _);
        for (int index = this.m_tailIndex - 2; index >= this.m_headIndex; --index)
        {
          if (this.m_array[index & this.m_mask] == obj)
          {
            bool lockTaken = false;
            try
            {
              this.m_foreignLock.Enter(ref lockTaken);
              if (this.m_array[index & this.m_mask] == null)
                return false;
              Volatile.Write<IThreadPoolWorkItem>(ref this.m_array[index & this.m_mask], (IThreadPoolWorkItem) null);
              if (index == this.m_tailIndex)
                --this.m_tailIndex;
              else if (index == this.m_headIndex)
                ++this.m_headIndex;
              return true;
            }
            finally
            {
              if (lockTaken)
                this.m_foreignLock.Exit(false);
            }
          }
        }
        return false;
      }

      public bool LocalPop(out IThreadPoolWorkItem obj)
      {
label_0:
        int num;
        int index1;
        do
        {
          int tailIndex = this.m_tailIndex;
          if (this.m_headIndex >= tailIndex)
          {
            obj = (IThreadPoolWorkItem) null;
            return false;
          }
          num = tailIndex - 1;
          Interlocked.Exchange(ref this.m_tailIndex, num);
          if (this.m_headIndex <= num)
          {
            index1 = num & this.m_mask;
            obj = Volatile.Read<IThreadPoolWorkItem>(ref this.m_array[index1]);
          }
          else
            goto label_5;
        }
        while (obj == null);
        this.m_array[index1] = (IThreadPoolWorkItem) null;
        return true;
label_5:
        bool lockTaken = false;
        try
        {
          this.m_foreignLock.Enter(ref lockTaken);
          if (this.m_headIndex <= num)
          {
            int index2 = num & this.m_mask;
            obj = Volatile.Read<IThreadPoolWorkItem>(ref this.m_array[index2]);
            if (obj != null)
            {
              this.m_array[index2] = (IThreadPoolWorkItem) null;
              return true;
            }
            goto label_0;
          }
          else
          {
            this.m_tailIndex = num + 1;
            obj = (IThreadPoolWorkItem) null;
            return false;
          }
        }
        finally
        {
          if (lockTaken)
            this.m_foreignLock.Exit(false);
        }
      }

      public bool TrySteal(out IThreadPoolWorkItem obj, ref bool missedSteal)
      {
        return this.TrySteal(out obj, ref missedSteal, 0);
      }

      private bool TrySteal(
        out IThreadPoolWorkItem obj,
        ref bool missedSteal,
        int millisecondsTimeout)
      {
        obj = (IThreadPoolWorkItem) null;
        while (this.m_headIndex < this.m_tailIndex)
        {
          bool lockTaken = false;
          try
          {
            this.m_foreignLock.TryEnter(millisecondsTimeout, ref lockTaken);
            if (lockTaken)
            {
              int headIndex = this.m_headIndex;
              Interlocked.Exchange(ref this.m_headIndex, headIndex + 1);
              if (headIndex < this.m_tailIndex)
              {
                int index = headIndex & this.m_mask;
                obj = Volatile.Read<IThreadPoolWorkItem>(ref this.m_array[index]);
                if (obj != null)
                {
                  this.m_array[index] = (IThreadPoolWorkItem) null;
                  return true;
                }
                continue;
              }
              this.m_headIndex = headIndex;
              obj = (IThreadPoolWorkItem) null;
              missedSteal = true;
            }
            else
              missedSteal = true;
          }
          finally
          {
            if (lockTaken)
              this.m_foreignLock.Exit(false);
          }
          return false;
        }
        return false;
      }
    }

    internal class QueueSegment
    {
      internal readonly IThreadPoolWorkItem[] nodes;
      private const int QueueSegmentLength = 256;
      private volatile int indexes;
      public volatile ThreadPoolWorkQueue.QueueSegment Next;
      private const int SixteenBits = 65535;

      private void GetIndexes(out int upper, out int lower)
      {
        int indexes = this.indexes;
        upper = indexes >> 16 & (int) ushort.MaxValue;
        lower = indexes & (int) ushort.MaxValue;
      }

      private bool CompareExchangeIndexes(
        ref int prevUpper,
        int newUpper,
        ref int prevLower,
        int newLower)
      {
        int comparand = prevUpper << 16 | prevLower & (int) ushort.MaxValue;
        int num = Interlocked.CompareExchange(ref this.indexes, newUpper << 16 | newLower & (int) ushort.MaxValue, comparand);
        prevUpper = num >> 16 & (int) ushort.MaxValue;
        prevLower = num & (int) ushort.MaxValue;
        return num == comparand;
      }

      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
      public QueueSegment() => this.nodes = new IThreadPoolWorkItem[256];

      public bool IsUsedUp()
      {
        int upper;
        int lower;
        this.GetIndexes(out upper, out lower);
        return upper == this.nodes.Length && lower == this.nodes.Length;
      }

      public bool TryEnqueue(IThreadPoolWorkItem node)
      {
        int upper;
        int lower;
        this.GetIndexes(out upper, out lower);
        while (upper != this.nodes.Length)
        {
          if (this.CompareExchangeIndexes(ref upper, upper + 1, ref lower, lower))
          {
            Volatile.Write<IThreadPoolWorkItem>(ref this.nodes[upper], node);
            return true;
          }
        }
        return false;
      }

      public bool TryDequeue(out IThreadPoolWorkItem node)
      {
        int upper;
        int lower;
        this.GetIndexes(out upper, out lower);
        while (lower != upper)
        {
          if (this.CompareExchangeIndexes(ref upper, upper, ref lower, lower + 1))
          {
            SpinWait spinWait = new SpinWait();
            while ((node = Volatile.Read<IThreadPoolWorkItem>(ref this.nodes[lower])) == null)
              spinWait.SpinOnce();
            this.nodes[lower] = (IThreadPoolWorkItem) null;
            return true;
          }
        }
        node = (IThreadPoolWorkItem) null;
        return false;
      }
    }
  }
}
