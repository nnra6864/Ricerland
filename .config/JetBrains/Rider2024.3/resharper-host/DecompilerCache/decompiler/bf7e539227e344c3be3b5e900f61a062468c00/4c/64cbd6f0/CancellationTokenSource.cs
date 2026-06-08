// Decompiled with JetBrains decompiler
// Type: System.Threading.CancellationTokenSource
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: BF7E5392-27E3-44C3-BE3B-5E900F61A062
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/MonoBleedingEdge/lib/mono/unityjit-linux/mscorlib.dll

using System.Collections.Generic;

#nullable disable
namespace System.Threading
{
  public class CancellationTokenSource : IDisposable
  {
    internal static readonly CancellationTokenSource s_canceledSource = new CancellationTokenSource()
    {
      _state = 3
    };
    internal static readonly CancellationTokenSource s_neverCanceledSource = new CancellationTokenSource()
    {
      _state = 0
    };
    private static readonly int s_nLists = PlatformHelper.ProcessorCount > 24 ? 24 : PlatformHelper.ProcessorCount;
    private volatile ManualResetEvent _kernelEvent;
    private volatile SparselyPopulatedArray<CancellationCallbackInfo>[] _registeredCallbacksLists;
    private const int CannotBeCanceled = 0;
    private const int NotCanceledState = 1;
    private const int NotifyingState = 2;
    private const int NotifyingCompleteState = 3;
    private volatile int _state;
    private volatile int _threadIDExecutingCallbacks = -1;
    private bool _disposed;
    private volatile CancellationCallbackInfo _executingCallback;
    private volatile Timer _timer;
    private static readonly TimerCallback s_timerCallback = new TimerCallback(CancellationTokenSource.TimerCallbackLogic);

    public bool IsCancellationRequested => this._state >= 2;

    internal bool IsCancellationCompleted => this._state == 3;

    internal bool IsDisposed => this._disposed;

    internal int ThreadIDExecutingCallbacks
    {
      get => this._threadIDExecutingCallbacks;
      set => this._threadIDExecutingCallbacks = value;
    }

    public CancellationToken Token
    {
      get
      {
        this.ThrowIfDisposed();
        return new CancellationToken(this);
      }
    }

    internal bool CanBeCanceled => this._state != 0;

    internal WaitHandle WaitHandle
    {
      get
      {
        this.ThrowIfDisposed();
        if (this._kernelEvent != null)
          return (WaitHandle) this._kernelEvent;
        ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        if (Interlocked.CompareExchange<ManualResetEvent>(ref this._kernelEvent, manualResetEvent, (ManualResetEvent) null) != null)
          manualResetEvent.Dispose();
        if (this.IsCancellationRequested)
          this._kernelEvent.Set();
        return (WaitHandle) this._kernelEvent;
      }
    }

    internal CancellationCallbackInfo ExecutingCallback => this._executingCallback;

    public CancellationTokenSource() => this._state = 1;

    public CancellationTokenSource(TimeSpan delay)
    {
      long totalMilliseconds = (long) delay.TotalMilliseconds;
      if (totalMilliseconds < -1L || totalMilliseconds > (long) int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (delay));
      this.InitializeWithTimer((int) totalMilliseconds);
    }

    public CancellationTokenSource(int millisecondsDelay)
    {
      if (millisecondsDelay < -1)
        throw new ArgumentOutOfRangeException(nameof (millisecondsDelay));
      this.InitializeWithTimer(millisecondsDelay);
    }

    private void InitializeWithTimer(int millisecondsDelay)
    {
      this._state = 1;
      this._timer = new Timer(CancellationTokenSource.s_timerCallback, (object) this, millisecondsDelay, -1);
    }

    public void Cancel() => this.Cancel(false);

    public void Cancel(bool throwOnFirstException)
    {
      this.ThrowIfDisposed();
      this.NotifyCancellation(throwOnFirstException);
    }

    public void CancelAfter(TimeSpan delay)
    {
      long totalMilliseconds = (long) delay.TotalMilliseconds;
      if (totalMilliseconds < -1L || totalMilliseconds > (long) int.MaxValue)
        throw new ArgumentOutOfRangeException(nameof (delay));
      this.CancelAfter((int) totalMilliseconds);
    }

