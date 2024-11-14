// Decompiled with JetBrains decompiler
// Type: System.Threading.Tasks.Task
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: F724BEAC-1C70-4825-AD74-571377C52F70
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.24f1/Editor/Data/MonoBleedingEdge/lib/mono/unityjit-linux/mscorlib.dll

using Internal.Runtime.Augments;
using Internal.Threading.Tasks.Tracing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;

#nullable disable
namespace System.Threading.Tasks
{
  [DebuggerDisplay("Id = {Id}, Status = {Status}, Method = {DebuggerDisplayMethodDescription}")]
  [DebuggerTypeProxy(typeof (SystemThreadingTasks_TaskDebugView))]
  public class Task : IThreadPoolWorkItem, IAsyncResult, IDisposable
  {
    internal static int s_taskIdCounter;
    private volatile int m_taskId;
    internal Delegate m_action;
    internal object m_stateObject;
    internal TaskScheduler m_taskScheduler;
    internal readonly Task m_parent;
    internal volatile int m_stateFlags;
    private const int OptionsMask = 65535;
    internal const int TASK_STATE_STARTED = 65536;
    internal const int TASK_STATE_DELEGATE_INVOKED = 131072;
    internal const int TASK_STATE_DISPOSED = 262144;
    internal const int TASK_STATE_EXCEPTIONOBSERVEDBYPARENT = 524288;
    internal const int TASK_STATE_CANCELLATIONACKNOWLEDGED = 1048576;
    internal const int TASK_STATE_FAULTED = 2097152;
    internal const int TASK_STATE_CANCELED = 4194304;
    internal const int TASK_STATE_WAITING_ON_CHILDREN = 8388608;
    internal const int TASK_STATE_RAN_TO_COMPLETION = 16777216;
    internal const int TASK_STATE_WAITINGFORACTIVATION = 33554432;
    internal const int TASK_STATE_COMPLETION_RESERVED = 67108864;
    internal const int TASK_STATE_THREAD_WAS_ABORTED = 134217728;
    internal const int TASK_STATE_WAIT_COMPLETION_NOTIFICATION = 268435456;
    private const int TASK_STATE_COMPLETED_MASK = 23068672;
    private const int CANCELLATION_REQUESTED = 1;
    private volatile object m_continuationObject;
    private static readonly object s_taskCompletionSentinel = new object();
    internal static bool s_asyncDebuggingEnabled;
    internal volatile Task.ContingentProperties m_contingentProperties;
    private static readonly Action<object> s_taskCancelCallback = new Action<object>(Task.TaskCancelCallback);
    [ThreadStatic]
    internal static Task t_currentTask;
    [ThreadStatic]
    private static StackGuard t_stackGuard;
    private static readonly Func<Task.ContingentProperties> s_createContingentProperties = (Func<Task.ContingentProperties>) (() => new Task.ContingentProperties());
    private static readonly Predicate<Task> s_IsExceptionObservedByParentPredicate = (Predicate<Task>) (t => t.IsExceptionObservedByParent);
    private static ContextCallback s_ecCallback;
    private static readonly Predicate<object> s_IsTaskContinuationNullPredicate = (Predicate<object>) (tc => tc == null);
    private static readonly Dictionary<int, Task> s_currentActiveTasks = new Dictionary<int, Task>();
    private static readonly object s_activeTasksLock = new object();

    private Task ParentForDebugger => this.m_parent;

    private int StateFlagsForDebugger => this.m_stateFlags;

    internal Task(bool canceled, TaskCreationOptions creationOptions, CancellationToken ct)
    {
      int num = (int) creationOptions;
      if (canceled)
      {
        this.m_stateFlags = 5242880 | num;
        Task.ContingentProperties contingentProperties;
        this.m_contingentProperties = contingentProperties = new Task.ContingentProperties();
        contingentProperties.m_cancellationToken = ct;
        contingentProperties.m_internalCancellationRequested = 1;
      }
      else
        this.m_stateFlags = 16777216 | num;
    }

    internal Task() => this.m_stateFlags = 33555456;

    internal Task(object state, TaskCreationOptions creationOptions, bool promiseStyle)
    {
      if ((creationOptions & ~(TaskCreationOptions.AttachedToParent | TaskCreationOptions.RunContinuationsAsynchronously)) != TaskCreationOptions.None)
        throw new ArgumentOutOfRangeException(nameof (creationOptions));
      if ((creationOptions & TaskCreationOptions.AttachedToParent) != TaskCreationOptions.None)
        this.m_parent = Task.InternalCurrent;
      this.TaskConstructorCore((Delegate) null, state, new CancellationToken(), creationOptions, InternalTaskOptions.PromiseTask, (TaskScheduler) null);
    }

    public Task(Action action)
      : this((Delegate) action, (object) null, (Task) null, new CancellationToken(), TaskCreationOptions.None, InternalTaskOptions.None, (TaskScheduler) null)
    {
    }

    public Task(Action action, CancellationToken cancellationToken)
      : this((Delegate) action, (object) null, (Task) null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, (TaskScheduler) null)
    {
    }

    public Task(Action action, TaskCreationOptions creationOptions)
      : this((Delegate) action, (object) null, Task.InternalCurrentIfAttached(creationOptions), new CancellationToken(), creationOptions, InternalTaskOptions.None, (TaskScheduler) null)
    {
    }

    public Task(
      Action action,
      CancellationToken cancellationToken,
      TaskCreationOptions creationOptions)
      : this((Delegate) action, (object) null, Task.InternalCurrentIfAttached(creationOptions), cancellationToken, creationOptions, InternalTaskOptions.None, (TaskScheduler) null)
    {
    }

    public Task(Action<object> action, object state)
      : this((Delegate) action, state, (Task) null, new CancellationToken(), TaskCreationOptions.None, InternalTaskOptions.None, (TaskScheduler) null)
    {
    }

    public Task(Action<object> action, object state, CancellationToken cancellationToken)
      : this((Delegate) action, state, (Task) null, cancellationToken, TaskCreationOptions.None, InternalTaskOptions.None, (TaskScheduler) null)
    {
    }

    public Task(Action<object> action, object state, TaskCreationOptions creationOptions)
      : this((Delegate) action, state, Task.InternalCurrentIfAttached(creationOptions), new CancellationToken(), creationOptions, InternalTaskOptions.None, (TaskScheduler) null)
    {
    }

    public Task(
      Action<object> action,
      object state,
      CancellationToken cancellationToken,
      TaskCreationOptions creationOptions)
      : this((Delegate) action, state, Task.InternalCurrentIfAttached(creationOptions), cancellationToken, creationOptions, InternalTaskOptions.None, (TaskScheduler) null)
    {
    }

    internal Task(
      Delegate action,
      object state,
      Task parent,
      CancellationToken cancellationToken,
      TaskCreationOptions creationOptions,
      InternalTaskOptions internalOptions,
      TaskScheduler scheduler)
    {
      if ((object) action == null)
        throw new ArgumentNullException(nameof (action));
      if ((creationOptions & TaskCreationOptions.AttachedToParent) != TaskCreationOptions.None)
        this.m_parent = parent;
      this.TaskConstructorCore(action, state, cancellationToken, creationOptions, internalOptions, scheduler);
    }

    internal void TaskConstructorCore(
      Delegate action,
      object state,
      CancellationToken cancellationToken,
      TaskCreationOptions creationOptions,
      InternalTaskOptions internalOptions,
      TaskScheduler scheduler)
    {
      this.m_action = action;
      this.m_stateObject = state;
      this.m_taskScheduler = scheduler;
      if ((creationOptions & ~(TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent | TaskCreationOptions.DenyChildAttach | TaskCreationOptions.HideScheduler | TaskCreationOptions.RunContinuationsAsynchronously)) != TaskCreationOptions.None)
        throw new ArgumentOutOfRangeException(nameof (creationOptions));
      int num = (int) (creationOptions | (TaskCreationOptions) internalOptions);
      if ((object) this.m_action == null || (internalOptions & InternalTaskOptions.ContinuationTask) != InternalTaskOptions.None)
        num |= 33554432;
      this.m_stateFlags = num;
      if (this.m_parent != null && (creationOptions & TaskCreationOptions.AttachedToParent) != TaskCreationOptions.None && (this.m_parent.CreationOptions & TaskCreationOptions.DenyChildAttach) == TaskCreationOptions.None)
        this.m_parent.AddNewChild();
      if (cancellationToken.CanBeCanceled)
        this.AssignCancellationToken(cancellationToken, (Task) null, (TaskContinuation) null);
      this.CapturedContext = ExecutionContext.Capture();
    }

    private void AssignCancellationToken(
      CancellationToken cancellationToken,
      Task antecedent,
      TaskContinuation continuation)
    {
      Task.ContingentProperties contingentProperties = this.EnsureContingentPropertiesInitialized(false);
      contingentProperties.m_cancellationToken = cancellationToken;
      try
      {
        if ((this.Options & (TaskCreationOptions) 13312) != TaskCreationOptions.None)
          return;
        if (cancellationToken.IsCancellationRequested)
        {
          this.InternalCancel(false);
        }
        else
        {
          CancellationTokenRegistration tokenRegistration = antecedent != null ? cancellationToken.InternalRegisterWithoutEC(Task.s_taskCancelCallback, (object) new Tuple<Task, Task, TaskContinuation>(this, antecedent, continuation)) : cancellationToken.InternalRegisterWithoutEC(Task.s_taskCancelCallback, (object) this);
          contingentProperties.m_cancellationRegistration = (object) tokenRegistration;
        }
      }
      catch
      {
        if (this.m_parent != null && (this.Options & TaskCreationOptions.AttachedToParent) != TaskCreationOptions.None && (this.m_parent.Options & TaskCreationOptions.DenyChildAttach) == TaskCreationOptions.None)
          this.m_parent.DisregardChild();
        throw;
      }
    }

    private static void TaskCancelCallback(object o)
    {
      switch (o)
      {
        case Tuple<Task, Task, TaskContinuation> tuple:
          pattern_0 = tuple.Item1;
          tuple.Item2.RemoveContinuation((object) tuple.Item3);
          break;
      }
      pattern_0.InternalCancel(false);
    }

    internal bool TrySetCanceled(CancellationToken tokenToRecord)
    {
      return this.TrySetCanceled(tokenToRecord, (object) null);
    }

    internal bool TrySetCanceled(CancellationToken tokenToRecord, object cancellationException)
    {
      bool flag = false;
      if (this.AtomicStateUpdate(67108864, 90177536))
      {
        this.RecordInternalCancellationRequest(tokenToRecord, cancellationException);
        this.CancellationCleanupLogic();
        flag = true;
      }
      return flag;
    }

    internal bool TrySetException(object exceptionObject)
    {
      bool flag = false;
      this.EnsureContingentPropertiesInitialized(true);
      if (this.AtomicStateUpdate(67108864, 90177536))
      {
        this.AddException(exceptionObject);
        this.Finish(false);
        flag = true;
      }
      return flag;
    }

