// Decompiled with JetBrains decompiler
// Type: UnityEngine.Object
// Assembly: UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4329C3C3-4EB0-4A1F-AE94-B355D2BED7F8
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll
// XML documentation location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.xml

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngineInternal;

#nullable disable
namespace UnityEngine
{
  /// <summary>
  ///   <para>Base class for all objects Unity can reference.</para>
  /// </summary>
  [NativeHeader("Runtime/SceneManager/SceneManager.h")]
  [NativeHeader("Runtime/Export/Scripting/UnityEngineObject.bindings.h")]
  [NativeHeader("Runtime/GameCode/CloneObject.h")]
  [RequiredByNativeCode(GenerateProxy = true)]
  [StructLayout(LayoutKind.Sequential)]
  public class Object
  {
    private const int kInstanceID_None = 0;
    private IntPtr m_CachedPtr;
    private int m_InstanceID;
    private string m_UnityRuntimeErrorString;
    private const string objectIsNullMessage = "The Object you want to instantiate is null.";
    private const string cloneDestroyedMessage = "Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.";

    /// <summary>
    ///   <para>Gets  the instance ID of the object.</para>
    /// </summary>
    /// <returns>
    ///   <para>Returns the instance ID of the object.</para>
    /// </returns>
    [SecuritySafeCritical]
    public int GetInstanceID()
    {
      this.EnsureRunningOnMainThread();
      return this.m_InstanceID;
    }

    public override int GetHashCode() => this.m_InstanceID;

    public override bool Equals(object other)
    {
      Object rhs = other as Object;
      return (!(rhs == (Object) null) || other == null || other is Object) && Object.CompareBaseObjects(this, rhs);
    }

    public static implicit operator bool([NotNullWhen(true), MaybeNullWhen(false)] Object exists)
    {
      return !Object.CompareBaseObjects(exists, (Object) null);
    }

    private static bool CompareBaseObjects(Object lhs, Object rhs)
    {
      bool flag1 = (object) lhs == null;
      bool flag2 = (object) rhs == null;
      if (flag2 & flag1)
        return true;
      if (flag2)
        return !Object.IsNativeObjectAlive(lhs);
      return flag1 ? !Object.IsNativeObjectAlive(rhs) : lhs.m_InstanceID == rhs.m_InstanceID;
    }

    private void EnsureRunningOnMainThread()
    {
      if (!Object.CurrentThreadIsMainThread())
        throw new InvalidOperationException("EnsureRunningOnMainThread can only be called from the main thread");
    }

    private static bool IsNativeObjectAlive(Object o)
    {
      if (o.GetCachedPtr() != IntPtr.Zero)
        return true;
      return !(o is MonoBehaviour) && !(o is ScriptableObject) && Object.DoesObjectWithInstanceIDExist(o.GetInstanceID());
    }

    private IntPtr GetCachedPtr() => this.m_CachedPtr;

