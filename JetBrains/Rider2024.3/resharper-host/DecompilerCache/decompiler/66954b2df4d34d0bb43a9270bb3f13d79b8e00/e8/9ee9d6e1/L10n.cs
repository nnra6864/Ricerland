// Decompiled with JetBrains decompiler
// Type: UnityEditor.L10n
// Assembly: UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 66954B2D-F4D3-4D0B-B43A-9270BB3F13D7
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEditor.CoreModule.dll
// XML documentation location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEditor.CoreModule.xml

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Internal;

#nullable disable
namespace UnityEditor
{
  /// <summary>
  ///   <para>Class for text localization.</para>
  /// </summary>
  public static class L10n
  {
    private static object lockObject = new object();
    private static Dictionary<Assembly, string> s_GroupNames = new Dictionary<Assembly, string>(128);
    private static Dictionary<L10n.LocKey, string> s_LocalizedStringCache = new Dictionary<L10n.LocKey, string>(10240);

    internal static void ClearCache()
    {
      lock (L10n.lockObject)
        L10n.s_LocalizedStringCache.Clear();
    }

    internal static string GetGroupName(Assembly assembly)
    {
      if (assembly == (Assembly) null)
        return (string) null;
      lock (L10n.lockObject)
      {
        string groupName;
        if (!L10n.s_GroupNames.TryGetValue(assembly, out groupName))
        {
          object[] customAttributes = assembly.GetCustomAttributes(typeof (LocalizationAttribute), true);
          if (customAttributes.Length != 0 && customAttributes[0] != null)
          {
            groupName = ((LocalizationAttribute) customAttributes[0]).locGroupName ?? assembly.GetName().Name;
            L10n.s_GroupNames[assembly] = groupName;
          }
          else
            L10n.s_GroupNames[assembly] = (string) null;
        }
        return groupName;
      }
    }

    /// <summary>
    ///   <para>This function referes a po file like ja.po as an asset. Asmdef and [assembly: UnityEditor.Localization] is needed.</para>
    /// </summary>
    /// <param name="str">Original text, basically English.</param>
    /// <returns>
    ///   <para>Localized text.</para>
    /// </returns>
    public static string Tr(string str) => L10n.Tr(str, Assembly.GetCallingAssembly());

    internal static string Tr(string str, object context)
    {
      return L10n.Tr(str, context?.GetType().Assembly);
    }

    internal static string Tr(string str, Assembly groupAssembly)
    {
      if (!LocalizationDatabase.enableEditorLocalization || string.IsNullOrEmpty(str))
        return str;
      lock (L10n.lockObject)
      {
        string groupName = L10n.GetGroupName(groupAssembly);
        L10n.LocKey key = new L10n.LocKey(str, groupName);
        string str1;
        if (L10n.s_LocalizedStringCache.TryGetValue(key, out str1))
          return str1;
        str1 = groupName != null ? LocalizationDatabase.GetLocalizedStringWithGroupName(str, groupName) : LocalizationDatabase.GetLocalizedString(str);
        L10n.s_LocalizedStringCache[key] = str1;
        return str1;
      }
    }

    public static string[] Tr(string[] str_list)
    {
      string[] strArray = new string[str_list.Length];
      for (int index = 0; index < strArray.Length; ++index)
        strArray[index] = L10n.Tr(str_list[index]);
      return strArray;
    }

    public static string Tr(string str, string groupName)
    {
      return LocalizationDatabase.GetLocalizedStringWithGroupName(str, groupName);
    }

    [ExcludeFromDocs]
    public static string TrPath(string path)
    {
      string[] separator = new string[1]{ "/" };
      StringBuilder stringBuilder = new StringBuilder(256);
      string[] strArray = path.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray.Length; ++index)
      {
        stringBuilder.Append(L10n.Tr(strArray[index]));
        if (index < strArray.Length - 1)
          stringBuilder.Append("/");
      }
      return stringBuilder.ToString();
    }