    private string DebuggerDisplayMethodDescription
    {
      get
      {
        Delegate action = this.m_action;
        return (object) action == null ? "{null}" : "0x" + action.GetNativeFunctionPointer().ToString("x");
      }
    }

    internal TaskCreationOptions Options => Task.OptionsMethod(this.m_stateFlags);

    internal static TaskCreationOptions OptionsMethod(int flags)
    {
      return (TaskCreationOptions) (flags & (int) ushort.MaxValue);
    }

    internal bool AtomicStateUpdate(int newBits, int illegalBits)
    {
      System.Threading.SpinWait spinWait = new System.Threading.SpinWait();
      while (true)
      {
        int stateFlags = this.m_stateFlags;
        if ((stateFlags & illegalBits) == 0)
        {
          if (Interlocked.CompareExchange(ref this.m_stateFlags, stateFlags | newBits, stateFlags) != stateFlags)
            spinWait.SpinOnce();
          else
            goto label_4;
        }
        else
          break;
      }
      return false;
label_4:
      return true;
    }

    internal bool AtomicStateUpdate(int newBits, int illegalBits, ref int oldFlags)
    {
      System.Threading.SpinWait spinWait = new System.Threading.SpinWait();
      while (true)
      {
        oldFlags = this.m_stateFlags;
        if ((oldFlags & illegalBits) == 0)
        {
          if (Interlocked.CompareExchange(ref this.m_stateFlags, oldFlags | newBits, oldFlags) != oldFlags)
            spinWait.SpinOnce();
          else
            goto label_4;
        }
        else
          break;
      }
      return false;
label_4:
      return true;
    }

    internal void SetNotificationForWaitCompletion(bool enabled)
    {
      if (enabled)
      {
        this.AtomicStateUpdate(268435456, 90177536);
      }
      else
      {
        System.Threading.SpinWait spinWait = new System.Threading.SpinWait();
        while (true)
        {
          int stateFlags = this.m_stateFlags;
          if (Interlocked.CompareExchange(ref this.m_stateFlags, stateFlags & -268435457, stateFlags) != stateFlags)
            spinWait.SpinOnce();
          else
            break;
        }
      }
    }

    internal bool NotifyDebuggerOfWaitCompletionIfNecessary()
    {
      if (!this.IsWaitNotificationEnabled || !this.ShouldNotifyDebuggerOfWaitCompletion)
        return false;
      this.NotifyDebuggerOfWaitCompletion();
      return true;
    }

    internal static bool AnyTaskRequiresNotifyDebuggerOfWaitCompletion(Task[] tasks)
    {
      foreach (Task task in tasks)
      {
        if (task != null && task.IsWaitNotificationEnabled && task.ShouldNotifyDebuggerOfWaitCompletion)
          return true;
      }
      return false;
    }

    internal bool IsWaitNotificationEnabledOrNotRanToCompletion
    {
      [MethodImpl(MethodImplOptions.AggressiveInlining)] get
      {
        return (this.m_stateFlags & 285212672) != 16777216;
      }
    }

    internal virtual bool ShouldNotifyDebuggerOfWaitCompletion => this.IsWaitNotificationEnabled;

    internal bool IsWaitNotificationEnabled => (this.m_stateFlags & 268435456) != 0;

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private void NotifyDebuggerOfWaitCompletion() => this.SetNotificationForWaitCompletion(false);

    internal bool MarkStarted() => this.AtomicStateUpdate(65536, 4259840);

    internal void AddNewChild()
    {
      Task.ContingentProperties contingentProperties = this.EnsureContingentPropertiesInitialized(true);
      if (contingentProperties.m_completionCountdown == 1)
        ++contingentProperties.m_completionCountdown;
      else
        Interlocked.Increment(ref contingentProperties.m_completionCountdown);
    }

    internal void DisregardChild()
    {
      Interlocked.Decrement(ref this.EnsureContingentPropertiesInitialized(true).m_completionCountdown);
    }

    public void Start() => this.Start(TaskScheduler.Current);

    public void Start(TaskScheduler scheduler)
    {
      int stateFlags = this.m_stateFlags;
      if (Task.IsCompletedMethod(stateFlags))
        throw new InvalidOperationException("Start may not be called on a task that has completed.");
      if (scheduler == null)
        throw new ArgumentNullException(nameof (scheduler));
      int num = (int) Task.OptionsMethod(stateFlags);
      if ((num & 1024) != 0)
        throw new InvalidOperationException("Start may not be called on a promise-style task.");
      if ((num & 512) != 0)
        throw new InvalidOperationException("Start may not be called on a continuation task.");
      if (Interlocked.CompareExchange<TaskScheduler>(ref this.m_taskScheduler, scheduler, (TaskScheduler) null) != null)
        throw new InvalidOperationException("Start may not be called on a task that was already started.");
      this.ScheduleAndStart(true);
    }

    public void RunSynchronously() => this.InternalRunSynchronously(TaskScheduler.Current, true);

    public void RunSynchronously(TaskScheduler scheduler)
    {
      if (scheduler == null)
        throw new ArgumentNullException(nameof (scheduler));
      this.InternalRunSynchronously(scheduler, true);
    }

    internal void InternalRunSynchronously(TaskScheduler scheduler, bool waitForCompletion)
    {
      int stateFlags = this.m_stateFlags;
      int num = (int) Task.OptionsMethod(stateFlags);
      if ((num & 512) != 0)
        throw new InvalidOperationException("RunSynchronously may not be called on a continuation task.");
      if ((num & 1024) != 0)
        throw new InvalidOperationException("RunSynchronously may not be called on a task not bound to a delegate, such as the task returned from an asynchronous method.");
      if (Task.IsCompletedMethod(stateFlags))
        throw new InvalidOperationException("RunSynchronously may not be called on a task that has already completed.");
      if (Interlocked.CompareExchange<TaskScheduler>(ref this.m_taskScheduler, scheduler, (TaskScheduler) null) != null)
        throw new InvalidOperationException("RunSynchronously may not be called on a task that was already started.");
      if (!this.MarkStarted())
        throw new InvalidOperationException("RunSynchronously may not be called on a task that has already completed.");
      bool flag = false;
      try
      {
        if (!scheduler.TryRunInline(this, false))
        {
          scheduler.QueueTask(this);
          flag = true;
        }
        if (!waitForCompletion || this.IsCompleted)
          return;
        this.SpinThenBlockingWait(-1, new CancellationToken());
      }
      catch (System.Exception ex)
      {
        if (!flag)
        {
          TaskSchedulerException exceptionObject = new TaskSchedulerException(ex);
          this.AddException((object) exceptionObject);
          this.Finish(false);
          this.m_contingentProperties.m_exceptionsHolder.MarkAsHandled(false);
          throw exceptionObject;
        }
        throw;
      }
    }

    internal static Task InternalStartNew(
      Task creatingTask,
      Delegate action,
      object state,
      CancellationToken cancellationToken,
      TaskScheduler scheduler,
      TaskCreationOptions options,
      InternalTaskOptions internalOptions)
    {
      if (scheduler == null)
        throw new ArgumentNullException(nameof (scheduler));
      Task task = new Task(action, state, creatingTask, cancellationToken, options, internalOptions | InternalTaskOptions.QueuedByRuntime, scheduler);
      task.ScheduleAndStart(false);
      return task;
    }

    public int Id
    {
      get
      {
        if (this.m_taskId == 0)
        {
          int num;
          do
          {
            num = Interlocked.Increment(ref Task.s_taskIdCounter);
          }
          while (num == 0);
          Interlocked.CompareExchange(ref this.m_taskId, num, 0);
        }
        return this.m_taskId;
      }
    }

    public static int? CurrentId => Task.InternalCurrent?.Id;

    internal static Task InternalCurrent => Task.t_currentTask;

    internal static Task InternalCurrentIfAttached(TaskCreationOptions creationOptions)
    {
      return (creationOptions & TaskCreationOptions.AttachedToParent) == TaskCreationOptions.None ? (Task) null : Task.InternalCurrent;
    }

    internal static StackGuard CurrentStackGuard
    {
      get
      {
        StackGuard currentStackGuard = Task.t_stackGuard;
        if (currentStackGuard == null)
          Task.t_stackGuard = currentStackGuard = new StackGuard();
        return currentStackGuard;
      }
    }

    public AggregateException Exception
    {
      get
      {
        AggregateException exception = (AggregateException) null;
        if (this.IsFaulted)
          exception = this.GetExceptions(false);
        return exception;
      }
    }

    public TaskStatus Status
    {
      get
      {
        int stateFlags = this.m_stateFlags;
        return (stateFlags & 2097152) == 0 ? ((stateFlags & 4194304) == 0 ? ((stateFlags & 16777216) == 0 ? ((stateFlags & 8388608) == 0 ? ((stateFlags & 131072) == 0 ? ((stateFlags & 65536) == 0 ? ((stateFlags & 33554432) == 0 ? TaskStatus.Created : TaskStatus.WaitingForActivation) : TaskStatus.WaitingToRun) : TaskStatus.Running) : TaskStatus.WaitingForChildrenToComplete) : TaskStatus.RanToCompletion) : TaskStatus.Canceled) : TaskStatus.Faulted;
      }
    }

    public bool IsCanceled => (this.m_stateFlags & 6291456) == 4194304;

    internal bool IsCancellationRequested
    {
      get
      {
        Task.ContingentProperties contingentProperties = this.m_contingentProperties;
        if (contingentProperties == null)
          return false;
        return contingentProperties.m_internalCancellationRequested == 1 || contingentProperties.m_cancellationToken.IsCancellationRequested;
      }
    }

    internal Task.ContingentProperties EnsureContingentPropertiesInitialized(bool needsProtection)
    {
      return this.m_contingentProperties ?? this.EnsureContingentPropertiesInitializedCore(needsProtection);
    }

    private Task.ContingentProperties EnsureContingentPropertiesInitializedCore(bool needsProtection)
    {
      return needsProtection ? LazyInitializer.EnsureInitialized<Task.ContingentProperties>(ref this.m_contingentProperties, Task.s_createContingentProperties) : (this.m_contingentProperties = new Task.ContingentProperties());
    }

    internal CancellationToken CancellationToken
    {
      get
      {
        Task.ContingentProperties contingentProperties = this.m_contingentProperties;
        return contingentProperties != null ? contingentProperties.m_cancellationToken : new CancellationToken();
      }
    }

    internal bool IsCancellationAcknowledged => (this.m_stateFlags & 1048576) != 0;

    public bool IsCompleted => Task.IsCompletedMethod(this.m_stateFlags);

    private static bool IsCompletedMethod(int flags) => (flags & 23068672) != 0;

    public bool IsCompletedSuccessfully => (this.m_stateFlags & 23068672) == 16777216;

    public TaskCreationOptions CreationOptions => this.Options & (TaskCreationOptions) -65281;

    WaitHandle IAsyncResult.AsyncWaitHandle
    {
      get
      {
        if ((this.m_stateFlags & 262144) != 0)
          throw new ObjectDisposedException((string) null, "The task has been disposed.");
        return this.CompletedEvent.WaitHandle;
      }
    }

    public object AsyncState => this.m_stateObject;

    bool IAsyncResult.CompletedSynchronously => false;

