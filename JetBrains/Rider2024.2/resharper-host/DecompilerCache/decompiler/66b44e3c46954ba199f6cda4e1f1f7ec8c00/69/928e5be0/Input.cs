// Decompiled with JetBrains decompiler
// Type: UnityEngine.Input
// Assembly: UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 66B44E3C-4695-4BA1-99F6-CDA4E1F1F7EC
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.20f1/Editor/Data/Managed/UnityEngine/UnityEngine.InputLegacyModule.dll
// XML documentation location: /home/nnra/Unity/Hub/Editor/6000.0.20f1/Editor/Data/Managed/UnityEngine/UnityEngine.InputLegacyModule.xml

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

#nullable disable
namespace UnityEngine
{
  /// <summary>
  ///   <para>Interface into the Legacy Input system.</para>
  /// </summary>
  [NativeHeader("Runtime/Input/InputBindings.h")]
  public class Input
  {
    private static LocationService locationServiceInstance;
    private static Compass compassInstance;
    private static Gyroscope s_MainGyro;

    /// <summary>
    ///   <para>Returns the value of the virtual axis identified by axisName.</para>
    /// </summary>
    /// <param name="axisName"></param>
    public static float GetAxis(string axisName) => InputUnsafeUtility.GetAxis(axisName);

    /// <summary>
    ///   <para>Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.</para>
    /// </summary>
    /// <param name="axisName"></param>
    public static float GetAxisRaw(string axisName) => InputUnsafeUtility.GetAxisRaw(axisName);

    /// <summary>
    ///   <para>Returns true while the virtual button identified by buttonName is held down.</para>
    /// </summary>
    /// <param name="buttonName">The name of the button such as Jump.</param>
    /// <returns>
    ///   <para>True when an axis has been pressed and not released.</para>
    /// </returns>
    public static bool GetButton(string buttonName) => InputUnsafeUtility.GetButton(buttonName);

    /// <summary>
    ///   <para>Returns true during the frame the user pressed down the virtual button identified by buttonName.</para>
    /// </summary>
    /// <param name="buttonName"></param>
    public static bool GetButtonDown(string buttonName)
    {
      return InputUnsafeUtility.GetButtonDown(buttonName);
    }

    /// <summary>
    ///   <para>Returns true the first frame the user releases the virtual button identified by buttonName.</para>
    /// </summary>
    /// <param name="buttonName"></param>
    public static bool GetButtonUp(string buttonName) => InputUnsafeUtility.GetButtonUp(buttonName);

    [NativeThrows]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool GetKeyInt(KeyCode key);

    [NativeThrows]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool GetKeyUpInt(KeyCode key);

    [NativeThrows]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool GetKeyDownInt(KeyCode key);

    /// <summary>
    ///   <para>Returns whether the given mouse button is held down.</para>
    /// </summary>
    /// <param name="button"></param>
    [NativeThrows]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern bool GetMouseButton(int button);

    /// <summary>
    ///   <para>Returns true during the frame the user pressed the given mouse button.</para>
    /// </summary>
    /// <param name="button"></param>
    [NativeThrows]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern bool GetMouseButtonDown(int button);

    /// <summary>
    ///   <para>Returns true during the frame the user releases the given mouse button.</para>
    /// </summary>
    /// <param name="button"></param>
    [NativeThrows]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern bool GetMouseButtonUp(int button);