    [ExcludeFromDocs]
    public static GUIContent TextContent(string text, string tooltip = null, Texture icon = null)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrTextContent(text, tooltip, icon);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrTextContent(text, tooltip, icon);
      string stringWithGroupName1 = LocalizationDatabase.GetLocalizedStringWithGroupName(text, groupName);
      string stringWithGroupName2 = LocalizationDatabase.GetLocalizedStringWithGroupName(tooltip, groupName);
      return new GUIContent(stringWithGroupName1)
      {
        tooltip = stringWithGroupName2,
        image = icon
      };
    }

    [ExcludeFromDocs]
    public static GUIContent TextContent(string text, string tooltip, string iconName)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrTextContent(text, tooltip, iconName);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrTextContent(text, tooltip, iconName);
      string stringWithGroupName1 = LocalizationDatabase.GetLocalizedStringWithGroupName(text, groupName);
      string stringWithGroupName2 = LocalizationDatabase.GetLocalizedStringWithGroupName(tooltip, groupName);
      return new GUIContent(stringWithGroupName1)
      {
        tooltip = stringWithGroupName2,
        image = (Texture) EditorGUIUtility.LoadIconRequired(iconName)
      };
    }

    [ExcludeFromDocs]
    public static GUIContent TextContent(string text, Texture icon)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrTextContentWithIcon(text, icon);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrTextContentWithIcon(text, icon);
      return new GUIContent(LocalizationDatabase.GetLocalizedStringWithGroupName(text, groupName))
      {
        image = icon
      };
    }

    [ExcludeFromDocs]
    public static GUIContent TextContentWithIcon(string text, Texture icon)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrTextContentWithIcon(text, icon);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrTextContentWithIcon(text, icon);
      return new GUIContent(LocalizationDatabase.GetLocalizedStringWithGroupName(text, groupName))
      {
        image = icon
      };
    }

    [ExcludeFromDocs]
    public static GUIContent TextContentWithIcon(string text, string iconName)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TextContentWithIcon(text, iconName);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TextContentWithIcon(text, iconName);
      return new GUIContent(LocalizationDatabase.GetLocalizedStringWithGroupName(text, groupName))
      {
        image = (Texture) EditorGUIUtility.LoadIconRequired(iconName)
      };
    }

    [ExcludeFromDocs]
    public static GUIContent TextContentWithIcon(string text, string tooltip, string iconName)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrTextContentWithIcon(text, tooltip, iconName);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrTextContentWithIcon(text, tooltip, iconName);
      string stringWithGroupName1 = LocalizationDatabase.GetLocalizedStringWithGroupName(text, groupName);
      string stringWithGroupName2 = LocalizationDatabase.GetLocalizedStringWithGroupName(tooltip, groupName);
      return new GUIContent(stringWithGroupName1)
      {
        tooltip = stringWithGroupName2,
        image = (Texture) EditorGUIUtility.LoadIconRequired(iconName)
      };
    }

    [ExcludeFromDocs]
    public static GUIContent TextContentWithIcon(string text, string tooltip, Texture icon)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrTextContentWithIcon(text, tooltip, icon);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrTextContentWithIcon(text, tooltip, icon);
      string stringWithGroupName1 = LocalizationDatabase.GetLocalizedStringWithGroupName(text, groupName);
      string stringWithGroupName2 = LocalizationDatabase.GetLocalizedStringWithGroupName(tooltip, groupName);
      return new GUIContent(stringWithGroupName1)
      {
        tooltip = stringWithGroupName2,
        image = icon
      };
    }

    [ExcludeFromDocs]
    public static GUIContent TextContentWithIcon(
      string text,
      string tooltip,
      MessageType messageType)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrTextContentWithIcon(text, tooltip, messageType);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrTextContentWithIcon(text, tooltip, messageType);
      string stringWithGroupName1 = LocalizationDatabase.GetLocalizedStringWithGroupName(text, groupName);
      string stringWithGroupName2 = LocalizationDatabase.GetLocalizedStringWithGroupName(tooltip, groupName);
      return new GUIContent(stringWithGroupName1)
      {
        tooltip = stringWithGroupName2,
        image = (Texture) EditorGUIUtility.GetHelpIcon(messageType)
      };
    }

    [ExcludeFromDocs]
    public static GUIContent TextContentWithIcon(string text, MessageType messageType)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrTextContentWithIcon(text, messageType);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrTextContentWithIcon(text, messageType);
      return new GUIContent(LocalizationDatabase.GetLocalizedStringWithGroupName(text, groupName))
      {
        image = (Texture) EditorGUIUtility.GetHelpIcon(messageType)
      };
    }

    [ExcludeFromDocs]
    public static GUIContent IconContent(string iconName, string tooltip = null)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrIconContent(iconName, tooltip);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrIconContent(iconName, tooltip);
      string stringWithGroupName = LocalizationDatabase.GetLocalizedStringWithGroupName(tooltip, groupName);
      return new GUIContent()
      {
        tooltip = stringWithGroupName,
        image = (Texture) EditorGUIUtility.LoadIconRequired(iconName)
      };
    }

    [ExcludeFromDocs]
    public static GUIContent IconContent(Texture icon, string tooltip = null)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TrIconContent(icon, tooltip);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TrIconContent(icon, tooltip);
      string stringWithGroupName = LocalizationDatabase.GetLocalizedStringWithGroupName(tooltip, groupName);
      return new GUIContent()
      {
        tooltip = stringWithGroupName,
        image = icon
      };
    }

    [ExcludeFromDocs]
    public static GUIContent TempContent(string t)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TempContent(t);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      return groupName != null ? EditorGUIUtility.TempContent(LocalizationDatabase.GetLocalizedStringWithGroupName(t, groupName)) : EditorGUIUtility.TempContent(t);
    }

    [ExcludeFromDocs]
    public static GUIContent[] TempContent(string[] texts)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TempContent(texts);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TempContent(texts);
      GUIContent[] guiContentArray = new GUIContent[texts.Length];
      for (int index = 0; index < texts.Length; ++index)
      {
        string stringWithGroupName = LocalizationDatabase.GetLocalizedStringWithGroupName(texts[index], groupName);
        guiContentArray[index] = new GUIContent(stringWithGroupName);
      }
      return guiContentArray;
    }

    [ExcludeFromDocs]
    public static GUIContent[] TempContent(string[] texts, string[] tooltips)
    {
      if (!LocalizationDatabase.enableEditorLocalization)
        return EditorGUIUtility.TempContent(texts, tooltips);
      string groupName = L10n.GetGroupName(Assembly.GetCallingAssembly());
      if (groupName == null)
        return EditorGUIUtility.TempContent(texts);
      GUIContent[] guiContentArray = new GUIContent[texts.Length];
      for (int index = 0; index < texts.Length; ++index)
      {
        string stringWithGroupName1 = LocalizationDatabase.GetLocalizedStringWithGroupName(texts[index], groupName);
        string stringWithGroupName2 = LocalizationDatabase.GetLocalizedStringWithGroupName(tooltips[index], groupName);
        guiContentArray[index] = new GUIContent(stringWithGroupName1, stringWithGroupName2);
      }
      return guiContentArray;
    }

    private readonly struct LocKey(string _defaultString, string _groupName) : 
      IEquatable<L10n.LocKey>
    {
      [NotNull]
      public readonly string defaultString = _defaultString ?? string.Empty;
      [NotNull]
      public readonly string groupName = _groupName ?? string.Empty;

      public bool Equals(L10n.LocKey other)
      {
        return this.defaultString == other.defaultString && this.groupName == other.groupName;
      }

      public override bool Equals(object obj) => obj is L10n.LocKey other && this.Equals(other);

      public override int GetHashCode()
      {
        return this.defaultString.GetHashCode() * 397 ^ this.groupName.GetHashCode();
      }
    }
  }
}