    public void CancelAfter(int millisecondsDelay)
    {
      this.ThrowIfDisposed();
      if (millisecondsDelay < -1)
        throw new ArgumentOutOfRangeException(nameof (millisecondsDelay));
      if (this.IsCancellationRequested)
        return;
      if (this._timer == null)
      {
        Timer timer = new Timer(CancellationTokenSource.s_timerCallback, (object) this, -1, -1);
        if (Interlocked.CompareExchange<Timer>(ref this._timer, timer, (Timer) null) != null)
          timer.Dispose();
      }
      try
      {
        this._timer.Change(millisecondsDelay, -1);
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    private static void TimerCallbackLogic(object obj)
    {
      CancellationTokenSource cancellationTokenSource = (CancellationTokenSource) obj;
      if (cancellationTokenSource.IsDisposed)
        return;
      try
      {
        cancellationTokenSource.Cancel();
      }
      catch (ObjectDisposedException ex)
      {
        if (cancellationTokenSource.IsDisposed)
          return;
        throw;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this._disposed)
        return;
      this._timer?.Dispose();
      this._registeredCallbacksLists = (SparselyPopulatedArray<CancellationCallbackInfo>[]) null;
      if (this._kernelEvent != null)
      {
        ManualResetEvent manualResetEvent = Interlocked.Exchange<ManualResetEvent>(ref this._kernelEvent, (ManualResetEvent) null);
        if (manualResetEvent != null && this._state != 2)
          manualResetEvent.Dispose();
      }
      this._disposed = true;
    }

    internal void ThrowIfDisposed()
    {
      if (!this._disposed)
        return;
      CancellationTokenSource.ThrowObjectDisposedException();
    }

    private static void ThrowObjectDisposedException()
    {
      throw new ObjectDisposedException((string) null, "The CancellationTokenSource has been disposed.");
    }

    internal CancellationTokenRegistration InternalRegister(
      Action<object> callback,
      object stateForCallback,
      SynchronizationContext targetSyncContext,
      ExecutionContext executionContext)
    {
      if (!this.IsCancellationRequested)
      {
        if (this._disposed)
          return new CancellationTokenRegistration();
        int index = Environment.CurrentManagedThreadId % CancellationTokenSource.s_nLists;
        CancellationCallbackInfo cancellationCallbackInfo = targetSyncContext != null ? (CancellationCallbackInfo) new CancellationCallbackInfo.WithSyncContext(callback, stateForCallback, executionContext, this, targetSyncContext) : new CancellationCallbackInfo(callback, stateForCallback, executionContext, this);
        SparselyPopulatedArray<CancellationCallbackInfo>[] sparselyPopulatedArrayArray1 = this._registeredCallbacksLists;
        if (sparselyPopulatedArrayArray1 == null)
        {
          SparselyPopulatedArray<CancellationCallbackInfo>[] sparselyPopulatedArrayArray2 = new SparselyPopulatedArray<CancellationCallbackInfo>[CancellationTokenSource.s_nLists];
          sparselyPopulatedArrayArray1 = Interlocked.CompareExchange<SparselyPopulatedArray<CancellationCallbackInfo>[]>(ref this._registeredCallbacksLists, sparselyPopulatedArrayArray2, (SparselyPopulatedArray<CancellationCallbackInfo>[]) null) ?? sparselyPopulatedArrayArray2;
        }
        SparselyPopulatedArray<CancellationCallbackInfo> sparselyPopulatedArray1 = Volatile.Read<SparselyPopulatedArray<CancellationCallbackInfo>>(ref sparselyPopulatedArrayArray1[index]);
        if (sparselyPopulatedArray1 == null)
        {
          SparselyPopulatedArray<CancellationCallbackInfo> sparselyPopulatedArray2 = new SparselyPopulatedArray<CancellationCallbackInfo>(4);
          Interlocked.CompareExchange<SparselyPopulatedArray<CancellationCallbackInfo>>(ref sparselyPopulatedArrayArray1[index], sparselyPopulatedArray2, (SparselyPopulatedArray<CancellationCallbackInfo>) null);
          sparselyPopulatedArray1 = sparselyPopulatedArrayArray1[index];
        }
        SparselyPopulatedArrayAddInfo<CancellationCallbackInfo> registrationInfo = sparselyPopulatedArray1.Add(cancellationCallbackInfo);
        CancellationTokenRegistration tokenRegistration = new CancellationTokenRegistration(cancellationCallbackInfo, registrationInfo);
        if (!this.IsCancellationRequested || !tokenRegistration.Unregister())
          return tokenRegistration;
      }
      callback(stateForCallback);
      return new CancellationTokenRegistration();
    }

    private void NotifyCancellation(bool throwOnFirstException)
    {
      if (this.IsCancellationRequested || Interlocked.CompareExchange(ref this._state, 2, 1) != 1)
        return;
      this._timer?.Dispose();
      this.ThreadIDExecutingCallbacks = Environment.CurrentManagedThreadId;
      this._kernelEvent?.Set();
      this.ExecuteCallbackHandlers(throwOnFirstException);
    }

    private void ExecuteCallbackHandlers(bool throwOnFirstException)
    {
      LowLevelListWithIList<Exception> innerExceptions = (LowLevelListWithIList<Exception>) null;
      SparselyPopulatedArray<CancellationCallbackInfo>[] registeredCallbacksLists = this._registeredCallbacksLists;
      if (registeredCallbacksLists == null)
      {
        Interlocked.Exchange(ref this._state, 3);
      }
      else
      {
        try
        {
          for (int index1 = 0; index1 < registeredCallbacksLists.Length; ++index1)
          {
            SparselyPopulatedArray<CancellationCallbackInfo> sparselyPopulatedArray = Volatile.Read<SparselyPopulatedArray<CancellationCallbackInfo>>(ref registeredCallbacksLists[index1]);
            if (sparselyPopulatedArray != null)
            {
              for (SparselyPopulatedArrayFragment<CancellationCallbackInfo> currArrayFragment = sparselyPopulatedArray.Tail; currArrayFragment != null; currArrayFragment = currArrayFragment.Prev)
              {
                for (int index2 = currArrayFragment.Length - 1; index2 >= 0; --index2)
                {
                  this._executingCallback = currArrayFragment[index2];
                  if (this._executingCallback != null)
                  {
                    CancellationCallbackCoreWorkArguments coreWorkArguments = new CancellationCallbackCoreWorkArguments(currArrayFragment, index2);
                    try
                    {
                      if (this._executingCallback is CancellationCallbackInfo.WithSyncContext executingCallback)
                      {
                        executingCallback.TargetSyncContext.Send(new SendOrPostCallback(this.CancellationCallbackCoreWork_OnSyncContext), (object) coreWorkArguments);
                        this.ThreadIDExecutingCallbacks = Environment.CurrentManagedThreadId;
                      }
                      else
                        this.CancellationCallbackCoreWork(coreWorkArguments);
                    }
                    catch (Exception ex)
                    {
                      if (throwOnFirstException)
                      {
                        throw;
                      }
                      else
                      {
                        if (innerExceptions == null)
                          innerExceptions = new LowLevelListWithIList<Exception>();
                        innerExceptions.Add(ex);
                      }
                    }
                  }
                }
              }
            }
          }
        }
        finally
        {
          this._state = 3;
          this._executingCallback = (CancellationCallbackInfo) null;
          Interlocked.MemoryBarrier();
        }
        if (innerExceptions != null)
          throw new AggregateException((IEnumerable<Exception>) innerExceptions);
      }
    }

    private void CancellationCallbackCoreWork_OnSyncContext(object obj)
    {
      this.CancellationCallbackCoreWork((CancellationCallbackCoreWorkArguments) obj);
    }

    private void CancellationCallbackCoreWork(CancellationCallbackCoreWorkArguments args)
    {
      CancellationCallbackInfo cancellationCallbackInfo = args._currArrayFragment.SafeAtomicRemove(args._currArrayIndex, this._executingCallback);
      if (cancellationCallbackInfo != this._executingCallback)
        return;
      cancellationCallbackInfo.CancellationTokenSource.ThreadIDExecutingCallbacks = Environment.CurrentManagedThreadId;
      cancellationCallbackInfo.ExecuteCallback();
    }

    public static CancellationTokenSource CreateLinkedTokenSource(
      CancellationToken token1,
      CancellationToken token2)
    {
      if (!token1.CanBeCanceled)
        return CancellationTokenSource.CreateLinkedTokenSource(token2);
      return !token2.CanBeCanceled ? (CancellationTokenSource) new CancellationTokenSource.Linked1CancellationTokenSource(token1) : (CancellationTokenSource) new CancellationTokenSource.Linked2CancellationTokenSource(token1, token2);
    }

    internal static CancellationTokenSource CreateLinkedTokenSource(CancellationToken token)
    {
      return !token.CanBeCanceled ? new CancellationTokenSource() : (CancellationTokenSource) new CancellationTokenSource.Linked1CancellationTokenSource(token);
    }

    public static CancellationTokenSource CreateLinkedTokenSource(params CancellationToken[] tokens)
    {
      if (tokens == null)
        throw new ArgumentNullException(nameof (tokens));
      switch (tokens.Length)
      {
        case 0:
          throw new ArgumentException("No tokens were supplied.");
        case 1:
          return CancellationTokenSource.CreateLinkedTokenSource(tokens[0]);
        case 2:
          return CancellationTokenSource.CreateLinkedTokenSource(tokens[0], tokens[1]);
        default:
          return (CancellationTokenSource) new CancellationTokenSource.LinkedNCancellationTokenSource(tokens);
      }
    }

    internal void WaitForCallbackToComplete(CancellationCallbackInfo callbackInfo)
    {
      SpinWait spinWait = new SpinWait();
      while (this.ExecutingCallback == callbackInfo)
        spinWait.SpinOnce();
    }

    private sealed class Linked1CancellationTokenSource : CancellationTokenSource
    {
      private readonly CancellationTokenRegistration _reg1;

      internal Linked1CancellationTokenSource(CancellationToken token1)
      {
        this._reg1 = token1.InternalRegisterWithoutEC(CancellationTokenSource.LinkedNCancellationTokenSource.s_linkedTokenCancelDelegate, (object) this);
      }

      protected override void Dispose(bool disposing)
      {
        if (!disposing || this._disposed)
          return;
        this._reg1.Dispose();
        base.Dispose(disposing);
      }
    }

    private sealed class Linked2CancellationTokenSource : CancellationTokenSource
    {
      private readonly CancellationTokenRegistration _reg1;
      private readonly CancellationTokenRegistration _reg2;

      internal Linked2CancellationTokenSource(CancellationToken token1, CancellationToken token2)
      {
        this._reg1 = token1.InternalRegisterWithoutEC(CancellationTokenSource.LinkedNCancellationTokenSource.s_linkedTokenCancelDelegate, (object) this);
        this._reg2 = token2.InternalRegisterWithoutEC(CancellationTokenSource.LinkedNCancellationTokenSource.s_linkedTokenCancelDelegate, (object) this);
      }

      protected override void Dispose(bool disposing)
      {
        if (!disposing || this._disposed)
          return;
        this._reg1.Dispose();
        this._reg2.Dispose();
        base.Dispose(disposing);
      }
    }

    private sealed class LinkedNCancellationTokenSource : CancellationTokenSource
    {
      internal static readonly Action<object> s_linkedTokenCancelDelegate = (Action<object>) (s => ((CancellationTokenSource) s).NotifyCancellation(false));
      private CancellationTokenRegistration[] _linkingRegistrations;

      internal LinkedNCancellationTokenSource(params CancellationToken[] tokens)
      {
        this._linkingRegistrations = new CancellationTokenRegistration[tokens.Length];
        for (int index = 0; index < tokens.Length; ++index)
        {
          if (tokens[index].CanBeCanceled)
            this._linkingRegistrations[index] = tokens[index].InternalRegisterWithoutEC(CancellationTokenSource.LinkedNCancellationTokenSource.s_linkedTokenCancelDelegate, (object) this);
        }
      }

      protected override void Dispose(bool disposing)
      {
        if (!disposing || this._disposed)
          return;
        CancellationTokenRegistration[] linkingRegistrations = this._linkingRegistrations;
        if (linkingRegistrations != null)
        {
          this._linkingRegistrations = (CancellationTokenRegistration[]) null;
          for (int index = 0; index < linkingRegistrations.Length; ++index)
            linkingRegistrations[index].Dispose();
        }
        base.Dispose(disposing);
      }
    }
  }
}
