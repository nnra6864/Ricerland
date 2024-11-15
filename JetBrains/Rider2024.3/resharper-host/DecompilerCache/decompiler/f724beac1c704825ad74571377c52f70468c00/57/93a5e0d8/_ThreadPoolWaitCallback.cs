// Decompiled with JetBrains decompiler
// Type: System.Threading._ThreadPoolWaitCallback
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: F724BEAC-1C70-4825-AD74-571377C52F70
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.24f1/Editor/Data/MonoBleedingEdge/lib/mono/unityjit-linux/mscorlib.dll

using System.Security;

#nullable disable
namespace System.Threading
{
  internal static class _ThreadPoolWaitCallback
  {
    [SecurityCritical]
    internal static bool PerformWaitCallback() => ThreadPoolWorkQueue.Dispatch();
  }
}