    /// <summary>
    ///   <para>The name of the object.</para>
    /// </summary>
    public string name
    {
      get => this.GetName();
      set => this.SetName(value);
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original) where T : Object
    {
      return Object.InstantiateAsync<T>(original, new InstantiateParameters()
      {
        worldSpace = true
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent) where T : Object
    {
      return Object.InstantiateAsync<T>(original, new InstantiateParameters()
      {
        worldSpace = true,
        parent = parent
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      Vector3 position,
      Quaternion rotation)
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, position, rotation, new InstantiateParameters()
      {
        worldSpace = true
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      Transform parent,
      Vector3 position,
      Quaternion rotation)
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, position, rotation, new InstantiateParameters()
      {
        worldSpace = true,
        parent = parent
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count) where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, new InstantiateParameters()
      {
        worldSpace = true
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      Transform parent)
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, new InstantiateParameters()
      {
        worldSpace = true,
        parent = parent
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      Vector3 position,
      Quaternion rotation)
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, position, rotation, new InstantiateParameters()
      {
        worldSpace = true
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      ReadOnlySpan<Vector3> positions,
      ReadOnlySpan<Quaternion> rotations)
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, positions, rotations, new InstantiateParameters()
      {
        worldSpace = true
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      Transform parent,
      Vector3 position,
      Quaternion rotation)
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, position, rotation, new InstantiateParameters()
      {
        worldSpace = true,
        parent = parent
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      Transform parent,
      Vector3 position,
      Quaternion rotation,
      CancellationToken cancellationToken)
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, position, rotation, new InstantiateParameters()
      {
        worldSpace = true,
        parent = parent
      }, cancellationToken);
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      Transform parent,
      ReadOnlySpan<Vector3> positions,
      ReadOnlySpan<Quaternion> rotations)
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, positions, rotations, new InstantiateParameters()
      {
        worldSpace = true,
        parent = parent
      });
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      Transform parent,
      ReadOnlySpan<Vector3> positions,
      ReadOnlySpan<Quaternion> rotations,
      CancellationToken cancellationToken)
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, positions, rotations, new InstantiateParameters()
      {
        worldSpace = true,
        parent = parent
      }, cancellationToken);
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      InstantiateParameters parameters,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, 1, parameters, cancellationToken);
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      InstantiateParameters parameters,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, ReadOnlySpan<Vector3>.Empty, ReadOnlySpan<Quaternion>.Empty, parameters, cancellationToken);
    }

    public static AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      Vector3 position,
      Quaternion rotation,
      InstantiateParameters parameters,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, 1, position, rotation, parameters, cancellationToken);
    }

    public static unsafe AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      Vector3 position,
      Quaternion rotation,
      InstantiateParameters parameters,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : Object
    {
      return Object.InstantiateAsync<T>(original, count, new ReadOnlySpan<Vector3>((void*) &position, 1), new ReadOnlySpan<Quaternion>((void*) &rotation, 1), parameters, cancellationToken);
    }

    [MethodImpl((MethodImplOptions) 768)]
    public static unsafe AsyncInstantiateOperation<T> InstantiateAsync<T>(
      T original,
      int count,
      ReadOnlySpan<Vector3> positions,
      ReadOnlySpan<Quaternion> rotations,
      InstantiateParameters parameters,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : Object
    {
      Object.CheckNullArgument((object) original, "The Object you want to instantiate is null.");
      if (count <= 0)
        throw new ArgumentException("Cannot call instantiate multiple with count less or equal to zero");
      if ((object) original is ScriptableObject)
        throw new ArgumentException("Cannot call instantiate multiple for a ScriptableObject");
      fixed (Vector3* positions1 = &positions.GetPinnableReference())
        fixed (Quaternion* rotations1 = &rotations.GetPinnableReference())
          return new AsyncInstantiateOperation<T>(Object.Internal_InstantiateAsyncWithParams((Object) original, count, parameters, (IntPtr) (void*) positions1, positions.Length, (IntPtr) (void*) rotations1, rotations.Length, cancellationToken.CanBeCanceled), cancellationToken);
    }

    /// <summary>
    ///   <para>Clones the object original and returns the clone.</para>
    /// </summary>
    /// <param name="original">An existing object that you want to make a copy of.</param>
    /// <param name="position">Position for the new object.</param>
    /// <param name="rotation">Orientation of the new object.</param>
    /// <param name="parent">Parent that will be assigned to the new object.</param>
    /// <param name="instantiateInWorldSpace">When you assign a parent Object, pass true to position the new object directly in world space. Pass false to set the Object’s position relative to its new parent.</param>
    /// <param name="parameters">A struct containing all the parameters.</param>
    /// <param name="scene"></param>
    /// <returns>
    ///   <para>The instantiated clone.</para>
    /// </returns>
    [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
    public static Object Instantiate(Object original, Vector3 position, Quaternion rotation)
    {
      Object.CheckNullArgument((object) original, "The Object you want to instantiate is null.");
      if (original is ScriptableObject)
        throw new ArgumentException("Cannot instantiate a ScriptableObject with a position and rotation");
      Object @object = Object.Internal_InstantiateSingle(original, position, rotation);
      return !(@object == (Object) null) ? @object : throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
    }

    /// <summary>
    ///   <para>Clones the object original and returns the clone.</para>
    /// </summary>
    /// <param name="original">An existing object that you want to make a copy of.</param>
    /// <param name="position">Position for the new object.</param>
    /// <param name="rotation">Orientation of the new object.</param>
    /// <param name="parent">Parent that will be assigned to the new object.</param>
    /// <param name="instantiateInWorldSpace">When you assign a parent Object, pass true to position the new object directly in world space. Pass false to set the Object’s position relative to its new parent.</param>
    /// <param name="parameters">A struct containing all the parameters.</param>
    /// <param name="scene"></param>
    /// <returns>
    ///   <para>The instantiated clone.</para>
    /// </returns>
    [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
    public static Object Instantiate(
      Object original,
      Vector3 position,
      Quaternion rotation,
      Transform parent)
    {
      if ((Object) parent == (Object) null)
        return Object.Instantiate(original, position, rotation);
      Object.CheckNullArgument((object) original, "The Object you want to instantiate is null.");
      Object @object = Object.Internal_InstantiateSingleWithParent(original, parent, position, rotation);
      return !(@object == (Object) null) ? @object : throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
    }

    /// <summary>
    ///   <para>Clones the object original and returns the clone.</para>
    /// </summary>
    /// <param name="original">An existing object that you want to make a copy of.</param>
    /// <param name="position">Position for the new object.</param>
    /// <param name="rotation">Orientation of the new object.</param>
    /// <param name="parent">Parent that will be assigned to the new object.</param>
    /// <param name="instantiateInWorldSpace">When you assign a parent Object, pass true to position the new object directly in world space. Pass false to set the Object’s position relative to its new parent.</param>
    /// <param name="parameters">A struct containing all the parameters.</param>
    /// <param name="scene"></param>
    /// <returns>
    ///   <para>The instantiated clone.</para>
    /// </returns>
    [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
    public static Object Instantiate(Object original)
    {
      Object.CheckNullArgument((object) original, "The Object you want to instantiate is null.");
      Object @object = Object.Internal_CloneSingle(original);
      return !(@object == (Object) null) ? @object : throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
    }

    /// <summary>
    ///   <para>Clones the object original and returns the clone.</para>
    /// </summary>
    /// <param name="original">An existing object that you want to make a copy of.</param>
    /// <param name="position">Position for the new object.</param>
    /// <param name="rotation">Orientation of the new object.</param>
    /// <param name="parent">Parent that will be assigned to the new object.</param>
    /// <param name="instantiateInWorldSpace">When you assign a parent Object, pass true to position the new object directly in world space. Pass false to set the Object’s position relative to its new parent.</param>
    /// <param name="parameters">A struct containing all the parameters.</param>
    /// <param name="scene"></param>
    /// <returns>
    ///   <para>The instantiated clone.</para>
    /// </returns>
    [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
    public static Object Instantiate(Object original, Scene scene)
    {
      Object.CheckNullArgument((object) original, "The Object you want to instantiate is null.");
      Object @object = Object.Internal_CloneSingleWithScene(original, scene);
      return !(@object == (Object) null) ? @object : throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
    }

    public static T Instantiate<T>(T original, InstantiateParameters parameters) where T : Object
    {
      Object.CheckNullArgument((object) original, "The Object you want to instantiate is null.");
      T obj = (T) Object.Internal_CloneSingleWithParams((Object) original, parameters);
      return !((Object) obj == (Object) null) ? obj : throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
    }

    public static T Instantiate<T>(
      T original,
      Vector3 position,
      Quaternion rotation,
      InstantiateParameters parameters)
      where T : Object
    {
      Object.CheckNullArgument((object) original, "The Object you want to instantiate is null.");
      T obj = (T) Object.Internal_InstantiateSingleWithParams((Object) original, position, rotation, parameters);
      return !((Object) obj == (Object) null) ? obj : throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
    }

    /// <summary>
    ///   <para>Clones the object original and returns the clone.</para>
    /// </summary>
    /// <param name="original">An existing object that you want to make a copy of.</param>
    /// <param name="position">Position for the new object.</param>
    /// <param name="rotation">Orientation of the new object.</param>
    /// <param name="parent">Parent that will be assigned to the new object.</param>
    /// <param name="instantiateInWorldSpace">When you assign a parent Object, pass true to position the new object directly in world space. Pass false to set the Object’s position relative to its new parent.</param>
    /// <param name="parameters">A struct containing all the parameters.</param>
    /// <param name="scene"></param>
    /// <returns>
    ///   <para>The instantiated clone.</para>
    /// </returns>
    [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
    public static Object Instantiate(Object original, Transform parent)
    {
      return Object.Instantiate(original, parent, false);
    }

    /// <summary>
    ///   <para>Clones the object original and returns the clone.</para>
    /// </summary>
    /// <param name="original">An existing object that you want to make a copy of.</param>
    /// <param name="position">Position for the new object.</param>
    /// <param name="rotation">Orientation of the new object.</param>
    /// <param name="parent">Parent that will be assigned to the new object.</param>
    /// <param name="instantiateInWorldSpace">When you assign a parent Object, pass true to position the new object directly in world space. Pass false to set the Object’s position relative to its new parent.</param>
    /// <param name="parameters">A struct containing all the parameters.</param>
    /// <param name="scene"></param>
    /// <returns>
    ///   <para>The instantiated clone.</para>
    /// </returns>
    [TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
    public static Object Instantiate(
      Object original,
      Transform parent,
      bool instantiateInWorldSpace)
    {
      if ((Object) parent == (Object) null)
        return Object.Instantiate(original);
      Object.CheckNullArgument((object) original, "The Object you want to instantiate is null.");
      Object @object = Object.Internal_CloneSingleWithParent(original, parent, instantiateInWorldSpace);
      return !(@object == (Object) null) ? @object : throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
    }

    public static T Instantiate<T>(T original) where T : Object
    {
      Object.CheckNullArgument((object) original, "The Object you want to instantiate is null.");
      T obj = (T) Object.Internal_CloneSingle((Object) original);
      return !((Object) obj == (Object) null) ? obj : throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
    }

    public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
    {
      return (T) Object.Instantiate((Object) original, position, rotation);
    }

    public static T Instantiate<T>(
      T original,
      Vector3 position,
      Quaternion rotation,
      Transform parent)
      where T : Object
    {
      return (T) Object.Instantiate((Object) original, position, rotation, parent);
    }

    public static T Instantiate<T>(T original, Transform parent) where T : Object
    {
      return Object.Instantiate<T>(original, parent, false);
    }

    public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
    {
      return (T) Object.Instantiate((Object) original, parent, worldPositionStays);
    }

    /// <summary>
    ///   <para>Removes a GameObject, component or asset.</para>
    /// </summary>
    /// <param name="obj">The object to destroy.</param>
    /// <param name="t">The optional amount of time to delay before destroying the object.</param>
    [NativeMethod(Name = "Scripting::DestroyObjectFromScripting", IsFreeFunction = true, ThrowsException = true)]
    public static void Destroy(Object obj, [DefaultValue("0.0F")] float t)
    {
      Object.Destroy_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), t);
    }

    /// <summary>
    ///   <para>Removes a GameObject, component or asset.</para>
    /// </summary>
    /// <param name="obj">The object to destroy.</param>
    /// <param name="t">The optional amount of time to delay before destroying the object.</param>
    [ExcludeFromDocs]
    public static void Destroy(Object obj)
    {
      float t = 0.0f;
      Object.Destroy(obj, t);
    }

    /// <summary>
    ///   <para>Destroys the object obj immediately. You are strongly recommended to use Destroy instead.</para>
    /// </summary>
    /// <param name="obj">Object to be destroyed.</param>
    /// <param name="allowDestroyingAssets">Set to true to allow assets to be destroyed.</param>
    [NativeMethod(Name = "Scripting::DestroyObjectFromScriptingImmediate", IsFreeFunction = true, ThrowsException = true)]
    public static void DestroyImmediate(Object obj, [DefaultValue("false")] bool allowDestroyingAssets)
    {
      Object.DestroyImmediate_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), allowDestroyingAssets);
    }

    /// <summary>
    ///   <para>Destroys the object obj immediately. You are strongly recommended to use Destroy instead.</para>
    /// </summary>
    /// <param name="obj">Object to be destroyed.</param>
    /// <param name="allowDestroyingAssets">Set to true to allow assets to be destroyed.</param>
    [ExcludeFromDocs]
    public static void DestroyImmediate(Object obj)
    {
      bool allowDestroyingAssets = false;
      Object.DestroyImmediate(obj, allowDestroyingAssets);
    }

    /// <summary>
    ///   <para>Gets a list of all loaded objects of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="includeInactive">If true, components attached to inactive GameObjects are also included.</param>
    /// <returns>
    ///   <para>The array of objects found matching the type specified.</para>
    /// </returns>
    [Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID, but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
    public static Object[] FindObjectsOfType(System.Type type)
    {
      return Object.FindObjectsOfType(type, false);
    }

    /// <summary>
    ///   <para>Gets a list of all loaded objects of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="includeInactive">If true, components attached to inactive GameObjects are also included.</param>
    /// <returns>
    ///   <para>The array of objects found matching the type specified.</para>
    /// </returns>
    [Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
    [FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
    [TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern Object[] FindObjectsOfType(System.Type type, bool includeInactive);

    /// <summary>
    ///   <para>Retrieves a list of all loaded objects of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="findObjectsInactive">Whether to include components attached to inactive GameObjects. If you don't specify this parameter, this function doesn't include inactive objects in the results.</param>
    /// <param name="sortMode">Whether and how to sort the returned array. Not sorting the array makes this function run significantly faster.</param>
    /// <returns>
    ///   <para>The array of objects found matching the type specified.</para>
    /// </returns>
    public static Object[] FindObjectsByType(System.Type type, FindObjectsSortMode sortMode)
    {
      return Object.FindObjectsByType(type, FindObjectsInactive.Exclude, sortMode);
    }

    /// <summary>
    ///   <para>Retrieves a list of all loaded objects of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="findObjectsInactive">Whether to include components attached to inactive GameObjects. If you don't specify this parameter, this function doesn't include inactive objects in the results.</param>
    /// <param name="sortMode">Whether and how to sort the returned array. Not sorting the array makes this function run significantly faster.</param>
    /// <returns>
    ///   <para>The array of objects found matching the type specified.</para>
    /// </returns>
    [TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
    [FreeFunction("UnityEngineObjectBindings::FindObjectsByType")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern Object[] FindObjectsByType(
      System.Type type,
      FindObjectsInactive findObjectsInactive,
      FindObjectsSortMode sortMode);

    /// <summary>
    ///   <para>Do not destroy the target Object when loading a new Scene.</para>
    /// </summary>
    /// <param name="target">An Object not destroyed on Scene change.</param>
    [FreeFunction("GetSceneManager().DontDestroyOnLoad", ThrowsException = true)]
    public static void DontDestroyOnLoad([UnityEngine.Bindings.NotNull] Object target)
    {
      if ((object) target == null)
        ThrowHelper.ThrowArgumentNullException((object) target, nameof (target));
      IntPtr target1 = Object.MarshalledUnityObject.MarshalNotNull<Object>(target);
      if (target1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) target, nameof (target));
      Object.DontDestroyOnLoad_Injected(target1);
    }

    /// <summary>
    ///   <para>Should the object be hidden, saved with the Scene or modifiable by the user?</para>
    /// </summary>
    public HideFlags hideFlags
    {
      get
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        return Object.get_hideFlags_Injected(_unity_self);
      }
      set
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        Object.set_hideFlags_Injected(_unity_self, value);
      }
    }

    [Obsolete("use Object.Destroy instead.")]
    public static void DestroyObject(Object obj, [DefaultValue("0.0F")] float t)
    {
      Object.Destroy(obj, t);
    }

    [ExcludeFromDocs]
    [Obsolete("use Object.Destroy instead.")]
    public static void DestroyObject(Object obj)
    {
      float t = 0.0f;
      Object.Destroy(obj, t);
    }

    [Obsolete("Object.FindSceneObjectsOfType has been deprecated, Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindSceneObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
    public static Object[] FindSceneObjectsOfType(System.Type type)
    {
      return Object.FindObjectsOfType(type);
    }

    /// <summary>
    ///   <para>Returns a list of all active and inactive loaded objects of Type type, including assets.</para>
    /// </summary>
    /// <param name="type">The type of object or asset to find.</param>
    /// <returns>
    ///   <para>The array of objects and assets found matching the type specified.</para>
    /// </returns>
    [Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
    [FreeFunction("UnityEngineObjectBindings::FindObjectsOfTypeIncludingAssets")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern Object[] FindObjectsOfTypeIncludingAssets(System.Type type);

    [Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
    public static T[] FindObjectsOfType<T>() where T : Object
    {
      return Resources.ConvertObjects<T>(Object.FindObjectsOfType(typeof (T), false));
    }

    public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode) where T : Object
    {
      return Resources.ConvertObjects<T>(Object.FindObjectsByType(typeof (T), FindObjectsInactive.Exclude, sortMode));
    }

    [Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
    public static T[] FindObjectsOfType<T>(bool includeInactive) where T : Object
    {
      return Resources.ConvertObjects<T>(Object.FindObjectsOfType(typeof (T), includeInactive));
    }

    public static T[] FindObjectsByType<T>(
      FindObjectsInactive findObjectsInactive,
      FindObjectsSortMode sortMode)
      where T : Object
    {
      return Resources.ConvertObjects<T>(Object.FindObjectsByType(typeof (T), findObjectsInactive, sortMode));
    }

    [Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
    public static T FindObjectOfType<T>() where T : Object
    {
      return (T) Object.FindObjectOfType(typeof (T), false);
    }

    [Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
    public static T FindObjectOfType<T>(bool includeInactive) where T : Object
    {
      return (T) Object.FindObjectOfType(typeof (T), includeInactive);
    }

    public static T FindFirstObjectByType<T>() where T : Object
    {
      return (T) Object.FindFirstObjectByType(typeof (T), FindObjectsInactive.Exclude);
    }

    public static T FindAnyObjectByType<T>() where T : Object
    {
      return (T) Object.FindAnyObjectByType(typeof (T), FindObjectsInactive.Exclude);
    }

    public static T FindFirstObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
    {
      return (T) Object.FindFirstObjectByType(typeof (T), findObjectsInactive);
    }

    public static T FindAnyObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
    {
      return (T) Object.FindAnyObjectByType(typeof (T), findObjectsInactive);
    }

    /// <summary>
    ///   <para>Returns a list of all active and inactive loaded objects of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <returns>
    ///   <para>The array of objects found matching the type specified.</para>
    /// </returns>
    [Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
    public static Object[] FindObjectsOfTypeAll(System.Type type)
    {
      return Resources.FindObjectsOfTypeAll(type);
    }

    private static void CheckNullArgument(object arg, string message)
    {
      if (arg == null)
        throw new ArgumentException(message);
    }

    /// <summary>
    ///   <para>Returns the first active loaded object of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="includeInactive"></param>
    /// <returns>
    ///   <para>Object The first active loaded object that matches the specified type. It returns null if no Object matches the type.</para>
    /// </returns>
    [Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
    [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
    public static Object FindObjectOfType(System.Type type)
    {
      Object[] objectsOfType = Object.FindObjectsOfType(type, false);
      return objectsOfType.Length != 0 ? objectsOfType[0] : (Object) null;
    }

    /// <summary>
    ///   <para>Retrieves the first active loaded object of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="findObjectsInactive">Whether to include components attached to inactive GameObjects. If you don't specify this parameter, this function doesn't include inactive objects in the results.</param>
    /// <returns>
    ///   <para>Returns the first active loaded object that matches the specified type. If no object matches the specified type, returns null.</para>
    /// </returns>
    public static Object FindFirstObjectByType(System.Type type)
    {
      Object[] objectsByType = Object.FindObjectsByType(type, FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
      return objectsByType.Length != 0 ? objectsByType[0] : (Object) null;
    }

    /// <summary>
    ///   <para>Retrieves any active loaded object of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="findObjectsInactive">Whether to include components attached to inactive GameObjects. If you don't specify this parameter, this function doesn't include inactive objects in the results.</param>
    /// <returns>
    ///   <para>Returns an arbitrary active loaded object that matches the specified type. If no object matches the specified type, returns null.</para>
    /// </returns>
    public static Object FindAnyObjectByType(System.Type type)
    {
      Object[] objectsByType = Object.FindObjectsByType(type, FindObjectsInactive.Exclude, FindObjectsSortMode.None);
      return objectsByType.Length != 0 ? objectsByType[0] : (Object) null;
    }

    /// <summary>
    ///   <para>Returns the first active loaded object of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="includeInactive"></param>
    /// <returns>
    ///   <para>Object The first active loaded object that matches the specified type. It returns null if no Object matches the type.</para>
    /// </returns>
    [TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
    [Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
    public static Object FindObjectOfType(System.Type type, bool includeInactive)
    {
      Object[] objectsOfType = Object.FindObjectsOfType(type, includeInactive);
      return objectsOfType.Length != 0 ? objectsOfType[0] : (Object) null;
    }

    /// <summary>
    ///   <para>Retrieves the first active loaded object of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="findObjectsInactive">Whether to include components attached to inactive GameObjects. If you don't specify this parameter, this function doesn't include inactive objects in the results.</param>
    /// <returns>
    ///   <para>Returns the first active loaded object that matches the specified type. If no object matches the specified type, returns null.</para>
    /// </returns>
    public static Object FindFirstObjectByType(System.Type type, FindObjectsInactive findObjectsInactive)
    {
      Object[] objectsByType = Object.FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.InstanceID);
      return objectsByType.Length != 0 ? objectsByType[0] : (Object) null;
    }

    /// <summary>
    ///   <para>Retrieves any active loaded object of Type type.</para>
    /// </summary>
    /// <param name="type">The type of object to find.</param>
    /// <param name="findObjectsInactive">Whether to include components attached to inactive GameObjects. If you don't specify this parameter, this function doesn't include inactive objects in the results.</param>
    /// <returns>
    ///   <para>Returns an arbitrary active loaded object that matches the specified type. If no object matches the specified type, returns null.</para>
    /// </returns>
    public static Object FindAnyObjectByType(System.Type type, FindObjectsInactive findObjectsInactive)
    {
      Object[] objectsByType = Object.FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.None);
      return objectsByType.Length != 0 ? objectsByType[0] : (Object) null;
    }

    /// <summary>
    ///   <para>Returns the name of the object.</para>
    /// </summary>
    /// <returns>
    ///   <para>The name returned by ToString.</para>
    /// </returns>
    public override string ToString() => Object.ToString(this);

    public static bool operator ==(Object x, Object y) => Object.CompareBaseObjects(x, y);

    public static bool operator !=(Object x, Object y) => !Object.CompareBaseObjects(x, y);

    [NativeMethod(Name = "Object::GetOffsetOfInstanceIdMember", IsFreeFunction = true, IsThreadSafe = true)]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern int GetOffsetOfInstanceIDInCPlusPlusObject();

    [NativeMethod(Name = "CurrentThreadIsMainThread", IsFreeFunction = true, IsThreadSafe = true)]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool CurrentThreadIsMainThread();

    [NativeMethod(Name = "CloneObject", IsFreeFunction = true, ThrowsException = true)]
    private static Object Internal_CloneSingle([UnityEngine.Bindings.NotNull] Object data)
    {
      if ((object) data == null)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      IntPtr data1 = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
      if (data1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_CloneSingle_Injected(data1));
    }

    [FreeFunction("CloneObjectToScene")]
    private static Object Internal_CloneSingleWithScene([UnityEngine.Bindings.NotNull] Object data, Scene scene)
    {
      if ((object) data == null)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      IntPtr data1 = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
      if (data1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_CloneSingleWithScene_Injected(data1, ref scene));
    }

    [FreeFunction("CloneObjectWithParams")]
    private static Object Internal_CloneSingleWithParams(
      [UnityEngine.Bindings.NotNull] Object data,
      InstantiateParameters parameters)
    {
      if ((object) data == null)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      IntPtr data1 = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
      if (data1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_CloneSingleWithParams_Injected(data1, ref parameters));
    }

    [FreeFunction("InstantiateObjectWithParams")]
    private static Object Internal_InstantiateSingleWithParams(
      [UnityEngine.Bindings.NotNull] Object data,
      Vector3 position,
      Quaternion rotation,
      InstantiateParameters parameters)
    {
      if ((object) data == null)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      IntPtr data1 = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
      if (data1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_InstantiateSingleWithParams_Injected(data1, ref position, ref rotation, ref parameters));
    }

    [FreeFunction("CloneObject")]
    private static Object Internal_CloneSingleWithParent(
      [UnityEngine.Bindings.NotNull] Object data,
      [UnityEngine.Bindings.NotNull] Transform parent,
      bool worldPositionStays)
    {
      if ((object) data == null)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      if (parent == null)
        ThrowHelper.ThrowArgumentNullException((object) parent, nameof (parent));
      IntPtr data1 = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
      if (data1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      IntPtr parent1 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(parent);
      if (parent1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) parent, nameof (parent));
      return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_CloneSingleWithParent_Injected(data1, parent1, worldPositionStays));
    }

    [FreeFunction("InstantiateAsyncObjects")]
    private static IntPtr Internal_InstantiateAsyncWithParams(
      [UnityEngine.Bindings.NotNull] Object original,
      int count,
      InstantiateParameters parameters,
      IntPtr positions,
      int positionsCount,
      IntPtr rotations,
      int rotationsCount,
      bool hasManagedCancellationToken)
    {
      if ((object) original == null)
        ThrowHelper.ThrowArgumentNullException((object) original, nameof (original));
      IntPtr original1 = Object.MarshalledUnityObject.MarshalNotNull<Object>(original);
      if (original1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) original, nameof (original));
      return Object.Internal_InstantiateAsyncWithParams_Injected(original1, count, ref parameters, positions, positionsCount, rotations, rotationsCount, hasManagedCancellationToken);
    }

    [FreeFunction("InstantiateObject")]
    private static Object Internal_InstantiateSingle([UnityEngine.Bindings.NotNull] Object data, Vector3 pos, Quaternion rot)
    {
      if ((object) data == null)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      IntPtr data1 = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
      if (data1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_InstantiateSingle_Injected(data1, ref pos, ref rot));
    }

    [FreeFunction("InstantiateObject")]
    private static Object Internal_InstantiateSingleWithParent(
      [UnityEngine.Bindings.NotNull] Object data,
      [UnityEngine.Bindings.NotNull] Transform parent,
      Vector3 pos,
      Quaternion rot)
    {
      if ((object) data == null)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      if (parent == null)
        ThrowHelper.ThrowArgumentNullException((object) parent, nameof (parent));
      IntPtr data1 = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
      if (data1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) data, nameof (data));
      IntPtr parent1 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(parent);
      if (parent1 == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) parent, nameof (parent));
      return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_InstantiateSingleWithParent_Injected(data1, parent1, ref pos, ref rot));
    }

    [FreeFunction("UnityEngineObjectBindings::ToString")]
    private static string ToString(Object obj)
    {
      ManagedSpanWrapper ret;
      string stringAndDispose;
      try
      {
        Object.ToString_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), out ret);
      }
      finally
      {
        stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
      }
      return stringAndDispose;
    }

    [FreeFunction("UnityEngineObjectBindings::GetName", HasExplicitThis = true)]
    private string GetName()
    {
      ManagedSpanWrapper ret;
      string stringAndDispose;
      try
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        Object.GetName_Injected(_unity_self, out ret);
      }
      finally
      {
        stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
      }
      return stringAndDispose;
    }

    [FreeFunction("UnityEngineObjectBindings::IsPersistent")]
    internal static bool IsPersistent([UnityEngine.Bindings.NotNull] Object obj)
    {
      if ((object) obj == null)
        ThrowHelper.ThrowArgumentNullException((object) obj, nameof (obj));
      IntPtr num = Object.MarshalledUnityObject.MarshalNotNull<Object>(obj);
      if (num == IntPtr.Zero)
        ThrowHelper.ThrowArgumentNullException((object) obj, nameof (obj));
      return Object.IsPersistent_Injected(num);
    }

    [FreeFunction("UnityEngineObjectBindings::SetName", HasExplicitThis = true)]
    private unsafe void SetName(string name)
    {
      try
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        ManagedSpanWrapper managedSpanWrapper;
        if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
        {
          ReadOnlySpan<char> readOnlySpan = name.AsSpan();
          fixed (char* begin = &readOnlySpan.GetPinnableReference())
            managedSpanWrapper = new ManagedSpanWrapper((void*) begin, readOnlySpan.Length);
        }
        Object.SetName_Injected(_unity_self, ref managedSpanWrapper);
      }
      finally
      {
        // ISSUE: fixed variable is out of scope
        // ISSUE: __unpin statement
        __unpin(begin);
      }
    }

    [NativeMethod(Name = "UnityEngineObjectBindings::DoesObjectWithInstanceIDExist", IsFreeFunction = true, IsThreadSafe = true)]
    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static extern bool DoesObjectWithInstanceIDExist(int instanceID);

    [VisibleToOtherModules]
    [FreeFunction("UnityEngineObjectBindings::FindObjectFromInstanceID")]
    internal static Object FindObjectFromInstanceID(int instanceID)
    {
      return Unmarshal.UnmarshalUnityObject<Object>(Object.FindObjectFromInstanceID_Injected(instanceID));
    }

    [FreeFunction("UnityEngineObjectBindings::GetPtrFromInstanceID")]
    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr GetPtrFromInstanceID(
      int instanceID,
      System.Type objectType,
      out bool isMonoBehaviour);

    [VisibleToOtherModules]
    [FreeFunction("UnityEngineObjectBindings::ForceLoadFromInstanceID")]
    internal static Object ForceLoadFromInstanceID(int instanceID)
    {
      return Unmarshal.UnmarshalUnityObject<Object>(Object.ForceLoadFromInstanceID_Injected(instanceID));
    }

    [VisibleToOtherModules(new string[] {"UnityEngine.UIElementsModule"})]
    internal static Object CreateMissingReferenceObject(int instanceID)
    {
      return new Object() { m_InstanceID = instanceID };
    }

    [FreeFunction("UnityEngineObjectBindings::MarkObjectDirty", HasExplicitThis = true)]
    internal void MarkDirty()
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      Object.MarkDirty_Injected(_unity_self);
    }

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void Destroy_Injected(IntPtr obj, [DefaultValue("0.0F")] float t);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void DestroyImmediate_Injected(IntPtr obj, [DefaultValue("false")] bool allowDestroyingAssets);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void DontDestroyOnLoad_Injected(IntPtr target);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern HideFlags get_hideFlags_Injected(IntPtr _unity_self);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void set_hideFlags_Injected(IntPtr _unity_self, HideFlags value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr Internal_CloneSingle_Injected(IntPtr data);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr Internal_CloneSingleWithScene_Injected(
      IntPtr data,
      [In] ref Scene scene);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr Internal_CloneSingleWithParams_Injected(
      IntPtr data,
      [In] ref InstantiateParameters parameters);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr Internal_InstantiateSingleWithParams_Injected(
      IntPtr data,
      [In] ref Vector3 position,
      [In] ref Quaternion rotation,
      [In] ref InstantiateParameters parameters);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr Internal_CloneSingleWithParent_Injected(
      IntPtr data,
      IntPtr parent,
      bool worldPositionStays);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr Internal_InstantiateAsyncWithParams_Injected(
      IntPtr original,
      int count,
      [In] ref InstantiateParameters parameters,
      IntPtr positions,
      int positionsCount,
      IntPtr rotations,
      int rotationsCount,
      bool hasManagedCancellationToken);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr Internal_InstantiateSingle_Injected(
      IntPtr data,
      [In] ref Vector3 pos,
      [In] ref Quaternion rot);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr Internal_InstantiateSingleWithParent_Injected(
      IntPtr data,
      IntPtr parent,
      [In] ref Vector3 pos,
      [In] ref Quaternion rot);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void ToString_Injected(IntPtr obj, out ManagedSpanWrapper ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void GetName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool IsPersistent_Injected(IntPtr obj);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void SetName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr FindObjectFromInstanceID_Injected(int instanceID);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr ForceLoadFromInstanceID_Injected(int instanceID);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void MarkDirty_Injected(IntPtr _unity_self);

    [VisibleToOtherModules]
    internal static class MarshalledUnityObject
    {
      private static readonly System.Type[] m_MonoBehaviorBaseClasses;

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static IntPtr Marshal<T>(T obj) where T : Object
      {
        return (object) obj == null ? IntPtr.Zero : Object.MarshalledUnityObject.MarshalNotNull<T>(obj);
      }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public static IntPtr MarshalNotNull<T>(T obj) where T : Object
      {
        return obj.m_CachedPtr != IntPtr.Zero ? obj.m_CachedPtr : Object.MarshalledUnityObject.MarshalFromInstanceId<T>(obj);
      }

      private static IntPtr MarshalFromInstanceId<T>(T obj) where T : Object
      {
        if (obj.m_InstanceID == 0)
          return IntPtr.Zero;
        bool isMonoBehaviour;
        IntPtr ptrFromInstanceId = Object.GetPtrFromInstanceID(obj.m_InstanceID, typeof (T), out isMonoBehaviour);
        return ptrFromInstanceId == IntPtr.Zero || isMonoBehaviour && !Object.MarshalledUnityObject.IsMonoBehaviourOrScriptableObjectOrParentClass((Object) obj) ? IntPtr.Zero : ptrFromInstanceId;
      }

      private static bool IsMonoBehaviourOrScriptableObjectOrParentClass(Object obj)
      {
        System.Type type = obj.GetType();
        return type == typeof (Object) || type == typeof (MonoBehaviour) || type == typeof (ScriptableObject) || Array.IndexOf<System.Type>(Object.MarshalledUnityObject.m_MonoBehaviorBaseClasses, type) >= 0;
      }

      static MarshalledUnityObject()
      {
        List<System.Type> typeList = new List<System.Type>();
        for (System.Type baseType = typeof (MonoBehaviour).BaseType; baseType != typeof (Object); baseType = baseType.BaseType)
          typeList.Add(baseType);
        for (System.Type baseType = typeof (ScriptableObject).BaseType; baseType != typeof (Object); baseType = baseType.BaseType)
          typeList.Add(baseType);
        Object.MarshalledUnityObject.m_MonoBehaviorBaseClasses = typeList.ToArray();
      }

      public static void TryThrowEditorNullExceptionObject(Object unityObj, string parameterName)
      {
        string str1 = unityObj.m_UnityRuntimeErrorString ?? "";
        if (unityObj.m_InstanceID != 0 && !str1.StartsWith("MissingReferenceException:"))
        {
          string message = "The object of type '" + unityObj.GetType().FullName + "' has been destroyed but you are still trying to access it.\nYour script should either check if it is null or you should not destroy the object.";
          if (!string.IsNullOrEmpty(parameterName))
            message = message + " Parameter name: " + parameterName;
          throw new MissingReferenceException(message);
        }
        int length = str1.IndexOf(':');
        if (length <= 0)
          return;
        string str2 = str1.Substring(0, length);
        string str3 = str1.Substring(length + 1);
        if (!string.IsNullOrEmpty(parameterName))
          str3 = str3 + " Parameter name: " + parameterName;
        System.Type type = System.Type.GetType("UnityEngine." + str2, false);
        if (type != (System.Type) null)
          throw (Exception) Activator.CreateInstance(type, (object) str3);
      }
    }
  }
}