    internal TaskScheduler ExecutingTaskScheduler => this.m_taskScheduler;

    public static TaskFactory Factory { get; } = new TaskFactory();

    public static Task CompletedTask { get; } = new Task(false, (TaskCreationOptions) 16384, new CancellationToken());

    internal ManualResetEventSlim CompletedEvent
    {
      get
      {
        Task.ContingentProperties contingentProperties = this.EnsureContingentPropertiesInitialized(true);
        if (contingentProperties.m_completionEvent == null)
        {
          bool isCompleted = this.IsCompleted;
          ManualResetEventSlim manualResetEventSlim = new ManualResetEventSlim(isCompleted);
          if (Interlocked.CompareExchange<ManualResetEventSlim>(ref contingentProperties.m_completionEvent, manualResetEventSlim, (ManualResetEventSlim) null) != null)
            manualResetEventSlim.Dispose();
          else if (!isCompleted && this.IsCompleted)
            manualResetEventSlim.Set();
        }
        return contingentProperties.m_completionEvent;
      }
    }

    internal bool ExceptionRecorded
    {
      get
      {
        Task.ContingentProperties contingentProperties = this.m_contingentProperties;
        return contingentProperties != null && contingentProperties.m_exceptionsHolder != null && contingentProperties.m_exceptionsHolder.ContainsFaultList;
      }
    }

    public bool IsFaulted => (this.m_stateFlags & 2097152) != 0;

    internal ExecutionContext CapturedContext
    {
      get
      {
        Task.ContingentProperties contingentProperties = this.m_contingentProperties;
        return contingentProperties != null && contingentProperties.m_capturedContext != null ? contingentProperties.m_capturedContext : ExecutionContext.Default;
      }
      set
      {
        if (value == ExecutionContext.Default)
          return;
        this.EnsureContingentPropertiesInitialized(false).m_capturedContext = value;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if ((this.Options & (TaskCreationOptions) 16384) != TaskCreationOptions.None)
          return;
        if (!this.IsCompleted)
          throw new InvalidOperationException("A task may only be disposed if it is in a completion state (RanToCompletion, Faulted or Canceled).");
        Task.ContingentProperties contingentProperties = Volatile.Read<Task.ContingentProperties>(ref this.m_contingentProperties);
        if (contingentProperties != null)
        {
          ManualResetEventSlim completionEvent = contingentProperties.m_completionEvent;
          if (completionEvent != null)
          {
            contingentProperties.m_completionEvent = (ManualResetEventSlim) null;
            if (!completionEvent.IsSet)
              completionEvent.Set();
            completionEvent.Dispose();
          }
        }
      }
      this.m_stateFlags |= 262144;
    }

    internal void ScheduleAndStart(bool needsProtection)
    {
      if (needsProtection)
      {
        if (!this.MarkStarted())
          return;
      }
      else
        this.m_stateFlags |= 65536;
      DebuggerSupport.AddToActiveTasks(this);
      if (DebuggerSupport.LoggingOn && (this.Options & (TaskCreationOptions) 512) == TaskCreationOptions.None)
        DebuggerSupport.TraceOperationCreation(CausalityTraceLevel.Required, this, "Task: " + this.m_action?.ToString(), 0UL);
      try
      {
        this.m_taskScheduler.QueueTask(this);
      }
      catch (System.Exception ex)
      {
        TaskSchedulerException exceptionObject = new TaskSchedulerException(ex);
        this.AddException((object) exceptionObject);
        this.Finish(false);
        if ((this.Options & (TaskCreationOptions) 512) == TaskCreationOptions.None)
          this.m_contingentProperties.m_exceptionsHolder.MarkAsHandled(false);
        throw exceptionObject;
      }
    }

    internal void AddException(object exceptionObject) => this.AddException(exceptionObject, false);

    internal void AddException(object exceptionObject, bool representsCancellation)
    {
      Task.ContingentProperties contingentProperties = this.EnsureContingentPropertiesInitialized(true);
      if (contingentProperties.m_exceptionsHolder == null)
      {
        TaskExceptionHolder taskExceptionHolder = new TaskExceptionHolder(this);
        if (Interlocked.CompareExchange<TaskExceptionHolder>(ref contingentProperties.m_exceptionsHolder, taskExceptionHolder, (TaskExceptionHolder) null) != null)
          taskExceptionHolder.MarkAsHandled(false);
      }
      lock (contingentProperties)
        contingentProperties.m_exceptionsHolder.Add(exceptionObject, representsCancellation);
    }

    private AggregateException GetExceptions(bool includeTaskCanceledExceptions)
    {
      System.Exception includeThisException = (System.Exception) null;
      if (includeTaskCanceledExceptions && this.IsCanceled)
        includeThisException = (System.Exception) new TaskCanceledException(this);
      if (this.ExceptionRecorded)
        return this.m_contingentProperties.m_exceptionsHolder.CreateExceptionObject(false, includeThisException);
      if (includeThisException == null)
        return (AggregateException) null;
      return new AggregateException(new System.Exception[1]
      {
        includeThisException
      });
    }

    internal ReadOnlyCollection<ExceptionDispatchInfo> GetExceptionDispatchInfos()
    {
      return (!this.IsFaulted ? 0 : (this.ExceptionRecorded ? 1 : 0)) == 0 ? new ReadOnlyCollection<ExceptionDispatchInfo>((IList<ExceptionDispatchInfo>) Array.Empty<ExceptionDispatchInfo>()) : this.m_contingentProperties.m_exceptionsHolder.GetExceptionDispatchInfos();
    }

    internal ExceptionDispatchInfo GetCancellationExceptionDispatchInfo()
    {
      return this.m_contingentProperties?.m_exceptionsHolder?.GetCancellationExceptionDispatchInfo();
    }

    internal void ThrowIfExceptional(bool includeTaskCanceledExceptions)
    {
      System.Exception exceptions = (System.Exception) this.GetExceptions(includeTaskCanceledExceptions);
      if (exceptions != null)
      {
        this.UpdateExceptionObservedStatus();
        throw exceptions;
      }
    }

    internal void UpdateExceptionObservedStatus()
    {
      if (this.m_parent == null || (this.Options & TaskCreationOptions.AttachedToParent) == TaskCreationOptions.None || (this.m_parent.CreationOptions & TaskCreationOptions.DenyChildAttach) != TaskCreationOptions.None || Task.InternalCurrent != this.m_parent)
        return;
      this.m_stateFlags |= 524288;
    }

    internal bool IsExceptionObservedByParent => (this.m_stateFlags & 524288) != 0;

    internal bool IsDelegateInvoked => (this.m_stateFlags & 131072) != 0;

    internal void Finish(bool bUserDelegateExecuted)
    {
      if (!bUserDelegateExecuted)
      {
        this.FinishStageTwo();
      }
      else
      {
        Task.ContingentProperties contingentProperties = this.m_contingentProperties;
        if (contingentProperties == null || contingentProperties.m_completionCountdown == 1 || Interlocked.Decrement(ref contingentProperties.m_completionCountdown) == 0)
          this.FinishStageTwo();
        else
          this.AtomicStateUpdate(8388608, 23068672);
        LowLevelListWithIList<Task> exceptionalChildren = contingentProperties?.m_exceptionalChildren;
        if (exceptionalChildren == null)
          return;
        lock (exceptionalChildren)
          exceptionalChildren.RemoveAll(Task.s_IsExceptionObservedByParentPredicate);
      }
    }

    internal void FinishStageTwo()
    {
      this.AddExceptionsFromChildren();
      int num;
      if (this.ExceptionRecorded)
      {
        num = 2097152;
        if (DebuggerSupport.LoggingOn)
          DebuggerSupport.TraceOperationCompletion(CausalityTraceLevel.Required, this, AsyncStatus.Error);
        DebuggerSupport.RemoveFromActiveTasks(this);
      }
      else if (this.IsCancellationRequested && this.IsCancellationAcknowledged)
      {
        num = 4194304;
        if (DebuggerSupport.LoggingOn)
          DebuggerSupport.TraceOperationCompletion(CausalityTraceLevel.Required, this, AsyncStatus.Canceled);
        DebuggerSupport.RemoveFromActiveTasks(this);
      }
      else
      {
        num = 16777216;
        if (DebuggerSupport.LoggingOn)
          DebuggerSupport.TraceOperationCompletion(CausalityTraceLevel.Required, this, AsyncStatus.Completed);
        DebuggerSupport.RemoveFromActiveTasks(this);
      }
      Interlocked.Exchange(ref this.m_stateFlags, this.m_stateFlags | num);
      Task.ContingentProperties contingentProperties = this.m_contingentProperties;
      if (contingentProperties != null)
      {
        contingentProperties.SetCompleted();
        contingentProperties.UnregisterCancellationCallback();
      }
      this.FinishStageThree();
    }

    internal void FinishStageThree()
    {
      this.m_action = (Delegate) null;
      if (this.m_parent != null && (this.m_parent.CreationOptions & TaskCreationOptions.DenyChildAttach) == TaskCreationOptions.None && (this.m_stateFlags & (int) ushort.MaxValue & 4) != 0)
        this.m_parent.ProcessChildCompletion(this);
      this.FinishContinuations();
    }

    internal void ProcessChildCompletion(Task childTask)
    {
      Task.ContingentProperties contingentProperties = this.m_contingentProperties;
      if (childTask.IsFaulted && !childTask.IsExceptionObservedByParent)
      {
        if (contingentProperties.m_exceptionalChildren == null)
          Interlocked.CompareExchange<LowLevelListWithIList<Task>>(ref contingentProperties.m_exceptionalChildren, new LowLevelListWithIList<Task>(), (LowLevelListWithIList<Task>) null);
        LowLevelListWithIList<Task> exceptionalChildren = contingentProperties.m_exceptionalChildren;
        if (exceptionalChildren != null)
        {
          lock (exceptionalChildren)
            exceptionalChildren.Add(childTask);
        }
      }
      if (Interlocked.Decrement(ref contingentProperties.m_completionCountdown) != 0)
        return;
      this.FinishStageTwo();
    }

    internal void AddExceptionsFromChildren()
    {
      Task.ContingentProperties contingentProperties = this.m_contingentProperties;
      LowLevelListWithIList<Task> exceptionalChildren = contingentProperties?.m_exceptionalChildren;
      if (exceptionalChildren == null)
        return;
      lock (exceptionalChildren)
      {
        foreach (Task task in (IEnumerable<Task>) exceptionalChildren)
        {
          if (task.IsFaulted && !task.IsExceptionObservedByParent)
            this.AddException((object) task.m_contingentProperties.m_exceptionsHolder.CreateExceptionObject(false, (System.Exception) null));
        }
      }
      contingentProperties.m_exceptionalChildren = (LowLevelListWithIList<Task>) null;
    }

    private void Execute()
    {
      try
      {
        this.InnerInvoke();
      }
      catch (System.Exception ex)
      {
        this.HandleException(ex);
      }
    }

    void IThreadPoolWorkItem.ExecuteWorkItem() => this.ExecuteEntry(false);

