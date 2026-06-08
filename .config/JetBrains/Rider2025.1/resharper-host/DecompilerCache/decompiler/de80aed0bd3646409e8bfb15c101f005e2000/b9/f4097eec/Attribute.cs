// Decompiled with JetBrains decompiler
// Type: System.Attribute
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: DE80AED0-BD36-4640-9E8B-FB15C101F005
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/UnityReferenceAssemblies/unity-4.8-api/mscorlib.dll

using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

#nullable disable
namespace System;

[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof (_Attribute))]
[ComVisible(true)]
[Serializable]
public abstract class Attribute : _Attribute
{
  public virtual object TypeId { get; }

  [SecuritySafeCritical]
  public override bool Equals(object obj);

  public static Attribute GetCustomAttribute(Assembly element, Type attributeType);

  public static Attribute GetCustomAttribute(Assembly element, Type attributeType, bool inherit);

  public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType);

  public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType, bool inherit);

  public static Attribute GetCustomAttribute(Module element, Type attributeType);

  public static Attribute GetCustomAttribute(Module element, Type attributeType, bool inherit);

  public static Attribute GetCustomAttribute(ParameterInfo element, Type attributeType);

  public static Attribute GetCustomAttribute(
    ParameterInfo element,
    Type attributeType,
    bool inherit);

  public static Attribute[] GetCustomAttributes(Assembly element);

  public static Attribute[] GetCustomAttributes(Assembly element, bool inherit);

  public static Attribute[] GetCustomAttributes(Assembly element, Type attributeType);

  public static Attribute[] GetCustomAttributes(Assembly element, Type attributeType, bool inherit);

  public static Attribute[] GetCustomAttributes(MemberInfo element);

  public static Attribute[] GetCustomAttributes(MemberInfo element, bool inherit);

  public static Attribute[] GetCustomAttributes(MemberInfo element, Type type);

  public static Attribute[] GetCustomAttributes(MemberInfo element, Type type, bool inherit);

  public static Attribute[] GetCustomAttributes(Module element);

  public static Attribute[] GetCustomAttributes(Module element, bool inherit);

  public static Attribute[] GetCustomAttributes(Module element, Type attributeType);

  public static Attribute[] GetCustomAttributes(Module element, Type attributeType, bool inherit);

  public static Attribute[] GetCustomAttributes(ParameterInfo element);

  public static Attribute[] GetCustomAttributes(ParameterInfo element, bool inherit);

  public static Attribute[] GetCustomAttributes(ParameterInfo element, Type attributeType);

  public static Attribute[] GetCustomAttributes(
    ParameterInfo element,
    Type attributeType,
    bool inherit);

  [SecuritySafeCritical]
  public override int GetHashCode();

  public virtual bool IsDefaultAttribute();

  public static bool IsDefined(Assembly element, Type attributeType);

  public static bool IsDefined(Assembly element, Type attributeType, bool inherit);

  public static bool IsDefined(MemberInfo element, Type attributeType);

  public static bool IsDefined(MemberInfo element, Type attributeType, bool inherit);

  public static bool IsDefined(Module element, Type attributeType);

  public static bool IsDefined(Module element, Type attributeType, bool inherit);

  public static bool IsDefined(ParameterInfo element, Type attributeType);

  public static bool IsDefined(ParameterInfo element, Type attributeType, bool inherit);

  public virtual bool Match(object obj);

  void _Attribute.GetIDsOfNames(
    [In] ref Guid riid,
    IntPtr rgszNames,
    uint cNames,
    uint lcid,
    IntPtr rgDispId);

  void _Attribute.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

  void _Attribute.GetTypeInfoCount(out uint pcTInfo);

  void _Attribute.Invoke(
    uint dispIdMember,
    [In] ref Guid riid,
    uint lcid,
    short wFlags,
    IntPtr pDispParams,
    IntPtr pVarResult,
    IntPtr pExcepInfo,
    IntPtr puArgErr);
}
