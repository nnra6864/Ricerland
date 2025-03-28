// Decompiled with JetBrains decompiler
// Type: UnityEngine.Font
// Assembly: UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 14BFC512-1C5A-497A-BDCD-8EB087D2153B
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEngine.TextRenderingModule.dll
// XML documentation location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEngine.TextRenderingModule.xml

using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

#nullable disable
namespace UnityEngine
{
  /// <summary>
  ///   <para>Script interface for.</para>
  /// </summary>
  [NativeClass("TextRendering::Font")]
  [NativeHeader("Modules/TextRendering/Public/Font.h")]
  [StaticAccessor("TextRenderingPrivate", StaticAccessorType.DoubleColon)]
  [NativeHeader("Modules/TextRendering/Public/FontImpl.h")]
  public sealed class Font : Object
  {
    public static event Action<Font> textureRebuilt;

    private event Font.FontTextureRebuildCallback m_FontTextureRebuildCallback;

    /// <summary>
    ///   <para>The material used for the font display.</para>
    /// </summary>
    public Material material
    {
      get
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        return Unmarshal.UnmarshalUnityObject<Material>(Font.get_material_Injected(_unity_self));
      }
      set
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        Font.set_material_Injected(_unity_self, Object.MarshalledUnityObject.Marshal<Material>(value));
      }
    }

    public string[] fontNames
    {
      [return: Unmarshalled] get
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        return Font.get_fontNames_Injected(_unity_self);
      }
      [param: Unmarshalled] set
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        Font.set_fontNames_Injected(_unity_self, value);
      }
    }

    /// <summary>
    ///   <para>Is the font a dynamic font.</para>
    /// </summary>
    public bool dynamic
    {
      get
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        return Font.get_dynamic_Injected(_unity_self);
      }
    }

    /// <summary>
    ///   <para>The ascent of the font.</para>
    /// </summary>
    public int ascent
    {
      get
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        return Font.get_ascent_Injected(_unity_self);
      }
    }

    /// <summary>
    ///   <para>The default size of the font.</para>
    /// </summary>
    public int fontSize
    {
      get
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        return Font.get_fontSize_Injected(_unity_self);
      }
    }

    /// <summary>
    ///   <para>Access an array of all characters contained in the font texture.</para>
    /// </summary>
    public unsafe CharacterInfo[] characterInfo
    {
      [FreeFunction("TextRenderingPrivate::GetFontCharacterInfo", HasExplicitThis = true)] get
      {
        BlittableArrayWrapper ret;
        CharacterInfo[] characterInfo;
        try
        {
          IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
          if (_unity_self == IntPtr.Zero)
            ThrowHelper.ThrowNullReferenceException((object) this);
          Font.get_characterInfo_Injected(_unity_self, out ret);
        }
        finally
        {
          CharacterInfo[] array;
          ret.Unmarshal<CharacterInfo>(ref array);
          characterInfo = array;
        }
        return characterInfo;
      }
      [FreeFunction("TextRenderingPrivate::SetFontCharacterInfo", HasExplicitThis = true)] set
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        Span<CharacterInfo> span = new Span<CharacterInfo>(value);
        fixed (CharacterInfo* begin = &span.GetPinnableReference())
        {
          ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*) begin, span.Length);
          Font.set_characterInfo_Injected(_unity_self, ref managedSpanWrapper);
        }
      }
    }

    /// <summary>
    ///   <para>The line height of the font.</para>
    /// </summary>
    [NativeProperty("LineSpacing", false, TargetType.Function)]
    public int lineHeight
    {
      get
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        return Font.get_lineHeight_Injected(_unity_self);
      }
    }

    [Obsolete("Font.textureRebuildCallback has been deprecated. Use Font.textureRebuilt instead.")]
    public Font.FontTextureRebuildCallback textureRebuildCallback { get; set; }

    /// <summary>
    ///   <para>Create a new Font.</para>
    /// </summary>
    /// <param name="name">The name of the created Font object.</param>
    public Font() => Font.Internal_CreateFont(this, (string) null);

    /// <summary>
    ///   <para>Create a new Font.</para>
    /// </summary>
    /// <param name="name">The name of the created Font object.</param>
    public Font(string name)
    {
      if (Path.GetDirectoryName(name) == string.Empty)
        Font.Internal_CreateFont(this, name);
      else
        Font.Internal_CreateFontFromPath(this, name);
    }

    private Font(string[] names, int size) => Font.Internal_CreateDynamicFont(this, names, size);

    /// <summary>
    ///   <para>Creates a Font object which lets you render a font installed on the user machine.</para>
    /// </summary>
    /// <param name="fontname">The name of the OS font to use for this font object.</param>
    /// <param name="size">The default character size of the generated font.</param>
    /// <param name="fontnames">Am array of names of OS fonts to use for this font object. When rendering characters using this font object, the first font which is installed on the machine, which contains the requested character will be used.</param>
    /// <returns>
    ///   <para>The generate Font object.</para>
    /// </returns>
    public static Font CreateDynamicFontFromOSFont(string fontname, int size)
    {
      return new Font(new string[1]{ fontname }, size);
    }

    /// <summary>
    ///   <para>Creates a Font object which lets you render a font installed on the user machine.</para>
    /// </summary>
    /// <param name="fontname">The name of the OS font to use for this font object.</param>
    /// <param name="size">The default character size of the generated font.</param>
    /// <param name="fontnames">Am array of names of OS fonts to use for this font object. When rendering characters using this font object, the first font which is installed on the machine, which contains the requested character will be used.</param>
    /// <returns>
    ///   <para>The generate Font object.</para>
    /// </returns>
    public static Font CreateDynamicFontFromOSFont(string[] fontnames, int size)
    {
      return new Font(fontnames, size);
    }

    [RequiredByNativeCode]
    internal static void InvokeTextureRebuilt_Internal(Font font)
    {
      Action<Font> textureRebuilt = Font.textureRebuilt;
      if (textureRebuilt != null)
        textureRebuilt(font);
      Font.FontTextureRebuildCallback textureRebuildCallback = font.m_FontTextureRebuildCallback;
      if (textureRebuildCallback == null)
        return;
      textureRebuildCallback();
    }

    /// <summary>
    ///   <para>Returns the maximum number of verts that the text generator may return for a given string.</para>
    /// </summary>
    /// <param name="str">Input string.</param>
    public static int GetMaxVertsForString(string str) => str.Length * 4 + 4;

    internal static Font GetDefault()
    {
      return Unmarshal.UnmarshalUnityObject<Font>(Font.GetDefault_Injected());
    }

    /// <summary>
    ///   <para>Does this font have a specific character?</para>
    /// </summary>
    /// <param name="c">The character to check for.</param>
    /// <returns>
    ///   <para>Whether or not the font has the character specified.</para>
    /// </returns>
    public bool HasCharacter(char c) => this.HasCharacter((int) c);

    private bool HasCharacter(int c)
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return Font.HasCharacter_Injected(_unity_self, c);
    }

    /// <summary>
    ///   <para>Get names of fonts installed on the machine.</para>
    /// </summary>
    /// <returns>
    ///   <para>An array of the names of all fonts installed on the machine.</para>
    /// </returns>
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern string[] GetOSInstalledFontNames();

    /// <summary>
    ///   <para>Gets the file paths of the fonts that are installed on the operating system.</para>
    /// </summary>
    /// <returns>
    ///   <para>An array of the file paths of all fonts installed on the machine.</para>
    /// </returns>
    [MethodImpl(MethodImplOptions.InternalCall)]
    public static extern string[] GetPathsToOSFonts();

    [VisibleToOtherModules(new string[] {"UnityEngine.TextCoreTextEngineModule"})]
    [MethodImpl(MethodImplOptions.InternalCall)]
    internal static extern string[] GetOSFallbacks();

    private static unsafe void Internal_CreateFont([Writable] Font self, string name)
    {
      try
      {
        Font self1 = self;
        ManagedSpanWrapper managedSpanWrapper;
        if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
        {
          ReadOnlySpan<char> readOnlySpan = name.AsSpan();
          fixed (char* begin = &readOnlySpan.GetPinnableReference())
            managedSpanWrapper = new ManagedSpanWrapper((void*) begin, readOnlySpan.Length);
        }
        ref ManagedSpanWrapper local = ref managedSpanWrapper;
        Font.Internal_CreateFont_Injected(self1, ref local);
      }
      finally
      {
        // ISSUE: fixed variable is out of scope
        // ISSUE: __unpin statement
        __unpin(begin);
      }
    }

    private static unsafe void Internal_CreateFontFromPath([Writable] Font self, string fontPath)
    {
      try
      {
        Font self1 = self;
        ManagedSpanWrapper managedSpanWrapper;
        if (!StringMarshaller.TryMarshalEmptyOrNullString(fontPath, ref managedSpanWrapper))
        {
          ReadOnlySpan<char> readOnlySpan = fontPath.AsSpan();
          fixed (char* begin = &readOnlySpan.GetPinnableReference())
            managedSpanWrapper = new ManagedSpanWrapper((void*) begin, readOnlySpan.Length);
        }
        ref ManagedSpanWrapper local = ref managedSpanWrapper;
        Font.Internal_CreateFontFromPath_Injected(self1, ref local);
      }
      finally
      {
        // ISSUE: fixed variable is out of scope
        // ISSUE: __unpin statement
        __unpin(begin);
      }
    }

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void Internal_CreateDynamicFont([Writable] Font self, [Unmarshalled] string[] _names, int size);

    [FreeFunction("TextRenderingPrivate::GetCharacterInfo", HasExplicitThis = true)]
    public bool GetCharacterInfo(char ch, out CharacterInfo info, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style)
    {
      IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
      if (_unity_self == IntPtr.Zero)
        ThrowHelper.ThrowNullReferenceException((object) this);
      return Font.GetCharacterInfo_Injected(_unity_self, ch, out info, size, style);
    }

    [ExcludeFromDocs]
    public bool GetCharacterInfo(char ch, out CharacterInfo info, int size)
    {
      return this.GetCharacterInfo(ch, out info, size, FontStyle.Normal);
    }

    [ExcludeFromDocs]
    public bool GetCharacterInfo(char ch, out CharacterInfo info)
    {
      return this.GetCharacterInfo(ch, out info, 0, FontStyle.Normal);
    }

    /// <summary>
    ///   <para>Request characters to be added to the font texture (dynamic fonts only).</para>
    /// </summary>
    /// <param name="characters">The characters which are needed to be in the font texture.</param>
    /// <param name="size">The size of the requested characters (the default value of zero will use the font's default size).</param>
    /// <param name="style">The style of the requested characters.</param>
    public unsafe void RequestCharactersInTexture(string characters, [DefaultValue("0")] int size, [DefaultValue("FontStyle.Normal")] FontStyle style)
    {
      try
      {
        IntPtr _unity_self = Object.MarshalledUnityObject.MarshalNotNull<Font>(this);
        if (_unity_self == IntPtr.Zero)
          ThrowHelper.ThrowNullReferenceException((object) this);
        ManagedSpanWrapper managedSpanWrapper;
        if (!StringMarshaller.TryMarshalEmptyOrNullString(characters, ref managedSpanWrapper))
        {
          ReadOnlySpan<char> readOnlySpan = characters.AsSpan();
          fixed (char* begin = &readOnlySpan.GetPinnableReference())
            managedSpanWrapper = new ManagedSpanWrapper((void*) begin, readOnlySpan.Length);
        }
        Font.RequestCharactersInTexture_Injected(_unity_self, ref managedSpanWrapper, size, style);
      }
      finally
      {
        // ISSUE: fixed variable is out of scope
        // ISSUE: __unpin statement
        __unpin(begin);
      }
    }

    [ExcludeFromDocs]
    public void RequestCharactersInTexture(string characters, int size)
    {
      this.RequestCharactersInTexture(characters, size, FontStyle.Normal);
    }

    [ExcludeFromDocs]
    public void RequestCharactersInTexture(string characters)
    {
      this.RequestCharactersInTexture(characters, 0, FontStyle.Normal);
    }

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr get_material_Injected(IntPtr _unity_self);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void set_material_Injected(IntPtr _unity_self, IntPtr value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern string[] get_fontNames_Injected(IntPtr _unity_self);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void set_fontNames_Injected(IntPtr _unity_self, string[] value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool get_dynamic_Injected(IntPtr _unity_self);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern int get_ascent_Injected(IntPtr _unity_self);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern int get_fontSize_Injected(IntPtr _unity_self);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void get_characterInfo_Injected(
      IntPtr _unity_self,
      out BlittableArrayWrapper ret);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void set_characterInfo_Injected(
      IntPtr _unity_self,
      ref ManagedSpanWrapper value);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern int get_lineHeight_Injected(IntPtr _unity_self);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern IntPtr GetDefault_Injected();

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool HasCharacter_Injected(IntPtr _unity_self, int c);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void Internal_CreateFont_Injected([Writable] Font self, ref ManagedSpanWrapper name);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void Internal_CreateFontFromPath_Injected(
      [Writable] Font self,
      ref ManagedSpanWrapper fontPath);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern bool GetCharacterInfo_Injected(
      IntPtr _unity_self,
      char ch,
      out CharacterInfo info,
      [DefaultValue("0")] int size,
      [DefaultValue("FontStyle.Normal")] FontStyle style);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private static extern void RequestCharactersInTexture_Injected(
      IntPtr _unity_self,
      ref ManagedSpanWrapper characters,
      [DefaultValue("0")] int size,
      [DefaultValue("FontStyle.Normal")] FontStyle style);

    public delegate void FontTextureRebuildCallback();
  }
}