    internal bool ExecuteEntry(bool bPreventDoubleExecution)
    {
      if (bPreventDoubleExecution)
      {
        int oldFlags = 0;
        if (!this.AtomicStateUpdate(131072, 23199744, ref oldFlags) && (oldFlags & 4194304) == 0)
          return false;
      }
      else
        this.m_stateFlags |= 131072;
      if (!this.IsCancellationRequested && !this.IsCanceled)
        this.ExecuteWithThreadLocal(ref Task.t_currentTask);
      else if (!this.IsCanceled && (Interlocked.Exchange(ref this.m_stateFlags, this.m_stateFlags | 4194304) & 4194304) == 0)
        this.CancellationCleanupLogic();
      return true;
    }

    private static void ExecutionContextCallback(object obj) => (obj as Task).Execute();

    internal virtual void InnerInvoke()
    {
      if (this.m_action is Action action1)
      {
        action1();
      }
      else
      {
        if (!(this.m_action is Action<object> action))
          return;
        action(this.m_stateObject);
      }
    }

    private void HandleException(System.Exception unhandledException)
    {
      if (unhandledException is OperationCanceledException exceptionObject && this.IsCancellationRequested && this.m_contingentProperties.m_cancellationToken == exceptionObject.CancellationToken)
      {
        this.SetCancellationAcknowledged();
        this.AddException((object) exceptionObject, true);
      }
      else
        this.AddException((object) unhandledException);
    }

    public TaskAwaiter GetAwaiter() => new TaskAwaiter(this);

    public ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext)
    {
      return new ConfiguredTaskAwaitable(this, continueOnCapturedContext);
    }

    internal void SetContinuationForAwait(
      Action continuationAction,
      bool continueOnCapturedContext,
      bool flowExecutionContext)
    {
      TaskContinuation tc = (TaskContinuation) null;
      if (continueOnCapturedContext)
      {
        SynchronizationContext current = SynchronizationContext.Current;
        if (current != null && current.GetType() != typeof (SynchronizationContext))
        {
          tc = (TaskContinuation) new SynchronizationContextAwaitTaskContinuation(current, continuationAction, flowExecutionContext);
        }
        else
        {
          TaskScheduler internalCurrent = TaskScheduler.InternalCurrent;
          if (internalCurrent != null && internalCurrent != TaskScheduler.Default)
            tc = (TaskContinuation) new TaskSchedulerAwaitTaskContinuation(internalCurrent, continuationAction, flowExecutionContext);
        }
      }
      if (tc != null)
      {
        if (this.AddTaskContinuation((object) tc, false))
          return;
        tc.Run(this, false);
      }
      else
      {
        if (this.AddTaskContinuation((object) continuationAction, false))
          return;
        AwaitTaskContinuation.UnsafeScheduleAction(continuationAction);
      }
    }

    public static YieldAwaitable Yield() => new YieldAwaitable();

    public void Wait() => this.Wait(-1, new CancellationToken());

    public bool Wait(TimeSpan timeout)
    {
      long totalMilliseconds = (long) timeout.TotalMilliseconds;
      return totalMilliseconds >= -1L && totalMilliseconds <= (long) int.MaxValue ? this.Wait((int) totalMilliseconds, new CancellationToken()) : throw new ArgumentOutOfRangeException(nameof (timeout));
    }

    public void Wait(CancellationToken cancellationToken) => this.Wait(-1, cancellationToken);

    public bool Wait(int millisecondsTimeout)
    {
      return this.Wait(millisecondsTimeout, new CancellationToken());
    }

    public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
    {
      if (millisecondsTimeout < -1)
        throw new ArgumentOutOfRangeException(nameof (millisecondsTimeout));
      if (!this.IsWaitNotificationEnabledOrNotRanToCompletion)
        return true;
      if (!this.InternalWait(millisecondsTimeout, cancellationToken))
        return false;
      if (this.IsWaitNotificationEnabledOrNotRanToCompletion)
      {
        this.NotifyDebuggerOfWaitCompletionIfNecessary();
        if (this.IsCanceled)
          cancellationToken.ThrowIfCancellationRequested();
        this.ThrowIfExceptional(true);
      }
      return true;
    }

