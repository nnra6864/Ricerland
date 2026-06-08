// Decompiled with JetBrains decompiler
// Type: UnityEngine.AudioChorusFilter
// Assembly: UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 50AEAEB4-2DC1-4809-BA2B-81C540D1CDC8
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEngine.AudioModule.dll
// XML documentation location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEngine.AudioModule.xml

using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

#nullable disable
namespace UnityEngine;

/// <summary>
///   <para>The Audio Chorus Filter takes an Audio Clip and processes it creating a chorus effect.</para>
/// </summary>
[RequireComponent(typeof (AudioBehaviour))]
public sealed class AudioChorusFilter : Behaviour
{
  /// <summary>
  ///   <para>Volume of original signal to pass to output. 0.0 to 1.0. Default = 0.5.</para>
  /// </summary>
  public float dryMix
  {
    get
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return AudioChorusFilter.get_dryMix_Injected(_unity_self);
    }
    set
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      AudioChorusFilter.set_dryMix_Injected(_unity_self, value);
    }
  }

  /// <summary>
  ///   <para>Volume of 1st chorus tap. 0.0 to 1.0. Default = 0.5.</para>
  /// </summary>
  public float wetMix1
  {
    get
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return AudioChorusFilter.get_wetMix1_Injected(_unity_self);
    }
    set
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      AudioChorusFilter.set_wetMix1_Injected(_unity_self, value);
    }
  }

  /// <summary>
  ///   <para>Volume of 2nd chorus tap. This tap is 90 degrees out of phase of the first tap. 0.0 to 1.0. Default = 0.5.</para>
  /// </summary>
  public float wetMix2
  {
    get
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return AudioChorusFilter.get_wetMix2_Injected(_unity_self);
    }
    set
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      AudioChorusFilter.set_wetMix2_Injected(_unity_self, value);
    }
  }

  /// <summary>
  ///   <para>Volume of 3rd chorus tap. This tap is 90 degrees out of phase of the second tap. 0.0 to 1.0. Default = 0.5.</para>
  /// </summary>
  public float wetMix3
  {
    get
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return AudioChorusFilter.get_wetMix3_Injected(_unity_self);
    }
    set
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      AudioChorusFilter.set_wetMix3_Injected(_unity_self, value);
    }
  }

  /// <summary>
  ///   <para>Chorus delay in ms. 0.1 to 100.0. Default = 40.0 ms.</para>
  /// </summary>
  public float delay
  {
    get
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return AudioChorusFilter.get_delay_Injected(_unity_self);
    }
    set
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      AudioChorusFilter.set_delay_Injected(_unity_self, value);
    }
  }

  /// <summary>
  ///   <para>Chorus modulation rate in hz. 0.0 to 20.0. Default = 0.8 hz.</para>
  /// </summary>
  public float rate
  {
    get
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return AudioChorusFilter.get_rate_Injected(_unity_self);
    }
    set
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      AudioChorusFilter.set_rate_Injected(_unity_self, value);
    }
  }

  /// <summary>
  ///   <para>Chorus modulation depth. 0.0 to 1.0. Default = 0.03.</para>
  /// </summary>
  public float depth
  {
    get
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return AudioChorusFilter.get_depth_Injected(_unity_self);
    }
    set
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<AudioChorusFilter>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      AudioChorusFilter.set_depth_Injected(_unity_self, value);
    }
  }

  /// <summary>
  ///   <para>Chorus feedback. Controls how much of the wet signal gets fed back into the chorus buffer. 0.0 to 1.0. Default = 0.0.</para>
  /// </summary>
  [Obsolete("Warning! Feedback is deprecated. This property does nothing.")]
  public float feedback
  {
    get
    {
      Debug.LogWarning((object) "Warning! Feedback is deprecated. This property does nothing.");
      return 0.0f;
    }
    set
    {
      Debug.LogWarning((object) "Warning! Feedback is deprecated. This property does nothing.");
    }
  }

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern float get_dryMix_Injected(IntPtr _unity_self);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern void set_dryMix_Injected(IntPtr _unity_self, float value);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern float get_wetMix1_Injected(IntPtr _unity_self);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern void set_wetMix1_Injected(IntPtr _unity_self, float value);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern float get_wetMix2_Injected(IntPtr _unity_self);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern void set_wetMix2_Injected(IntPtr _unity_self, float value);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern float get_wetMix3_Injected(IntPtr _unity_self);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern void set_wetMix3_Injected(IntPtr _unity_self, float value);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern float get_delay_Injected(IntPtr _unity_self);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern void set_delay_Injected(IntPtr _unity_self, float value);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern float get_rate_Injected(IntPtr _unity_self);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern void set_rate_Injected(IntPtr _unity_self, float value);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern float get_depth_Injected(IntPtr _unity_self);

  [MethodImpl(MethodImplOptions.InternalCall)]
  private static extern void set_depth_Injected(IntPtr _unity_self, float value);
}