    /// <summary>
    ///   <para>Resets all input. After ResetInputAxes all axes return to 0 and all buttons return to 0 for one frame.</para>
    /// </summary>
    [FreeFunction("ResetInput")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern void ResetInputAxes();

    /// <summary>
    ///   <para>Determine whether a particular joystick model has been preconfigured by Unity. (Linux-only).</para>
    /// </summary>
    /// <param name="joystickName">The name of the joystick to check (returned by Input.GetJoystickNames).</param>
    /// <returns>
    ///   <para>True if the joystick layout has been preconfigured; false otherwise.</para>
    /// </returns>
    public static bool IsJoystickPreconfigured(string joystickName)
    {
      return InputUnsafeUtility.IsJoystickPreconfigured(joystickName);
    }

    /// <summary>
    ///   <para>Retrieves a list of input device names corresponding to the index of an Axis configured within Input Manager.</para>
    /// </summary>
    /// <returns>
    ///   <para>Returns an array of joystick and gamepad device names.</para>
    /// </returns>
    [NativeThrows]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern string[] GetJoystickNames();

    /// <summary>
    ///   <para>Call Input.GetTouch to obtain a Touch struct.</para>
    /// </summary>
    /// <param name="index">The touch input on the device screen.</param>
    /// <returns>
    ///   <para>Touch details in the struct.</para>
    /// </returns>
    [NativeThrows]
    public static Touch GetTouch(int index)
    {
      Touch ret;
      Input.GetTouch_Injected(index, out ret);
      return ret;
    }

    /// <summary>
    ///   <para>Returns the PenData for the pen event at the given index in the pen event queue.</para>
    /// </summary>
    /// <param name="index"></param>
    /// <returns>
    ///   <para>Pen event details in the struct.</para>
    /// </returns>
    [NativeThrows]
    public static PenData GetPenEvent(int index)
    {
      PenData ret;
      Input.GetPenEvent_Injected(index, out ret);
      return ret;
    }

    /// <summary>
    ///   <para>Returns the PenData for the last stored pen up or down event.</para>
    /// </summary>
    /// <returns>
    ///   <para>Pen event details in the struct.</para>
    /// </returns>
    [NativeThrows]
    public static PenData GetLastPenContactEvent()
    {
      PenData ret;
      Input.GetLastPenContactEvent_Injected(out ret);
      return ret;
    }

    /// <summary>
    ///   <para>Clears the pen event queue.</para>
    /// </summary>
    [NativeThrows]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern void ResetPenEvents();

    /// <summary>
    ///         <para>Clears the last stored pen event.
    /// Calling this function may impact event handling for UIToolKit elements.</para>
    ///       </summary>
    [NativeThrows]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern void ClearLastPenContactEvent();

    /// <summary>
    ///   <para>Returns specific acceleration measurement which occurred during last frame. (Does not allocate temporary variables).</para>
    /// </summary>
    /// <param name="index"></param>
    [NativeThrows]
    public static AccelerationEvent GetAccelerationEvent(int index)
    {
      AccelerationEvent ret;
      Input.GetAccelerationEvent_Injected(index, out ret);
      return ret;
    }

    /// <summary>
    ///   <para>Returns true while the user holds down the key identified by the key KeyCode enum parameter.</para>
    /// </summary>
    /// <param name="key"></param>
    public static bool GetKey(KeyCode key) => Input.GetKeyInt(key);

    /// <summary>
    ///   <para>Returns true while the user holds down the key identified by name.</para>
    /// </summary>
    /// <param name="name"></param>
    public static bool GetKey(string name) => InputUnsafeUtility.GetKeyString(name);

    /// <summary>
    ///   <para>Returns true during the frame the user releases the key identified by the key KeyCode enum parameter.</para>
    /// </summary>
    /// <param name="key"></param>
    public static bool GetKeyUp(KeyCode key) => Input.GetKeyUpInt(key);

    /// <summary>
    ///   <para>Returns true during the frame the user releases the key identified by name.</para>
    /// </summary>
    /// <param name="name"></param>
    public static bool GetKeyUp(string name) => InputUnsafeUtility.GetKeyUpString(name);

    /// <summary>
    ///   <para>Returns true during the frame the user starts pressing down the key identified by the key KeyCode enum parameter.</para>
    /// </summary>
    /// <param name="key"></param>
    public static bool GetKeyDown(KeyCode key) => Input.GetKeyDownInt(key);

    /// <summary>
    ///   <para>Returns true during the frame the user starts pressing down the key identified by name.</para>
    /// </summary>
    /// <param name="name"></param>
    public static bool GetKeyDown(string name) => InputUnsafeUtility.GetKeyDownString(name);

    [Conditional("UNITY_EDITOR")]
    internal static void SimulateTouch(Touch touch)
    {
      Input.SimulateTouchInternal(touch, DateTime.Now.Ticks);
    }

    [Conditional("UNITY_EDITOR")]
    [FreeFunction("SimulateTouch")]
    [NativeConditional("UNITY_EDITOR")]
    private static void SimulateTouchInternal(Touch touch, long timestamp)
    {
      Input.SimulateTouchInternal_Injected(ref touch, timestamp);
    }

    /// <summary>
    ///   <para>Enables/Disables mouse simulation with touches. By default this option is enabled.</para>
    /// </summary>
    public static extern bool simulateMouseWithTouches { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

    /// <summary>
    ///   <para>Is any key or mouse button currently held down? (Read Only)</para>
    /// </summary>
    [NativeThrows]
    public static extern bool anyKey { [MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>Returns true the first frame the user hits any key or mouse button. (Read Only)</para>
    /// </summary>
    [NativeThrows]
    public static extern bool anyKeyDown { [MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>Returns the keyboard input entered this frame. (Read Only)</para>
    /// </summary>
    [NativeThrows]
    public static string inputString
    {
      get
      {
        ManagedSpanWrapper ret;
        string stringAndDispose;
        try
        {
          Input.get_inputString_Injected(out ret);
        }
        finally
        {
          stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
        }
        return stringAndDispose;
      }
    }

    /// <summary>
    ///   <para>The current mouse position in pixel coordinates. (Read Only).</para>
    /// </summary>
    [NativeThrows]
    public static Vector3 mousePosition
    {
      get
      {
        Vector3 ret;
        Input.get_mousePosition_Injected(out ret);
        return ret;
      }
    }

    /// <summary>
    ///   <para>The current mouse position delta in pixel coordinates. (Read Only).</para>
    /// </summary>
    [NativeThrows]
    public static Vector3 mousePositionDelta
    {
      get
      {
        Vector3 ret;
        Input.get_mousePositionDelta_Injected(out ret);
        return ret;
      }
    }

    /// <summary>
    ///   <para>The current mouse scroll delta. (Read Only)</para>
    /// </summary>
    [NativeThrows]
    public static Vector2 mouseScrollDelta
    {
      get
      {
        Vector2 ret;
        Input.get_mouseScrollDelta_Injected(out ret);
        return ret;
      }
    }

    /// <summary>
    ///   <para>Controls enabling and disabling of IME input composition.</para>
    /// </summary>
    public static extern IMECompositionMode imeCompositionMode { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

    /// <summary>
    ///   <para>The current IME composition string being typed by the user.</para>
    /// </summary>
    public static string compositionString
    {
      get
      {
        ManagedSpanWrapper ret;
        string stringAndDispose;
        try
        {
          Input.get_compositionString_Injected(out ret);
        }
        finally
        {
          stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
        }
        return stringAndDispose;
      }
    }

    /// <summary>
    ///   <para>Does the user have an IME keyboard input source selected?</para>
    /// </summary>
    public static extern bool imeIsSelected { [MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>The current text input position used by IMEs to open windows.</para>
    /// </summary>
    public static Vector2 compositionCursorPos
    {
      get
      {
        Vector2 ret;
        Input.get_compositionCursorPos_Injected(out ret);
        return ret;
      }
      set => Input.set_compositionCursorPos_Injected(ref value);
    }

    /// <summary>
    ///   <para>Property indicating whether keypresses are eaten by a textinput if it has focus (default true).</para>
    /// </summary>
    [Obsolete("eatKeyPressOnTextFieldFocus property is deprecated, and only provided to support legacy behavior.")]
    public static extern bool eatKeyPressOnTextFieldFocus { [MethodImpl(MethodImplOptions.InternalCall)] get; [MethodImpl(MethodImplOptions.InternalCall)] set; }

    internal static bool simulateTouchEnabled { get; set; }

    [FreeFunction("GetMousePresent")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool GetMousePresentInternal();

    [FreeFunction("IsTouchSupported")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool GetTouchSupportedInternal();

    /// <summary>
    ///   <para>Indicates if a mouse device is detected.</para>
    /// </summary>
    public static bool mousePresent
    {
      get => !Input.simulateTouchEnabled && Input.GetMousePresentInternal();
    }

    /// <summary>
    ///   <para>Returns whether the device on which application is currently running supports touch input.</para>
    /// </summary>
    public static bool touchSupported
    {
      get => Input.simulateTouchEnabled || Input.GetTouchSupportedInternal();
    }

    /// <summary>
    ///   <para>Returns the number of queued pen events that can be accessed by calling GetPenEvent().</para>
    /// </summary>
    public static extern int penEventCount { [FreeFunction("GetPenEventCount"), MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>Number of touches. Guaranteed not to change throughout the frame. (Read Only)</para>
    /// </summary>
    public static extern int touchCount { [FreeFunction("GetTouchCount"), MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>Bool value which let's users check if touch pressure is supported.</para>
    /// </summary>
    public static extern bool touchPressureSupported { [FreeFunction("IsTouchPressureSupported"), MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>Returns true when Stylus Touch is supported by a device or platform.</para>
    /// </summary>
    public static extern bool stylusTouchSupported { [FreeFunction("IsStylusTouchSupported"), MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>Property indicating whether the system handles multiple touches.</para>
    /// </summary>
    public static extern bool multiTouchEnabled { [FreeFunction("IsMultiTouchEnabled"), MethodImpl(MethodImplOptions.InternalCall)] get; [FreeFunction("SetMultiTouchEnabled"), MethodImpl(MethodImplOptions.InternalCall)] set; }

    [Obsolete("isGyroAvailable property is deprecated. Please use SystemInfo.supportsGyroscope instead.")]
    public static extern bool isGyroAvailable { [FreeFunction("IsGyroAvailable"), MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>Device physical orientation as reported by OS. (Read Only)</para>
    /// </summary>
    public static extern DeviceOrientation deviceOrientation { [FreeFunction("GetDeviceOrientation"), MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>Last measured linear acceleration of a device in three-dimensional space. (Read Only)</para>
    /// </summary>
    public static Vector3 acceleration
    {
      [FreeFunction("GetAcceleration")] get
      {
        Vector3 ret;
        Input.get_acceleration_Injected(out ret);
        return ret;
      }
    }

    /// <summary>
    ///   <para>This property controls if input sensors should be compensated for screen orientation.</para>
    /// </summary>
    public static extern bool compensateSensors { [FreeFunction("IsCompensatingSensors"), MethodImpl(MethodImplOptions.InternalCall)] get; [FreeFunction("SetCompensatingSensors"), MethodImpl(MethodImplOptions.InternalCall)] set; }

    /// <summary>
    ///   <para>Number of acceleration measurements which occurred during last frame.</para>
    /// </summary>
    public static extern int accelerationEventCount { [FreeFunction("GetAccelerationCount"), MethodImpl(MethodImplOptions.InternalCall)] get; }

    /// <summary>
    ///   <para>Should  Back button quit the application?</para>
    /// </summary>
    public static extern bool backButtonLeavesApp { [FreeFunction("GetBackButtonLeavesApp"), MethodImpl(MethodImplOptions.InternalCall)] get; [FreeFunction("SetBackButtonLeavesApp"), MethodImpl(MethodImplOptions.InternalCall)] set; }

    /// <summary>
    ///   <para>Property for accessing device location (handheld devices only). (Read Only)</para>
    /// </summary>
    public static LocationService location
    {
      get
      {
        if (Input.locationServiceInstance == null)
          Input.locationServiceInstance = new LocationService();
        return Input.locationServiceInstance;
      }
    }

    /// <summary>
    ///   <para>Property for accessing compass (handheld devices only). (Read Only)</para>
    /// </summary>
    public static Compass compass
    {
      get
      {
        if (Input.compassInstance == null)
          Input.compassInstance = new Compass();
        return Input.compassInstance;
      }
    }

    [FreeFunction("GetGyro")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern int GetGyroInternal();

    /// <summary>
    ///   <para>Returns default gyroscope.</para>
    /// </summary>
    public static Gyroscope gyro
    {
      get
      {
        if (Input.s_MainGyro == null)
          Input.s_MainGyro = new Gyroscope(Input.GetGyroInternal());
        return Input.s_MainGyro;
      }
    }

    /// <summary>
    ///   <para>Returns list of objects representing status of all touches during last frame. (Read Only) (Allocates temporary variables).</para>
    /// </summary>
    public static Touch[] touches
    {
      get
      {
        int touchCount = Input.touchCount;
        Touch[] touches = new Touch[touchCount];
        for (int index = 0; index < touchCount; ++index)
          touches[index] = Input.GetTouch(index);
        return touches;
      }
    }

    /// <summary>
    ///   <para>Returns list of acceleration measurements which occurred during the last frame. (Read Only) (Allocates temporary variables).</para>
    /// </summary>
    public static AccelerationEvent[] accelerationEvents
    {
      get
      {
        int accelerationEventCount = Input.accelerationEventCount;
        AccelerationEvent[] accelerationEvents = new AccelerationEvent[accelerationEventCount];
        for (int index = 0; index < accelerationEventCount; ++index)
          accelerationEvents[index] = Input.GetAccelerationEvent(index);
        return accelerationEvents;
      }
    }

    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static extern bool CheckDisabled();

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void GetTouch_Injected(int index, out Touch ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void GetPenEvent_Injected(int index, out PenData ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void GetLastPenContactEvent_Injected(out PenData ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void GetAccelerationEvent_Injected(int index, out AccelerationEvent ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void SimulateTouchInternal_Injected([In] ref Touch touch, long timestamp);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void get_inputString_Injected(out ManagedSpanWrapper ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void get_mousePosition_Injected(out Vector3 ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void get_mousePositionDelta_Injected(out Vector3 ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void get_mouseScrollDelta_Injected(out Vector2 ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void get_compositionString_Injected(out ManagedSpanWrapper ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void get_compositionCursorPos_Injected(out Vector2 ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void set_compositionCursorPos_Injected([In] ref Vector2 value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void get_acceleration_Injected(out Vector3 ret);
  }
}