    private bool WrappedTryRunInline()
    {
      if (this.m_taskScheduler == null)
        return false;
      try
      {
        return this.m_taskScheduler.TryRunInline(this, true);
      }
      catch (System.Exception ex)
      {
        throw new TaskSchedulerException(ex);
      }
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    internal bool InternalWait(int millisecondsTimeout, CancellationToken cancellationToken)
    {
      if (TaskTrace.Enabled)
      {
        Task internalCurrent = Task.InternalCurrent;
        TaskTrace.TaskWaitBegin_Synchronous(internalCurrent != null ? internalCurrent.m_taskScheduler.Id : TaskScheduler.Default.Id, internalCurrent != null ? internalCurrent.Id : 0, this.Id);
      }
      bool flag = this.IsCompleted;
      if (!flag)
        flag = millisecondsTimeout == -1 && !cancellationToken.CanBeCanceled && this.WrappedTryRunInline() && this.IsCompleted || this.SpinThenBlockingWait(millisecondsTimeout, cancellationToken);
      if (TaskTrace.Enabled)
      {
        Task internalCurrent = Task.InternalCurrent;
        if (internalCurrent != null)
          TaskTrace.TaskWaitEnd(internalCurrent.m_taskScheduler.Id, internalCurrent.Id, this.Id);
        else
          TaskTrace.TaskWaitEnd(TaskScheduler.Default.Id, 0, this.Id);
      }
      return flag;
    }

    private bool SpinThenBlockingWait(int millisecondsTimeout, CancellationToken cancellationToken)
    {
      bool flag1 = millisecondsTimeout == -1;
      uint tickCount = flag1 ? 0U : (uint) Environment.TickCount;
      bool flag2 = this.SpinWait(millisecondsTimeout);
      if (!flag2)
      {
        Task.SetOnInvokeMres setOnInvokeMres = new Task.SetOnInvokeMres();
        try
        {
          this.AddCompletionAction((ITaskCompletionAction) setOnInvokeMres, true);
          if (flag1)
          {
            flag2 = setOnInvokeMres.Wait(-1, cancellationToken);
          }
          else
          {
            uint num = (uint) Environment.TickCount - tickCount;
            if ((long) num < (long) millisecondsTimeout)
              flag2 = setOnInvokeMres.Wait((int) ((long) millisecondsTimeout - (long) num), cancellationToken);
          }
        }
        finally
        {
          if (!this.IsCompleted)
            this.RemoveContinuation((object) setOnInvokeMres);
        }
      }
      return flag2;
    }

    private bool SpinWait(int millisecondsTimeout)
    {
      if (this.IsCompleted)
        return true;
      if (millisecondsTimeout == 0)
        return false;
      int countforSpinBeforeWait = System.Threading.SpinWait.SpinCountforSpinBeforeWait;
      System.Threading.SpinWait spinWait = new System.Threading.SpinWait();
      while (spinWait.Count < countforSpinBeforeWait)
      {
        spinWait.SpinOnce(-1);
        if (this.IsCompleted)
          return true;
      }
      return false;
    }

    internal bool InternalCancel(bool bCancelNonExecutingOnly)
    {
      bool flag1 = false;
      bool flag2 = false;
      TaskSchedulerException schedulerException = (TaskSchedulerException) null;
      if ((this.m_stateFlags & 65536) != 0)
      {
        TaskScheduler taskScheduler = this.m_taskScheduler;
        try
        {
          flag1 = taskScheduler != null && taskScheduler.TryDequeue(this);
        }
        catch (System.Exception ex)
        {
          schedulerException = new TaskSchedulerException(ex);
        }
        bool flag3 = taskScheduler != null && taskScheduler.RequiresAtomicStartTransition;
        if (!flag1 & bCancelNonExecutingOnly & flag3)
          flag2 = this.AtomicStateUpdate(4194304, 4325376);
      }
      if (!bCancelNonExecutingOnly | flag1 | flag2)
      {
        this.RecordInternalCancellationRequest();
        if (flag1)
          flag2 = this.AtomicStateUpdate(4194304, 4325376);
        else if (!flag2 && (this.m_stateFlags & 65536) == 0)
          flag2 = this.AtomicStateUpdate(4194304, 23265280);
        if (flag2)
          this.CancellationCleanupLogic();
      }
      if (schedulerException != null)
        throw schedulerException;
      return flag2;
    }

    internal void RecordInternalCancellationRequest()
    {
      this.EnsureContingentPropertiesInitialized(true).m_internalCancellationRequested = 1;
    }

    internal void RecordInternalCancellationRequest(CancellationToken tokenToRecord)
    {
      this.RecordInternalCancellationRequest();
      if (!(tokenToRecord != new CancellationToken()))
        return;
      this.m_contingentProperties.m_cancellationToken = tokenToRecord;
    }

    internal void RecordInternalCancellationRequest(
      CancellationToken tokenToRecord,
      object cancellationException)
    {
      this.RecordInternalCancellationRequest(tokenToRecord);
      if (cancellationException == null)
        return;
      this.AddException(cancellationException, true);
    }

    internal void CancellationCleanupLogic()
    {
      Interlocked.Exchange(ref this.m_stateFlags, this.m_stateFlags | 4194304);
      Task.ContingentProperties contingentProperties = this.m_contingentProperties;
      if (contingentProperties != null)
      {
        contingentProperties.SetCompleted();
        contingentProperties.UnregisterCancellationCallback();
      }
      if (DebuggerSupport.LoggingOn)
        DebuggerSupport.TraceOperationCompletion(CausalityTraceLevel.Required, this, AsyncStatus.Canceled);
      DebuggerSupport.RemoveFromActiveTasks(this);
      this.FinishStageThree();
    }

    private void SetCancellationAcknowledged() => this.m_stateFlags |= 1048576;

    internal void FinishContinuations()
    {
      object obj1 = Interlocked.Exchange(ref this.m_continuationObject, Task.s_taskCompletionSentinel);
      if (obj1 == null)
        return;
      if (DebuggerSupport.LoggingOn)
        DebuggerSupport.TraceSynchronousWorkStart(CausalityTraceLevel.Required, this, CausalitySynchronousWork.CompletionNotification);
      bool flag = (this.m_stateFlags & 134217728) == 0 && (this.m_stateFlags & 64) == 0;
      switch (obj1)
      {
        case Action action2:
          AwaitTaskContinuation.RunOrScheduleAction(action2, flag, ref Task.t_currentTask);
          this.LogFinishCompletionNotification();
          break;
        case ITaskCompletionAction action3:
          if (flag || !action3.InvokeMayRunArbitraryCode)
            action3.Invoke(this);
          else
            ThreadPool.UnsafeQueueCustomWorkItem((IThreadPoolWorkItem) new CompletionActionInvoker(action3, this), false);
          this.LogFinishCompletionNotification();
          break;
        case TaskContinuation taskContinuation1:
          taskContinuation1.Run(this, flag);
          this.LogFinishCompletionNotification();
          break;
        case LowLevelListWithIList<object> levelListWithIlist:
          lock (levelListWithIlist)
            ;
          int count = levelListWithIlist.Count;
          for (int index = 0; index < count; ++index)
          {
            if (levelListWithIlist[index] is StandardTaskContinuation taskContinuation && (taskContinuation.m_options & TaskContinuationOptions.ExecuteSynchronously) == TaskContinuationOptions.None)
            {
              levelListWithIlist[index] = (object) null;
              taskContinuation.Run(this, flag);
            }
          }
          for (int index = 0; index < count; ++index)
          {
            object obj2 = levelListWithIlist[index];
            if (obj2 != null)
            {
              levelListWithIlist[index] = (object) null;
              if (obj2 is Action action1)
                AwaitTaskContinuation.RunOrScheduleAction(action1, flag, ref Task.t_currentTask);
              else if (obj2 is TaskContinuation taskContinuation)
              {
                taskContinuation.Run(this, flag);
              }
              else
              {
                ITaskCompletionAction action = (ITaskCompletionAction) obj2;
                if (flag || !action.InvokeMayRunArbitraryCode)
                  action.Invoke(this);
                else
                  ThreadPool.UnsafeQueueCustomWorkItem((IThreadPoolWorkItem) new CompletionActionInvoker(action, this), false);
              }
            }
          }
          this.LogFinishCompletionNotification();
          break;
        default:
          this.LogFinishCompletionNotification();
          break;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LogFinishCompletionNotification()
    {
      if (!DebuggerSupport.LoggingOn)
        return;
      DebuggerSupport.TraceSynchronousWorkCompletion(CausalityTraceLevel.Required, CausalitySynchronousWork.CompletionNotification);
    }

    public Task ContinueWith(Action<Task> continuationAction)
    {
      return this.ContinueWith(continuationAction, TaskScheduler.Current, new CancellationToken(), TaskContinuationOptions.None);
    }

    public Task ContinueWith(Action<Task> continuationAction, CancellationToken cancellationToken)
    {
      return this.ContinueWith(continuationAction, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
    }

    public Task ContinueWith(Action<Task> continuationAction, TaskScheduler scheduler)
    {
      return this.ContinueWith(continuationAction, scheduler, new CancellationToken(), TaskContinuationOptions.None);
    }

    public Task ContinueWith(
      Action<Task> continuationAction,
      TaskContinuationOptions continuationOptions)
    {
      return this.ContinueWith(continuationAction, TaskScheduler.Current, new CancellationToken(), continuationOptions);
    }

    public Task ContinueWith(
      Action<Task> continuationAction,
      CancellationToken cancellationToken,
      TaskContinuationOptions continuationOptions,
      TaskScheduler scheduler)
    {
      return this.ContinueWith(continuationAction, scheduler, cancellationToken, continuationOptions);
    }

    private Task ContinueWith(
      Action<Task> continuationAction,
      TaskScheduler scheduler,
      CancellationToken cancellationToken,
      TaskContinuationOptions continuationOptions)
    {
      if (continuationAction == null)
        throw new ArgumentNullException(nameof (continuationAction));
      if (scheduler == null)
        throw new ArgumentNullException(nameof (scheduler));
      TaskCreationOptions creationOptions;
      InternalTaskOptions internalOptions;
      Task.CreationOptionsFromContinuationOptions(continuationOptions, out creationOptions, out internalOptions);
      Task continuationTask = (Task) new ContinuationTaskFromTask(this, (Delegate) continuationAction, (object) null, creationOptions, internalOptions);
      this.ContinueWithCore(continuationTask, scheduler, cancellationToken, continuationOptions);
      return continuationTask;
    }

    public Task ContinueWith(Action<Task, object> continuationAction, object state)
    {
      return this.ContinueWith(continuationAction, state, TaskScheduler.Current, new CancellationToken(), TaskContinuationOptions.None);
    }

    public Task ContinueWith(
      Action<Task, object> continuationAction,
      object state,
      CancellationToken cancellationToken)
    {
      return this.ContinueWith(continuationAction, state, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
    }

    public Task ContinueWith(
      Action<Task, object> continuationAction,
      object state,
      TaskScheduler scheduler)
    {
      return this.ContinueWith(continuationAction, state, scheduler, new CancellationToken(), TaskContinuationOptions.None);
    }

    public Task ContinueWith(
      Action<Task, object> continuationAction,
      object state,
      TaskContinuationOptions continuationOptions)
    {
      return this.ContinueWith(continuationAction, state, TaskScheduler.Current, new CancellationToken(), continuationOptions);
    }

    public Task ContinueWith(
      Action<Task, object> continuationAction,
      object state,
      CancellationToken cancellationToken,
      TaskContinuationOptions continuationOptions,
      TaskScheduler scheduler)
    {
      return this.ContinueWith(continuationAction, state, scheduler, cancellationToken, continuationOptions);
    }

    private Task ContinueWith(
      Action<Task, object> continuationAction,
      object state,
      TaskScheduler scheduler,
      CancellationToken cancellationToken,
      TaskContinuationOptions continuationOptions)
    {
      if (continuationAction == null)
        throw new ArgumentNullException(nameof (continuationAction));
      if (scheduler == null)
        throw new ArgumentNullException(nameof (scheduler));
      TaskCreationOptions creationOptions;
      InternalTaskOptions internalOptions;
      Task.CreationOptionsFromContinuationOptions(continuationOptions, out creationOptions, out internalOptions);
      Task continuationTask = (Task) new ContinuationTaskFromTask(this, (Delegate) continuationAction, state, creationOptions, internalOptions);
      this.ContinueWithCore(continuationTask, scheduler, cancellationToken, continuationOptions);
      return continuationTask;
    }

    public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction)
    {
      return this.ContinueWith<TResult>(continuationFunction, TaskScheduler.Current, new CancellationToken(), TaskContinuationOptions.None);
    }

    public Task<TResult> ContinueWith<TResult>(
      Func<Task, TResult> continuationFunction,
      CancellationToken cancellationToken)
    {
      return this.ContinueWith<TResult>(continuationFunction, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
    }

    public Task<TResult> ContinueWith<TResult>(
      Func<Task, TResult> continuationFunction,
      TaskScheduler scheduler)
    {
      return this.ContinueWith<TResult>(continuationFunction, scheduler, new CancellationToken(), TaskContinuationOptions.None);
    }

    public Task<TResult> ContinueWith<TResult>(
      Func<Task, TResult> continuationFunction,
      TaskContinuationOptions continuationOptions)
    {
      return this.ContinueWith<TResult>(continuationFunction, TaskScheduler.Current, new CancellationToken(), continuationOptions);
    }

    public Task<TResult> ContinueWith<TResult>(
      Func<Task, TResult> continuationFunction,
      CancellationToken cancellationToken,
      TaskContinuationOptions continuationOptions,
      TaskScheduler scheduler)
    {
      return this.ContinueWith<TResult>(continuationFunction, scheduler, cancellationToken, continuationOptions);
    }

    private Task<TResult> ContinueWith<TResult>(
      Func<Task, TResult> continuationFunction,
      TaskScheduler scheduler,
      CancellationToken cancellationToken,
      TaskContinuationOptions continuationOptions)
    {
      if (continuationFunction == null)
        throw new ArgumentNullException(nameof (continuationFunction));
      if (scheduler == null)
        throw new ArgumentNullException(nameof (scheduler));
      TaskCreationOptions creationOptions;
      InternalTaskOptions internalOptions;
      Task.CreationOptionsFromContinuationOptions(continuationOptions, out creationOptions, out internalOptions);
      Task<TResult> continuationTask = (Task<TResult>) new ContinuationResultTaskFromTask<TResult>(this, (Delegate) continuationFunction, (object) null, creationOptions, internalOptions);
      this.ContinueWithCore((Task) continuationTask, scheduler, cancellationToken, continuationOptions);
      return continuationTask;
    }

    public Task<TResult> ContinueWith<TResult>(
      Func<Task, object, TResult> continuationFunction,
      object state)
    {
      return this.ContinueWith<TResult>(continuationFunction, state, TaskScheduler.Current, new CancellationToken(), TaskContinuationOptions.None);
    }

    public Task<TResult> ContinueWith<TResult>(
      Func<Task, object, TResult> continuationFunction,
      object state,
      CancellationToken cancellationToken)
    {
      return this.ContinueWith<TResult>(continuationFunction, state, TaskScheduler.Current, cancellationToken, TaskContinuationOptions.None);
    }

    public Task<TResult> ContinueWith<TResult>(
      Func<Task, object, TResult> continuationFunction,
      object state,
      TaskScheduler scheduler)
    {
      return this.ContinueWith<TResult>(continuationFunction, state, scheduler, new CancellationToken(), TaskContinuationOptions.None);
    }

    public Task<TResult> ContinueWith<TResult>(
      Func<Task, object, TResult> continuationFunction,
      object state,
      TaskContinuationOptions continuationOptions)
    {
      return this.ContinueWith<TResult>(continuationFunction, state, TaskScheduler.Current, new CancellationToken(), continuationOptions);
    }

    public Task<TResult> ContinueWith<TResult>(
      Func<Task, object, TResult> continuationFunction,
      object state,
      CancellationToken cancellationToken,
      TaskContinuationOptions continuationOptions,
      TaskScheduler scheduler)
    {
      return this.ContinueWith<TResult>(continuationFunction, state, scheduler, cancellationToken, continuationOptions);
    }

    private Task<TResult> ContinueWith<TResult>(
      Func<Task, object, TResult> continuationFunction,
      object state,
      TaskScheduler scheduler,
      CancellationToken cancellationToken,
      TaskContinuationOptions continuationOptions)
    {
      if (continuationFunction == null)
        throw new ArgumentNullException(nameof (continuationFunction));
      if (scheduler == null)
        throw new ArgumentNullException(nameof (scheduler));
      TaskCreationOptions creationOptions;
      InternalTaskOptions internalOptions;
      Task.CreationOptionsFromContinuationOptions(continuationOptions, out creationOptions, out internalOptions);
      Task<TResult> continuationTask = (Task<TResult>) new ContinuationResultTaskFromTask<TResult>(this, (Delegate) continuationFunction, state, creationOptions, internalOptions);
      this.ContinueWithCore((Task) continuationTask, scheduler, cancellationToken, continuationOptions);
      return continuationTask;
    }

    internal static void CreationOptionsFromContinuationOptions(
      TaskContinuationOptions continuationOptions,
      out TaskCreationOptions creationOptions,
      out InternalTaskOptions internalOptions)
    {
      TaskContinuationOptions continuationOptions1 = TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.NotOnRanToCompletion;
      TaskContinuationOptions continuationOptions2 = TaskContinuationOptions.PreferFairness | TaskContinuationOptions.LongRunning | TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.HideScheduler | TaskContinuationOptions.RunContinuationsAsynchronously;
      TaskContinuationOptions continuationOptions3 = TaskContinuationOptions.LongRunning | TaskContinuationOptions.ExecuteSynchronously;
      if ((continuationOptions & continuationOptions3) == continuationOptions3)
        throw new ArgumentOutOfRangeException(nameof (continuationOptions), "The specified TaskContinuationOptions combined LongRunning and ExecuteSynchronously.  Synchronous continuations should not be long running.");
      if ((continuationOptions & ~(continuationOptions2 | continuationOptions1 | TaskContinuationOptions.LazyCancellation | TaskContinuationOptions.ExecuteSynchronously)) != TaskContinuationOptions.None)
        throw new ArgumentOutOfRangeException(nameof (continuationOptions));
      if ((continuationOptions & continuationOptions1) == continuationOptions1)
        throw new ArgumentOutOfRangeException(nameof (continuationOptions), "The specified TaskContinuationOptions excluded all continuation kinds.");
      creationOptions = (TaskCreationOptions) (continuationOptions & continuationOptions2);
      internalOptions = InternalTaskOptions.ContinuationTask;
      if ((continuationOptions & TaskContinuationOptions.LazyCancellation) == TaskContinuationOptions.None)
        return;
      internalOptions |= InternalTaskOptions.LazyCancellation;
    }

    internal void ContinueWithCore(
      Task continuationTask,
      TaskScheduler scheduler,
      CancellationToken cancellationToken,
      TaskContinuationOptions options)
    {
      TaskContinuation taskContinuation = (TaskContinuation) new StandardTaskContinuation(continuationTask, options, scheduler);
      if (cancellationToken.CanBeCanceled)
      {
        if (this.IsCompleted || cancellationToken.IsCancellationRequested)
          continuationTask.AssignCancellationToken(cancellationToken, (Task) null, (TaskContinuation) null);
        else
          continuationTask.AssignCancellationToken(cancellationToken, this, taskContinuation);
      }
      if (continuationTask.IsCompleted || this.AddTaskContinuation((object) taskContinuation, false))
        return;
      taskContinuation.Run(this, true);
    }

    internal void AddCompletionAction(ITaskCompletionAction action)
    {
      this.AddCompletionAction(action, false);
    }

    private void AddCompletionAction(ITaskCompletionAction action, bool addBeforeOthers)
    {
      if (this.AddTaskContinuation((object) action, addBeforeOthers))
        return;
      action.Invoke(this);
    }

    private bool AddTaskContinuationComplex(object tc, bool addBeforeOthers)
    {
      object continuationObject1 = this.m_continuationObject;
      if (continuationObject1 != Task.s_taskCompletionSentinel && !(continuationObject1 is LowLevelListWithIList<object>))
      {
        LowLevelListWithIList<object> levelListWithIlist = new LowLevelListWithIList<object>();
        levelListWithIlist.Add(continuationObject1);
        Interlocked.CompareExchange(ref this.m_continuationObject, (object) levelListWithIlist, continuationObject1);
      }
      if (this.m_continuationObject is LowLevelListWithIList<object> continuationObject2)
      {
        lock (continuationObject2)
        {
          if (this.m_continuationObject != Task.s_taskCompletionSentinel)
          {
            if (continuationObject2.Count == continuationObject2.Capacity)
              continuationObject2.RemoveAll(Task.s_IsTaskContinuationNullPredicate);
            if (addBeforeOthers)
              continuationObject2.Insert(0, tc);
            else
              continuationObject2.Add(tc);
            return true;
          }
        }
      }
      return false;
    }

    private bool AddTaskContinuation(object tc, bool addBeforeOthers)
    {
      if (this.IsCompleted)
        return false;
      return this.m_continuationObject == null && Interlocked.CompareExchange(ref this.m_continuationObject, tc, (object) null) == null || this.AddTaskContinuationComplex(tc, addBeforeOthers);
    }

    internal void RemoveContinuation(object continuationObject)
    {
      object continuationObject1 = this.m_continuationObject;
      if (continuationObject1 == Task.s_taskCompletionSentinel)
        return;
      if (!(continuationObject1 is LowLevelListWithIList<object> levelListWithIlist))
      {
        if (Interlocked.CompareExchange(ref this.m_continuationObject, (object) new LowLevelListWithIList<object>(), continuationObject) == continuationObject)
          return;
        levelListWithIlist = this.m_continuationObject as LowLevelListWithIList<object>;
      }
      if (levelListWithIlist == null)
        return;
      lock (levelListWithIlist)
      {
        if (this.m_continuationObject == Task.s_taskCompletionSentinel)
          return;
        int index = levelListWithIlist.IndexOf(continuationObject);
        if (index == -1)
          return;
        levelListWithIlist[index] = (object) null;
      }
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static void WaitAll(params Task[] tasks) => Task.WaitAll(tasks, -1);

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static bool WaitAll(Task[] tasks, TimeSpan timeout)
    {
      long totalMilliseconds = (long) timeout.TotalMilliseconds;
      return totalMilliseconds >= -1L && totalMilliseconds <= (long) int.MaxValue ? Task.WaitAll(tasks, (int) totalMilliseconds) : throw new ArgumentOutOfRangeException(nameof (timeout));
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static bool WaitAll(Task[] tasks, int millisecondsTimeout)
    {
      return Task.WaitAll(tasks, millisecondsTimeout, new CancellationToken());
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static void WaitAll(Task[] tasks, CancellationToken cancellationToken)
    {
      Task.WaitAll(tasks, -1, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static bool WaitAll(
      Task[] tasks,
      int millisecondsTimeout,
      CancellationToken cancellationToken)
    {
      if (tasks == null)
        throw new ArgumentNullException(nameof (tasks));
      if (millisecondsTimeout < -1)
        throw new ArgumentOutOfRangeException(nameof (millisecondsTimeout));
      cancellationToken.ThrowIfCancellationRequested();
      LowLevelListWithIList<System.Exception> exceptions = (LowLevelListWithIList<System.Exception>) null;
      LowLevelListWithIList<Task> list1 = (LowLevelListWithIList<Task>) null;
      LowLevelListWithIList<Task> list2 = (LowLevelListWithIList<Task>) null;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = true;
      for (int index = tasks.Length - 1; index >= 0; --index)
      {
        Task task = tasks[index];
        bool flag4 = task != null ? task.IsCompleted : throw new ArgumentException("The tasks array included at least one null element.", nameof (tasks));
        if (!flag4)
        {
          if (millisecondsTimeout != -1 || cancellationToken.CanBeCanceled)
          {
            Task.AddToList<Task>(task, ref list1, tasks.Length);
          }
          else
          {
            flag4 = task.WrappedTryRunInline() && task.IsCompleted;
            if (!flag4)
              Task.AddToList<Task>(task, ref list1, tasks.Length);
          }
        }
        if (flag4)
        {
          if (task.IsFaulted)
            flag1 = true;
          else if (task.IsCanceled)
            flag2 = true;
          if (task.IsWaitNotificationEnabled)
            Task.AddToList<Task>(task, ref list2, 1);
        }
      }
      if (list1 != null)
      {
        flag3 = Task.WaitAllBlockingCore(list1, millisecondsTimeout, cancellationToken);
        if (flag3)
        {
          foreach (Task task in (IEnumerable<Task>) list1)
          {
            if (task.IsFaulted)
              flag1 = true;
            else if (task.IsCanceled)
              flag2 = true;
            if (task.IsWaitNotificationEnabled)
              Task.AddToList<Task>(task, ref list2, 1);
          }
        }
        GC.KeepAlive((object) tasks);
      }
      if (flag3 && list2 != null)
      {
        foreach (Task task in (IEnumerable<Task>) list2)
        {
          if (task.NotifyDebuggerOfWaitCompletionIfNecessary())
            break;
        }
      }
      if (flag3 && flag1 | flag2)
      {
        if (!flag1)
          cancellationToken.ThrowIfCancellationRequested();
        foreach (Task task in tasks)
          Task.AddExceptionsForCompletedTask(ref exceptions, task);
        throw new AggregateException((IEnumerable<System.Exception>) exceptions);
      }
      return flag3;
    }

    private static void AddToList<T>(T item, ref LowLevelListWithIList<T> list, int initSize)
    {
      if (list == null)
        list = new LowLevelListWithIList<T>(initSize);
      list.Add(item);
    }

    private static bool WaitAllBlockingCore(
      LowLevelListWithIList<Task> tasks,
      int millisecondsTimeout,
      CancellationToken cancellationToken)
    {
      bool flag = false;
      Task.SetOnCountdownMres setOnCountdownMres = new Task.SetOnCountdownMres(tasks.Count);
      try
      {
        foreach (Task task in (IEnumerable<Task>) tasks)
          task.AddCompletionAction((ITaskCompletionAction) setOnCountdownMres, true);
        flag = setOnCountdownMres.Wait(millisecondsTimeout, cancellationToken);
        return flag;
      }
      finally
      {
        if (!flag)
        {
          foreach (Task task in (IEnumerable<Task>) tasks)
          {
            if (!task.IsCompleted)
              task.RemoveContinuation((object) setOnCountdownMres);
          }
        }
      }
    }

    internal static void AddExceptionsForCompletedTask(
      ref LowLevelListWithIList<System.Exception> exceptions,
      Task t)
    {
      AggregateException exceptions1 = t.GetExceptions(true);
      if (exceptions1 == null)
        return;
      t.UpdateExceptionObservedStatus();
      if (exceptions == null)
        exceptions = new LowLevelListWithIList<System.Exception>(exceptions1.InnerExceptions.Count);
      exceptions.AddRange((IEnumerable<System.Exception>) exceptions1.InnerExceptions);
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static int WaitAny(params Task[] tasks) => Task.WaitAny(tasks, -1);

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static int WaitAny(Task[] tasks, TimeSpan timeout)
    {
      long totalMilliseconds = (long) timeout.TotalMilliseconds;
      return totalMilliseconds >= -1L && totalMilliseconds <= (long) int.MaxValue ? Task.WaitAny(tasks, (int) totalMilliseconds) : throw new ArgumentOutOfRangeException(nameof (timeout));
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static int WaitAny(Task[] tasks, CancellationToken cancellationToken)
    {
      return Task.WaitAny(tasks, -1, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static int WaitAny(Task[] tasks, int millisecondsTimeout)
    {
      return Task.WaitAny(tasks, millisecondsTimeout, new CancellationToken());
    }

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static int WaitAny(
      Task[] tasks,
      int millisecondsTimeout,
      CancellationToken cancellationToken)
    {
      if (tasks == null)
        throw new ArgumentNullException(nameof (tasks));
      if (millisecondsTimeout < -1)
        throw new ArgumentOutOfRangeException(nameof (millisecondsTimeout));
      cancellationToken.ThrowIfCancellationRequested();
      int num = -1;
      for (int index = 0; index < tasks.Length; ++index)
      {
        Task task = tasks[index];
        if (task == null)
          throw new ArgumentException("The tasks array included at least one null element.", nameof (tasks));
        if (num == -1 && task.IsCompleted)
          num = index;
      }
      if (num == -1 && tasks.Length != 0)
      {
        Task<Task> task = TaskFactory.CommonCWAnyLogic((IList<Task>) tasks);
        if (task.Wait(millisecondsTimeout, cancellationToken))
          num = Array.IndexOf<Task>(tasks, task.Result);
      }
      GC.KeepAlive((object) tasks);
      return num;
    }

    public static Task<TResult> FromResult<TResult>(TResult result) => new Task<TResult>(result);

    public static Task FromException(System.Exception exception)
    {
      return (Task) Task.FromException<VoidTaskResult>(exception);
    }

    public static Task<TResult> FromException<TResult>(System.Exception exception)
    {
      if (exception == null)
        throw new ArgumentNullException(nameof (exception));
      Task<TResult> task = new Task<TResult>();
      task.TrySetException((object) exception);
      return task;
    }

    internal static Task FromCancellation(CancellationToken cancellationToken)
    {
      return cancellationToken.IsCancellationRequested ? new Task(true, TaskCreationOptions.None, cancellationToken) : throw new ArgumentOutOfRangeException(nameof (cancellationToken));
    }

    public static Task FromCanceled(CancellationToken cancellationToken)
    {
      return Task.FromCancellation(cancellationToken);
    }

    internal static Task<TResult> FromCancellation<TResult>(CancellationToken cancellationToken)
    {
      return cancellationToken.IsCancellationRequested ? new Task<TResult>(true, default (TResult), TaskCreationOptions.None, cancellationToken) : throw new ArgumentOutOfRangeException(nameof (cancellationToken));
    }

    public static Task<TResult> FromCanceled<TResult>(CancellationToken cancellationToken)
    {
      return Task.FromCancellation<TResult>(cancellationToken);
    }

    internal static Task<TResult> FromCancellation<TResult>(OperationCanceledException exception)
    {
      if (exception == null)
        throw new ArgumentNullException(nameof (exception));
      Task<TResult> task = new Task<TResult>();
      task.TrySetCanceled(exception.CancellationToken, (object) exception);
      return task;
    }

    public static Task Run(Action action)
    {
      return Task.InternalStartNew((Task) null, (Delegate) action, (object) null, new CancellationToken(), TaskScheduler.Default, TaskCreationOptions.DenyChildAttach, InternalTaskOptions.None);
    }

    public static Task Run(Action action, CancellationToken cancellationToken)
    {
      return Task.InternalStartNew((Task) null, (Delegate) action, (object) null, cancellationToken, TaskScheduler.Default, TaskCreationOptions.DenyChildAttach, InternalTaskOptions.None);
    }

    public static Task<TResult> Run<TResult>(Func<TResult> function)
    {
      return Task<TResult>.StartNew((Task) null, function, new CancellationToken(), TaskCreationOptions.DenyChildAttach, InternalTaskOptions.None, TaskScheduler.Default);
    }

    public static Task<TResult> Run<TResult>(
      Func<TResult> function,
      CancellationToken cancellationToken)
    {
      return Task<TResult>.StartNew((Task) null, function, cancellationToken, TaskCreationOptions.DenyChildAttach, InternalTaskOptions.None, TaskScheduler.Default);
    }

    public static Task Run(Func<Task> function) => Task.Run(function, new CancellationToken());

    public static Task Run(Func<Task> function, CancellationToken cancellationToken)
    {
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      return cancellationToken.IsCancellationRequested ? Task.FromCancellation(cancellationToken) : (Task) new UnwrapPromise<VoidTaskResult>((Task) Task.Factory.StartNew<Task>(function, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default), true);
    }

    public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
    {
      return Task.Run<TResult>(function, new CancellationToken());
    }

    public static Task<TResult> Run<TResult>(
      Func<Task<TResult>> function,
      CancellationToken cancellationToken)
    {
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      return cancellationToken.IsCancellationRequested ? Task.FromCancellation<TResult>(cancellationToken) : (Task<TResult>) new UnwrapPromise<TResult>((Task) Task.Factory.StartNew<Task<TResult>>(function, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default), true);
    }

    public static Task Delay(TimeSpan delay) => Task.Delay(delay, new CancellationToken());

    public static Task Delay(TimeSpan delay, CancellationToken cancellationToken)
    {
      long totalMilliseconds = (long) delay.TotalMilliseconds;
      return totalMilliseconds >= -1L && totalMilliseconds <= (long) int.MaxValue ? Task.Delay((int) totalMilliseconds, cancellationToken) : throw new ArgumentOutOfRangeException(nameof (delay), "The value needs to translate in milliseconds to -1 (signifying an infinite timeout), 0 or a positive integer less than or equal to Int32.MaxValue.");
    }

    public static Task Delay(int millisecondsDelay)
    {
      return Task.Delay(millisecondsDelay, new CancellationToken());
    }

    public static Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
    {
      if (millisecondsDelay < -1)
        throw new ArgumentOutOfRangeException(nameof (millisecondsDelay), "The value needs to be either -1 (signifying an infinite timeout), 0 or a positive integer.");
      if (cancellationToken.IsCancellationRequested)
        return Task.FromCancellation(cancellationToken);
      if (millisecondsDelay == 0)
        return Task.CompletedTask;
      Task.DelayPromise state1 = new Task.DelayPromise(cancellationToken);
      if (cancellationToken.CanBeCanceled)
        state1.Registration = cancellationToken.InternalRegisterWithoutEC((Action<object>) (state => ((Task.DelayPromise) state).Complete()), (object) state1);
      if (millisecondsDelay != -1)
      {
        state1.Timer = new Timer((TimerCallback) (state => ((Task.DelayPromise) state).Complete()), (object) state1, millisecondsDelay, -1);
        state1.Timer.KeepRootedWhileScheduled();
      }
      return (Task) state1;
    }

    public static Task WhenAll(IEnumerable<Task> tasks)
    {
      switch (tasks)
      {
        case Task[] taskArray:
          return Task.WhenAll(taskArray);
        case ICollection<Task> tasks2:
          int num = 0;
          Task[] tasks1 = new Task[tasks2.Count];
          foreach (Task task in tasks)
            tasks1[num++] = task != null ? task : throw new ArgumentException("The tasks argument included a null value.", nameof (tasks));
          return Task.InternalWhenAll(tasks1);
        case null:
          throw new ArgumentNullException(nameof (tasks));
        default:
          LowLevelListWithIList<Task> levelListWithIlist = new LowLevelListWithIList<Task>();
          foreach (Task task in tasks)
          {
            if (task == null)
              throw new ArgumentException("The tasks argument included a null value.", nameof (tasks));
            levelListWithIlist.Add(task);
          }
          return Task.InternalWhenAll(levelListWithIlist.ToArray());
      }
    }

    public static Task WhenAll(params Task[] tasks)
    {
      int length = tasks != null ? tasks.Length : throw new ArgumentNullException(nameof (tasks));
      if (length == 0)
        return Task.InternalWhenAll(tasks);
      Task[] tasks1 = new Task[length];
      for (int index = 0; index < length; ++index)
        tasks1[index] = tasks[index] ?? throw new ArgumentException("The tasks argument included a null value.", nameof (tasks));
      return Task.InternalWhenAll(tasks1);
    }

    private static Task InternalWhenAll(Task[] tasks)
    {
      return tasks.Length != 0 ? (Task) new Task.WhenAllPromise(tasks) : Task.CompletedTask;
    }

    public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
    {
      switch (tasks)
      {
        case Task<TResult>[] taskArray:
          return Task.WhenAll<TResult>(taskArray);
        case ICollection<Task<TResult>> tasks2:
          int num = 0;
          Task<TResult>[] tasks1 = new Task<TResult>[tasks2.Count];
          foreach (Task<TResult> task in tasks)
            tasks1[num++] = task != null ? task : throw new ArgumentException("The tasks argument included a null value.", nameof (tasks));
          return Task.InternalWhenAll<TResult>(tasks1);
        case null:
          throw new ArgumentNullException(nameof (tasks));
        default:
          LowLevelListWithIList<Task<TResult>> levelListWithIlist = new LowLevelListWithIList<Task<TResult>>();
          foreach (Task<TResult> task in tasks)
          {
            if (task == null)
              throw new ArgumentException("The tasks argument included a null value.", nameof (tasks));
            levelListWithIlist.Add(task);
          }
          return Task.InternalWhenAll<TResult>(levelListWithIlist.ToArray());
      }
    }

    public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
    {
      int length = tasks != null ? tasks.Length : throw new ArgumentNullException(nameof (tasks));
      if (length == 0)
        return Task.InternalWhenAll<TResult>(tasks);
      Task<TResult>[] tasks1 = new Task<TResult>[length];
      for (int index = 0; index < length; ++index)
        tasks1[index] = tasks[index] ?? throw new ArgumentException("The tasks argument included a null value.", nameof (tasks));
      return Task.InternalWhenAll<TResult>(tasks1);
    }

    private static Task<TResult[]> InternalWhenAll<TResult>(Task<TResult>[] tasks)
    {
      return tasks.Length != 0 ? (Task<TResult[]>) new Task.WhenAllPromise<TResult>(tasks) : new Task<TResult[]>(false, Array.Empty<TResult>(), TaskCreationOptions.None, new CancellationToken());
    }

    public static Task<Task> WhenAny(params Task[] tasks)
    {
      if (tasks == null)
        throw new ArgumentNullException(nameof (tasks));
      int length = tasks.Length != 0 ? tasks.Length : throw new ArgumentException("The tasks argument contains no tasks.", nameof (tasks));
      Task[] tasks1 = new Task[length];
      for (int index = 0; index < length; ++index)
        tasks1[index] = tasks[index] ?? throw new ArgumentException("The tasks argument included a null value.", nameof (tasks));
      return TaskFactory.CommonCWAnyLogic((IList<Task>) tasks1);
    }

    public static Task<Task> WhenAny(IEnumerable<Task> tasks)
    {
      if (tasks == null)
        throw new ArgumentNullException(nameof (tasks));
      LowLevelListWithIList<Task> tasks1 = new LowLevelListWithIList<Task>();
      foreach (Task task in tasks)
      {
        if (task == null)
          throw new ArgumentException("The tasks argument included a null value.", nameof (tasks));
        tasks1.Add(task);
      }
      return tasks1.Count != 0 ? TaskFactory.CommonCWAnyLogic((IList<Task>) tasks1) : throw new ArgumentException("The tasks argument contains no tasks.", nameof (tasks));
    }

    public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
    {
      return Task.WhenAny((Task[]) tasks).ContinueWith<Task<TResult>>(Task<TResult>.TaskWhenAnyCast.Value, new CancellationToken(), TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
    {
      return Task.WhenAny((IEnumerable<Task>) tasks).ContinueWith<Task<TResult>>(Task<TResult>.TaskWhenAnyCast.Value, new CancellationToken(), TaskContinuationOptions.DenyChildAttach | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    public static Task<TResult> CreateUnwrapPromise<TResult>(Task outerTask, bool lookForOce)
    {
      return (Task<TResult>) new UnwrapPromise<TResult>(outerTask, lookForOce);
    }

    internal virtual Delegate[] GetDelegateContinuationsForDebugger()
    {
      return Task.GetDelegatesFromContinuationObject(this.m_continuationObject);
    }

    private static Delegate[] GetDelegatesFromContinuationObject(object continuationObject)
    {
      switch (continuationObject)
      {
        case Action action:
          return new Delegate[1]
          {
            (Delegate) AsyncMethodBuilderCore.TryGetStateMachineForDebugger(action)
          };
        case TaskContinuation taskContinuation:
          return taskContinuation.GetDelegateContinuationsForDebugger();
        case Task task:
          return task.GetDelegateContinuationsForDebugger();
        case ITaskCompletionAction completionAction:
          return new Delegate[1]
          {
            (Delegate) new Action<Task>(completionAction.Invoke)
          };
        case LowLevelListWithIList<object> levelListWithIlist2:
          LowLevelListWithIList<Delegate> levelListWithIlist1 = new LowLevelListWithIList<Delegate>();
          foreach (object continuationObject1 in (IEnumerable<object>) levelListWithIlist2)
          {
            Delegate[] continuationObject2 = Task.GetDelegatesFromContinuationObject(continuationObject1);
            if (continuationObject2 != null)
            {
              foreach (Delegate @delegate in continuationObject2)
              {
                if ((object) @delegate != null)
                  levelListWithIlist1.Add(@delegate);
              }
            }
          }
          return levelListWithIlist1.ToArray();
        default:
          return (Delegate[]) null;
      }
    }

    private static Task GetActiveTaskFromId(int taskId)
    {
      return DebuggerSupport.GetActiveTaskFromId(taskId);
    }

    [FriendAccessAllowed]
    internal static bool AddToActiveTasks(Task task)
    {
      lock (Task.s_activeTasksLock)
        Task.s_currentActiveTasks[task.Id] = task;
      return true;
    }

    [FriendAccessAllowed]
    internal static void RemoveFromActiveTasks(int taskId)
    {
      lock (Task.s_activeTasksLock)
        Task.s_currentActiveTasks.Remove(taskId);
    }

    public void MarkAborted(ThreadAbortException e)
    {
    }

    [SecurityCritical]
    private void ExecuteWithThreadLocal(ref Task currentTaskSlot)
    {
      Task task = currentTaskSlot;
      try
      {
        currentTaskSlot = this;
        ExecutionContext capturedContext = this.CapturedContext;
        if (capturedContext == null)
        {
          this.Execute();
        }
        else
        {
          ContextCallback callback = Task.s_ecCallback;
          if (callback == null)
            Task.s_ecCallback = callback = new ContextCallback(Task.ExecutionContextCallback);
          ExecutionContext.Run(capturedContext, callback, (object) this, true);
        }
        if (AsyncCausalityTracer.LoggingOn)
          AsyncCausalityTracer.TraceSynchronousWorkCompletion(CausalityTraceLevel.Required, CausalitySynchronousWork.Execution);
        this.Finish(true);
      }
      finally
      {
        currentTaskSlot = task;
      }
    }

    internal class ContingentProperties
    {
      internal ExecutionContext m_capturedContext;
      internal volatile ManualResetEventSlim m_completionEvent;
      internal volatile TaskExceptionHolder m_exceptionsHolder;
      internal CancellationToken m_cancellationToken;
      internal object m_cancellationRegistration;
      internal volatile int m_internalCancellationRequested;
      internal volatile int m_completionCountdown = 1;
      internal volatile LowLevelListWithIList<Task> m_exceptionalChildren;

      internal void SetCompleted() => this.m_completionEvent?.Set();

      internal void UnregisterCancellationCallback()
      {
        if (this.m_cancellationRegistration == null)
          return;
        try
        {
          ((CancellationTokenRegistration) this.m_cancellationRegistration).Dispose();
        }
        catch (ObjectDisposedException ex)
        {
        }
        this.m_cancellationRegistration = (object) null;
      }
    }

    private sealed class SetOnInvokeMres : ManualResetEventSlim, ITaskCompletionAction
    {
      internal SetOnInvokeMres()
        : base(false, 0)
      {
      }

      public void Invoke(Task completingTask) => this.Set();

      public bool InvokeMayRunArbitraryCode => false;
    }

    private sealed class SetOnCountdownMres : ManualResetEventSlim, ITaskCompletionAction
    {
      private int _count;

      internal SetOnCountdownMres(int count) => this._count = count;

      public void Invoke(Task completingTask)
      {
        if (Interlocked.Decrement(ref this._count) != 0)
          return;
        this.Set();
      }

      public bool InvokeMayRunArbitraryCode => false;
    }

    private sealed class DelayPromise : Task<VoidTaskResult>
    {
      internal readonly CancellationToken Token;
      internal CancellationTokenRegistration Registration;
      internal Timer Timer;

      internal DelayPromise(CancellationToken token)
      {
        this.Token = token;
        if (DebuggerSupport.LoggingOn)
          DebuggerSupport.TraceOperationCreation(CausalityTraceLevel.Required, (Task) this, "Task.Delay", 0UL);
        DebuggerSupport.AddToActiveTasks((Task) this);
      }

      internal void Complete()
      {
        bool flag;
        if (this.Token.IsCancellationRequested)
        {
          flag = this.TrySetCanceled(this.Token);
        }
        else
        {
          if (DebuggerSupport.LoggingOn)
            DebuggerSupport.TraceOperationCompletion(CausalityTraceLevel.Required, (Task) this, AsyncStatus.Completed);
          DebuggerSupport.RemoveFromActiveTasks((Task) this);
          flag = this.TrySetResult(new VoidTaskResult());
        }
        if (!flag)
          return;
        if (this.Timer != null)
          this.Timer.Dispose();
        this.Registration.Dispose();
      }
    }

    private sealed class WhenAllPromise : Task<VoidTaskResult>, ITaskCompletionAction
    {
      private readonly Task[] m_tasks;
      private int m_count;

      internal WhenAllPromise(Task[] tasks)
      {
        if (DebuggerSupport.LoggingOn)
          DebuggerSupport.TraceOperationCreation(CausalityTraceLevel.Required, (Task) this, "Task.WhenAll", 0UL);
        DebuggerSupport.AddToActiveTasks((Task) this);
        this.m_tasks = tasks;
        this.m_count = tasks.Length;
        foreach (Task task in tasks)
        {
          if (task.IsCompleted)
            this.Invoke(task);
          else
            task.AddCompletionAction((ITaskCompletionAction) this);
        }
      }

      public void Invoke(Task ignored)
      {
        if (DebuggerSupport.LoggingOn)
          DebuggerSupport.TraceOperationRelation(CausalityTraceLevel.Important, (Task) this, CausalityRelation.Join);
        if (Interlocked.Decrement(ref this.m_count) != 0)
          return;
        LowLevelListWithIList<ExceptionDispatchInfo> exceptionObject = (LowLevelListWithIList<ExceptionDispatchInfo>) null;
        Task task1 = (Task) null;
        for (int index = 0; index < this.m_tasks.Length; ++index)
        {
          Task task2 = this.m_tasks[index];
          if (task2.IsFaulted)
          {
            if (exceptionObject == null)
              exceptionObject = new LowLevelListWithIList<ExceptionDispatchInfo>();
            exceptionObject.AddRange((IEnumerable<ExceptionDispatchInfo>) task2.GetExceptionDispatchInfos());
          }
          else if (task2.IsCanceled && task1 == null)
            task1 = task2;
          if (task2.IsWaitNotificationEnabled)
            this.SetNotificationForWaitCompletion(true);
          else
            this.m_tasks[index] = (Task) null;
        }
        if (exceptionObject != null)
          this.TrySetException((object) exceptionObject);
        else if (task1 != null)
        {
          this.TrySetCanceled(task1.CancellationToken, (object) task1.GetCancellationExceptionDispatchInfo());
        }
        else
        {
          if (DebuggerSupport.LoggingOn)
            DebuggerSupport.TraceOperationCompletion(CausalityTraceLevel.Required, (Task) this, AsyncStatus.Completed);
          DebuggerSupport.RemoveFromActiveTasks((Task) this);
          this.TrySetResult(new VoidTaskResult());
        }
      }

      internal override bool ShouldNotifyDebuggerOfWaitCompletion
      {
        get
        {
          return base.ShouldNotifyDebuggerOfWaitCompletion && Task.AnyTaskRequiresNotifyDebuggerOfWaitCompletion(this.m_tasks);
        }
      }

      public bool InvokeMayRunArbitraryCode => true;
    }

    private sealed class WhenAllPromise<T> : Task<T[]>, ITaskCompletionAction
    {
      private readonly Task<T>[] m_tasks;
      private int m_count;

      internal WhenAllPromise(Task<T>[] tasks)
      {
        this.m_tasks = tasks;
        this.m_count = tasks.Length;
        if (DebuggerSupport.LoggingOn)
          DebuggerSupport.TraceOperationCreation(CausalityTraceLevel.Required, (Task) this, "Task.WhenAll", 0UL);
        DebuggerSupport.AddToActiveTasks((Task) this);
        foreach (Task<T> task in tasks)
        {
          if (task.IsCompleted)
            this.Invoke((Task) task);
          else
            task.AddCompletionAction((ITaskCompletionAction) this);
        }
      }

      public void Invoke(Task ignored)
      {
        if (DebuggerSupport.LoggingOn)
          DebuggerSupport.TraceOperationRelation(CausalityTraceLevel.Important, (Task) this, CausalityRelation.Join);
        if (Interlocked.Decrement(ref this.m_count) != 0)
          return;
        T[] result = new T[this.m_tasks.Length];
        LowLevelListWithIList<ExceptionDispatchInfo> exceptionObject = (LowLevelListWithIList<ExceptionDispatchInfo>) null;
        Task task1 = (Task) null;
        for (int index = 0; index < this.m_tasks.Length; ++index)
        {
          Task<T> task2 = this.m_tasks[index];
          if (task2.IsFaulted)
          {
            if (exceptionObject == null)
              exceptionObject = new LowLevelListWithIList<ExceptionDispatchInfo>();
            exceptionObject.AddRange((IEnumerable<ExceptionDispatchInfo>) task2.GetExceptionDispatchInfos());
          }
          else if (task2.IsCanceled)
          {
            if (task1 == null)
              task1 = (Task) task2;
          }
          else
            result[index] = task2.GetResultCore(false);
          if (task2.IsWaitNotificationEnabled)
            this.SetNotificationForWaitCompletion(true);
          else
            this.m_tasks[index] = (Task<T>) null;
        }
        if (exceptionObject != null)
          this.TrySetException((object) exceptionObject);
        else if (task1 != null)
        {
          this.TrySetCanceled(task1.CancellationToken, (object) task1.GetCancellationExceptionDispatchInfo());
        }
        else
        {
          if (DebuggerSupport.LoggingOn)
            DebuggerSupport.TraceOperationCompletion(CausalityTraceLevel.Required, (Task) this, AsyncStatus.Completed);
          DebuggerSupport.RemoveFromActiveTasks((Task) this);
          this.TrySetResult(result);
        }
      }

      internal override bool ShouldNotifyDebuggerOfWaitCompletion
      {
        get
        {
          return base.ShouldNotifyDebuggerOfWaitCompletion && Task.AnyTaskRequiresNotifyDebuggerOfWaitCompletion((Task[]) this.m_tasks);
        }
      }

      public bool InvokeMayRunArbitraryCode => true;
    }
  }
}
