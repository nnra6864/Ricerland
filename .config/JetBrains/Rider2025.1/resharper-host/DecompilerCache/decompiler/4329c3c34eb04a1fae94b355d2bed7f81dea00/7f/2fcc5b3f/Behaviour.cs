// Decompiled with JetBrains decompiler
// Type: UnityEngine.Behaviour
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4329C3C3-4EB0-4A1F-AE94-B355D2BED7F8
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.xml

using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

#nullable disable
namespace UnityEngine;

/// <summary>
///   <para>Behaviours are Components that can be enabled or disabled.</para>
/// </summary>
[NativeHeader("Runtime/Mono/MonoBehaviour.h")]
[UsedByNativeCode]
public class Behaviour : Component
{
  /// <summary>
  ///   <para>Enabled Behaviours are Updated, disabled Behaviours are not.</para>
  /// </summary>
  [RequiredByNativeCode]
  [NativeProperty]
  public bool enabled
  {
    get
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Behaviour>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return Behaviour.get_enabled_Injected(_unity_self);
    }
    set
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Behaviour>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      Behaviour.set_enabled_Injected(_unity_self, value);
    }
  }

  /// <summary>
  ///   <para>Reports whether a GameObject and its associated Behaviour is active and enabled.</para>
  /// </summary>
  [NativeProperty]
  public bool isActiveAndEnabled
  {
    [NativeMethod("IsAddedToManager")] get
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Behaviour>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return Behaviour.get_isActiveAndEnabled_Injected(_unity_self);
    }
  }

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern bool get_enabled_Injected(IntPtr _unity_self);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern bool get_isActiveAndEnabled_Injected(IntPtr _unity_self);
}
