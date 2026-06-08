// Decompiled with JetBrains decompiler
// Type: UnityEditor.SceneView
// Assembly: UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 66954B2D-F4D3-4D0B-B43A-9270BB3F13D7
// Assembly location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEditor.CoreModule.dll
// XML documentation location: /home/nnra/Unity/Hub/Editor/6000.0.34f1/Editor/Data/Managed/UnityEngine/UnityEditor.CoreModule.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEditor.Actions;
using UnityEditor.AnimatedValues;
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEditor.Profiling;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

#nullable disable
namespace UnityEditor;

/// <summary>
///   <para>Use this class to manage SceneView settings, change the SceneView camera properties, subscribe to events, call SceneView methods, and render open scenes.</para>
/// </summary>
[EditorWindowTitle(title = "Scene", useTypeNameAsIconName = true)]
public class SceneView : SearchableEditorWindow, IHasCustomMenu, ISupportsOverlays
{
  private static SceneView s_LastActiveSceneView;
  /// <summary>
  ///   <para>Register to this callback to get notified when the active Scene View changes.</para>
  /// </summary>
  public static Action<SceneView, SceneView> lastActiveSceneViewChanged;
  private static SceneView s_CurrentDrawingSceneView;
  internal static readonly PrefColor kSceneViewBackground = new PrefColor("Scene/Background", 0.278431f, 0.278431f, 0.278431f, 1f);
  internal static readonly PrefColor kSceneViewPrefabBackground = new PrefColor("Scene/Background for Prefabs", 0.132f, 0.231f, 0.33f, 1f);
  private static readonly PrefColor kSceneViewWire = new PrefColor("Scene/Wireframe", 0.0f, 0.0f, 0.0f, 0.5f);
  private static readonly PrefColor kSceneViewWireOverlay = new PrefColor("Scene/Wireframe Overlay", 0.0f, 0.0f, 0.0f, 0.25f);
  private static readonly PrefColor kSceneViewSelectedOutline = new PrefColor("Scene/Selected Outline", 1f, 0.4f, 0.0f, 0.0f);
  private static readonly PrefColor kSceneViewSelectedSubmeshOutline = new PrefColor("Scene/Selected Material Highlight", 0.78431374f, 0.0f, 0.0f, 0.39215687f);
  private static readonly PrefColor kSceneViewSelectedChildrenOutline = new PrefColor("Scene/Selected Children Outline", 0.36862746f, 0.46666667f, 0.60784316f, 0.0f);
  private static readonly PrefColor kSceneViewSelectedWire = new PrefColor("Scene/Wireframe Selected", 0.36862746f, 0.46666667f, 0.60784316f, 0.2509804f);
  internal static Color kSceneViewFrontLight = new Color(0.769f, 0.769f, 0.769f, 1f);
  internal static Color kSceneViewUpLight = new Color(0.212f, 0.227f, 0.259f, 1f);
  internal static Color kSceneViewMidLight = new Color(57f / 500f, 0.125f, 0.133f, 1f);
  internal static Color kSceneViewDownLight = new Color(0.047f, 0.043f, 0.035f, 1f);
  private const string k_StyleCommon = "StyleSheets/SceneView/SceneViewCommon.uss";
  private const string k_StyleDark = "StyleSheets/SceneView/SceneViewDark.uss";
  private const string k_StyleLight = "StyleSheets/SceneView/SceneViewLight.uss";
  internal static SavedBool s_PreferenceIgnoreAlwaysRefreshWhenNotFocused = new SavedBool("SceneView.ignoreAlwaysRefreshWhenNotFocused", false);
  internal static SavedBool s_PreferenceEnableFilteringWhileSearching = new SavedBool("SceneView.enableFilteringWhileSearching", true);
  internal static SavedBool s_PreferenceEnableFilteringWhileLodGroupEditing = new SavedBool("SceneView.enableFilteringWhileLodGroupEditing", true);
  internal static SavedFloat s_DrawModeExposure = new SavedFloat("SceneView.drawModeExposure", 0.0f);
  private static SavedBool s_DrawBackfaceHighlights = new SavedBool("SceneView.drawBackfaceHighlights", false);
  private static readonly HashSet<DrawCameraMode> s_ShowExposureDrawCameraModes = new HashSet<DrawCameraMode>()
  {
    DrawCameraMode.BakedEmissive,
    DrawCameraMode.BakedLightmap,
    DrawCameraMode.RealtimeEmissive,
    DrawCameraMode.RealtimeIndirect
  };
  private static readonly HashSet<DrawCameraMode> s_ShowLightmapResolutionDrawCameraModes = new HashSet<DrawCameraMode>()
  {
    DrawCameraMode.BakedEmissive,
    DrawCameraMode.RealtimeEmissive,
    DrawCameraMode.BakedLightmap,
    DrawCameraMode.RealtimeIndirect,
    DrawCameraMode.BakedDirectionality,
    DrawCameraMode.RealtimeDirectionality,
    DrawCameraMode.BakedAlbedo,
    DrawCameraMode.RealtimeAlbedo,
    DrawCameraMode.BakedCharting,
    DrawCameraMode.RealtimeCharting,
    DrawCameraMode.BakedTexelValidity,
    DrawCameraMode.BakedUVOverlap,
    DrawCameraMode.ShadowMasks,
    DrawCameraMode.Systems,
    DrawCameraMode.GIContributorsReceivers,
    DrawCameraMode.BakedLightmapCulling,
    DrawCameraMode.BakedIndices,
    DrawCameraMode.LightOverlap
  };
  private static readonly HashSet<DrawCameraMode> s_ShowInteractiveLightBakingToggleCameraModes = new HashSet<DrawCameraMode>()
  {
    DrawCameraMode.BakedLightmap,
    DrawCameraMode.BakedDirectionality,
    DrawCameraMode.ShadowMasks,
    DrawCameraMode.BakedAlbedo,
    DrawCameraMode.BakedEmissive,
    DrawCameraMode.BakedCharting,
    DrawCameraMode.BakedTexelValidity,
    DrawCameraMode.BakedUVOverlap,
    DrawCameraMode.BakedIndices,
    DrawCameraMode.LightOverlap
  };
  private static List<Editor> s_ActiveEditors = new List<Editor>();
  private static bool s_ActiveEditorsDirty;
  private static bool s_SelectionCacheDirty;
  [SerializeField]
  private string m_WindowGUID;
  private SceneView m_PreviousScene = (SceneView) null;
  [SerializeField]
  private bool m_Gizmos = true;
  private const float kSubmeshPingDuration = 1f;
  private Scene m_CustomScene;
  [SerializeField]
  private ulong m_OverrideSceneCullingMask;
  private SceneViewStageHandling m_StageHandling;
  private Transform m_CustomParentForNewGameObjects;
  [NonSerialized]
  private static readonly Quaternion kDefaultRotation = Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f));
  private const float kDefaultViewSize = 10f;
  [NonSerialized]
  private static readonly Vector3 kDefaultPivot = Vector3.zero;
  private const float kOrthoThresholdAngle = 3f;
  private const float kOneOverSqrt2 = 0.70710677f;
  internal const float k_MaxSceneViewSize = 3.2E+34f;
  internal const float k_MaxCameraFarClip = 1.844674E+19f;
  internal const float k_MinCameraNearClip = 1E-05f;
  [NonSerialized]
  private static ActiveEditorTracker s_SharedTracker;
  [SerializeField]
  private bool m_SceneIsLit = true;
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("m_SceneLighting has been deprecated. Use sceneLighting instead (UnityUpgradable) -> UnityEditor.SceneView.sceneLighting", true)]
  public bool m_SceneLighting = true;
  internal bool m_WasFocused = false;
  private static int[] s_CachedParentRenderersForOutlining;
  private static int[] s_CachedChildRenderersForOutlining;
  private static HashSet<int> s_CachedChildRenderersForOutliningHashSet;
  [SerializeField]
  private bool m_2DMode;
  [SerializeField]
  private bool m_isRotationLocked = false;
  [SerializeField]
  private bool m_PlayAudio = false;
  /// <summary>
  ///   <para>M_AudioPlay has been deprecated. Use audioPlay instead (UnityUpgradable) -&gt; audioPlay.</para>
  /// </summary>
  [Obsolete("m_AudioPlay has been deprecated. Use audioPlay instead (UnityUpgradable) -> audioPlay", true)]
  public bool m_AudioPlay = false;
  private static SceneView s_AudioSceneView;
  [SerializeField]
  private bool m_DebugDrawModesUseInteractiveLightBakingData = false;
  [SerializeField]
  internal AnimVector3 m_Position = new AnimVector3(SceneView.kDefaultPivot);
  [Obsolete("onSceneGUIDelegate has been deprecated. Use duringSceneGui instead.")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static SceneView.OnSceneFunc onSceneGUIDelegate;
  /// <summary>
  ///   <para>M_RenderMode has been deprecated. Use cameraMode instead.</para>
  /// </summary>
  [Obsolete("Use cameraMode instead", false)]
  public DrawCameraMode m_RenderMode = DrawCameraMode.Textured;
  [SerializeField]
  private SceneView.CameraMode m_CameraMode;
  internal SceneOrientationGizmo m_OrientationGizmo;
  /// <summary>
  ///   <para>M_ValidateTrueMetals has been deprecated. Use validateTrueMetals instead (UnityUpgradable) -&gt; validateTrueMetals.</para>
  /// </summary>
  [Obsolete("m_ValidateTrueMetals has been deprecated. Use validateTrueMetals instead (UnityUpgradable) -> validateTrueMetals", true)]
  public bool m_ValidateTrueMetals = false;
  [SerializeField]
  private bool m_DoValidateTrueMetals = false;
  [SerializeField]
  private SceneView.SceneViewState m_SceneViewState;
  [SerializeField]
  private SceneViewGrid m_Grid;
  [SerializeField]
  internal AnimQuaternion m_Rotation = new AnimQuaternion(SceneView.kDefaultRotation);
  [SerializeField]
  private AnimFloat m_Size = new AnimFloat(10f);
  [SerializeField]
  internal AnimBool m_Ortho = new AnimBool();
  [NonSerialized]
  private Camera m_Camera;
  private VisualElement m_CameraViewVisualElement;
  private static readonly string s_CameraRectVisualElementName = "unity-scene-view-camera-rect";
  private static List<Overlay> s_ActiveViewOverlays = new List<Overlay>();
  [SerializeField]
  private SceneView.CameraSettings m_CameraSettings;
  [SerializeField]
  private Quaternion m_LastSceneViewRotation;
  [SerializeField]
  private bool m_LastSceneViewOrtho;
  private static MouseCursor s_LastCursor = MouseCursor.Arrow;
  private static readonly List<SceneView.CursorRect> s_MouseRects = new List<SceneView.CursorRect>();
  [NonSerialized]
  private Scene m_CustomLightsScene;
  [NonSerialized]
  private Light[] m_Light = new Light[3];
  private RectSelection m_RectSelection;
  private SceneViewMotion m_SceneViewMotion;
  [SerializeField]
  private SceneViewViewpoint m_Viewpoint = new SceneViewViewpoint();
  private const float kDefaultPerspectiveFov = 60f;
  private static ArrayList s_SceneViews = new ArrayList();
  private static List<Camera> s_AllSceneCameraList = new List<Camera>();
  private static Camera[] s_AllSceneCameras = new Camera[0];
  private static Material s_AlphaOverlayMaterial;
  private static Material s_DeferredOverlayMaterial;
  private static Shader s_ShowOverdrawShader;
  private static Shader s_ShowMipsShader;
  private static Shader s_ShowTextureStreamingShader;
  private static Shader s_AuraShader;
  private static Material s_FadeMaterial;
  private static Material s_ApplyFilterMaterial;
  private static Texture2D s_MipColorsTexture;
  private double m_StartSearchFilterTime = -1.0;
  private RenderTexture m_SceneTargetTexture;
  private int m_MainViewControlID;
  [SerializeField]
  private Shader m_ReplacementShader;
  [SerializeField]
  private string m_ReplacementString;
  [SerializeField]
  private bool m_SceneVisActive = true;
  private string m_SceneVisHiddenCount = "0";
  internal bool m_ShowSceneViewWindows = false;
  internal EditorCache m_DragEditorCache;
  private SceneView.DraggingLockedState m_DraggingLockedState;
  [SerializeField]
  private UnityEngine.Object m_LastLockedObject;
  [SerializeField]
  private DrawCameraMode m_LastDebugDrawMode = DrawCameraMode.GIContributorsReceivers;
  [SerializeField]
  private bool m_ViewIsLockedToObject;
  private IMGUIContainer m_PrefabToolbar;
  private bool m_ForceSceneViewFiltering;
  private bool m_ForceSceneViewFilteringForLodGroupEditing;
  private bool m_ForceSceneViewFilteringForStageHandling;
  private double m_lastRenderedTime;
  private static SceneView.EditorActionCache s_OnSceneGuiCache = new SceneView.EditorActionCache("OnSceneGUI");
  private static SceneView.EditorActionCache s_OnPreSceneGuiCache = new SceneView.EditorActionCache("OnPreSceneGUI");

  [RequiredByNativeCode]
  internal static DrawCameraMode[] GetInteractiveDrawCameraModeValues()
  {
    List<DrawCameraMode> drawCameraModeList = new List<DrawCameraMode>();
    foreach (SceneView sceneView in SceneView.sceneViews)
    {
      if (sceneView.usesInteractiveLightBakingData)
        drawCameraModeList.Add(sceneView.cameraMode.drawMode);
    }
    return drawCameraModeList.ToArray();
  }

  internal static bool NeedsInteractiveBaking()
  {
    foreach (SceneView sceneView in SceneView.sceneViews)
    {
      if (sceneView.usesInteractiveLightBakingData)
        return true;
    }
    return false;
  }

  private static string GetLegacyOverlayId(OverlayWindow overlayData)
  {
    return "legacy-overlay::" + overlayData.title.text;
  }

  internal void ShowLegacyOverlay(OverlayWindow overlayData)
  {
    LegacyOverlay overlay = this.overlayCanvas.GetOrCreateOverlay<LegacyOverlay>(SceneView.GetLegacyOverlayId(overlayData));
    if (overlay == null)
      return;
    overlay.displayName = overlayData.title.text;
    overlay.data = overlayData;
    overlay.showRequested = true;
  }

  private void LegacyOverlayPreOnGUI()
  {
    if (UnityEngine.Event.current.type != UnityEngine.EventType.Layout)
      return;
    foreach (Overlay overlay in this.overlayCanvas.overlays)
    {
      if (overlay is LegacyOverlay legacyOverlay)
        legacyOverlay.showRequested = false;
    }
  }

  /// <summary>
  ///   <para>The SceneView that was most recently in focus.</para>
  /// </summary>
  public static SceneView lastActiveSceneView
  {
    get
    {
      if ((UnityEngine.Object) SceneView.s_LastActiveSceneView == (UnityEngine.Object) null && SceneView.s_SceneViews.Count > 0)
        SceneView.lastActiveSceneView = SceneView.s_SceneViews[0] as SceneView;
      return SceneView.s_LastActiveSceneView;
    }
    private set
    {
      if ((UnityEngine.Object) value == (UnityEngine.Object) SceneView.s_LastActiveSceneView)
        return;
      SceneView lastActiveSceneView = SceneView.s_LastActiveSceneView;
      SceneView.s_LastActiveSceneView = value;
      Action<SceneView, SceneView> sceneViewChanged = SceneView.lastActiveSceneViewChanged;
      if (sceneViewChanged != null)
        sceneViewChanged(lastActiveSceneView, value);
      SceneView.MoveOverlaysToActiveView(lastActiveSceneView, value);
    }
  }

  /// <summary>
  ///   <para>The SceneView that is being drawn.</para>
  /// </summary>
  public static SceneView currentDrawingSceneView => SceneView.s_CurrentDrawingSceneView;

  /// <summary>
  ///   <para>Gets the Color of selected outline.</para>
  /// </summary>
  public static Color selectedOutlineColor => SceneView.kSceneViewSelectedOutline.Color;

  /// <summary>
  ///   <para>Whether this SceneView is using scene filtering.</para>
  /// </summary>
  public bool isUsingSceneFiltering => this.UseSceneFiltering();

  internal static event Action<bool> onDrawBackfaceHighlightsChanged;

  [RequiredByNativeCode]
  internal static float GetDrawModeExposure() => (float) SceneView.s_DrawModeExposure;

  [RequiredByNativeCode]
  internal static bool GetDrawBackfaceHighlights() => (bool) SceneView.s_DrawBackfaceHighlights;

  internal static void SetDrawBackfaceHighlights(bool value)
  {
    if (value == SceneView.s_DrawBackfaceHighlights.value)
      return;
    SceneView.s_DrawBackfaceHighlights.value = value;
    Action<bool> highlightsChanged = SceneView.onDrawBackfaceHighlightsChanged;
    if (highlightsChanged != null)
      highlightsChanged(value);
  }

  internal bool showExposureSettings
  {
    get => SceneView.s_ShowExposureDrawCameraModes.Contains(this.cameraMode.drawMode);
  }

  internal bool showLightmapResolutionToggle
  {
    get => SceneView.s_ShowLightmapResolutionDrawCameraModes.Contains(this.cameraMode.drawMode);
  }

  internal bool showBackfaceHighlightsToggle => this.showLightmapResolutionToggle;

  internal bool currentDrawModeMayUseInteractiveLightBakingData
  {
    get
    {
      return SceneView.s_ShowInteractiveLightBakingToggleCameraModes.Contains(this.cameraMode.drawMode);
    }
  }

  internal bool showLightingVisualizationPanel
  {
    get
    {
      return this.showExposureSettings || this.showBackfaceHighlightsToggle || this.showLightmapResolutionToggle || this.currentDrawModeMayUseInteractiveLightBakingData;
    }
  }

  internal static Transform GetDefaultParentObjectIfSet()
  {
    Transform parentObjectIfSet = (Transform) null;
    PrefabStage currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
    int parentForSession = SceneHierarchy.GetDefaultParentForSession((UnityEngine.Object) currentPrefabStage != (UnityEngine.Object) null ? currentPrefabStage.scene.guid : SceneManager.GetActiveScene().guid);
    if (parentForSession != 0)
      parentObjectIfSet = EditorUtility.InstanceIDToObject(parentForSession) is GameObject gameObject ? gameObject.gameObject?.transform : (Transform) null;
    return parentObjectIfSet;
  }

  private static void OnSelectedObjectWasDestroyed(int unused)
  {
    SceneView.s_ActiveEditorsDirty = true;
    SceneView.s_SelectionCacheDirty = true;
  }

  private static void OnNonSelectedObjectWasDestroyed(int instanceID)
  {
    if (SceneView.s_ActiveEditorsDirty && SceneView.s_SelectionCacheDirty || SceneView.s_CachedChildRenderersForOutliningHashSet == null || !SceneView.s_CachedChildRenderersForOutliningHashSet.Contains(instanceID))
      return;
    SceneView.s_ActiveEditorsDirty = true;
    SceneView.s_SelectionCacheDirty = true;
  }

  private static void OnEditorTrackerRebuilt()
  {
    SceneView.s_ActiveEditorsDirty = true;
    SceneView.s_SelectionCacheDirty = true;
  }

  internal static void SetActiveEditorsDirty(bool forceRepaint = false)
  {
    SceneView.s_ActiveEditorsDirty = true;
    if (!forceRepaint)
      return;
    SceneView.RepaintAll();
  }

  internal static IEnumerable<Editor> activeEditors
  {
    get
    {
      SceneView.CollectActiveEditors();
      return (IEnumerable<Editor>) SceneView.s_ActiveEditors;
    }
  }

  private static void CollectActiveEditors()
  {
    if (!SceneView.s_ActiveEditorsDirty)
      return;
    SceneView.s_ActiveEditorsDirty = false;
    SceneView.s_ActiveEditors.Clear();
    bool flag = false;
    foreach (InspectorWindow inspector in InspectorWindow.GetInspectors())
    {
      if (inspector.isLocked)
      {
        foreach (Editor activeEditor in inspector.tracker.activeEditors)
          SceneView.s_ActiveEditors.Add(activeEditor);
      }
      else if (!flag && inspector.isVisible && inspector.inspectorMode == InspectorMode.Normal)
      {
        flag = true;
        foreach (Editor activeEditor in inspector.tracker.activeEditors)
          SceneView.s_ActiveEditors.Add(activeEditor);
      }
    }
    if (flag)
      return;
    if (SceneView.s_SharedTracker == null)
      SceneView.s_SharedTracker = ActiveEditorTracker.sharedTracker;
    foreach (Editor activeEditor in SceneView.s_SharedTracker.activeEditors)
      SceneView.s_ActiveEditors.Add(activeEditor);
  }

  internal string windowGUID => this.m_WindowGUID;

  /// <summary>
  ///   <para>Sets the visibility of all Gizmos in the Scene view.</para>
  /// </summary>
  public bool drawGizmos
  {
    get => this.m_Gizmos;
    set
    {
      if (this.m_Gizmos == value)
        return;
      this.m_Gizmos = value;
      Action<bool> drawGizmosChanged = this.drawGizmosChanged;
      if (drawGizmosChanged == null)
        return;
      drawGizmosChanged(value);
    }
  }

  internal bool isPingingObject { get; set; } = false;

  internal float alphaMultiplier { get; set; } = 0.0f;

  internal int submeshOutlineMaterialId { get; set; } = 0;

  protected internal Scene customScene
  {
    get => this.m_CustomScene;
    set
    {
      this.m_CustomScene = value;
      this.m_Camera.scene = this.m_CustomScene;
      StageUtility.SetSceneToRenderInStage(this.m_CustomLightsScene, StageUtility.GetStageHandle(this.m_CustomScene));
    }
  }

  internal ulong overrideSceneCullingMask
  {
    get => this.m_OverrideSceneCullingMask;
    set
    {
      this.m_OverrideSceneCullingMask = value;
      this.m_Camera.overrideSceneCullingMask = value;
    }
  }

  /// <summary>
  ///   <para>The position and size of the area that the camera renders.</para>
  /// </summary>
  public Rect cameraViewport => this.cameraViewVisualElement.rect;

  protected internal Transform customParentForDraggedObjects
  {
    get => this.customParentForNewGameObjects;
    set => this.customParentForNewGameObjects = value;
  }

  internal Transform customParentForNewGameObjects
  {
    get => this.m_CustomParentForNewGameObjects;
    set => this.m_CustomParentForNewGameObjects = value;
  }

  /// <summary>
  ///   <para>Whether lighting is enabled or disabled in the Scene view.</para>
  /// </summary>
  public bool sceneLighting
  {
    get => this.m_SceneIsLit;
    set
    {
      if (this.m_SceneIsLit == value)
        return;
      this.m_SceneIsLit = value;
      Action<bool> sceneLightingChanged = this.sceneLightingChanged;
      if (sceneLightingChanged != null)
        sceneLightingChanged(value);
    }
  }

  public event Func<SceneView.CameraMode, bool> onValidateCameraMode;

  public event Action<SceneView.CameraMode> onCameraModeChanged;

  public event Action<bool> gridVisibilityChanged;

  internal event Action<bool> sceneLightingChanged;

  internal event Action<bool> sceneAudioChanged;

  internal event Action<bool> debugDrawModesUseInteractiveLightBakingDataChanged;

  internal event Action<bool> sceneVisActiveChanged;

  internal event Action<bool> drawGizmosChanged;

  internal event Action<bool> modeChanged2D;

  /// <summary>
  ///   <para>Whether the SceneView is in 2D mode.</para>
  /// </summary>
  public bool in2DMode
  {
    get => this.m_2DMode;
    set
    {
      if (this.m_2DMode == value)
        return;
      this.m_2DMode = value;
      this.On2DModeChange();
      Action<bool> modeChanged2D = this.modeChanged2D;
      if (modeChanged2D != null)
        modeChanged2D(value);
    }
  }

  /// <summary>
  ///   <para>Whether the Scene view camera can be rotated.</para>
  /// </summary>
  public bool isRotationLocked
  {
    get => this.m_isRotationLocked;
    set => this.m_isRotationLocked = value;
  }

  internal static List<SceneView.CameraMode> userDefinedModes { get; } = new List<SceneView.CameraMode>();

  /// <summary>
  ///   <para>Enables or disables Scene view audio effects.</para>
  /// </summary>
  public bool audioPlay
  {
    get => this.m_PlayAudio;
    set
    {
      if (value == this.m_PlayAudio)
        return;
      this.m_PlayAudio = value;
      Action<bool> sceneAudioChanged = this.sceneAudioChanged;
      if (sceneAudioChanged != null)
        sceneAudioChanged(value);
      this.RefreshAudioPlay();
    }
  }

  internal bool debugDrawModesUseInteractiveLightBakingData
  {
    get => this.m_DebugDrawModesUseInteractiveLightBakingData;
    set
    {
      if (value == this.m_DebugDrawModesUseInteractiveLightBakingData)
        return;
      this.m_DebugDrawModesUseInteractiveLightBakingData = value;
      Action<bool> bakingDataChanged = this.debugDrawModesUseInteractiveLightBakingDataChanged;
      if (bakingDataChanged != null)
        bakingDataChanged(this.m_DebugDrawModesUseInteractiveLightBakingData);
      Lightmapping.Internal_CallLightingDataUpdatedFunctions();
    }
  }

  internal bool usesInteractiveLightBakingData
  {
    get
    {
      return this.debugDrawModesUseInteractiveLightBakingData && this.currentDrawModeMayUseInteractiveLightBakingData;
    }
  }

  public static event Action<SceneView> beforeSceneGui;

  public static event Action<SceneView> duringSceneGui;

  internal static event Func<SceneView, VisualElement> addCustomVisualElementToSceneView;

  internal static event Action<SceneView> onGUIStarted;

  internal static event Action<SceneView> onGUIEnded;

  /// <summary>
  ///   <para>RenderMode has been deprecated. Use cameraMode instead.</para>
  /// </summary>
  [Obsolete("Use cameraMode instead", false)]
  public DrawCameraMode renderMode
  {
    get => this.m_CameraMode.drawMode;
    set
    {
      this.cameraMode = value != DrawCameraMode.UserDefined ? SceneRenderModeWindow.GetBuiltinCameraMode(value) : throw new ArgumentException("Use cameraMode to set user-defined modes");
    }
  }

  /// <summary>
  ///   <para>The current DrawCameraMode for the Scene view camera.</para>
  /// </summary>
  public SceneView.CameraMode cameraMode
  {
    get
    {
      if (string.IsNullOrEmpty(this.m_CameraMode.name))
        this.m_CameraMode = SceneRenderModeWindow.GetBuiltinCameraMode(this.m_CameraMode.drawMode);
      return this.m_CameraMode;
    }
    set
    {
      this.m_CameraMode = SceneView.IsValidCameraMode(value) ? value : throw new ArgumentException($"The provided camera mode {value} is not registered!");
      if (this.onCameraModeChanged == null)
        return;
      this.onCameraModeChanged(this.m_CameraMode);
    }
  }

  /// <summary>
  ///   <para>Whether the albedo is black for materials with an average specular color above 0.45.</para>
  /// </summary>
  public bool validateTrueMetals
  {
    get => this.m_DoValidateTrueMetals;
    set
    {
      if (this.m_DoValidateTrueMetals == value)
        return;
      this.m_DoValidateTrueMetals = value;
      Shader.SetGlobalFloat("_CheckPureMetal", this.m_DoValidateTrueMetals ? 1f : 0.0f);
    }
  }

  /// <summary>
  ///   <para>Use SceneViewState to set the debug options for the Scene view.</para>
  /// </summary>
  public SceneView.SceneViewState sceneViewState
  {
    get => this.m_SceneViewState;
    set => this.m_SceneViewState = value;
  }

  /// <summary>
  ///   <para>Gets or sets whether to enable the grid for an instance of the SceneView.</para>
  /// </summary>
  public bool showGrid
  {
    get => this.sceneViewGrids.showGrid;
    set => this.sceneViewGrids.showGrid = value;
  }

  internal VisualElement cameraViewVisualElement
  {
    get
    {
      if (this.m_CameraViewVisualElement == null)
        this.m_CameraViewVisualElement = this.CreateCameraRectVisualElement();
      return this.m_CameraViewVisualElement;
    }
  }

  /// <summary>
  ///   <para>Use CameraSettings to set the properties for the SceneView Camera.</para>
  /// </summary>
  public SceneView.CameraSettings cameraSettings
  {
    get => this.m_CameraSettings;
    set => this.m_CameraSettings = value;
  }

  internal Vector2 GetDynamicClipPlanes()
  {
    float y = Mathf.Clamp(2000f * this.size, 1000f, 1.844674E+19f);
    return new Vector2(y * 5E-06f, y);
  }

  internal SceneViewGrid sceneViewGrids => this.m_Grid;

  /// <summary>
  ///   <para>Resets the CameraSettings for the SceneView Camera to default.</para>
  /// </summary>
  public void ResetCameraSettings() => this.m_CameraSettings = new SceneView.CameraSettings();

  internal bool showGlobalGrid
  {
    get => this.showGrid;
    set => this.showGrid = value;
  }

  /// <summary>
  ///   <para>When the Scene view is in 2D mode, this property contains the last camera rotation.</para>
  /// </summary>
  public Quaternion lastSceneViewRotation
  {
    get
    {
      if (this.m_LastSceneViewRotation == new Quaternion(0.0f, 0.0f, 0.0f, 0.0f))
        this.m_LastSceneViewRotation = Quaternion.identity;
      return this.m_LastSceneViewRotation;
    }
    set => this.m_LastSceneViewRotation = value;
  }

  internal static void AddCursorRect(Rect rect, MouseCursor cursor)
  {
    UnityEngine.EventType type = UnityEngine.Event.current.type;
    if (type != UnityEngine.EventType.Repaint && type != UnityEngine.EventType.MouseMove)
      return;
    SceneView.s_MouseRects.Add(new SceneView.CursorRect(rect, cursor));
  }

  private static float GetPerspectiveCameraDistance(float objectSize, float fov)
  {
    return objectSize / Mathf.Sin((float) ((double) fov * 0.5 * (Math.PI / 180.0)));
  }

  /// <summary>
  ///   <para>The distance from camera to pivot.</para>
  /// </summary>
  public float cameraDistance
  {
    get
    {
      return Mathf.Clamp(this.camera.orthographic ? this.size * 2f : SceneView.GetPerspectiveCameraDistance(this.size, this.m_Ortho.Fade(this.perspectiveFov, 0.0f)), -3.2E+34f, 3.2E+34f);
    }
  }

  internal RectSelection rectSelection => this.m_RectSelection;

  internal SceneViewMotion sceneViewMotion => this.m_SceneViewMotion;

  internal SceneViewViewpoint viewpoint => this.m_Viewpoint;

  /// <summary>
  ///   <para>The list of all open Scene view windows.</para>
  /// </summary>
  public static ArrayList sceneViews => SceneView.s_SceneViews;

  internal float pingStartTime { get; set; } = 0.0f;

  /// <summary>
  ///   <para>The Camera that is rendering this SceneView.</para>
  /// </summary>
  public Camera camera => this.m_Camera;

  internal bool sceneVisActive
  {
    get => this.m_SceneVisActive;
    set
    {
      if (this.m_SceneVisActive == value)
        return;
      this.m_SceneVisActive = value;
      Action<bool> visActiveChanged = this.sceneVisActiveChanged;
      if (visActiveChanged == null)
        return;
      visActiveChanged(value);
    }
  }

  /// <summary>
  ///   <para>Sets a replacement shader for rendering this Scene view.</para>
  /// </summary>
  /// <param name="shader">The replacement shader.</param>
  /// <param name="replaceString">The replacement shader tag.</param>
  public void SetSceneViewShaderReplace(Shader shader, string replaceString)
  {
    this.m_ReplacementShader = shader;
    this.m_ReplacementString = replaceString;
  }

  internal SceneView.DraggingLockedState draggingLocked
  {
    set => this.m_DraggingLockedState = value;
    get => this.m_DraggingLockedState;
  }

  internal bool viewIsLockedToObject
  {
    get => this.m_ViewIsLockedToObject;
    set
    {
      this.m_LastLockedObject = !value ? (UnityEngine.Object) null : Selection.activeObject;
      this.m_ViewIsLockedToObject = value;
      this.draggingLocked = SceneView.DraggingLockedState.LookAt;
    }
  }

  [RequiredByNativeCode]
  private static void FrameSelectedMenuItem(bool locked)
  {
    string commandName = locked ? "FrameSelectedWithLock" : "FrameSelected";
    EditorWindow editorWindow = EditorWindow.mouseOverWindow;
    bool flag = (UnityEngine.Object) editorWindow != (UnityEngine.Object) null && editorWindow.SendEvent(EditorGUIUtility.CommandEvent(commandName));
    if (flag)
    {
      editorWindow.Focus();
    }
    else
    {
      editorWindow = EditorWindow.focusedWindow;
      flag = (UnityEngine.Object) editorWindow != (UnityEngine.Object) null && editorWindow.SendEvent(EditorGUIUtility.CommandEvent(commandName));
    }
    if (flag && !(editorWindow is SceneHierarchyWindow) || !((UnityEngine.Object) SceneView.lastActiveSceneView != (UnityEngine.Object) null))
      return;
    SceneView.lastActiveSceneView.SendEvent(EditorGUIUtility.CommandEvent(commandName));
  }

  /// <summary>
  ///   <para>Frames the currently selected object(s) in the last active Scene view.</para>
  /// </summary>
  /// <returns>
  ///   <para>Returns true if the camera frame successfully frames the current selection.</para>
  /// </returns>
  [RequiredByNativeCode]
  public static bool FrameLastActiveSceneView()
  {
    return !((UnityEngine.Object) SceneView.lastActiveSceneView == (UnityEngine.Object) null) && SceneView.lastActiveSceneView.SendEvent(EditorGUIUtility.CommandEvent("FrameSelected"));
  }

  public static bool FrameLastActiveSceneViewWithLock()
  {
    return !((UnityEngine.Object) SceneView.lastActiveSceneView == (UnityEngine.Object) null) && SceneView.lastActiveSceneView.SendEvent(EditorGUIUtility.CommandEvent("FrameSelectedWithLock"));
  }

  private static List<Camera> GetAllSceneCamerasAsList()
  {
    SceneView.s_AllSceneCameraList.Clear();
    for (int index = 0; index < SceneView.s_SceneViews.Count; ++index)
    {
      Camera camera = ((SceneView) SceneView.s_SceneViews[index]).m_Camera;
      if ((UnityEngine.Object) camera != (UnityEngine.Object) null)
        SceneView.s_AllSceneCameraList.Add(camera);
    }
    return SceneView.s_AllSceneCameraList;
  }

  /// <summary>
  ///   <para>Retrieves an array of all camera components from all open Scene views.</para>
  /// </summary>
  /// <returns>
  ///   <para>Returns an array of camera components.</para>
  /// </returns>
  [RequiredByNativeCode]
  public static Camera[] GetAllSceneCameras()
  {
    List<Camera> sceneCamerasAsList = SceneView.GetAllSceneCamerasAsList();
    if (sceneCamerasAsList.Count == SceneView.s_AllSceneCameras.Length)
    {
      bool flag = true;
      for (int index = 0; index < sceneCamerasAsList.Count; ++index)
      {
        if (SceneView.s_AllSceneCameras[index] != sceneCamerasAsList[index])
        {
          flag = false;
          break;
        }
      }
      if (flag)
        return SceneView.s_AllSceneCameras;
    }
    SceneView.s_AllSceneCameras = sceneCamerasAsList.ToArray();
    return SceneView.s_AllSceneCameras;
  }

  /// <summary>
  ///   <para>Repaints every open SceneView.</para>
  /// </summary>
  [RequiredByNativeCode]
  public static void RepaintAll()
  {
    foreach (EditorWindow sceneView in SceneView.s_SceneViews)
      sceneView.Repaint();
  }

  internal override void SetSearchFilter(
    string searchFilter,
    SearchableEditorWindow.SearchMode mode,
    bool setAll,
    bool delayed)
  {
    if (this.m_SearchFilter == "" || searchFilter == "")
      this.m_StartSearchFilterTime = EditorApplication.timeSinceStartup;
    base.SetSearchFilter(searchFilter, mode, setAll, delayed);
  }

  internal void OnLostFocus()
  {
    if (!((UnityEngine.Object) SceneView.lastActiveSceneView == (UnityEngine.Object) this))
      return;
    this.m_SceneViewMotion.ResetMotion();
    this.m_SceneViewMotion.CompleteSceneViewMotionTool();
  }

  private void OnBeforeRemovedAsTab() => this.m_PreviousScene = (SceneView) null;

  internal void OnAddedAsTab()
  {
    if (!EditorApplication.isPlaying && !EditorApplication.isPaused || !this.m_Parent.vSyncEnabled)
      return;
    this.m_Parent.EnableVSync(false);
  }

  public override void OnEnable()
  {
    this.baseRootVisualElement.Insert(0, (VisualElement) this.prefabToolbar);
    this.rootVisualElement.Add(this.cameraViewVisualElement);
    this.m_SceneViewMotion = new SceneViewMotion();
    this.rootVisualElement.RegisterCallback<MouseEnterEvent>((EventCallback<MouseEnterEvent>) (e => this.m_SceneViewMotion.viewportsUnderMouse = true));
    this.rootVisualElement.RegisterCallback<MouseLeaveEvent>((EventCallback<MouseLeaveEvent>) (e => this.m_SceneViewMotion.viewportsUnderMouse = false));
    this.rootVisualElement.RegisterCallback<MouseEnterWindowEvent>((EventCallback<MouseEnterWindowEvent>) (e => this.m_SceneViewMotion.viewportsUnderMouse = true));
    this.rootVisualElement.RegisterCallback<MouseLeaveWindowEvent>((EventCallback<MouseLeaveWindowEvent>) (e => this.m_SceneViewMotion.viewportsUnderMouse = false));
    this.m_OrientationGizmo = this.overlayCanvas.overlays.FirstOrDefault<Overlay>((Func<Overlay, bool>) (x => x is SceneOrientationGizmo)) as SceneOrientationGizmo;
    this.titleContent = this.GetLocalizedTitleContent();
    this.m_RectSelection = new RectSelection();
    if (SceneView.s_SceneViews.Count == 0)
    {
      this.m_SceneViewMotion.RegisterShortcutContexts();
      this.m_RectSelection.RegisterShortcutContext();
    }
    this.m_SceneViewMotion.CompleteSceneViewMotionTool();
    this.m_Viewpoint.AssignSceneView(this);
    if (this.m_Grid == null)
      this.m_Grid = new SceneViewGrid();
    this.sceneViewGrids.OnEnable(this);
    this.ResetGridPivot();
    this.autoRepaintOnSceneChange = true;
    this.m_Rotation.valueChanged.AddListener(new UnityAction(((EditorWindow) this).Repaint));
    this.m_Position.valueChanged.AddListener(new UnityAction(((EditorWindow) this).Repaint));
    this.m_Size.valueChanged.AddListener(new UnityAction(((EditorWindow) this).Repaint));
    this.m_Ortho.valueChanged.AddListener(new UnityAction(((EditorWindow) this).Repaint));
    this.sceneViewGrids.gridVisibilityChanged += new Action<bool>(this.GridOnGridVisibilityChanged);
    this.wantsMouseMove = true;
    this.wantsLessLayoutEvents = true;
    this.wantsMouseEnterLeaveWindow = true;
    SceneView.s_SceneViews.Add((object) this);
    this.UpdateHiddenObjectCount();
    ObjectFactory.componentWasAdded += new Action<UnityEngine.Component>(this.OnComponentWasAdded);
    EditorApplication.playModeStateChanged += new Action<PlayModeStateChange>(this.OnPlayModeStateChanged);
    EditorApplication.modifierKeysChanged += new EditorApplication.CallbackFunction(SceneView.RepaintAll);
    SceneVisibilityManager.visibilityChanged += new Action(this.VisibilityChanged);
    SceneVisibilityManager.currentStageIsIsolated += new Action<bool>(this.CurrentStageIsolated);
    ActiveEditorTracker.editorTrackerRebuilt += new Action(SceneView.OnEditorTrackerRebuilt);
    Selection.selectedObjectWasDestroyed += new Action<int>(SceneView.OnSelectedObjectWasDestroyed);
    Selection.nonSelectedObjectWasDestroyed += new Action<int>(SceneView.OnNonSelectedObjectWasDestroyed);
    Lightmapping.lightingDataUpdated += new Action(SceneView.RepaintAll);
    this.onCameraModeChanged += (Action<SceneView.CameraMode>) (_param1 =>
    {
      if (this.cameraMode.drawMode == DrawCameraMode.ShadowCascades)
        this.sceneLighting = true;
      if (this.cameraMode.drawMode == DrawCameraMode.Textured || this.cameraMode.drawMode == DrawCameraMode.Wireframe || this.cameraMode.drawMode == DrawCameraMode.TexturedWire)
        return;
      this.m_LastDebugDrawMode = this.cameraMode.drawMode;
    });
    this.m_DraggingLockedState = SceneView.DraggingLockedState.NotDragging;
    this.CreateSceneCameraAndLights();
    if (this.m_2DMode)
      this.LookAt(this.pivot, Quaternion.identity, this.size, true, true);
    if (this.m_CameraMode.drawMode == DrawCameraMode.UserDefined && !SceneView.userDefinedModes.Contains(this.m_CameraMode))
      SceneView.AddCameraMode(this.m_CameraMode.name, this.m_CameraMode.section);
    base.OnEnable();
    if (this.SupportsStageHandling())
    {
      this.m_StageHandling = new SceneViewStageHandling(this);
      this.m_StageHandling.OnEnable();
    }
    SceneView.s_ActiveEditorsDirty = true;
    VisualElementStyleSheetSet styleSheets = this.baseRootVisualElement.styleSheets;
    styleSheets.Add(EditorGUIUtility.Load("StyleSheets/SceneView/SceneViewCommon.uss") as StyleSheet);
    styleSheets = this.baseRootVisualElement.styleSheets;
    styleSheets.Add(EditorGUIUtility.Load(EditorGUIUtility.isProSkin ? "StyleSheets/SceneView/SceneViewDark.uss" : "StyleSheets/SceneView/SceneViewLight.uss") as StyleSheet);
    SceneView.s_SelectionCacheDirty = true;
  }

  private IMGUIContainer prefabToolbar
  {
    get
    {
      if (this.m_PrefabToolbar == null)
      {
        IMGUIContainer imguiContainer = new IMGUIContainer();
        imguiContainer.onGUIHandler = (Action) (() =>
        {
          if (this.m_StageHandling != null && this.m_StageHandling.isShowingBreadcrumbBar)
          {
            this.m_PrefabToolbar.style.height = (StyleLength) this.m_StageHandling.breadcrumbHeight;
            this.m_StageHandling.BreadcrumbGUI();
          }
          else
            this.m_PrefabToolbar.style.height = (StyleLength) 0.0f;
        });
        imguiContainer.name = VisualElementUtils.GetUniqueName("prefab-toolbar");
        imguiContainer.pickingMode = PickingMode.Position;
        imguiContainer.viewDataKey = this.name;
        imguiContainer.renderHints = RenderHints.ClipWithScissors;
        this.m_PrefabToolbar = imguiContainer;
        UIElementsEditorUtility.AddDefaultEditorStyleSheets((VisualElement) this.m_PrefabToolbar);
        this.m_PrefabToolbar.style.overflow = (StyleEnum<Overflow>) Overflow.Hidden;
      }
      return this.m_PrefabToolbar;
    }
  }

  private VisualElement CreateCameraRectVisualElement()
  {
    IMGUIContainer imguiContainer = new IMGUIContainer();
    imguiContainer.onGUIHandler = new Action(this.OnSceneGUI);
    imguiContainer.name = SceneView.s_CameraRectVisualElementName;
    imguiContainer.pickingMode = PickingMode.Position;
    imguiContainer.viewDataKey = this.name;
    imguiContainer.renderHints = RenderHints.ClipWithScissors;
    imguiContainer.requireMeasureFunction = false;
    IMGUIContainer ve = imguiContainer;
    UIElementsEditorUtility.AddDefaultEditorStyleSheets((VisualElement) ve);
    ve.style.overflow = (StyleEnum<Overflow>) Overflow.Hidden;
    ve.style.flexGrow = (StyleFloat) 1f;
    if (SceneView.addCustomVisualElementToSceneView != null)
    {
      foreach (Delegate invocation in SceneView.addCustomVisualElementToSceneView.GetInvocationList())
        ve.Add((VisualElement) invocation.DynamicInvoke((object) this));
    }
    return (VisualElement) ve;
  }

  private void OnComponentWasAdded(UnityEngine.Component component)
  {
    if (!((UnityEngine.Object) (component as Renderer) != (UnityEngine.Object) null))
      return;
    SceneView.s_SelectionCacheDirty = true;
  }

  private void GridOnGridVisibilityChanged(bool visible)
  {
    Action<bool> visibilityChanged = this.gridVisibilityChanged;
    if (visibilityChanged == null)
      return;
    visibilityChanged(visible);
  }

  /// <summary>
  ///   <para>Override this method to control whether the Scene view should change when you switch from one stage to another stage.</para>
  /// </summary>
  /// <returns>
  ///   <para>True if the Scene view automatically reacts to stage changes.</para>
  /// </returns>
  protected virtual bool SupportsStageHandling() => true;

  private void CurrentStageIsolated(bool isolated)
  {
    if (!isolated)
      return;
    this.m_SceneVisActive = true;
    this.Repaint();
  }

  private void VisibilityChanged()
  {
    this.UpdateHiddenObjectCount();
    this.Repaint();
  }

  private void UpdateHiddenObjectCount()
  {
    this.m_SceneVisHiddenCount = SceneVisibilityState.GetHiddenObjectCount().ToString();
  }

  public SceneView()
  {
    this.m_HierarchyType = HierarchyType.GameObjects;
    this.depthBufferBits = 32 /*0x20*/;
  }

  internal void Awake()
  {
    if (string.IsNullOrEmpty(this.m_WindowGUID))
      this.m_WindowGUID = GUID.Generate().ToString();
    if (this.sceneViewState == null && this.m_CameraSettings == null && (UnityEngine.Object) SceneView.lastActiveSceneView != (UnityEngine.Object) null)
    {
      this.CopyLastActiveSceneViewSettings();
    }
    else
    {
      if (this.sceneViewState == null)
        this.m_SceneViewState = new SceneView.SceneViewState();
      if (this.m_CameraSettings == null)
        this.m_CameraSettings = new SceneView.CameraSettings();
    }
    if (this.m_2DMode || EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D)
    {
      this.m_LastSceneViewRotation = Quaternion.LookRotation(new Vector3(-1f, -0.7f, -1f));
      this.m_LastSceneViewOrtho = false;
      this.m_Rotation.value = Quaternion.identity;
      this.m_Ortho.value = true;
      if (!this.m_2DMode)
      {
        this.m_2DMode = true;
        Action<bool> modeChanged2D = this.modeChanged2D;
        if (modeChanged2D != null)
          modeChanged2D(this.m_2DMode);
      }
      if (Tools.current == Tool.Move)
        Tools.current = Tool.Rect;
    }
    this.m_PreviousScene = SceneView.lastActiveSceneView;
  }

  [RequiredByNativeCode]
  internal static void PlaceGameObjectInFrontOfSceneView(GameObject go)
  {
    if (SceneView.s_SceneViews.Count < 1)
      return;
    SceneView lastActiveSceneView = SceneView.lastActiveSceneView;
    if ((bool) (UnityEngine.Object) lastActiveSceneView)
      lastActiveSceneView.MoveToView(go.transform);
  }

  internal static Camera GetLastActiveSceneViewCamera()
  {
    SceneView lastActiveSceneView = SceneView.lastActiveSceneView;
    return (bool) (UnityEngine.Object) lastActiveSceneView ? lastActiveSceneView.camera : (Camera) null;
  }

  public override void OnDisable()
  {
    EditorApplication.modifierKeysChanged -= new EditorApplication.CallbackFunction(SceneView.RepaintAll);
    EditorApplication.playModeStateChanged -= new Action<PlayModeStateChange>(this.OnPlayModeStateChanged);
    SceneVisibilityManager.visibilityChanged -= new Action(this.VisibilityChanged);
    SceneVisibilityManager.currentStageIsIsolated -= new Action<bool>(this.CurrentStageIsolated);
    Lightmapping.lightingDataUpdated -= new Action(SceneView.RepaintAll);
    ActiveEditorTracker.editorTrackerRebuilt -= new Action(SceneView.OnEditorTrackerRebuilt);
    Selection.selectedObjectWasDestroyed -= new Action<int>(SceneView.OnSelectedObjectWasDestroyed);
    Selection.nonSelectedObjectWasDestroyed -= new Action<int>(SceneView.OnNonSelectedObjectWasDestroyed);
    this.sceneViewGrids.gridVisibilityChanged -= new Action<bool>(this.GridOnGridVisibilityChanged);
    this.sceneViewGrids.OnDisable(this);
    if ((bool) (UnityEngine.Object) this.m_Camera)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_Camera.gameObject, true);
    if ((bool) (UnityEngine.Object) this.m_Light[0])
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_Light[0].gameObject, true);
    if ((bool) (UnityEngine.Object) this.m_Light[1])
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_Light[1].gameObject, true);
    if ((bool) (UnityEngine.Object) this.m_Light[2])
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_Light[2].gameObject, true);
    EditorSceneManager.ClosePreviewScene(this.m_CustomLightsScene);
    if ((bool) (UnityEngine.Object) SceneView.s_MipColorsTexture)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) SceneView.s_MipColorsTexture, true);
    SceneView.s_SceneViews.Remove((object) this);
    if (SceneView.s_SceneViews.Count == 0)
    {
      this.m_SceneViewMotion.UnregisterShortcutContexts();
      this.m_RectSelection.UnregisterShortcutContext();
    }
    if ((UnityEngine.Object) SceneView.s_LastActiveSceneView == (UnityEngine.Object) this)
      SceneView.lastActiveSceneView = SceneView.s_SceneViews.Count > 0 ? SceneView.s_SceneViews[0] as SceneView : (SceneView) null;
    this.CleanupEditorDragFunctions();
    if (this.m_StageHandling != null)
      this.m_StageHandling.OnDisable();
    ObjectFactory.componentWasAdded -= new Action<UnityEngine.Component>(this.OnComponentWasAdded);
    base.OnDisable();
  }

  public void OnDestroy()
  {
    if (!this.audioPlay)
      return;
    this.audioPlay = false;
  }

  internal void OnPlayModeStateChanged(PlayModeStateChange state)
  {
    if (!this.audioPlay)
      return;
    this.audioPlay = false;
  }

  internal void OnStageChanged(Stage previousStage, Stage newStage)
  {
    this.VisibilityChanged();
    if (EditorApplication.isPlaying)
      return;
    this.RefreshAudioPlay();
  }

  internal override void OnMaximized()
  {
    this.m_SceneViewMotion.CompleteSceneViewMotionTool();
    this.Repaint();
  }

  internal void ToolbarSearchFieldGUI()
  {
    if (this.m_MainViewControlID != GUIUtility.keyboardControl && UnityEngine.Event.current.type == UnityEngine.EventType.KeyDown && !string.IsNullOrEmpty(this.m_SearchFilter))
    {
      switch (UnityEngine.Event.current.keyCode)
      {
        case KeyCode.UpArrow:
        case KeyCode.DownArrow:
          if (UnityEngine.Event.current.keyCode == KeyCode.UpArrow)
            this.SelectPreviousSearchResult();
          else
            this.SelectNextSearchResult();
          this.FrameSelected(false);
          UnityEngine.Event.current.Use();
          GUIUtility.ExitGUI();
          return;
      }
    }
    EditorGUIUtility.labelWidth = 0.0f;
    this.SearchFieldGUI(EditorGUILayout.kLabelFloatMaxW);
  }

  private void RefreshAudioPlay()
  {
    if ((UnityEngine.Object) SceneView.s_AudioSceneView != (UnityEngine.Object) null && (UnityEngine.Object) SceneView.s_AudioSceneView != (UnityEngine.Object) this && SceneView.s_AudioSceneView.m_PlayAudio)
    {
      SceneView.s_AudioSceneView.m_PlayAudio = false;
      Action<bool> sceneAudioChanged = SceneView.s_AudioSceneView.sceneAudioChanged;
      if (sceneAudioChanged != null)
        sceneAudioChanged(false);
      SceneView.s_AudioSceneView.Repaint();
    }
    foreach (AudioSource target in (AudioSource[]) Resources.FindObjectsOfTypeAll(typeof (AudioSource)))
    {
      if (!EditorUtility.IsPersistent((UnityEngine.Object) target) && target.playOnAwake)
      {
        if (!this.m_PlayAudio || !StageUtility.IsGameObjectRenderedByCamera(target.gameObject, this.m_Camera))
          target.Stop();
        else if (!target.isPlaying && target.isActiveAndEnabled)
          target.Play();
      }
    }
    foreach (AudioReverbZone target in (AudioReverbZone[]) Resources.FindObjectsOfTypeAll(typeof (AudioReverbZone)))
    {
      if (!EditorUtility.IsPersistent((UnityEngine.Object) target))
        target.active = this.m_PlayAudio && StageUtility.IsGameObjectRenderedByCamera(target.gameObject, this.m_Camera);
    }
    AudioUtil.SetListenerTransform(this.m_PlayAudio ? this.m_Camera.transform : (Transform) null);
    SceneView.s_AudioSceneView = this;
    if (!this.m_PlayAudio)
      return;
    AudioMixerWindow.RepaintAudioMixerWindow();
  }

  private void OnSelectionChange()
  {
    if (Selection.activeObject != (UnityEngine.Object) null && this.m_LastLockedObject != Selection.activeObject)
      this.viewIsLockedToObject = false;
    this.m_WasFocused = false;
    SceneView.s_SelectionCacheDirty = true;
    this.Repaint();
  }

  public virtual void AddItemsToMenu(GenericMenu menu)
  {
    if (!RenderDoc.IsInstalled() || RenderDoc.IsLoaded())
      return;
    menu.AddItem(RenderDocUtil.LoadRenderDocMenuItem, false, new GenericMenu.MenuFunction(RenderDoc.LoadRenderDoc));
  }

  public static void AddOverlayToActiveView<T>(T overlay) where T : Overlay
  {
    SceneView.s_ActiveViewOverlays.Add((Overlay) overlay);
    if (!((UnityEngine.Object) SceneView.lastActiveSceneView != (UnityEngine.Object) null))
      return;
    SceneView.lastActiveSceneView.overlayCanvas.Add((Overlay) overlay);
  }

  public static void RemoveOverlayFromActiveView<T>(T overlay) where T : Overlay
  {
    if (!SceneView.s_ActiveViewOverlays.Remove((Overlay) overlay) || !((UnityEngine.Object) SceneView.lastActiveSceneView != (UnityEngine.Object) null))
      return;
    SceneView.lastActiveSceneView.overlayCanvas.Remove((Overlay) overlay);
  }

  private static void MoveOverlaysToActiveView(SceneView previous, SceneView active)
  {
    if ((UnityEngine.Object) previous != (UnityEngine.Object) null)
    {
      foreach (Overlay activeViewOverlay in SceneView.s_ActiveViewOverlays)
        previous.overlayCanvas.Remove(activeViewOverlay);
    }
    if (!((UnityEngine.Object) active != (UnityEngine.Object) null))
      return;
    foreach (Overlay activeViewOverlay in SceneView.s_ActiveViewOverlays)
      active.overlayCanvas.Add(activeViewOverlay);
  }

  private static bool ValidateMenuMoveToFrontOrBack(Transform[] transforms, bool isFront)
  {
    if (transforms.Length == 0)
      return false;
    int num1 = 0;
    foreach (Transform transform in transforms)
    {
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null || (UnityEngine.Object) transform.parent == (UnityEngine.Object) null || PrefabUtility.IsPartOfNonAssetPrefabInstance((UnityEngine.Object) transform.parent))
        return false;
      int num2 = isFront ? 0 : transform.parent.childCount - 1;
      if (transform.GetSiblingIndex() == num2)
        ++num1;
    }
    return num1 < transforms.Length;
  }

  private static void RegisterMenuMoveChildrenUndo(Transform[] transforms, string message)
  {
    HashSet<Transform> transformSet = new HashSet<Transform>();
    foreach (Transform transform in transforms)
    {
      if (!transformSet.Contains(transform.parent))
      {
        Undo.RegisterChildrenOrderUndo((UnityEngine.Object) transform.parent, message);
        transformSet.Add(transform.parent);
      }
    }
  }

  [MenuItem("GameObject/Set as first sibling %=", secondaryPriority = 1f)]
  internal static void MenuMoveToFront()
  {
    Transform[] transforms = Selection.transforms;
    SceneView.RegisterMenuMoveChildrenUndo(transforms, "Set as first sibling");
    foreach (Transform transform in transforms)
      transform.SetAsFirstSibling();
  }

  [MenuItem("GameObject/Set as first sibling %=", true)]
  internal static bool ValidateMenuMoveToFront()
  {
    return SceneView.ValidateMenuMoveToFrontOrBack(Selection.transforms, true);
  }

  [MenuItem("GameObject/Set as last sibling %-", secondaryPriority = 2f)]
  internal static void MenuMoveToBack()
  {
    Transform[] transforms = Selection.transforms;
    SceneView.RegisterMenuMoveChildrenUndo(transforms, "Set as last sibling");
    foreach (Transform transform in transforms)
      transform.SetAsLastSibling();
  }

  [MenuItem("GameObject/Set as last sibling %-", true)]
  internal static bool ValidateMenuMoveToBack()
  {
    return SceneView.ValidateMenuMoveToFrontOrBack(Selection.transforms, false);
  }

  [MenuItem("GameObject/Move To View %&f", secondaryPriority = 3f)]
  internal static void MenuMoveToView()
  {
    if (!SceneView.ValidateMoveToView())
      return;
    SceneView.lastActiveSceneView.MoveToView();
  }

  [MenuItem("GameObject/Move To View %&f", true)]
  private static bool ValidateMoveToView()
  {
    return (UnityEngine.Object) SceneView.lastActiveSceneView != (UnityEngine.Object) null && Selection.transforms.Length != 0;
  }

  [MenuItem("GameObject/Align With View %#f", secondaryPriority = 4f)]
  internal static void MenuAlignWithView()
  {
    if (!SceneView.ValidateAlignWithView())
      return;
    SceneView.lastActiveSceneView.AlignWithView();
  }

  [MenuItem("GameObject/Align With View %#f", true)]
  internal static bool ValidateAlignWithView()
  {
    return (UnityEngine.Object) SceneView.lastActiveSceneView != (UnityEngine.Object) null && (UnityEngine.Object) Selection.activeTransform != (UnityEngine.Object) null;
  }

  [MenuItem("GameObject/Align View to Selected", secondaryPriority = 5f)]
  internal static void MenuAlignViewToSelected()
  {
    if (!SceneView.ValidateAlignViewToSelected())
      return;
    SceneView.lastActiveSceneView.AlignViewToObject(Selection.activeTransform);
  }

  [MenuItem("GameObject/Align View to Selected", true)]
  internal static bool ValidateAlignViewToSelected()
  {
    return (UnityEngine.Object) SceneView.lastActiveSceneView != (UnityEngine.Object) null && (UnityEngine.Object) Selection.activeTransform != (UnityEngine.Object) null;
  }

  [MenuItem("GameObject/Toggle Active State &#a", secondaryPriority = 6f)]
  internal static void ActivateSelection()
  {
    if (!((UnityEngine.Object) Selection.activeTransform != (UnityEngine.Object) null))
      return;
    GameObject[] gameObjects = Selection.gameObjects;
    Undo.RecordObjects((UnityEngine.Object[]) gameObjects, "Toggle Active State");
    bool flag = !Selection.activeGameObject.activeSelf;
    foreach (GameObject gameObject in gameObjects)
      gameObject.SetActive(flag);
  }

  [MenuItem("GameObject/Toggle Active State &#a", true)]
  internal static bool ValidateActivateSelection()
  {
    return (UnityEngine.Object) Selection.activeTransform != (UnityEngine.Object) null;
  }

  private static void CreateMipColorsTexture()
  {
    if ((bool) (UnityEngine.Object) SceneView.s_MipColorsTexture)
      return;
    Texture2D texture2D = new Texture2D(32 /*0x20*/, 32 /*0x20*/, TextureFormat.RGBA32, true);
    texture2D.hideFlags = HideFlags.HideAndDontSave;
    SceneView.s_MipColorsTexture = texture2D;
    Color[] colorArray = new Color[6]
    {
      new Color(0.0f, 0.0f, 1f, 0.8f),
      new Color(0.0f, 0.5f, 1f, 0.4f),
      new Color(1f, 1f, 1f, 0.0f),
      new Color(1f, 0.7f, 0.0f, 0.2f),
      new Color(1f, 0.3f, 0.0f, 0.6f),
      new Color(1f, 0.0f, 0.0f, 0.8f)
    };
    int num = Mathf.Min(6, SceneView.s_MipColorsTexture.mipmapCount);
    for (int miplevel = 0; miplevel < num; ++miplevel)
    {
      Color[] colors = new Color[Mathf.Max(SceneView.s_MipColorsTexture.width >> miplevel, 1) * Mathf.Max(SceneView.s_MipColorsTexture.height >> miplevel, 1)];
      for (int index = 0; index < colors.Length; ++index)
        colors[index] = colorArray[miplevel];
      SceneView.s_MipColorsTexture.SetPixels(colors, miplevel);
    }
    SceneView.s_MipColorsTexture.filterMode = UnityEngine.FilterMode.Trilinear;
    SceneView.s_MipColorsTexture.Apply(false);
    Shader.SetGlobalTexture("_SceneViewMipcolorsTexture", (Texture) SceneView.s_MipColorsTexture);
  }

  internal void SetSceneViewFiltering(bool enable) => this.m_ForceSceneViewFiltering = enable;

  internal void SetSceneViewFilteringForLODGroups(bool enable)
  {
    this.m_ForceSceneViewFilteringForLodGroupEditing = enable;
  }

  internal void SetSceneViewFilteringForStages(bool enable)
  {
    this.m_ForceSceneViewFilteringForStageHandling = enable;
  }

  private bool forceSceneViewFilteringForLodGroupEditing
  {
    get
    {
      return this.m_ForceSceneViewFilteringForLodGroupEditing && (bool) SceneView.s_PreferenceEnableFilteringWhileLodGroupEditing;
    }
  }

  private bool UseSceneFiltering()
  {
    return !string.IsNullOrEmpty(this.m_SearchFilter) && (bool) SceneView.s_PreferenceEnableFilteringWhileSearching || this.forceSceneViewFilteringForLodGroupEditing || this.m_ForceSceneViewFilteringForStageHandling || this.m_ForceSceneViewFiltering;
  }

  internal bool SceneViewIsRenderingHDR()
  {
    return (UnityEngine.Object) this.m_Camera != (UnityEngine.Object) null && this.m_Camera.allowHDR;
  }

  private void OnFocus() => SceneView.lastActiveSceneView = this;

  private void HandleClickAndDragToFocus()
  {
    UnityEngine.Event current = UnityEngine.Event.current;
    if (current.type == UnityEngine.EventType.MouseDrag)
      this.draggingLocked = SceneView.DraggingLockedState.Dragging;
    else if (GUIUtility.hotControl == 0 && this.draggingLocked == SceneView.DraggingLockedState.Dragging)
      this.draggingLocked = SceneView.DraggingLockedState.LookAt;
    if (current.type == UnityEngine.EventType.MouseDown)
    {
      Tools.s_ButtonDown = current.button;
      if (Application.platform != RuntimePlatform.OSXEditor)
        return;
      this.Focus();
    }
    else
    {
      if (current.type != UnityEngine.EventType.MouseUp || Tools.s_ButtonDown != current.button)
        return;
      Tools.s_ButtonDown = -1;
    }
  }

  private void SetupFogAndShadowDistance(out bool oldFog, out float oldShadowDistance)
  {
    oldFog = RenderSettings.fog;
    oldShadowDistance = QualitySettings.shadowDistance;
    if (UnityEngine.Event.current.type != UnityEngine.EventType.Repaint)
      return;
    if (!this.sceneViewState.fogEnabled)
      Unsupported.SetRenderSettingsUseFogNoDirty(false);
    if (this.m_Camera.orthographic)
      Unsupported.SetQualitySettingsShadowDistanceTemporarily(QualitySettings.shadowDistance + 0.5f * this.cameraDistance);
  }

  private void RestoreFogAndShadowDistance(bool oldFog, float oldShadowDistance)
  {
    if (UnityEngine.Event.current.type != UnityEngine.EventType.Repaint)
      return;
    Unsupported.SetRenderSettingsUseFogNoDirty(oldFog);
    Unsupported.SetQualitySettingsShadowDistanceTemporarily(oldShadowDistance);
  }

  private void CreateCameraTargetTexture(Rect cameraRect, bool hdr)
  {
    GraphicsFormat colorFormat = !hdr || !SystemInfo.IsFormatSupported(GraphicsFormat.R16G16B16A16_SFloat, GraphicsFormatUsage.Render) ? SystemInfo.GetGraphicsFormat(DefaultFormat.LDR) : GraphicsFormat.R16G16B16A16_SFloat;
    if ((UnityEngine.Object) this.m_SceneTargetTexture != (UnityEngine.Object) null && this.m_SceneTargetTexture.graphicsFormat != colorFormat)
    {
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.m_SceneTargetTexture);
      this.m_SceneTargetTexture = (RenderTexture) null;
    }
    Rect cameraRect1 = Handles.GetCameraRect(cameraRect);
    int num1 = (int) Mathf.Max(1f, cameraRect1.width);
    int num2 = (int) Mathf.Max(1f, cameraRect1.height);
    if ((UnityEngine.Object) this.m_SceneTargetTexture == (UnityEngine.Object) null)
    {
      RenderTexture renderTexture = new RenderTexture(0, 0, colorFormat, SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil));
      renderTexture.name = "SceneView RT";
      renderTexture.antiAliasing = 1;
      renderTexture.hideFlags = HideFlags.HideAndDontSave;
      this.m_SceneTargetTexture = renderTexture;
    }
    if (this.m_SceneTargetTexture.width != num1 || this.m_SceneTargetTexture.height != num2)
    {
      this.m_SceneTargetTexture.Release();
      this.m_SceneTargetTexture.width = num1;
      this.m_SceneTargetTexture.height = num2;
    }
    this.m_SceneTargetTexture.Create();
  }

  public bool IsCameraDrawModeSupported(SceneView.CameraMode mode)
  {
    return Handles.IsCameraDrawModeSupported(this.m_Camera, mode.drawMode) && (this.onValidateCameraMode == null || ((IEnumerable<Delegate>) this.onValidateCameraMode.GetInvocationList()).All<Delegate>((Func<Delegate, bool>) (validate => ((Func<SceneView.CameraMode, bool>) validate)(mode))));
  }

  public bool IsCameraDrawModeEnabled(SceneView.CameraMode mode)
  {
    return Handles.IsCameraDrawModeEnabled(this.m_Camera, mode.drawMode) && (this.onValidateCameraMode == null || ((IEnumerable<Delegate>) this.onValidateCameraMode.GetInvocationList()).All<Delegate>((Func<Delegate, bool>) (validate => ((Func<SceneView.CameraMode, bool>) validate)(mode))));
  }

  internal bool IsSceneCameraDeferred()
  {
    return !((UnityEngine.Object) this.m_Camera == (UnityEngine.Object) null | (UnityEngine.Object) GraphicsSettings.currentRenderPipeline != (UnityEngine.Object) null) && this.m_Camera.actualRenderingPath == RenderingPath.DeferredShading;
  }

  internal static bool DoesCameraDrawModeSupportDeferred(DrawCameraMode mode)
  {
    return mode == DrawCameraMode.Normal || mode == DrawCameraMode.Textured || mode == DrawCameraMode.TexturedWire || mode == DrawCameraMode.ShadowCascades || mode == DrawCameraMode.RenderPaths || mode == DrawCameraMode.AlphaChannel || mode == DrawCameraMode.DeferredDiffuse || mode == DrawCameraMode.DeferredSpecular || mode == DrawCameraMode.DeferredSmoothness || mode == DrawCameraMode.DeferredNormal || mode == DrawCameraMode.RealtimeCharting || mode == DrawCameraMode.Systems || mode == DrawCameraMode.Clustering || mode == DrawCameraMode.LitClustering || mode == DrawCameraMode.RealtimeAlbedo || mode == DrawCameraMode.RealtimeEmissive || mode == DrawCameraMode.RealtimeIndirect || mode == DrawCameraMode.RealtimeDirectionality || mode == DrawCameraMode.BakedLightmap || mode == DrawCameraMode.ValidateAlbedo || mode == DrawCameraMode.ValidateMetalSpecular;
  }

  internal static bool DoesCameraDrawModeSupportHDR(DrawCameraMode mode)
  {
    return mode == DrawCameraMode.Textured || mode == DrawCameraMode.TexturedWire;
  }

  private void PrepareCameraTargetTexture(Rect cameraRect)
  {
    bool hdr = this.SceneViewIsRenderingHDR();
    this.CreateCameraTargetTexture(cameraRect, hdr);
    this.m_Camera.targetTexture = this.m_SceneTargetTexture;
    if (!this.UseSceneFiltering() && SceneView.DoesCameraDrawModeSupportDeferred(this.m_CameraMode.drawMode) || !this.IsSceneCameraDeferred())
      return;
    this.m_Camera.renderingPath = RenderingPath.Forward;
  }

  private void PrepareCameraReplacementShader()
  {
    if (UnityEngine.Event.current.type != UnityEngine.EventType.Repaint)
      return;
    Handles.SetSceneViewColors((Color) SceneView.kSceneViewWire, (Color) SceneView.kSceneViewWireOverlay, (Color) SceneView.kSceneViewSelectedOutline, (Color) SceneView.kSceneViewSelectedChildrenOutline, (Color) SceneView.kSceneViewSelectedWire);
    if (this.m_CameraMode.drawMode == DrawCameraMode.Overdraw)
    {
      if (!(bool) (UnityEngine.Object) SceneView.s_ShowOverdrawShader)
        SceneView.s_ShowOverdrawShader = EditorGUIUtility.LoadRequired("SceneView/SceneViewShowOverdraw.shader") as Shader;
      this.m_Camera.SetReplacementShader(SceneView.s_ShowOverdrawShader, "RenderType");
    }
    else if (this.m_CameraMode.drawMode == DrawCameraMode.Mipmaps)
    {
      Texture.SetStreamingTextureMaterialDebugProperties();
      if (!(bool) (UnityEngine.Object) SceneView.s_ShowMipsShader)
        SceneView.s_ShowMipsShader = EditorGUIUtility.LoadRequired("SceneView/SceneViewShowMips.shader") as Shader;
      if ((UnityEngine.Object) SceneView.s_ShowMipsShader != (UnityEngine.Object) null && SceneView.s_ShowMipsShader.isSupported)
      {
        SceneView.CreateMipColorsTexture();
        this.m_Camera.SetReplacementShader(SceneView.s_ShowMipsShader, "RenderType");
      }
      else
        this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
    }
    else if (this.m_CameraMode.drawMode == DrawCameraMode.TextureStreaming)
    {
      Texture.SetStreamingTextureMaterialDebugProperties();
      if (!(bool) (UnityEngine.Object) SceneView.s_ShowTextureStreamingShader)
        SceneView.s_ShowTextureStreamingShader = EditorGUIUtility.LoadRequired("SceneView/SceneViewShowTextureStreaming.shader") as Shader;
      if ((UnityEngine.Object) SceneView.s_ShowTextureStreamingShader != (UnityEngine.Object) null && SceneView.s_ShowTextureStreamingShader.isSupported)
        this.m_Camera.SetReplacementShader(SceneView.s_ShowTextureStreamingShader, "RenderType");
      else
        this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
    }
    else
      this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
  }

  private bool SceneCameraRendersIntoRT() => (UnityEngine.Object) this.m_Camera.targetTexture != (UnityEngine.Object) null;

  private void DoDrawCamera(
    Rect windowSpaceCameraRect,
    Rect groupSpaceCameraRect,
    out bool pushedGUIClipNeedsToBePopped)
  {
    pushedGUIClipNeedsToBePopped = false;
    if (!this.m_Camera.gameObject.activeInHierarchy)
      return;
    bool asyncCompilation = ShaderUtil.allowAsyncCompilation;
    ShaderUtil.allowAsyncCompilation = EditorSettings.asyncShaderCompilation;
    DrawGridParameters gridParam = this.sceneViewGrids.PrepareGridRender(this.camera, this.pivot, this.m_Rotation.target, this.size, this.m_Ortho.target);
    UnityEngine.Event current = UnityEngine.Event.current;
    if (this.UseSceneFiltering())
    {
      bool flag = this.SceneCameraRendersIntoRT();
      if (flag)
      {
        GUIClip.Push(groupSpaceCameraRect, Vector2.zero, Vector2.zero, true);
        GUIClip.Internal_PushParentClip(Matrix4x4.identity, GUIClip.GetParentMatrix(), groupSpaceCameraRect);
      }
      if (current.type == UnityEngine.EventType.Repaint)
        this.RenderFilteredScene(groupSpaceCameraRect);
      this.DrawPingedObjectSubmeshOutlineIfNeeded();
      if (flag)
      {
        GUIClip.Internal_PopParentClip();
        GUIClip.Pop();
      }
      if (current.type == UnityEngine.EventType.Repaint)
        RenderTexture.active = (RenderTexture) null;
      GUI.EndGroup();
      GUI.BeginGroup(windowSpaceCameraRect);
      if (current.type == UnityEngine.EventType.Repaint)
        Graphics.DrawTexture(groupSpaceCameraRect, (Texture) this.m_SceneTargetTexture, new Rect(0.0f, 0.0f, 1f, 1f), 0, 0, 0, 0, GUI.color, GUI.blitMaterial);
      Handles.SetCamera(groupSpaceCameraRect, this.m_Camera);
    }
    else
    {
      if (this.SceneCameraRendersIntoRT())
      {
        Rect position = this.position;
        double width = (double) position.width;
        position = this.position;
        double height = (double) position.height;
        GUIClip.Push(new Rect(0.0f, 0.0f, (float) width, (float) height), Vector2.zero, Vector2.zero, true);
        GUIClip.Internal_PushParentClip(Matrix4x4.identity, GUIClip.GetParentMatrix(), groupSpaceCameraRect);
        pushedGUIClipNeedsToBePopped = true;
      }
      Handles.DrawCameraStep1(groupSpaceCameraRect, this.m_Camera, this.m_CameraMode.drawMode, gridParam, this.drawGizmos, true);
      if (current.type == UnityEngine.EventType.Repaint)
      {
        if (SceneView.s_SelectionCacheDirty)
        {
          HandleUtility.FilterInstanceIDs((IEnumerable<GameObject>) Selection.gameObjects, out SceneView.s_CachedParentRenderersForOutlining, out SceneView.s_CachedChildRenderersForOutlining, out SceneView.s_CachedChildRenderersForOutliningHashSet);
          SceneView.s_SelectionCacheDirty = false;
        }
        OutlineDrawMode outlineMode = (OutlineDrawMode) 0;
        if (AnnotationUtility.showSelectionOutline)
          outlineMode |= OutlineDrawMode.SelectionOutline;
        if (AnnotationUtility.showSelectionWire)
          outlineMode |= OutlineDrawMode.SelectionWire;
        if (outlineMode != 0)
          Handles.DrawOutlineOrWireframeInternal((Color) SceneView.kSceneViewSelectedOutline, (Color) SceneView.kSceneViewSelectedChildrenOutline, 1f - this.alphaMultiplier, SceneView.s_CachedParentRenderersForOutlining, SceneView.s_CachedChildRenderersForOutlining, outlineMode);
      }
      this.DrawRenderModeOverlay(groupSpaceCameraRect);
      this.DrawPingedObjectSubmeshOutlineIfNeeded();
    }
    ShaderUtil.allowAsyncCompilation = asyncCompilation;
  }

  private void DrawPingedObjectSubmeshOutlineIfNeeded()
  {
    if (!this.isPingingObject)
      return;
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    if ((double) realtimeSinceStartup - (double) this.pingStartTime > 1.0)
    {
      this.isPingingObject = false;
      this.alphaMultiplier = 0.0f;
      this.submeshOutlineMaterialId = 0;
    }
    else
    {
      this.alphaMultiplier = Mathf.SmoothStep(1f, 0.0f, (realtimeSinceStartup - this.pingStartTime) / 1f);
      if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
      {
        Handles.DrawSubmeshOutline((Color) SceneView.kSceneViewSelectedSubmeshOutline, (Color) SceneView.kSceneViewSelectedSubmeshOutline, this.alphaMultiplier, this.submeshOutlineMaterialId);
        this.Repaint();
      }
    }
  }

  private void RenderFilteredScene(Rect groupSpaceCameraRect)
  {
    RenderingPath renderingPath = this.m_Camera.renderingPath;
    this.DoClearCamera(groupSpaceCameraRect);
    Handles.DrawCamera(groupSpaceCameraRect, this.m_Camera, this.m_CameraMode.drawMode, this.drawGizmos);
    RenderTextureDescriptor descriptor1 = this.m_SceneTargetTexture.descriptor with
    {
      depthBufferBits = 0
    };
    RenderTexture temporary1 = RenderTexture.GetTemporary(descriptor1);
    temporary1.name = "SavedColorRT";
    Graphics.Blit((Texture) this.m_SceneTargetTexture, temporary1);
    float num = this.UseSceneFiltering() ? 1f : Mathf.Clamp01((float) (EditorApplication.timeSinceStartup - this.m_StartSearchFilterTime));
    if (!(bool) (UnityEngine.Object) SceneView.s_FadeMaterial)
      SceneView.s_FadeMaterial = EditorGUIUtility.LoadRequired("SceneView/SceneViewGrayscaleEffectFade.mat") as Material;
    SceneView.s_FadeMaterial.SetFloat("_Fade", num);
    Graphics.Blit((Texture) temporary1, this.m_SceneTargetTexture, SceneView.s_FadeMaterial);
    this.m_Camera.renderingPath = RenderingPath.Forward;
    if (!(bool) (UnityEngine.Object) SceneView.s_AuraShader)
      SceneView.s_AuraShader = EditorGUIUtility.LoadRequired("SceneView/SceneViewAura.shader") as Shader;
    this.m_Camera.SetReplacementShader(SceneView.s_AuraShader, "");
    Handles.SetCameraFilterMode(this.m_Camera, Handles.CameraFilterMode.ShowFiltered);
    Handles.DrawCamera(groupSpaceCameraRect, this.m_Camera, this.m_CameraMode.drawMode, this.drawGizmos);
    RenderTextureDescriptor descriptor2 = this.m_SceneTargetTexture.descriptor;
    descriptor1.depthBufferBits = 0;
    RenderTexture temporary2 = RenderTexture.GetTemporary(descriptor2);
    temporary2.name = "FadedColorRT";
    Graphics.Blit((Texture) this.m_SceneTargetTexture, temporary2);
    bool fxEnabled = this.sceneViewState.fxEnabled;
    bool imageEffectsEnabled = this.sceneViewState.imageEffectsEnabled;
    this.UpdateImageEffects(false);
    this.sceneViewState.fxEnabled = false;
    Material skybox = RenderSettings.skybox;
    RenderSettings.skybox = (Material) null;
    RenderTexture.active = this.m_SceneTargetTexture;
    GL.Clear(false, true, Color.clear);
    this.m_Camera.ResetReplacementShader();
    Handles.DrawCamera(groupSpaceCameraRect, this.m_Camera, this.m_CameraMode.drawMode, this.drawGizmos);
    this.UpdateImageEffects(imageEffectsEnabled);
    this.sceneViewState.fxEnabled = fxEnabled;
    RenderSettings.skybox = skybox;
    if (!(bool) (UnityEngine.Object) SceneView.s_ApplyFilterMaterial)
      SceneView.s_ApplyFilterMaterial = EditorGUIUtility.LoadRequired("SceneView/SceneViewApplyFilter.mat") as Material;
    SceneView.s_ApplyFilterMaterial.SetTexture("_MaskTex", (Texture) this.m_SceneTargetTexture);
    Graphics.Blit((Texture) temporary2, temporary1, SceneView.s_ApplyFilterMaterial);
    Graphics.Blit((Texture) temporary1, this.m_SceneTargetTexture);
    RenderTexture.ReleaseTemporary(temporary1);
    RenderTexture.ReleaseTemporary(temporary2);
    OutlineDrawMode outlineMode = (OutlineDrawMode) 0;
    if (AnnotationUtility.showSelectionOutline)
      outlineMode |= OutlineDrawMode.SelectionOutline;
    if (AnnotationUtility.showSelectionWire)
      outlineMode |= OutlineDrawMode.SelectionWire;
    if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint && outlineMode != 0)
    {
      if (SceneView.s_SelectionCacheDirty)
      {
        HandleUtility.FilterInstanceIDs((IEnumerable<GameObject>) Selection.gameObjects, out SceneView.s_CachedParentRenderersForOutlining, out SceneView.s_CachedChildRenderersForOutlining, out SceneView.s_CachedChildRenderersForOutliningHashSet);
        SceneView.s_SelectionCacheDirty = false;
      }
      Handles.DrawOutlineOrWireframeInternal((Color) SceneView.kSceneViewSelectedOutline, (Color) SceneView.kSceneViewSelectedChildrenOutline, 1f - this.alphaMultiplier, SceneView.s_CachedParentRenderersForOutlining, SceneView.s_CachedChildRenderersForOutlining, outlineMode);
      Handles.Internal_FinishDrawingCamera(this.m_Camera, this.drawGizmos);
    }
    this.m_Camera.SetReplacementShader(this.m_ReplacementShader, this.m_ReplacementString);
    this.m_Camera.renderingPath = renderingPath;
    if ((double) num >= 1.0)
      return;
    this.Repaint();
  }

  private void DoClearCamera(Rect cameraRect)
  {
    float verticalFov = this.GetVerticalFOV(this.m_CameraSettings.fieldOfView);
    float fieldOfView = this.m_Camera.fieldOfView;
    CameraClearFlags clearFlags = this.m_Camera.clearFlags;
    if ((UnityEngine.Object) GraphicsSettings.currentRenderPipeline != (UnityEngine.Object) null)
      this.m_Camera.clearFlags = CameraClearFlags.Color;
    this.m_Camera.fieldOfView = verticalFov;
    Handles.ClearCamera(cameraRect, this.m_Camera);
    this.m_Camera.clearFlags = clearFlags;
    this.m_Camera.fieldOfView = fieldOfView;
  }

  private void SetupCustomSceneLighting()
  {
    if (this.m_SceneIsLit)
      return;
    this.m_Light[0].transform.rotation = this.m_Camera.transform.rotation;
    if (UnityEngine.Event.current.type != UnityEngine.EventType.Repaint)
      return;
    InternalEditorUtility.SetCustomLighting(this.m_Light, SceneView.kSceneViewMidLight);
  }

  private void CleanupCustomSceneLighting()
  {
    if (this.m_SceneIsLit || UnityEngine.Event.current.type != UnityEngine.EventType.Repaint)
      return;
    InternalEditorUtility.RemoveCustomLighting();
  }

  private void HandleViewToolCursor(Rect cameraRect)
  {
    if (!Tools.viewToolActive || UnityEngine.Event.current.type != UnityEngine.EventType.Repaint || EditorWindow.mouseOverWindow is SceneView mouseOverWindow && ((UnityEngine.Object) EditorWindow.mouseOverWindow != (UnityEngine.Object) this || !mouseOverWindow.sceneViewMotion.viewportsUnderMouse))
      return;
    MouseCursor cursor = MouseCursor.Arrow;
    switch (Tools.viewTool)
    {
      case ViewTool.Orbit:
        cursor = MouseCursor.Orbit;
        break;
      case ViewTool.Pan:
        cursor = MouseCursor.Pan;
        break;
      case ViewTool.Zoom:
        cursor = MouseCursor.Zoom;
        break;
      case ViewTool.FPS:
        cursor = MouseCursor.FPS;
        break;
    }
    if (cursor == 0)
      return;
    SceneView.AddCursorRect(cameraRect, cursor);
  }

  private static bool ComponentHasImageEffectAttribute(UnityEngine.Component c)
  {
    return !((UnityEngine.Object) c == (UnityEngine.Object) null) && Attribute.IsDefined((MemberInfo) c.GetType(), typeof (ImageEffectAllowedInSceneView));
  }

  private void UpdateImageEffects(bool enable)
  {
    if (UnityEngine.Event.current.type != UnityEngine.EventType.Repaint)
      return;
    Camera mainCamera = SceneView.GetMainCamera();
    if (!enable || (UnityEngine.Object) mainCamera == (UnityEngine.Object) null)
      ComponentUtility.DestroyComponentsMatching(this.m_Camera.gameObject, new ComponentUtility.IsDesiredComponent(SceneView.ComponentHasImageEffectAttribute));
    else
      ComponentUtility.ReplaceComponentsIfDifferent(mainCamera.gameObject, this.m_Camera.gameObject, new ComponentUtility.IsDesiredComponent(SceneView.ComponentHasImageEffectAttribute));
  }

  private void DoOnPreSceneGUICallbacks(Rect cameraRect)
  {
    if (this.hasSearchFilter)
      return;
    this.CallOnPreSceneGUI();
  }

  [Obsolete("OnGUI has been deprecated. Use OnSceneGUI instead.")]
  protected virtual void OnGUI()
  {
  }

  protected virtual void OnSceneGUI() => this.DoOnGUI();

  private void DoOnGUI()
  {
    Action<SceneView> onGuiStarted = SceneView.onGUIStarted;
    if (onGuiStarted != null)
      onGuiStarted(this);
    UnityEngine.Event current = UnityEngine.Event.current;
    if (current.type != UnityEngine.EventType.Layout)
    {
      bool flag = (UnityEngine.Object) SceneView.lastActiveSceneView == (UnityEngine.Object) this;
      foreach (Overlay overlay in this.overlayCanvas.overlays)
      {
        if (overlay is ITransientOverlay transientOverlay)
          overlay.displayed = flag && transientOverlay.visible;
      }
    }
    this.LegacyOverlayPreOnGUI();
    SceneView.s_CurrentDrawingSceneView = this;
    if (current.type == UnityEngine.EventType.Layout)
    {
      SceneView.s_MouseRects.Clear();
      Tools.InvalidateHandlePosition();
    }
    this.sceneViewGrids.UpdateGridColor();
    Color color = GUI.color;
    Rect rect1 = this.m_Camera.rect;
    Rect cameraViewport = this.cameraViewport;
    if ((double) cameraViewport.width <= 0.0 || (double) cameraViewport.height <= 0.0)
      return;
    this.HandleClickAndDragToFocus();
    this.BeginWindows();
    if (current.type == UnityEngine.EventType.Layout)
      this.m_ShowSceneViewWindows = (UnityEngine.Object) SceneView.lastActiveSceneView == (UnityEngine.Object) this;
    bool oldFog;
    float oldShadowDistance;
    this.SetupFogAndShadowDistance(out oldFog, out oldShadowDistance);
    GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
    GUI.color = Color.white;
    EditorGUIUtility.labelWidth = 100f;
    this.SetupCamera();
    RenderingPath renderingPath = this.m_Camera.renderingPath;
    bool flag1 = false;
    if (this.m_CustomScene.IsValid())
      flag1 = Unsupported.SetOverrideLightingSettings(this.m_CustomScene);
    this.m_StageHandling?.StartOnGUI();
    this.SetupCustomSceneLighting();
    GUI.BeginGroup(cameraViewport);
    Rect rect2 = new Rect(0.0f, 0.0f, cameraViewport.width, cameraViewport.height);
    Rect pixels = EditorGUIUtility.PointsToPixels(rect2);
    this.HandleViewToolCursor(cameraViewport);
    this.PrepareCameraTargetTexture(pixels);
    this.DoClearCamera(pixels);
    this.m_Camera.cullingMask = Tools.visibleLayers;
    Handles.SetCamera(pixels, this.m_Camera);
    this.DoOnPreSceneGUICallbacks(pixels);
    this.PrepareCameraReplacementShader();
    this.m_MainViewControlID = GUIUtility.GetControlID(FocusType.Keyboard);
    if (current.GetTypeForControl(this.m_MainViewControlID) == UnityEngine.EventType.MouseDown && rect2.Contains(current.mousePosition))
      GUIUtility.keyboardControl = this.m_MainViewControlID;
    bool pushedGUIClipNeedsToBePopped;
    this.DoDrawCamera(cameraViewport, rect2, out pushedGUIClipNeedsToBePopped);
    this.CleanupCustomSceneLighting();
    if (flag1)
      Unsupported.RestoreOverrideLightingSettings();
    bool flag2 = (UnityEngine.Object) this.m_Parent != (UnityEngine.Object) null && (UnityEngine.Object) this.m_Parent.actualView == (UnityEngine.Object) this && this.m_Parent.hdrActive;
    if (!this.UseSceneFiltering() && current.type == UnityEngine.EventType.Repaint && GraphicsFormatUtility.IsIEEE754Format(this.m_SceneTargetTexture.graphicsFormat) && !flag2)
    {
      RenderTexture temporary = RenderTexture.GetTemporary(this.m_SceneTargetTexture.descriptor with
      {
        graphicsFormat = SystemInfo.GetGraphicsFormat(DefaultFormat.LDR),
        depthBufferBits = 0
      });
      temporary.name = "LDRSceneTarget";
      Graphics.Blit((Texture) this.m_SceneTargetTexture, temporary);
      Graphics.Blit((Texture) temporary, this.m_SceneTargetTexture);
      Graphics.SetRenderTarget(this.m_SceneTargetTexture.colorBuffer, this.m_SceneTargetTexture.depthBuffer);
      RenderTexture.ReleaseTemporary(temporary);
    }
    if (!this.UseSceneFiltering() && !this.isPingingObject && this.m_Camera.gameObject.activeInHierarchy)
      Handles.DrawCameraStep2(this.m_Camera, this.m_CameraMode.drawMode, this.drawGizmos);
    this.RestoreFogAndShadowDistance(oldFog, oldShadowDistance);
    this.m_Camera.renderingPath = renderingPath;
    if (!this.UseSceneFiltering())
    {
      if (current.type == UnityEngine.EventType.Repaint)
      {
        Profiler.BeginSample("SceneView.BlitRT");
        Graphics.SetRenderTarget((RenderTexture) null);
      }
      if (pushedGUIClipNeedsToBePopped)
      {
        GUIClip.Internal_PopParentClip();
        GUIClip.Pop();
      }
      if (current.type == UnityEngine.EventType.Repaint)
      {
        Graphics.DrawTexture(rect2, (Texture) this.m_SceneTargetTexture, new Rect(0.0f, 0.0f, 1f, 1f), 0, 0, 0, 0, GUI.color, EditorGUIUtility.GUITextureBlit2SRGBMaterial);
        Profiler.EndSample();
      }
    }
    GUIClip.Push(new Rect(0.0f, 0.0f, (float) this.m_SceneTargetTexture.width, (float) this.m_SceneTargetTexture.height), Vector2.zero, Vector2.zero, true);
    if (current.type == UnityEngine.EventType.Repaint)
    {
      Graphics.SetRenderTarget(this.m_SceneTargetTexture);
      GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
      GUIClip.Internal_PushParentClip(Matrix4x4.identity, GUIClip.GetParentMatrix(), rect2);
    }
    this.HandleSelectionAndOnSceneGUI();
    this.DefaultHandles();
    this.m_SceneViewMotion.DoViewTool(this);
    this.m_Viewpoint.UpdateViewpointMotion(this.m_Position.isAnimating || this.m_Rotation.isAnimating);
    Handles.SetCameraFilterMode(Camera.current, this.UseSceneFiltering() ? Handles.CameraFilterMode.ShowFiltered : Handles.CameraFilterMode.Off);
    if (current.type == UnityEngine.EventType.ExecuteCommand || current.type == UnityEngine.EventType.ValidateCommand || current.keyCode == KeyCode.Escape)
      this.CommandsGUI();
    Handles.SetCameraFilterMode(Camera.current, Handles.CameraFilterMode.Off);
    Handles.SetCameraFilterMode(this.m_Camera, Handles.CameraFilterMode.Off);
    this.HandleDragging(current);
    if (current.type == UnityEngine.EventType.Repaint)
    {
      Graphics.SetRenderTarget((RenderTexture) null);
      GUIClip.Internal_PopParentClip();
    }
    GUIClip.Pop();
    GUI.EndGroup();
    GUI.BeginGroup(cameraViewport);
    if (current.type == UnityEngine.EventType.Repaint)
      Graphics.DrawTexture(rect2, (Texture) this.m_SceneTargetTexture, new Rect(0.0f, 0.0f, 1f, 1f), 0, 0, 0, 0, GUI.color, EditorGUIUtility.GUITextureBlitSceneGUIMaterial);
    GUI.EndGroup();
    GUI.color = color;
    this.EndWindows();
    this.HandleMouseCursor();
    SceneView.s_CurrentDrawingSceneView = (SceneView) null;
    this.m_Camera.rect = rect1;
    if (this.m_Viewpoint.hasActiveViewpoint)
      this.m_Viewpoint.OnGUIDrawCameraOverscan();
    Action<SceneView> onGuiEnded = SceneView.onGUIEnded;
    if (onGuiEnded != null)
      onGuiEnded(this);
    if (this.m_StageHandling == null)
      return;
    this.m_StageHandling.EndOnGUI();
  }

  [Shortcut("Scene View/Menu", typeof (SceneViewMotion.SceneViewViewport), KeyCode.Mouse1, ShortcutModifiers.None)]
  private static void OpenActionMenu(ShortcutArguments args)
  {
    if (EditorWindow.mouseOverWindow?.GetType() != typeof (SceneView))
      return;
    VisualElement visualElement = EditorWindow.focusedWindow.rootVisualElement.panel.Pick(UnityEngine.Event.current.mousePosition);
    if (visualElement == null)
      return;
    SceneViewMotion.SceneViewViewport context = args.context as SceneViewMotion.SceneViewViewport;
    if (visualElement != context.window.cameraViewVisualElement)
      return;
    ContextMenuUtility.ShowActionMenu();
    context.window.SendEvent(new UnityEngine.Event()
    {
      type = UnityEngine.EventType.Layout
    });
  }

  internal void SwitchToRenderMode(DrawCameraMode mode, bool sceneLighting = true)
  {
    this.sceneLighting = sceneLighting;
    this.cameraMode = SceneView.GetBuiltinCameraMode(mode);
  }

  internal void SwitchToUnlit() => this.SwitchToRenderMode(DrawCameraMode.Textured, false);

  internal void ToggleLastDebugDrawMode()
  {
    if (this.cameraMode.drawMode == this.m_LastDebugDrawMode)
      this.SwitchToRenderMode(DrawCameraMode.Textured);
    else
      this.SwitchToRenderMode(this.m_LastDebugDrawMode);
  }

  [Shortcut("Scene View/Render Mode/Wireframe", typeof (SceneView), KeyCode.Alpha1, ShortcutModifiers.Alt)]
  private static void SetWireframeMode(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null))
      return;
    context.SwitchToRenderMode(DrawCameraMode.Wireframe);
  }

  [Shortcut("Scene View/Render Mode/Shaded Wireframe", typeof (SceneView), KeyCode.Alpha2, ShortcutModifiers.Alt)]
  private static void SetShadedWireframeMode(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null))
      return;
    context.SwitchToRenderMode(DrawCameraMode.TexturedWire);
  }

  [Shortcut("Scene View/Render Mode/Unlit", typeof (SceneView), KeyCode.Alpha3, ShortcutModifiers.Alt)]
  private static void SetUnlitMode(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null))
      return;
    context.SwitchToUnlit();
  }

  [Shortcut("Scene View/Render Mode/Shaded", typeof (SceneView), KeyCode.Alpha4, ShortcutModifiers.Alt)]
  private static void SetShadedMode(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null))
      return;
    context.SwitchToRenderMode(DrawCameraMode.Normal);
  }

  [Shortcut("Scene View/Render Mode/Last Debug Draw Mode", typeof (SceneView), KeyCode.Alpha6, ShortcutModifiers.Alt)]
  private static void SetLastDebugDrawMode(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null))
      return;
    context.SwitchToRenderMode(context.m_LastDebugDrawMode);
  }

  [FormerlyPrefKeyAs("Tools/2D Mode", "2")]
  [Shortcut("Scene View/Toggle 2D Mode", typeof (SceneView), KeyCode.Alpha2, ShortcutModifiers.None)]
  private static void Toggle2DMode(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null))
      return;
    context.in2DMode = !context.in2DMode;
  }

  [Shortcut("Scene View/Toggle Orthographic Projection", typeof (SceneView))]
  private static void ToggleOrthoView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewSetOrtho(context, !context.orthographic);
  }

  [Shortcut("Scene View/Set Orthographic Right View", typeof (SceneView))]
  private static void SetOrthoRightView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 0, true);
  }

  [Shortcut("Scene View/Set Right View", typeof (SceneView))]
  private static void SetRightView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 0, context.orthographic);
  }

  [Shortcut("Scene View/Set Top View", typeof (SceneView))]
  private static void SetTopView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 1, context.orthographic);
  }

  [Shortcut("Scene View/Set Orthographic Top View", typeof (SceneView))]
  private static void SetOrthoTopView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 1, true);
  }

  [Shortcut("Scene View/Set Front View", typeof (SceneView))]
  private static void SetFrontView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 2, context.orthographic);
  }

  [Shortcut("Scene View/Set Orthographic Front View", typeof (SceneView))]
  private static void SetOrthoFrontView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 2, true);
  }

  [Shortcut("Scene View/Set Left View", typeof (SceneView))]
  private static void SetLeftView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 3, context.orthographic);
  }

  [Shortcut("Scene View/Set Orthographic Left View", typeof (SceneView))]
  private static void SetOrthoLeftView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 3, true);
  }

  [Shortcut("Scene View/Set Bottom View", typeof (SceneView))]
  private static void SetBottomView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 4, context.orthographic);
  }

  [Shortcut("Scene View/Set Orthographic Bottom View", typeof (SceneView))]
  private static void SetOrthoBottomView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 4, true);
  }

  [Shortcut("Scene View/Set Back View", typeof (SceneView))]
  private static void SetBackView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 5, context.orthographic);
  }

  [Shortcut("Scene View/Set Orthographic Back View", typeof (SceneView))]
  private static void SetOrthoBackView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewAxisDirection(context, 5, true);
  }

  [Shortcut("Scene View/Set Free View", typeof (SceneView))]
  private static void SetFreeView(ShortcutArguments args)
  {
    SceneView context = args.context as SceneView;
    if (!((UnityEngine.Object) context != (UnityEngine.Object) null) || context.isRotationLocked)
      return;
    context.m_OrientationGizmo?.ViewFromNiceAngle(context, false);
  }

  private void HandleMouseCursor()
  {
    if (EditorWindow.mouseOverWindow is SceneView && (UnityEngine.Object) EditorWindow.mouseOverWindow != (UnityEngine.Object) this)
      return;
    UnityEngine.Event current = UnityEngine.Event.current;
    Rect position = new Rect(0.0f, 0.0f, this.position.width, this.position.height);
    bool flag1 = current.type == UnityEngine.EventType.Repaint;
    if (flag1)
    {
      bool flag2 = false;
      MouseCursor mouseCursor = MouseCursor.Arrow;
      foreach (SceneView.CursorRect mouseRect in SceneView.s_MouseRects)
      {
        if (mouseRect.rect.Contains(current.mousePosition))
        {
          mouseCursor = mouseRect.cursor;
          position = mouseRect.rect;
          flag2 = true;
        }
      }
      bool flag3 = mouseCursor != SceneView.s_LastCursor;
      if (flag3)
      {
        SceneView.s_LastCursor = mouseCursor;
        InternalEditorUtility.ResetCursor();
      }
      if (flag2 | flag3)
        this.Repaint();
    }
    if (!flag1 || SceneView.s_LastCursor == 0)
      return;
    EditorGUIUtility.AddCursorRect(position, SceneView.s_LastCursor);
  }

  private void DrawRenderModeOverlay(Rect cameraRect)
  {
    if (this.m_CameraMode.drawMode == DrawCameraMode.AlphaChannel)
    {
      if (!(bool) (UnityEngine.Object) SceneView.s_AlphaOverlayMaterial)
        SceneView.s_AlphaOverlayMaterial = EditorGUIUtility.LoadRequired("SceneView/SceneViewAlphaMaterial.mat") as Material;
      Handles.BeginGUI();
      if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
        Graphics.DrawTexture(cameraRect, (Texture) EditorGUIUtility.whiteTexture, SceneView.s_AlphaOverlayMaterial);
      Handles.EndGUI();
    }
    if (this.m_CameraMode.drawMode != DrawCameraMode.DeferredDiffuse && this.m_CameraMode.drawMode != DrawCameraMode.DeferredSpecular && this.m_CameraMode.drawMode != DrawCameraMode.DeferredSmoothness && this.m_CameraMode.drawMode != DrawCameraMode.DeferredNormal)
      return;
    if (!(bool) (UnityEngine.Object) SceneView.s_DeferredOverlayMaterial)
      SceneView.s_DeferredOverlayMaterial = EditorGUIUtility.LoadRequired("SceneView/SceneViewDeferredMaterial.mat") as Material;
    Handles.BeginGUI();
    if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
    {
      SceneView.s_DeferredOverlayMaterial.SetFloat("_DisplayMode", (float) (this.m_CameraMode.drawMode - 8));
      Graphics.DrawTexture(cameraRect, (Texture) EditorGUIUtility.whiteTexture, SceneView.s_DeferredOverlayMaterial);
    }
    Handles.EndGUI();
  }

  private void HandleSelectionAndOnSceneGUI()
  {
    this.m_RectSelection.OnGUI();
    this.CallOnSceneGUI();
  }

  /// <summary>
  ///   <para>The central point that the camera orbits within the Scene view.</para>
  /// </summary>
  public Vector3 pivot
  {
    get => this.m_Position.value;
    set => this.m_Position.value = value;
  }

  /// <summary>
  ///   <para>The direction of the camera to the pivot of the SceneView.</para>
  /// </summary>
  public Quaternion rotation
  {
    get => !this.m_2DMode ? this.m_Rotation.value : Quaternion.identity;
    set
    {
      if (this.m_2DMode)
        Debug.LogWarning((object) "SceneView rotation is fixed to identity when in 2D mode. This will be an error in future versions of Unity.");
      else
        this.m_Rotation.value = value;
    }
  }

  private static float ValidateSceneSize(float value)
  {
    if ((double) value == 0.0 || float.IsNaN(value))
      return float.Epsilon;
    if ((double) value > 3.1999999823863273E+34)
      return 3.2E+34f;
    return (double) value < -3.1999999823863273E+34 ? -3.2E+34f : value;
  }

  /// <summary>
  ///   <para>The size of the Scene view measured diagonally.</para>
  /// </summary>
  public float size
  {
    get => this.m_Size.value;
    set => this.m_Size.value = SceneView.ValidateSceneSize(value);
  }

  internal float targetSize
  {
    get => this.m_Size.target;
    set => this.m_Size.target = SceneView.ValidateSceneSize(value);
  }

  private float perspectiveFov => this.m_CameraSettings.fieldOfView;

  /// <summary>
  ///   <para>Whether the Scene view camera is set to orthographic mode.</para>
  /// </summary>
  public bool orthographic
  {
    get => this.m_Ortho.value;
    set
    {
      this.m_Ortho.value = value;
      this.m_OrientationGizmo?.UpdateGizmoLabel(this, this.m_Rotation.target * Vector3.forward, this.m_Ortho.target);
    }
  }

  public void FixNegativeSize()
  {
    if ((double) this.size == 0.0)
      this.size = float.Epsilon;
    float perspectiveFov = this.perspectiveFov;
    if ((double) this.size >= 0.0)
      return;
    Vector3 vector3 = this.m_Position.value + this.rotation * new Vector3(0.0f, 0.0f, -SceneView.GetPerspectiveCameraDistance(this.size, perspectiveFov));
    this.size = -this.size;
    float perspectiveCameraDistance = SceneView.GetPerspectiveCameraDistance(this.size, perspectiveFov);
    this.m_Position.value = vector3 + this.rotation * new Vector3(0.0f, 0.0f, perspectiveCameraDistance);
  }

  internal float CalcCameraDist()
  {
    float fov = this.m_Ortho.Fade(this.perspectiveFov, 0.0f);
    if ((double) fov <= 3.0)
      return 0.0f;
    this.m_Camera.orthographic = false;
    return SceneView.GetPerspectiveCameraDistance(this.size, fov);
  }

  private void ResetIfNaN()
  {
    if (float.IsInfinity(this.m_Position.value.x) || float.IsNaN(this.m_Position.value.x))
      this.m_Position.value = Vector3.zero;
    if (!float.IsInfinity(this.m_Rotation.value.x) && !float.IsNaN(this.m_Rotation.value.x))
      return;
    this.m_Rotation.value = Quaternion.identity;
  }

  internal static Camera GetMainCamera()
  {
    Camera main = Camera.main;
    if ((UnityEngine.Object) main != (UnityEngine.Object) null)
      return main;
    Camera[] allCameras = Camera.allCameras;
    return allCameras != null && allCameras.Length == 1 ? allCameras[0] : (Camera) null;
  }

  internal static RenderingPath GetSceneViewRenderingPath()
  {
    Camera mainCamera = SceneView.GetMainCamera();
    return (UnityEngine.Object) mainCamera != (UnityEngine.Object) null ? mainCamera.renderingPath : RenderingPath.UsePlayerSettings;
  }

  internal static bool IsUsingDeferredRenderingPath()
  {
    int num;
    switch (SceneView.GetSceneViewRenderingPath())
    {
      case RenderingPath.UsePlayerSettings:
        num = EditorGraphicsSettings.GetCurrentTierSettings().renderingPath == RenderingPath.DeferredShading ? 1 : 0;
        break;
      case RenderingPath.DeferredShading:
        num = 1;
        break;
      default:
        num = 0;
        break;
    }
    return num != 0;
  }

  internal bool CheckDrawModeForRenderingPath(DrawCameraMode mode)
  {
    RenderingPath actualRenderingPath = this.m_Camera.actualRenderingPath;
    return mode != DrawCameraMode.DeferredDiffuse && mode != DrawCameraMode.DeferredSpecular && mode != DrawCameraMode.DeferredSmoothness && mode != DrawCameraMode.DeferredNormal || actualRenderingPath == RenderingPath.DeferredShading;
  }

  private void UpdateSceneCameraSettings()
  {
    Camera mainCamera = SceneView.GetMainCamera();
    this.m_Camera.useInteractiveLightBakingData = this.usesInteractiveLightBakingData;
    if ((UnityEngine.Object) mainCamera != (UnityEngine.Object) null)
    {
      this.m_Camera.iso = mainCamera.iso;
      this.m_Camera.shutterSpeed = mainCamera.shutterSpeed;
      this.m_Camera.aperture = mainCamera.aperture;
      this.m_Camera.anamorphism = mainCamera.anamorphism;
    }
    if (!this.m_SceneIsLit || !SceneView.DoesCameraDrawModeSupportHDR(this.m_CameraMode.drawMode))
    {
      this.m_Camera.allowHDR = false;
      this.m_Camera.depthTextureMode = DepthTextureMode.None;
      this.m_Camera.clearStencilAfterLightingPass = false;
    }
    else if ((UnityEngine.Object) mainCamera == (UnityEngine.Object) null)
    {
      this.m_Camera.allowHDR = false;
      this.m_Camera.depthTextureMode = DepthTextureMode.None;
      this.m_Camera.clearStencilAfterLightingPass = false;
    }
    else
    {
      this.m_Camera.allowHDR = mainCamera.allowHDR;
      this.m_Camera.depthTextureMode = mainCamera.depthTextureMode;
      this.m_Camera.clearStencilAfterLightingPass = mainCamera.clearStencilAfterLightingPass;
    }
  }

  private void SetupCamera()
  {
    this.m_Camera.backgroundColor = this.m_CameraMode.drawMode != DrawCameraMode.Overdraw ? (this.m_StageHandling == null ? (Color) SceneView.kSceneViewBackground : ScriptableSingleton<StageNavigationManager>.instance.currentStage.GetBackgroundColor()) : Color.black;
    if (UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
      this.UpdateImageEffects(this.m_CameraMode.drawMode == DrawCameraMode.Textured && this.sceneViewState.imageEffectsEnabled);
    EditorUtility.SetCameraAnimateMaterials(this.m_Camera, this.sceneViewState.alwaysRefreshEnabled);
    ParticleSystemEditorUtils.renderInSceneView = this.m_SceneViewState.particleSystemsEnabled;
    UnityEngine.VFX.VFXManager.renderInSceneView = this.m_SceneViewState.visualEffectGraphsEnabled;
    ScriptableSingleton<SceneVisibilityManager>.instance.enableSceneVisibility = this.m_SceneVisActive;
    this.m_Camera.renderCloudsInSceneView = this.m_SceneViewState.cloudsEnabled;
    this.ResetIfNaN();
    this.m_Camera.transform.rotation = this.GetTransformRotation();
    this.m_Camera.transform.position = this.GetTransformPosition();
    if (this.m_Viewpoint.hasActiveViewpoint)
      this.m_Viewpoint.ApplyCameraLensFromViewpoint((double) this.m_Ortho.Fade(this.perspectiveFov, 0.0f) > 3.0);
    else
      this.ApplyDefaultCameraLens();
    if (this.m_2DMode && (double) this.m_Camera.transform.position.z >= 0.0)
    {
      Vector3 position = this.m_Camera.transform.position;
      float num = (float) -(100.0 + (double) this.m_Camera.nearClipPlane + 0.009999999776482582);
      this.m_Camera.farClipPlane += position.z - num;
      position.z = num;
      this.m_Camera.transform.position = position;
    }
    this.m_Camera.renderingPath = SceneView.GetSceneViewRenderingPath();
    if (!this.CheckDrawModeForRenderingPath(this.m_CameraMode.drawMode))
      this.m_CameraMode = SceneView.GetBuiltinCameraMode(DrawCameraMode.Textured);
    this.UpdateSceneCameraSettings();
    if (this.m_CameraMode.drawMode == DrawCameraMode.Textured || this.m_CameraMode.drawMode == DrawCameraMode.TexturedWire || this.m_CameraMode.drawMode == DrawCameraMode.UserDefined)
    {
      Handles.EnableCameraFlares(this.m_Camera, this.sceneViewState.flaresEnabled);
      Handles.EnableCameraSkybox(this.m_Camera, this.sceneViewState.skyboxEnabled);
    }
    else
    {
      Handles.EnableCameraFlares(this.m_Camera, false);
      Handles.EnableCameraSkybox(this.m_Camera, false);
    }
    this.m_Light[0].transform.position = this.m_Camera.transform.position;
    this.m_Light[0].transform.rotation = this.m_Camera.transform.rotation;
    if (this.m_PlayAudio)
    {
      AudioUtil.SetListenerTransform(this.m_Camera.transform);
      AudioUtil.UpdateAudio();
    }
    if (!this.m_ViewIsLockedToObject || Selection.gameObjects.Length == 0)
      return;
    Bounds selectionBounds = InternalEditorUtility.CalculateSelectionBounds(false, Tools.pivotMode == PivotMode.Pivot);
    switch (this.draggingLocked)
    {
      case SceneView.DraggingLockedState.NotDragging:
        this.m_Position.value = selectionBounds.center;
        break;
      case SceneView.DraggingLockedState.LookAt:
        if (!this.m_Position.value.Equals(this.m_Position.target))
        {
          this.Frame(selectionBounds, EditorApplication.isPlaying);
          break;
        }
        this.draggingLocked = SceneView.DraggingLockedState.NotDragging;
        break;
    }
  }

  internal void ApplyDefaultCameraLens()
  {
    float aspectNeutralFOV = this.m_Ortho.Fade(this.perspectiveFov, 0.0f);
    if ((double) aspectNeutralFOV > 3.0)
    {
      this.m_Camera.orthographic = false;
      this.m_Camera.fieldOfView = this.GetVerticalFOV(aspectNeutralFOV);
    }
    else
    {
      this.m_Camera.orthographic = true;
      this.m_Camera.orthographicSize = this.GetVerticalOrthoSize();
    }
    if (this.cameraSettings.dynamicClip)
    {
      Vector2 dynamicClipPlanes = this.GetDynamicClipPlanes();
      this.m_Camera.nearClipPlane = dynamicClipPlanes.x;
      this.m_Camera.farClipPlane = dynamicClipPlanes.y;
    }
    else
    {
      this.m_Camera.nearClipPlane = this.m_CameraSettings.nearClip;
      this.m_Camera.farClipPlane = this.m_CameraSettings.farClip;
    }
    this.m_Camera.useOcclusionCulling = this.m_CameraSettings.occlusionCulling;
  }

  private void OnBecameVisible()
  {
    if ((EditorApplication.isPlaying || EditorApplication.isPaused) && this.m_Parent.vSyncEnabled)
      this.m_Parent.EnableVSync(false);
    EditorApplication.update += new EditorApplication.CallbackFunction(this.UpdateAnimatedMaterials);
  }

  private void OnBecameInvisible()
  {
    EditorApplication.update -= new EditorApplication.CallbackFunction(this.UpdateAnimatedMaterials);
  }

  private void UpdateAnimatedMaterials()
  {
    bool flag = false;
    if ((EditorApplication.isFocused || !SceneView.s_PreferenceIgnoreAlwaysRefreshWhenNotFocused.value) && this.m_lastRenderedTime + 0.032999999821186066 < EditorApplication.timeSinceStartup)
      flag = this.sceneViewState.alwaysRefreshEnabled;
    if (!(flag | LODUtility.IsLODAnimating(this.m_Camera)))
      return;
    this.m_lastRenderedTime = EditorApplication.timeSinceStartup;
    this.Repaint();
  }

  internal Quaternion cameraTargetRotation => this.m_Rotation.target;

  internal Vector3 cameraTargetPosition
  {
    get
    {
      return this.m_Position.target + this.m_Rotation.target * new Vector3(0.0f, 0.0f, -this.cameraDistance);
    }
  }

  internal float GetVerticalFOV(float aspectNeutralFOV, float multiplier = 1f)
  {
    if ((double) this.m_Camera.aspect < 1.0)
      multiplier /= this.m_Camera.aspect;
    return (float) ((double) Mathf.Atan(Mathf.Tan((float) ((double) aspectNeutralFOV * 0.5 * (Math.PI / 180.0))) * multiplier) * 2.0 * 57.295780181884766);
  }

  private float GetVerticalOrthoSize()
  {
    float size = this.size;
    if ((double) this.m_Camera.aspect < 1.0)
      size /= this.m_Camera.aspect;
    return size;
  }

  internal Quaternion GetTransformRotation()
  {
    return !this.m_2DMode || this.m_Rotation.isAnimating ? this.m_Rotation.value : Quaternion.identity;
  }

  internal Vector3 GetTransformPosition()
  {
    return this.m_Position.value + this.m_Camera.transform.rotation * new Vector3(0.0f, 0.0f, -this.cameraDistance);
  }

  /// <summary>
  ///   <para>Moves the Scene view to focus on a target.</para>
  /// </summary>
  /// <param name="point">The position in world space to frame.</param>
  /// <param name="direction">The direction that the Scene view should view the target point from.</param>
  /// <param name="newSize">The amount of camera zoom. Sets size.</param>
  /// <param name="ortho">Whether the camera focus is in orthographic mode (true) or perspective mode (false).</param>
  /// <param name="instant">Apply the movement immediately (true) or animate the transition (false).</param>
  public void LookAt(Vector3 point)
  {
    this.FixNegativeSize();
    this.m_Position.target = point;
  }

  /// <summary>
  ///   <para>Moves the Scene view to focus on a target.</para>
  /// </summary>
  /// <param name="point">The position in world space to frame.</param>
  /// <param name="direction">The direction that the Scene view should view the target point from.</param>
  /// <param name="newSize">The amount of camera zoom. Sets size.</param>
  /// <param name="ortho">Whether the camera focus is in orthographic mode (true) or perspective mode (false).</param>
  /// <param name="instant">Apply the movement immediately (true) or animate the transition (false).</param>
  public void LookAt(Vector3 point, Quaternion direction)
  {
    this.FixNegativeSize();
    this.m_Position.target = point;
    this.m_Rotation.target = direction;
    this.m_OrientationGizmo?.UpdateGizmoLabel(this, direction * Vector3.forward, this.m_Ortho.target);
  }

  /// <summary>
  ///   <para>.LookAt without animating the scene movement.</para>
  /// </summary>
  /// <param name="point">The position in world space to frame.</param>
  /// <param name="direction">The direction from which the Scene view should view the point.</param>
  /// <param name="newSize">The amount of camera zoom. Sets size.</param>
  public void LookAtDirect(Vector3 point, Quaternion direction)
  {
    this.FixNegativeSize();
    this.m_Position.value = point;
    this.m_Rotation.value = direction;
    this.m_OrientationGizmo?.UpdateGizmoLabel(this, direction * Vector3.forward, this.m_Ortho.target);
  }

  /// <summary>
  ///   <para>Moves the Scene view to focus on a target.</para>
  /// </summary>
  /// <param name="point">The position in world space to frame.</param>
  /// <param name="direction">The direction that the Scene view should view the target point from.</param>
  /// <param name="newSize">The amount of camera zoom. Sets size.</param>
  /// <param name="ortho">Whether the camera focus is in orthographic mode (true) or perspective mode (false).</param>
  /// <param name="instant">Apply the movement immediately (true) or animate the transition (false).</param>
  public void LookAt(Vector3 point, Quaternion direction, float newSize)
  {
    this.FixNegativeSize();
    this.m_Position.target = point;
    this.m_Rotation.target = direction;
    this.m_Size.target = SceneView.ValidateSceneSize(Mathf.Abs(newSize));
    this.m_OrientationGizmo?.UpdateGizmoLabel(this, direction * Vector3.forward, this.m_Ortho.target);
  }

  /// <summary>
  ///   <para>.LookAt without animating the scene movement.</para>
  /// </summary>
  /// <param name="point">The position in world space to frame.</param>
  /// <param name="direction">The direction from which the Scene view should view the point.</param>
  /// <param name="newSize">The amount of camera zoom. Sets size.</param>
  public void LookAtDirect(Vector3 point, Quaternion direction, float newSize)
  {
    this.FixNegativeSize();
    this.m_Position.value = point;
    this.m_Rotation.value = direction;
    this.size = Mathf.Abs(newSize);
    this.m_OrientationGizmo?.UpdateGizmoLabel(this, direction * Vector3.forward, this.m_Ortho.target);
  }

  /// <summary>
  ///   <para>Moves the Scene view to focus on a target.</para>
  /// </summary>
  /// <param name="point">The position in world space to frame.</param>
  /// <param name="direction">The direction that the Scene view should view the target point from.</param>
  /// <param name="newSize">The amount of camera zoom. Sets size.</param>
  /// <param name="ortho">Whether the camera focus is in orthographic mode (true) or perspective mode (false).</param>
  /// <param name="instant">Apply the movement immediately (true) or animate the transition (false).</param>
  public void LookAt(Vector3 point, Quaternion direction, float newSize, bool ortho)
  {
    this.LookAt(point, direction, newSize, ortho, false);
  }

  /// <summary>
  ///   <para>Moves the Scene view to focus on a target.</para>
  /// </summary>
  /// <param name="point">The position in world space to frame.</param>
  /// <param name="direction">The direction that the Scene view should view the target point from.</param>
  /// <param name="newSize">The amount of camera zoom. Sets size.</param>
  /// <param name="ortho">Whether the camera focus is in orthographic mode (true) or perspective mode (false).</param>
  /// <param name="instant">Apply the movement immediately (true) or animate the transition (false).</param>
  public void LookAt(
    Vector3 point,
    Quaternion direction,
    float newSize,
    bool ortho,
    bool instant)
  {
    this.m_SceneViewMotion.ResetMotion();
    this.FixNegativeSize();
    if (instant)
    {
      this.m_Position.value = point;
      this.m_Rotation.value = direction;
      this.size = Mathf.Abs(newSize);
      this.m_Ortho.value = ortho;
      this.draggingLocked = SceneView.DraggingLockedState.NotDragging;
    }
    else
    {
      this.m_Position.target = point;
      this.m_Rotation.target = direction;
      this.m_Size.target = SceneView.ValidateSceneSize(Mathf.Abs(newSize));
      this.m_Ortho.target = ortho;
    }
    this.m_OrientationGizmo?.UpdateGizmoLabel(this, direction * Vector3.forward, this.m_Ortho.target);
  }

  internal void UpdateOrientationGizmos()
  {
    this.m_OrientationGizmo?.UpdateGizmoLabel(this, this.rotation * Vector3.forward, this.m_Ortho.target);
  }

  private void DefaultHandles()
  {
    EditorGUI.BeginChangeCheck();
    bool flag1 = UnityEngine.Event.current.GetTypeForControl(GUIUtility.hotControl) == UnityEngine.EventType.MouseDrag;
    bool flag2 = UnityEngine.Event.current.GetTypeForControl(GUIUtility.hotControl) == UnityEngine.EventType.MouseUp;
    EditorToolManager.OnToolGUI((EditorWindow) this);
    if (((!EditorGUI.EndChangeCheck() ? 0 : (EditorApplication.isPlaying ? 1 : 0)) & (flag1 ? 1 : 0)) != 0)
      Physics2D.SetEditorDragMovement(true, Selection.gameObjects);
    if (!(EditorApplication.isPlaying & flag2))
      return;
    Physics2D.SetEditorDragMovement(false, Selection.gameObjects);
  }

  private void CleanupEditorDragFunctions()
  {
    this.m_DragEditorCache?.Dispose();
    this.m_DragEditorCache = (EditorCache) null;
  }

  private bool CallEditorDragFunctions(IList<UnityEngine.Object> dragAndDropObjects)
  {
    UnityEngine.Event current = UnityEngine.Event.current;
    SpriteUtility.OnSceneDrag(this);
    if (current.type == UnityEngine.EventType.Used || dragAndDropObjects.Count == 0)
      return true;
    if (this.m_DragEditorCache == null)
      this.m_DragEditorCache = new EditorCache(EditorFeatures.OnSceneDrag);
    bool flag = true;
    for (int index = dragAndDropObjects.Count - 1; index >= 0; --index)
    {
      if (!(dragAndDropObjects[index] == (UnityEngine.Object) null))
      {
        EditorWrapper editorWrapper = this.m_DragEditorCache[dragAndDropObjects[index]];
        if (editorWrapper == null)
          flag = false;
        else
          editorWrapper.OnSceneDrag(this, dragAndDropObjects.Count - 1 - index);
      }
    }
    return flag;
  }

  internal static bool CanDoDrag(ICollection<UnityEngine.Object> objects)
  {
    if (objects.Count < 2)
      return true;
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    foreach (UnityEngine.Object @object in (IEnumerable<UnityEngine.Object>) objects)
    {
      if (@object.GetType() == typeof (GameObject))
      {
        ++num1;
      }
      else
      {
        ++num2;
        if (@object.GetType() == typeof (Material))
          ++num3;
      }
      if (num1 > 0 && num2 > 0 || num3 > 1)
        return false;
    }
    return true;
  }

  internal void HandleDragging(UnityEngine.Event evt)
  {
    UnityEngine.Object[] objectReferences = DragAndDrop.objectReferences;
    switch (evt.type)
    {
      case UnityEngine.EventType.DragUpdated:
      case UnityEngine.EventType.DragPerform:
        if (evt.type == UnityEngine.EventType.DragPerform && GameObjectInspector.s_CyclicNestingDetected)
        {
          PrefabUtility.ShowCyclicNestingWarningDialog();
          break;
        }
        if (!SceneView.CanDoDrag((ICollection<UnityEngine.Object>) objectReferences))
        {
          DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
          break;
        }
        bool perform = evt.type == UnityEngine.EventType.DragPerform;
        GameObject dropUpon = (GameObject) null;
        Transform parentTransform = (Transform) null;
        bool flag1 = false;
        Vector3 normal;
        if (DragAndDrop.HasHandler(DragAndDropWindowTarget.sceneView))
        {
          this.PickObject(ref dropUpon, ref parentTransform);
          Vector3 position;
          Vector3 worldPosition = HandleUtility.PlaceObject(UnityEngine.Event.current.mousePosition, out position, out normal) ? position : this.pivot;
          DragAndDrop.visualMode = DragAndDrop.DropOnSceneWindow((UnityEngine.Object) dropUpon, worldPosition, UnityEngine.Event.current.mousePosition, parentTransform, perform);
          flag1 = DragAndDrop.visualMode != 0;
        }
        bool flag2 = false;
        if (!flag1)
          flag2 = this.CallEditorDragFunctions((IList<UnityEngine.Object>) objectReferences);
        if (evt.type == UnityEngine.EventType.Used | flag2)
          break;
        if (!flag1)
        {
          if ((UnityEngine.Object) dropUpon == (UnityEngine.Object) null || (UnityEngine.Object) parentTransform == (UnityEngine.Object) null)
            this.PickObject(ref dropUpon, ref parentTransform);
          Vector3 position;
          Vector3 worldPosition = HandleUtility.PlaceObject(UnityEngine.Event.current.mousePosition, out position, out normal) ? position : this.pivot;
          DragAndDrop.visualMode = InternalEditorUtility.SceneViewDrag((UnityEngine.Object) dropUpon, worldPosition, UnityEngine.Event.current.mousePosition, parentTransform, perform);
        }
        evt.Use();
        if (!perform || DragAndDrop.visualMode == DragAndDropVisualMode.None || DragAndDrop.visualMode == DragAndDropVisualMode.Rejected)
          break;
        DragAndDrop.AcceptDrag();
        GUIUtility.ExitGUI();
        break;
      case UnityEngine.EventType.DragExited:
        this.CallEditorDragFunctions((IList<UnityEngine.Object>) objectReferences);
        this.CleanupEditorDragFunctions();
        break;
    }
  }

  private void PickObject(ref GameObject dropUpon, ref Transform parentTransform)
  {
    Transform parentObjectIfSet = SceneView.GetDefaultParentObjectIfSet();
    parentTransform = (UnityEngine.Object) parentObjectIfSet != (UnityEngine.Object) null ? parentObjectIfSet : this.customParentForDraggedObjects;
    dropUpon = HandleUtility.PickGameObject(UnityEngine.Event.current.mousePosition, true);
  }

  private void CommandsGUI()
  {
    bool flag = UnityEngine.Event.current.type == UnityEngine.EventType.ExecuteCommand;
    switch (UnityEngine.Event.current.commandName)
    {
      case "Copy":
        if (flag)
          ClipboardUtility.CopyGO();
        UnityEngine.Event.current.Use();
        break;
      case "Cut":
        if (flag)
          ClipboardUtility.CutGO();
        UnityEngine.Event.current.Use();
        break;
      case "Delete":
      case "SoftDelete":
        if (flag)
          Unsupported.DeleteGameObjectSelection();
        UnityEngine.Event.current.Use();
        break;
      case "DeselectAll":
        if (flag)
          Selection.activeGameObject = (GameObject) null;
        UnityEngine.Event.current.Use();
        break;
      case "Duplicate":
        if (flag)
          ClipboardUtility.DuplicateGO(this.customParentForNewGameObjects);
        UnityEngine.Event.current.Use();
        break;
      case "Find":
        if (flag)
          this.FocusSearchField();
        UnityEngine.Event.current.Use();
        break;
      case "FrameSelected":
        if (flag && Tools.s_ButtonDown != 1)
          this.FrameSelected(false);
        UnityEngine.Event.current.Use();
        break;
      case "FrameSelectedWithLock":
        if (flag && Tools.s_ButtonDown != 1)
          this.FrameSelected(true);
        UnityEngine.Event.current.Use();
        break;
      case "InvertSelection":
        if (flag)
          Selection.objects = (UnityEngine.Object[]) ((IEnumerable<GameObject>) UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID)).Except<GameObject>((IEnumerable<GameObject>) Selection.gameObjects).Where<GameObject>(new Func<GameObject, bool>(ScriptableSingleton<SceneVisibilityManager>.instance.IsSelectable)).ToArray<GameObject>();
        UnityEngine.Event.current.Use();
        break;
      case "Paste":
        if (flag)
          ClipboardUtility.PasteGO(this.customParentForNewGameObjects);
        UnityEngine.Event.current.Use();
        break;
      case "SelectAll":
        if (flag)
        {
          GameObject[] objectsByType = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID);
          List<UnityEngine.Object> objectList = new List<UnityEngine.Object>(objectsByType.Length);
          foreach (GameObject go in objectsByType)
          {
            if (ScriptableSingleton<SceneVisibilityManager>.instance.IsSelectable(go))
              objectList.Add((UnityEngine.Object) go);
          }
          Selection.objects = objectList.ToArray();
        }
        UnityEngine.Event.current.Use();
        break;
      case "SelectChildren":
        if (flag)
        {
          List<GameObject> source = new List<GameObject>((IEnumerable<GameObject>) Selection.gameObjects);
          foreach (GameObject gameObject in Selection.gameObjects)
            source.AddRange(((IEnumerable<Transform>) gameObject.transform.GetComponentsInChildren<Transform>(true)).Select<Transform, GameObject>((Func<Transform, GameObject>) (t => t.gameObject)));
          Selection.objects = source.Distinct<GameObject>().Cast<UnityEngine.Object>().ToArray<UnityEngine.Object>();
        }
        UnityEngine.Event.current.Use();
        break;
      case "SelectPrefabRoot":
        if (flag)
        {
          List<GameObject> source = new List<GameObject>(Selection.gameObjects.Length);
          foreach (UnityEngine.Object gameObject in Selection.gameObjects)
          {
            GameObject prefabInstanceRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
            if ((UnityEngine.Object) prefabInstanceRoot != (UnityEngine.Object) null)
              source.Add(prefabInstanceRoot);
          }
          Selection.objects = source.Distinct<GameObject>().Cast<UnityEngine.Object>().ToArray<UnityEngine.Object>();
        }
        UnityEngine.Event.current.Use();
        break;
    }
    if (UnityEngine.Event.current.keyCode != KeyCode.Escape || !CutBoard.hasCutboardData)
      return;
    ClipboardUtility.ResetCutboardAndRepaintHierarchyWindows();
    this.Repaint();
  }

  /// <summary>
  ///   <para>Moves the Scene view to frame a transform.</para>
  /// </summary>
  /// <param name="t">The transform to frame in the Scene view.</param>
  public void AlignViewToObject(Transform t)
  {
    this.FixNegativeSize();
    this.size = 10f;
    this.LookAt(t.position + t.forward * this.CalcCameraDist(), t.rotation);
  }

  /// <summary>
  ///   <para>Aligns the current selection with the position and rotation of the Scene view camera.</para>
  /// </summary>
  public void AlignWithView()
  {
    this.FixNegativeSize();
    Vector3 position = this.camera.transform.position;
    Vector3 vector3 = position - Tools.handlePosition;
    float angle;
    Vector3 axis1;
    (Quaternion.Inverse(Selection.activeTransform.rotation) * this.camera.transform.rotation).ToAngleAxis(out angle, out axis1);
    Vector3 axis2 = Selection.activeTransform.TransformDirection(axis1);
    Undo.RecordObjects((UnityEngine.Object[]) Selection.transforms, "Align with view");
    foreach (Transform transform in Selection.transforms)
    {
      transform.position += vector3;
      transform.RotateAround(position, axis2, angle);
    }
  }

  /// <summary>
  ///   <para>Transforms all selected object to the scene pivot.</para>
  /// </summary>
  /// <param name="target">A transform to place at the scene pivot.</param>
  public void MoveToView()
  {
    this.FixNegativeSize();
    Vector3 vector3 = this.pivot - Tools.handlePosition;
    Undo.RecordObjects((UnityEngine.Object[]) Selection.transforms, "Move to view");
    foreach (Transform transform in Selection.transforms)
      transform.position += vector3;
  }

  /// <summary>
  ///   <para>Transforms all selected object to the scene pivot.</para>
  /// </summary>
  /// <param name="target">A transform to place at the scene pivot.</param>
  public void MoveToView(Transform target) => target.position = this.pivot;

  internal bool IsGameObjectInThisSceneView(GameObject gameObject)
  {
    return !((UnityEngine.Object) gameObject == (UnityEngine.Object) null) && !StageUtility.IsGizmoCulledBySceneCullingMasksOrFocusedScene(gameObject, this.camera);
  }

  /// <summary>
  ///   <para>Frame the object selection in the Scene view.</para>
  /// </summary>
  /// <param name="lockView">Whether the view should be locked to the selection.</param>
  /// <returns>
  ///   <para>Returns true if the current selection fits in the Scene view. Returns false otherwise.</para>
  /// </returns>
  public bool FrameSelected() => this.FrameSelected(false);

  /// <summary>
  ///   <para>Frame the object selection in the Scene view.</para>
  /// </summary>
  /// <param name="lockView">Whether the view should be locked to the selection.</param>
  /// <returns>
  ///   <para>Returns true if the current selection fits in the Scene view. Returns false otherwise.</para>
  /// </returns>
  public bool FrameSelected(bool lockView) => this.FrameSelected(lockView, false);

  public virtual bool FrameSelected(bool lockView, bool instant)
  {
    if (!this.IsGameObjectInThisSceneView(Selection.activeGameObject))
      return false;
    this.viewIsLockedToObject = lockView;
    this.FixNegativeSize();
    bounds = this.m_WasFocused ? new Bounds(Tools.handlePosition, Vector3.one) : InternalEditorUtility.CalculateSelectionBounds(false, Tools.pivotMode == PivotMode.Pivot, true);
    foreach (Editor activeEditor in SceneView.activeEditors)
    {
      System.Reflection.MethodInfo method1 = activeEditor.GetType().GetMethod("HasFrameBounds", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
      if (method1 != (System.Reflection.MethodInfo) null && method1.Invoke((object) activeEditor, (object[]) null) is bool flag && flag)
      {
        System.Reflection.MethodInfo method2 = activeEditor.GetType().GetMethod("OnGetFrameBounds", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (!(method2 != (System.Reflection.MethodInfo) null) || !(method2.Invoke((object) activeEditor, (object[]) null) is Bounds bounds))
          ;
      }
    }
    this.m_WasFocused = !this.m_WasFocused;
    return this.Frame(bounds, EditorApplication.isPlaying | instant);
  }

  /// <summary>
  ///   <para>Frames the given bounds in the Scene view.</para>
  /// </summary>
  /// <param name="bounds">The bounds to frame in the Scene view.</param>
  /// <param name="instant">Set to true to immediately frame the camera. Set to false to animate the action.</param>
  /// <returns>
  ///   <para>Returns true if the given bounds can be encompassed in the Scene view. Returns false otherwise.</para>
  /// </returns>
  public bool Frame(Bounds bounds, bool instant = true)
  {
    float num = bounds.extents.magnitude;
    if (float.IsInfinity(num))
      return false;
    if ((double) num < (double) Mathf.Epsilon)
      num = 10f;
    this.LookAt(bounds.center, this.m_Rotation.target, num, this.m_Ortho.value, instant);
    return true;
  }

  private void CreateSceneCameraAndLights()
  {
    GameObject objectWithHideFlags1 = EditorUtility.CreateGameObjectWithHideFlags("SceneCamera", HideFlags.HideAndDontSave, typeof (Camera));
    objectWithHideFlags1.AddComponent<FlareLayer>();
    this.m_Camera = objectWithHideFlags1.GetComponent<Camera>();
    this.m_Camera.enabled = false;
    this.m_Camera.cameraType = CameraType.SceneView;
    this.m_Camera.scene = this.m_CustomScene;
    if (this.m_OverrideSceneCullingMask > 0UL)
      this.m_Camera.overrideSceneCullingMask = this.m_OverrideSceneCullingMask;
    this.m_CustomLightsScene = EditorSceneManager.NewPreviewScene();
    this.m_CustomLightsScene.name = "CustomLightsScene-SceneView" + this.m_WindowGUID;
    for (int index = 0; index < 3; ++index)
    {
      GameObject objectWithHideFlags2 = EditorUtility.CreateGameObjectWithHideFlags("SceneLight", HideFlags.HideAndDontSave, typeof (Light));
      this.m_Light[index] = objectWithHideFlags2.GetComponent<Light>();
      this.m_Light[index].type = LightType.Directional;
      this.m_Light[index].intensity = 1f;
      this.m_Light[index].enabled = false;
      SceneManager.MoveGameObjectToScene(objectWithHideFlags2, this.m_CustomLightsScene);
    }
    this.m_Light[0].color = SceneView.kSceneViewFrontLight;
    this.m_Light[1].color = SceneView.kSceneViewUpLight - SceneView.kSceneViewMidLight;
    this.m_Light[1].transform.LookAt(Vector3.down);
    this.m_Light[1].renderMode = LightRenderMode.ForceVertex;
    this.m_Light[2].color = SceneView.kSceneViewDownLight - SceneView.kSceneViewMidLight;
    this.m_Light[2].transform.LookAt(Vector3.up);
    this.m_Light[2].renderMode = LightRenderMode.ForceVertex;
    HandleUtility.handleMaterial.SetColor("_SkyColor", SceneView.kSceneViewUpLight * 1.5f);
    HandleUtility.handleMaterial.SetColor("_GroundColor", SceneView.kSceneViewDownLight * 1.5f);
    HandleUtility.handleMaterial.SetColor("_Color", SceneView.kSceneViewFrontLight * 1.5f);
  }

  private void CallOnSceneGUI()
  {
    if (this.drawGizmos)
    {
      foreach (Editor activeEditor in SceneView.activeEditors)
      {
        if (EditorGUIUtility.IsGizmosAllowedForObject(activeEditor.target))
        {
          Action<Editor> action = SceneView.s_OnSceneGuiCache.GetAction(activeEditor.GetType());
          if (action != null)
          {
            System.Reflection.MethodInfo method = activeEditor.GetType().GetMethod("IsSceneGUIEnabled", BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, (Binder) null, System.Type.EmptyTypes, (ParameterModifier[]) null);
            if (!(method != (System.Reflection.MethodInfo) null) || (bool) method.Invoke((object) null, (object[]) null))
            {
              using (new EditorPerformanceMarker($"Editor.{activeEditor.GetType().Name}.OnSceneGUI", activeEditor.GetType()).Auto())
              {
                Editor.m_AllowMultiObjectAccess = true;
                bool editMultipleObjects = activeEditor.canEditMultipleObjects;
                for (int index = 0; index < activeEditor.targets.Length; ++index)
                {
                  this.ResetOnSceneGUIState();
                  activeEditor.referenceTargetIndex = index;
                  EditorGUI.BeginChangeCheck();
                  Editor.m_AllowMultiObjectAccess = !editMultipleObjects;
                  action(activeEditor);
                  Editor.m_AllowMultiObjectAccess = true;
                  if (EditorGUI.EndChangeCheck())
                    activeEditor.serializedObject.SetIsDifferentCacheDirty();
                }
                this.ResetOnSceneGUIState();
              }
            }
            if ((UnityEngine.Object) SceneView.s_CurrentDrawingSceneView == (UnityEngine.Object) null)
              GUIUtility.ExitGUI();
          }
        }
      }
      EditorToolManager.InvokeOnSceneGUICustomEditorTools();
    }
    if (SceneView.duringSceneGui == null)
      return;
    this.ResetOnSceneGUIState();
    if (SceneView.duringSceneGui != null)
      SceneView.duringSceneGui(this);
    if (SceneView.onSceneGUIDelegate != null)
      SceneView.onSceneGUIDelegate(this);
    this.ResetOnSceneGUIState();
  }

  private void ResetOnSceneGUIState()
  {
    Handles.ClearHandles();
    HandleUtility.s_CustomPickDistance = 5f;
    EditorGUIUtility.ResetGUIState();
    GUI.color = Color.white;
  }

  private void CallOnPreSceneGUI()
  {
    foreach (Editor activeEditor in SceneView.activeEditors)
    {
      Handles.ClearHandles();
      UnityEngine.Component target = activeEditor.target as UnityEngine.Component;
      if (!(bool) (UnityEngine.Object) target || target.gameObject.activeInHierarchy)
      {
        Action<Editor> action = SceneView.s_OnPreSceneGuiCache.GetAction(activeEditor.GetType());
        if (action != null)
        {
          using (new EditorPerformanceMarker($"Editor.{activeEditor.GetType().Name}.OnPreSceneGUI", activeEditor.GetType()).Auto())
          {
            bool editMultipleObjects = activeEditor.canEditMultipleObjects;
            Editor.m_AllowMultiObjectAccess = true;
            for (int index = 0; index < activeEditor.targets.Length; ++index)
            {
              activeEditor.referenceTargetIndex = index;
              Editor.m_AllowMultiObjectAccess = !editMultipleObjects;
              action(activeEditor);
              Editor.m_AllowMultiObjectAccess = true;
            }
          }
        }
      }
    }
    if (SceneView.beforeSceneGui != null)
    {
      Handles.ClearHandles();
      SceneView.beforeSceneGui(this);
    }
    Handles.ClearHandles();
  }

  internal static void ShowNotification(string notificationText)
  {
    UnityEngine.Object[] objectsOfTypeAll = Resources.FindObjectsOfTypeAll(typeof (SceneView));
    List<EditorWindow> editorWindowList = new List<EditorWindow>();
    foreach (SceneView sceneView in objectsOfTypeAll)
    {
      if (sceneView.m_Parent is DockArea)
      {
        DockArea parent = (DockArea) sceneView.m_Parent;
        if ((bool) (UnityEngine.Object) parent && (UnityEngine.Object) parent.actualView == (UnityEngine.Object) sceneView)
          editorWindowList.Add((EditorWindow) sceneView);
      }
    }
    if (editorWindowList.Count > 0)
    {
      foreach (EditorWindow editorWindow in editorWindowList)
        editorWindow.ShowNotification(GUIContent.Temp(notificationText));
    }
    else
      Debug.LogError((object) notificationText);
  }

  [RequiredByNativeCode]
  private static void ShowCompileErrorNotification()
  {
    SceneView.ShowNotification("All compiler errors have to be fixed before you can enter playmode!");
  }

  [RequiredByNativeCode]
  internal static void ShowSceneViewPlayModeSaveWarning()
  {
    PlayModeView editorWindowOfType = (PlayModeView) WindowLayout.FindEditorWindowOfType(typeof (PlayModeView));
    if ((UnityEngine.Object) editorWindowOfType != (UnityEngine.Object) null && editorWindowOfType.hasFocus)
      editorWindowOfType.ShowNotification(EditorGUIUtility.TrTextContent("You must exit play mode to save the scene!"));
    else
      SceneView.ShowNotification("You must exit play mode to save the scene!");
  }

  private void ResetToDefaults(EditorBehaviorMode behaviorMode)
  {
    if (behaviorMode == EditorBehaviorMode.Mode2D)
    {
      this.m_2DMode = true;
      this.m_Rotation.value = Quaternion.identity;
      this.m_Position.value = SceneView.kDefaultPivot;
      this.size = 10f;
      this.m_Ortho.value = true;
      this.m_LastSceneViewRotation = SceneView.kDefaultRotation;
      this.m_LastSceneViewOrtho = false;
    }
    else
    {
      this.m_2DMode = false;
      this.m_Rotation.value = SceneView.kDefaultRotation;
      this.m_Position.value = SceneView.kDefaultPivot;
      this.size = 10f;
      this.m_Ortho.value = false;
    }
  }

  internal void OnNewProjectLayoutWasCreated()
  {
    this.ResetToDefaults(EditorSettings.defaultBehaviorMode);
  }

  private void On2DModeChange()
  {
    if (this.m_2DMode)
    {
      this.lastSceneViewRotation = this.m_Rotation.target;
      this.m_LastSceneViewOrtho = this.orthographic;
      this.LookAt(this.pivot, Quaternion.identity, this.size, true);
      if (Tools.current == Tool.Move)
        Tools.current = Tool.Rect;
    }
    else
    {
      this.LookAt(this.pivot, this.lastSceneViewRotation, this.size, this.m_LastSceneViewOrtho);
      if (Tools.current == Tool.Rect)
        Tools.current = Tool.Move;
    }
    HandleUtility.ignoreRaySnapObjects = (Transform[]) null;
    Tools.vertexDragging = false;
    Tools.handleOffset = Vector3.zero;
  }

  /// <summary>
  ///   <para>Add a custom camera mode to the Scene view camera mode list.</para>
  /// </summary>
  /// <param name="name">The name for the new mode.</param>
  /// <param name="section">The section in which the new mode will be added. This can be an existing or new section.</param>
  /// <returns>
  ///   <para>A CameraMode with the provided name and section.</para>
  /// </returns>
  public static SceneView.CameraMode AddCameraMode(string name, string section)
  {
    if (string.IsNullOrEmpty(name))
      throw new ArgumentException("Cannot be null or empty", nameof (name));
    SceneView.CameraMode cameraMode = !string.IsNullOrEmpty(section) ? new SceneView.CameraMode(DrawCameraMode.UserDefined, name, section) : throw new ArgumentException("Cannot be null or empty", nameof (section));
    if (SceneView.userDefinedModes.Contains(cameraMode))
      throw new InvalidOperationException($"A mode named {name} already exists in section {section}");
    SceneView.userDefinedModes.Add(cameraMode);
    return cameraMode;
  }

  private static bool IsValidCameraMode(SceneView.CameraMode cameraMode)
  {
    foreach (object obj in Enum.GetValues(typeof (DrawCameraMode)))
    {
      if (SceneRenderModeWindow.DrawCameraModeExists((DrawCameraMode) obj) && cameraMode == SceneView.GetBuiltinCameraMode((DrawCameraMode) obj))
        return true;
    }
    foreach (SceneView.CameraMode userDefinedMode in SceneView.userDefinedModes)
    {
      if (userDefinedMode == cameraMode)
        return true;
    }
    return false;
  }

  /// <summary>
  ///   <para>Remove all user-defined camera modes.</para>
  /// </summary>
  public static void ClearUserDefinedCameraModes() => SceneView.userDefinedModes.Clear();

  /// <summary>
  ///   <para>Gets the built-in CameraMode that matches the specified DrawCameraMode.</para>
  /// </summary>
  /// <param name="mode">The DrawCameraMode to match.</param>
  /// <returns>
  ///   <para>Returns a built-in CameraMode.</para>
  /// </returns>
  public static SceneView.CameraMode GetBuiltinCameraMode(DrawCameraMode mode)
  {
    return SceneRenderModeWindow.GetBuiltinCameraMode(mode);
  }

  internal void RebuildBreadcrumbBar()
  {
    if (!this.SupportsStageHandling())
      return;
    this.m_StageHandling.RebuildBreadcrumbBar();
  }

  internal static void RebuildBreadcrumbBarInAll()
  {
    foreach (SceneView sceneView in SceneView.s_SceneViews)
      sceneView.RebuildBreadcrumbBar();
  }

  internal void ResetGridPivot() => this.sceneViewGrids.SetAllGridsPivot(Vector3.zero);

  private void CopyLastActiveSceneViewSettings()
  {
    SceneView lastActiveSceneView = SceneView.lastActiveSceneView;
    this.m_CameraMode = lastActiveSceneView.m_CameraMode;
    this.sceneLighting = lastActiveSceneView.sceneLighting;
    this.m_SceneViewState = new SceneView.SceneViewState(SceneView.lastActiveSceneView.m_SceneViewState);
    this.m_CameraSettings = new SceneView.CameraSettings(SceneView.lastActiveSceneView.m_CameraSettings);
    this.m_2DMode = lastActiveSceneView.m_2DMode;
    this.pivot = lastActiveSceneView.pivot;
    if (!this.m_2DMode)
      this.rotation = lastActiveSceneView.rotation;
    this.size = lastActiveSceneView.size;
    this.m_Ortho.value = lastActiveSceneView.orthographic;
    if (this.m_Grid == null)
      this.m_Grid = new SceneViewGrid();
    this.m_Grid.showGrid = lastActiveSceneView.showGrid;
  }

  internal void SetOverlayVisible(string id, bool show)
  {
    Overlay match;
    if (!this.TryGetOverlay(id, out match))
      return;
    match.displayed = show;
  }

  /// <summary>
  ///   <para>Describes a built-in Scene view mode.</para>
  /// </summary>
  [Serializable]
  public struct CameraMode
  {
    /// <summary>
    ///   <para>The CameraDrawMode associated with the CameraMode.</para>
    /// </summary>
    public DrawCameraMode drawMode;
    /// <summary>
    ///   <para>The name of the CameraMode.</para>
    /// </summary>
    public string name;
    /// <summary>
    ///   <para>The section in the toolbar drop-down that this CameraMode belongs to.</para>
    /// </summary>
    public string section;
    internal bool show;

    internal CameraMode(DrawCameraMode drawMode, string name, string section, bool show = true)
    {
      this.drawMode = drawMode;
      this.name = name;
      this.section = section;
      this.show = show;
    }

    public static bool operator ==(SceneView.CameraMode a, SceneView.CameraMode z)
    {
      return a.drawMode == z.drawMode && a.name == z.name && a.section == z.section;
    }

    public static bool operator !=(SceneView.CameraMode a, SceneView.CameraMode z) => !(a == z);

    /// <summary>
    ///   <para>Compares this CameraMode object against a specified CameraMode object.</para>
    /// </summary>
    /// <param name="otherObject">The CameraMode to compare.</param>
    /// <returns>
    ///   <para>Returns true if the CameraMode objects are equal. Returns false otherwise.</para>
    /// </returns>
    public override bool Equals(object otherObject)
    {
      return otherObject != null && otherObject is SceneView.CameraMode cameraMode && this == cameraMode;
    }

    public override int GetHashCode() => this.ToString().GetHashCode();

    /// <summary>
    ///   <para>Gets a string summary of this CameraMode.</para>
    /// </summary>
    public override string ToString()
    {
      return UnityString.Format("{0}||{1}||{2}", (object) this.drawMode, (object) this.name, (object) this.section);
    }
  }

  /// <summary>
  ///   <para>A collection of graphic settings for this SceneView. All graphic settings are boolean.</para>
  /// </summary>
  [Serializable]
  public class SceneViewState
  {
    [SerializeField]
    [FormerlySerializedAs("showMaterialUpdate")]
    private bool m_AlwaysRefresh;
    /// <summary>
    ///   <para>Whether fog rendering is enabled in this SceneView.</para>
    /// </summary>
    public bool showFog = true;
    /// <summary>
    ///   <para>Whether the skybox rendering is enabled in this SceneView.</para>
    /// </summary>
    public bool showSkybox = true;
    /// <summary>
    ///   <para>Whether lens flare rendering is enabled in this SceneView.</para>
    /// </summary>
    public bool showFlares = true;
    /// <summary>
    ///   <para>Whether image effects (post processing) rendering is enabled in this SceneView.</para>
    /// </summary>
    public bool showImageEffects = true;
    /// <summary>
    ///   <para>Whether particle systems rendering is enabled in this SceneView.</para>
    /// </summary>
    public bool showParticleSystems = true;
    /// <summary>
    ///   <para>Whether visual effect graphs rendering is enabled in this SceneView.</para>
    /// </summary>
    public bool showVisualEffectGraphs = true;
    private bool m_ShowClouds = true;
    [SerializeField]
    private bool m_FxEnabled = true;

    /// <summary>
    ///   <para>Whether animated materials rendering is enabled in this SceneView.</para>
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Obsolete msg (UnityUpgradable) -> alwaysRefresh")]
    public bool showMaterialUpdate
    {
      get => this.m_AlwaysRefresh;
      set => this.m_AlwaysRefresh = value;
    }

    /// <summary>
    ///   <para>Whether to redraw SceneView at a fixed interval.</para>
    /// </summary>
    public bool alwaysRefresh
    {
      get => this.m_AlwaysRefresh;
      set => this.m_AlwaysRefresh = value;
    }

    /// <summary>
    ///   <para>Whether the clouds are rendered in this SceneView.</para>
    /// </summary>
    public bool showClouds
    {
      get => this.m_ShowClouds;
      set => this.m_ShowClouds = value;
    }

    /// <summary>
    ///   <para>Whether fog renders in this SceneView.</para>
    /// </summary>
    public bool fogEnabled => this.fxEnabled && this.showFog;

    /// <summary>
    ///   <para>Whether animated materials render in this SceneView.</para>
    /// </summary>
    [Obsolete("Obsolete msg (UnityUpgradable) -> alwaysRefreshEnabled")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool materialUpdateEnabled => this.alwaysRefreshEnabled;

    /// <summary>
    ///   <para>Whether to redraw SceneView at a fixed interval.</para>
    /// </summary>
    public bool alwaysRefreshEnabled => this.fxEnabled && this.alwaysRefresh;

    /// <summary>
    ///   <para>Whether the skybox renders in this SceneView.</para>
    /// </summary>
    public bool skyboxEnabled => this.fxEnabled && this.showSkybox;

    /// <summary>
    ///   <para>Whether the clouds are rendered in this SceneView.</para>
    /// </summary>
    public bool cloudsEnabled => this.fxEnabled && this.showClouds;

    /// <summary>
    ///   <para>Whether lens flares render in this SceneView.</para>
    /// </summary>
    public bool flaresEnabled => this.fxEnabled && this.showFlares;

    /// <summary>
    ///   <para>Whether image effects (post processing) render in this SceneView.</para>
    /// </summary>
    public bool imageEffectsEnabled => this.fxEnabled && this.showImageEffects;

    /// <summary>
    ///   <para>Whether particle systems render in this SceneView.</para>
    /// </summary>
    public bool particleSystemsEnabled => this.fxEnabled && this.showParticleSystems;

    /// <summary>
    ///   <para>Whether visual effect graphs render in this SceneView.</para>
    /// </summary>
    public bool visualEffectGraphsEnabled => this.fxEnabled && this.showVisualEffectGraphs;

    internal event Action<bool> fxEnableChanged;

    /// <summary>
    ///   <para>Creates a new SceneViewState with either default values or values from another SceneViewState.</para>
    /// </summary>
    /// <param name="other">Specify a SceneViewState to copy values from when creating the new SceneViewState. If this param is not specified, the new SceneViewState is created with default values.</param>
    public SceneViewState()
    {
    }

    public SceneViewState(SceneView.SceneViewState other)
    {
      this.fxEnabled = other.fxEnabled;
      this.showFog = other.showFog;
      this.alwaysRefresh = other.alwaysRefresh;
      this.showSkybox = other.showSkybox;
      this.showClouds = other.showClouds;
      this.showFlares = other.showFlares;
      this.showImageEffects = other.showImageEffects;
      this.showParticleSystems = other.showParticleSystems;
      this.showVisualEffectGraphs = other.showVisualEffectGraphs;
    }

    [Obsolete("IsAllOn() has been deprecated. Use allEnabled instead (UnityUpgradable) -> allEnabled")]
    public bool IsAllOn() => this.allEnabled;

    /// <summary>
    ///   <para>Whether all graphic settings are enabled for this SceneViewState.</para>
    /// </summary>
    public bool allEnabled
    {
      get
      {
        bool allEnabled = this.showFog && this.alwaysRefresh && this.showSkybox && this.showClouds && this.showFlares && this.showImageEffects && this.showParticleSystems;
        if (UnityEngine.VFX.VFXManager.activateVFX)
          allEnabled = allEnabled && this.showVisualEffectGraphs;
        return allEnabled;
      }
    }

    [Obsolete("Toggle() has been deprecated. Use SetAllEnabled() instead (UnityUpgradable) -> SetAllEnabled(*)")]
    public void Toggle(bool value) => this.SetAllEnabled(value);

    /// <summary>
    ///   <para>Sets all graphic settings, for this SceneViewState, to either true or false.</para>
    /// </summary>
    /// <param name="value">The new value for all graphic settings in this SceneViewState. Possible values are true or false.</param>
    public void SetAllEnabled(bool value)
    {
      this.showFog = value;
      this.alwaysRefresh = value;
      this.showSkybox = value;
      this.showClouds = value;
      this.showFlares = value;
      this.showImageEffects = value;
      this.showParticleSystems = value;
      this.showVisualEffectGraphs = value;
    }

    /// <summary>
    ///   <para>Whether to render (when enabled) effects in this SceneView.</para>
    /// </summary>
    public bool fxEnabled
    {
      get => this.m_FxEnabled;
      set
      {
        if (this.m_FxEnabled == value)
          return;
        this.m_FxEnabled = value;
        Action<bool> fxEnableChanged = this.fxEnableChanged;
        if (fxEnableChanged == null)
          return;
        fxEnableChanged(value);
      }
    }
  }

  [Obsolete("OnSceneFunc() has been deprecated. Use System.Action instead.")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public delegate void OnSceneFunc(SceneView sceneView);

  /// <summary>
  ///   <para>Use this class to set SceneView Camera properties.</para>
  /// </summary>
  [Serializable]
  public class CameraSettings
  {
    private const float defaultEasingDuration = 0.4f;
    internal const float kAbsoluteSpeedMin = 0.0001f;
    internal const float kAbsoluteSpeedMax = 10000f;
    private const float kAbsoluteEasingDurationMin = 0.1f;
    private const float kAbsoluteEasingDurationMax = 2f;
    [SerializeField]
    private float m_Speed;
    [SerializeField]
    private float m_SpeedNormalized;
    [SerializeField]
    private float m_SpeedMin;
    [SerializeField]
    private float m_SpeedMax;
    [SerializeField]
    private bool m_EasingEnabled;
    [SerializeField]
    private float m_EasingDuration;
    [SerializeField]
    private bool m_AccelerationEnabled;
    [SerializeField]
    private float m_FieldOfViewHorizontalOrVertical;
    [SerializeField]
    private float m_NearClip;
    [SerializeField]
    private float m_FarClip;
    [SerializeField]
    private bool m_DynamicClip;
    [SerializeField]
    private bool m_OcclusionCulling;

    /// <summary>
    ///   <para>Create a new CameraSettings object.</para>
    /// </summary>
    public CameraSettings()
    {
      this.m_Speed = 1f;
      this.m_SpeedNormalized = 0.5f;
      this.m_SpeedMin = 0.01f;
      this.m_SpeedMax = 2f;
      this.m_EasingEnabled = true;
      this.m_EasingDuration = 0.4f;
      this.fieldOfView = 60f;
      this.m_DynamicClip = true;
      this.m_OcclusionCulling = false;
      this.m_NearClip = 0.03f;
      this.m_FarClip = 10000f;
      this.m_AccelerationEnabled = true;
    }

    internal CameraSettings(SceneView.CameraSettings other)
    {
      this.m_Speed = other.m_Speed;
      this.m_SpeedNormalized = other.m_SpeedNormalized;
      this.m_SpeedMin = other.m_SpeedMin;
      this.m_SpeedMax = other.m_SpeedMax;
      this.m_EasingEnabled = other.m_EasingEnabled;
      this.m_EasingDuration = other.m_EasingDuration;
      this.fieldOfView = other.fieldOfView;
      this.m_DynamicClip = other.m_DynamicClip;
      this.m_OcclusionCulling = other.m_OcclusionCulling;
      this.m_NearClip = other.m_NearClip;
      this.m_FarClip = other.m_FarClip;
      this.m_AccelerationEnabled = other.m_AccelerationEnabled;
    }

    /// <summary>
    ///   <para>The speed of the SceneView Camera.</para>
    /// </summary>
    public float speed
    {
      get => this.m_Speed;
      set => this.speedNormalized = Mathf.InverseLerp(this.m_SpeedMin, this.m_SpeedMax, value);
    }

    /// <summary>
    ///   <para>The normalized speed of the SceneView Camera, relative to the current minimum/maximum range. Valid values are between [0, 1].</para>
    /// </summary>
    public float speedNormalized
    {
      get => this.m_SpeedNormalized;
      set
      {
        this.m_SpeedNormalized = Mathf.Clamp01(value);
        this.m_Speed = Mathf.Lerp(this.m_SpeedMin, this.m_SpeedMax, this.m_SpeedNormalized);
      }
    }

    /// <summary>
    ///   <para>The minimum speed of the SceneView Camera. Valid values are between [0.0001, 9999].</para>
    /// </summary>
    public float speedMin
    {
      get => this.m_SpeedMin;
      set => this.SetSpeedMinMax(value, this.m_SpeedMax);
    }

    /// <summary>
    ///   <para>The maximum speed of the SceneView Camera. Valid values are between [0.0002, 10000].</para>
    /// </summary>
    public float speedMax
    {
      get => this.m_SpeedMax;
      set => this.SetSpeedMinMax(this.m_SpeedMin, value);
    }

    /// <summary>
    ///   <para>Enables Camera movement easing in the SceneView. This makes the Camera ease in when it starts moving, and ease out when it stops.</para>
    /// </summary>
    public bool easingEnabled
    {
      get => this.m_EasingEnabled;
      set => this.m_EasingEnabled = value;
    }

    /// <summary>
    ///   <para>How long it takes for the speed of the SceneView Camera to accelerate to its initial full speed. Measured in seconds. Valid values are between [0.1, 2].</para>
    /// </summary>
    public float easingDuration
    {
      get => this.m_EasingDuration;
      set => this.m_EasingDuration = (float) Math.Round((double) Mathf.Clamp(value, 0.1f, 2f), 1);
    }

    /// <summary>
    ///   <para>Enables Camera movement acceleration in the SceneView. This makes the Camera accelerate for the duration of movement.</para>
    /// </summary>
    public bool accelerationEnabled
    {
      get => this.m_AccelerationEnabled;
      set => this.m_AccelerationEnabled = value;
    }

    internal float RoundSpeedToNearestSignificantDecimal(float value)
    {
      if ((double) value <= (double) this.speedMin)
        return this.speedMin;
      if ((double) value >= (double) this.speedMax)
        return this.speedMax;
      float num = this.speedMax - this.speedMin;
      int a = (double) this.speedMin < 1.0 / 1000.0 ? 4 : ((double) this.speedMin < 0.009999999776482582 ? 3 : ((double) this.speedMin < 0.10000000149011612 ? 2 : ((double) this.speedMin < 1.0 ? 1 : 0)));
      int b = (double) num < 0.10000000149011612 ? 3 : ((double) num < 1.0 ? 2 : ((double) num < 10.0 ? 1 : 0));
      return (float) Math.Round((double) value, Mathf.Max(a, b));
    }

    internal void SetSpeedMinMax(float min, float max)
    {
      float num = (double) min < 1.0 / 1000.0 ? 0.0001f : ((double) min < 0.009999999776482582 ? 1f / 1000f : ((double) min < 0.10000000149011612 ? 0.01f : ((double) min < 1.0 ? 0.1f : 1f)));
      min = Mathf.Clamp(min, 0.0001f, 10000f - num);
      max = Mathf.Clamp(max, min + num, 10000f);
      this.m_SpeedMin = min;
      this.m_SpeedMax = max;
      this.speed = this.m_Speed;
    }

    internal void SetClipPlanes(float near, float far)
    {
      this.farClip = Mathf.Clamp(far, float.Epsilon, 1.844674E+19f);
      this.nearClip = Mathf.Max(1E-05f, near);
    }

    /// <summary>
    ///   <para>The height of the view angle for the SceneView Camera. Measured in degrees vertically, or along the local Y axis.</para>
    /// </summary>
    public float fieldOfView
    {
      get => this.m_FieldOfViewHorizontalOrVertical;
      set => this.m_FieldOfViewHorizontalOrVertical = value;
    }

    /// <summary>
    ///   <para>The closest point to the SceneView Camera where drawing occurs. The valid minimum value is 0.01.</para>
    /// </summary>
    public float nearClip
    {
      get => this.m_NearClip;
      set => this.m_NearClip = value;
    }

    /// <summary>
    ///   <para>The furthest point from the SceneView Camera that drawing occurs. The valid minimum value is 0.02.</para>
    /// </summary>
    public float farClip
    {
      get => this.m_FarClip;
      set => this.m_FarClip = value;
    }

    /// <summary>
    ///   <para>When enabled, the SceneView Camera's near and far clipping planes are calculated relative to the viewport size of the Scene. When disabled, nearClip and farClip are used instead.</para>
    /// </summary>
    public bool dynamicClip
    {
      get => this.m_DynamicClip;
      set => this.m_DynamicClip = value;
    }

    /// <summary>
    ///   <para>Enables occlusion culling in the SceneView. This prevents Unity from rendering GameObjects that the Camera cannot see because they are hidden by other GameObjects.</para>
    /// </summary>
    public bool occlusionCulling
    {
      get => this.m_OcclusionCulling;
      set => this.m_OcclusionCulling = value;
    }
  }

  private struct CursorRect(Rect rect, MouseCursor cursor)
  {
    public Rect rect = rect;
    public MouseCursor cursor = cursor;
  }

  internal static class Styles
  {
    public static GUIContent toolsContent = EditorGUIUtility.TrIconContent("SceneViewTools", "Hide or show the Component Editor Tools panel in the Scene view.");
    public static GUIContent lighting = EditorGUIUtility.TrIconContent("SceneviewLighting", "When toggled on, the Scene lighting is used. When toggled off, a light attached to the Scene view camera is used.");
    public static GUIContent fx = EditorGUIUtility.TrIconContent("SceneviewFx", "Toggle skybox, fog, and various other effects.");
    public static GUIContent audioPlayContent = EditorGUIUtility.TrIconContent("SceneviewAudio", "Toggle audio on or off.");
    public static GUIContent gizmosContent = EditorGUIUtility.TrTextContent("Gizmos", "Toggle visibility of all Gizmos in the Scene view");
    public static GUIContent gizmosDropDownContent = EditorGUIUtility.TrTextContent("", "Toggle the visibility of different Gizmos in the Scene view.");
    public static GUIContent mode2DContent = EditorGUIUtility.TrIconContent("SceneView2D", "When toggled on, the Scene is in 2D view. When toggled off, the Scene is in 3D view.");
    public static GUIContent gridXToolbarContent = EditorGUIUtility.TrIconContent("GridAxisX", "Toggle the visibility of the grid");
    public static GUIContent gridYToolbarContent = EditorGUIUtility.TrIconContent("GridAxisY", "Toggle the visibility of the grid");
    public static GUIContent gridZToolbarContent = EditorGUIUtility.TrIconContent("GridAxisZ", "Toggle the visibility of the grid");
    public static GUIContent metalFrameCaptureContent = EditorGUIUtility.TrIconContent("FrameCapture", "Capture the current view and open in Xcode frame debugger");
    public static GUIContent sceneVisToolbarButtonContent = EditorGUIUtility.TrIconContent("SceneViewVisibility", "Number of hidden objects, click to toggle scene visibility");
    public static GUIStyle gizmoButtonStyle;
    public static GUIContent sceneViewCameraContent = EditorGUIUtility.TrIconContent("SceneViewCamera", "Settings for the Scene view camera.");

    static Styles() => SceneView.Styles.gizmoButtonStyle = (GUIStyle) "GV Gizmo DropDown";
  }

  internal enum DraggingLockedState
  {
    NotDragging,
    Dragging,
    LookAt,
  }

  private struct EditorActionCache(string methodName)
  {
    private readonly Dictionary<System.Type, Action<Editor>> m_Cache = new Dictionary<System.Type, Action<Editor>>();
    private readonly string m_MethodName = methodName;

    public Action<Editor> GetAction(System.Type type)
    {
      Action<Editor> action1;
      if (!this.m_Cache.TryGetValue(type, out action1))
      {
        System.Reflection.MethodInfo method = type.GetMethod(this.m_MethodName, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, (Binder) null, System.Type.EmptyTypes, (ParameterModifier[]) null);
        if (method == (System.Reflection.MethodInfo) null)
        {
          this.m_Cache[type] = (Action<Editor>) null;
        }
        else
        {
          ParameterExpression parameterExpression = System.Linq.Expressions.Expression.Parameter(typeof (Editor), "a");
          Dictionary<System.Type, Action<Editor>> cache = this.m_Cache;
          System.Type key = type;
          MethodCallExpression body = System.Linq.Expressions.Expression.Call((System.Linq.Expressions.Expression) System.Linq.Expressions.Expression.Convert((System.Linq.Expressions.Expression) parameterExpression, type), method);
          ParameterExpression[] parameterExpressionArray = new ParameterExpression[1]
          {
            parameterExpression
          };
          Action<Editor> action2;
          Action<Editor> action3 = action2 = System.Linq.Expressions.Expression.Lambda<Action<Editor>>((System.Linq.Expressions.Expression) body, parameterExpressionArray).Compile();
          cache[key] = action2;
          action1 = action3;
        }
      }
      return action1;
    }
  }

  [Overlay(typeof (SceneView), "Scene View/Scene Visibility", "Isolation View", false)]
  internal class SceneViewIsolationOverlay : TransientSceneViewOverlay
  {
    public const string k_OverlayID = "Scene View/Scene Visibility";
    private const string k_DisplayName = "Isolation View";
    private bool m_ShouldDisplay;

    public override bool visible => this.m_ShouldDisplay;

    public override void OnCreated()
    {
      SceneVisibilityManager.currentStageIsIsolated += new Action<bool>(this.CurrentStageIsolated);
      this.CurrentStageIsolated(SceneVisibilityState.isolation);
    }

    public override void OnWillBeDestroyed()
    {
      SceneVisibilityManager.currentStageIsIsolated -= new Action<bool>(this.CurrentStageIsolated);
    }

    private void CurrentStageIsolated(bool isolated) => this.m_ShouldDisplay = isolated;

    public override void OnGUI()
    {
      if (!GUILayout.Button(SceneView.SceneViewIsolationOverlay.Styles.isolationModeExitButton, GUILayout.MinWidth(120f)))
        return;
      ScriptableSingleton<SceneVisibilityManager>.instance.ExitIsolation();
    }

    internal static class Styles
    {
      public static GUIContent isolationModeExitButton = EditorGUIUtility.TrTextContent("Exit", "Exit isolation mode");
    }
  }
}
