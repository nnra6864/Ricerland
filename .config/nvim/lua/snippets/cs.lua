-- Source: https://github.com/Saladin1812/dotfiles/blob/main/.config/nvim/lua/snippets/cs.lua
-- Unity snippets for LuaSnip (converted from VS Code)
local ls = require 'luasnip'
local s = ls.snippet
local t = ls.text_node
local i = ls.insert_node
local f = ls.function_node

-- Helper to get filename without extension
local function filename_base() return vim.fn.expand '%:t:r' end

ls.add_snippets('cs', {
  -- Unity MonoBehaviour class template.
  s('MonoBehaviour', {
    t { 'using UnityEngine;', '', 'public class ' },
    f(filename_base, {}),
    t { ' : MonoBehaviour {', '	' },
    i(0),
    t { '', '}' },
  }),

  -- Unity StateMachineBehaviour class template.
  s('StateMachineBehaviour', {
    t 'using UnityEngine;',
    t '',
    t 'public class ',
    f(filename_base, {}),
    t ' : StateMachineBehaviour {',
    t '	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {',
    t '		',
    i(0),
    t '	}',
    t '}',
  }),

  -- Unity NetworkBehaviour class template.
  s('NetworkBehaviour', {
    t 'using UnityEngine;',
    t 'using UnityEngine.Networking;',
    t '',
    t 'public class ',
    f(filename_base, {}),
    t ' : NetworkBehaviour {',
    t '	',
    i(0),
    t '}',
  }),

  -- Unity ScriptableObject class template.
  s('ScriptableObject', {
    t 'using UnityEngine;',
    t '',
    t '[CreateAssetMenu(fileName = "',
    i(1, 'fileName'),
    t '", menuName = "',
    i(2, 'menuName'),
    t '", order = ',
    i(3, '0'),
    t ')]',
    t 'public class ',
    f(filename_base, {}),
    t ' : ScriptableObject {',
    t '	',
    i(0),
    t '}',
  }),

  -- Unity Editor class template.
  s('Editor', {
    t 'using UnityEngine;',
    t 'using UnityEditor;',
    t '',
    t '[CustomEditor(typeof(',
    i(1, 'Component'),
    t '))]',
    t 'public class ',
    f(filename_base, {}),
    t ' : Editor {',
    t '	public override void OnInspectorGUI() {',
    t '		base.OnInspectorGUI();',
    t '		',
    i(0),
    t '	}',
    t '}',
  }),

  -- Unity Editor class template with a ReorderableList.
  s('EditorWithReorderableList', {
    t 'using UnityEngine;',
    t 'using UnityEditor;',
    t 'using UnityEditorInternal;',
    t '',
    t '[CustomEditor(typeof(',
    i(1, 'Component'),
    t '))]',
    t 'public class ',
    f(filename_base, {}),
    t ' : Editor {',
    t '	private SerializedProperty _property;',
    t '	private ReorderableList _list;',
    t '',
    t '	private void OnEnable() {',
    t '		_property = serializedObject.FindProperty("',
    i(2, 'propertyName'),
    t '");',
    t '		_list = new ReorderableList(serializedObject, _property, true, true, true, true) {',
    t '			drawHeaderCallback = DrawListHeader,',
    t '			drawElementCallback = DrawListElement',
    t '		};',
    t '	}',
    t '',
    t '	private void DrawListHeader(Rect rect) {',
    t '		GUI.Label(rect, "',
    i(2, 'propertyName'),
    t '");',
    t '	}',
    t '',
    t '	private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused) {',
    t '		var item = _property.GetArrayElementAtIndex(index);',
    t '		EditorGUI.PropertyField(rect, item);',
    t '		',
    i(0),
    t '	}',
    t '',
    t '	public override void OnInspectorGUI() {',
    t '		serializedObject.Update();',
    t '		EditorGUILayout.Space();',
    t '		_list.DoLayoutList();',
    t '		serializedObject.ApplyModifiedProperties();',
    t '	}',
    t '}',
  }),

  -- Unity EditorWindow class template.
  s('EditorWindow', {
    t 'using UnityEngine;',
    t 'using UnityEditor;',
    t '',
    t 'public class ',
    f(filename_base, {}),
    t ' : EditorWindow {',
    t '	[MenuItem("',
    i(1, 'Window/My Window'),
    t '")]',
    t '	private static void ShowWindow() {',
    t '		var window = GetWindow<',
    f(filename_base, {}),
    t '>();',
    t '		window.titleContent = new GUIContent("',
    i(2, 'My Window'),
    t '");',
    t '		window.Show();',
    t '	}',
    t '',
    t '	private void OnGUI() {',
    t '		',
    i(0),
    t '	}',
    t '}',
  }),

  -- Unity PropertyDrawer class template.
  s('PropertyDrawer', {
    t 'using UnityEngine;',
    t 'using UnityEditor;',
    t '',
    t '[CustomPropertyDrawer(typeof(',
    i(1, 'PropertyType'),
    t '))]',
    t 'public class ',
    f(filename_base, {}),
    t ': PropertyDrawer {',
    t '	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {',
    t '		',
    i(0),
    t '	}',
    t '}',
  }),

  -- Unity ScriptableWizard class template.
  s('ScriptableWizard', {
    t 'using UnityEngine;',
    t 'using UnityEditor;',
    t '',
    t 'public class ',
    f(filename_base, {}),
    t ': ScriptableWizard {',
    t '	[MenuItem("',
    i(1, 'Tools/My Wizard'),
    t '")]',
    t '	private static void MenuEntryCall() {',
    t '		DisplayWizard<',
    f(filename_base, {}),
    t '>',
    i(2, 'Title'),
    t ');',
    t '	}',
    t '',
    t '	private void OnWizardCreate() {',
    t '		',
    i(0),
    t '	}',
    t '}',
  }),

  -- Creates a standard class.
  s('class', {
    t 'public class ',
    f(filename_base, {}),
    t ' {',
    t '	',
    i(0),
    t '}',
  }),

  -- Creates a standard interface.
  s('interface', {
    t 'public interface ',
    f(filename_base, {}),
    t ' {',
    t '	',
    i(0),
    t '}',
  }),

  -- Awake is called when the script instance is being loaded.
  s('Awake', {
    t 'private void Awake() {',
    t '	',
    i(0),
    t '}',
  }),

  -- This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
  s('FixedUpdate', {
    t 'private void FixedUpdate() {',
    t '	',
    i(0),
    t '}',
  }),

  -- LateUpdate is called every frame, if the Behaviour is enabled. It is called after all Update functions have been called.
  s('LateUpdate', {
    t 'private void LateUpdate() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Callback for setting up animation IK (inverse kinematics).
  s('OnAnimatorIK', {
    t 'private void OnAnimatorIK(int layerIndex) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Callback for processing animation movements for modifying root motion.
  s('OnAnimatorMove', {
    t 'private void OnAnimatorMove() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Callback sent to all game objects when the player gets or loses focus.
  s('OnApplicationFocus', {
    t 'private void OnApplicationFocus(bool focusStatus) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Callback sent to all game objects when the player pauses.
  s('OnApplicationPause', {
    t 'private void OnApplicationPause(bool pauseStatus) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Callback sent to all game objects before the application is quit.
  s('OnApplicationQuit', {
    t 'private void OnApplicationQuit() {',
    t '	',
    i(0),
    t '}',
  }),

  -- If OnAudioFilterRead is implemented, Unity will insert a custom filter into the audio DSP chain.
  s('OnAudioFilterRead', {
    t 'private void OnAudioFilterRead(float[] data, int channels) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnBecameInvisible is called when the renderer is no longer visible by any camera.
  s('OnBecameInvisible', {
    t 'private void OnBecameInvisible() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnBecameVisible is called when the renderer became visible by any camera.
  s('OnBecameVisible', {
    t 'private void OnBecameVisible() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
  s('OnCollisionEnter', {
    t 'private void OnCollisionEnter(Collision other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Sent when an incoming collider makes contact with this object's collider (2D physics only).
  s('OnCollisionEnter2D', {
    t 'private void OnCollisionEnter2D(Collision2D other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
  s('OnCollisionExit', {
    t 'private void OnCollisionExit(Collision other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Sent when a collider on another object stops touching this object's collider (2D physics only).
  s('OnCollisionExit2D', {
    t 'private void OnCollisionExit2D(Collision2D other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider.
  s('OnCollisionStay', {
    t 'private void OnCollisionStay(Collision other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Sent each frame where a collider on another object is touching this object's collider (2D physics only).
  s('OnCollisionStay2D', {
    t 'private void OnCollisionStay2D(Collision2D other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on the client when you have successfully connected to a server.
  s('OnConnectedToServer', {
    t 'private void OnConnectedToServer() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnControllerColliderHit is called when the controller hits a collider while performing a Move.
  s('OnControllerColliderHit', {
    t 'private void OnControllerColliderHit(ControllerColliderHit hit) {',
    t '	',
    i(0),
    t '}',
  }),

  -- This function is called when the MonoBehaviour will be destroyed.
  s('OnDestroy', {
    t 'private void OnDestroy() {',
    t '	',
    i(0),
    t '}',
  }),

  -- This function is called when the behaviour becomes disabled or inactive.
  s('OnDisable', {
    t 'private void OnDisable() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on the client when the connection was lost or you disconnected from the server.
  s('OnDisconnectedFromServer', {
    t 'private void OnDisconnectedFromServer(NetworkDisconnection info) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Callback to draw gizmos that are pickable and always drawn.
  s('OnDrawGizmos', {
    t 'private void OnDrawGizmos() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Callback to draw gizmos only if the object is selected.
  s('OnDrawGizmosSelected', {
    t 'private void OnDrawGizmosSelected() {',
    t '	',
    i(0),
    t '}',
  }),

  -- This function is called when the object becomes enabled and active.
  s('OnEnable', {
    t 'private void OnEnable() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on the client when a connection attempt fails for some reason.
  s('OnFailedToConnect', {
    t 'private void OnFailedToConnect(NetworkConnectionError error) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on clients or servers when there is a problem connecting to the MasterServer.
  s('OnFailedToConnectToMasterServer', {
    t 'private void OnFailedToConnectToMasterServer(NetworkConnectionError error) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnGUI is called for rendering and handling GUI events.
  s('OnGUI', {
    t 'private void OnGUI() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called when a joint attached to the same game object broke.
  s('OnJointBreak', {
    t 'private void OnJointBreak(float breakForce) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called when a Joint2D attached to the same game object breaks.
  s('OnJointBreak2D', {
    t 'private void OnJointBreak2D(Joint2D brokenJoint) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on clients or servers when reporting events from the MasterServer.
  s('OnMasterServerEvent', {
    t 'private void OnMasterServerEvent(MasterServerEvent msEvent) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider.
  s('OnMouseDown', {
    t 'private void OnMouseDown() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.
  s('OnMouseDrag', {
    t 'private void OnMouseDrag() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called when the mouse enters the GUIElement or Collider.
  s('OnMouseEnter', {
    t 'private void OnMouseEnter() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called when the mouse is not any longer over the GUIElement or Collider.
  s('OnMouseExit', {
    t 'private void OnMouseExit() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called every frame while the mouse is over the GUIElement or Collider.
  s('OnMouseOver', {
    t 'private void OnMouseOver() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnMouseUp is called when the user has released the mouse button.
  s('OnMouseUp', {
    t 'private void OnMouseUp() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnMouseUpAsButton is only called when the mouse is released over the same GUIElement or Collider as it was pressed.
  s('OnMouseUpAsButton', {
    t 'private void OnMouseUpAsButton() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on objects which have been network instantiated with Network.Instantiate.
  s('OnNetworkInstantiate', {
    t 'private void OnNetworkInstantiate(NetworkMessageInfo info) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnParticleCollision is called when a particle hits a collider.
  s('OnParticleCollision', {
    t 'private void OnParticleCollision(GameObject other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnParticleSystemStopped is called when all particles in the system have died, and no new particles will be born. New particles cease to be created either after Stop is called, or when the duration property of a non-looping system has been exceeded.
  s('OnParticleSystemStopped', {
    t 'private void OnParticleSystemStopped() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnParticleTrigger is called when any particles in a particle system meet the conditions in the trigger module.
  s('OnParticleTrigger', {
    t 'private void OnParticleTrigger() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on the server whenever a new player has successfully connected.
  s('OnPlayerConnected', {
    t 'private void OnPlayerConnected(NetworkPlayer player) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on the server whenever a player disconnected from the server.
  s('OnPlayerDisconnected', {
    t 'private void OnPlayerDisconnected(NetworkPlayer player) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnPostRender is called after a camera finishes rendering the scene.
  s('OnPostRender', {
    t 'private void OnPostRender() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnPreCull is called before a camera culls the scene.
  s('OnPreCull', {
    t 'private void OnPreCull() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnPreRender is called before a camera starts rendering the scene.
  s('OnPreRender', {
    t 'private void OnPreRender() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnRenderImage is called after all rendering is complete to render image.
  s('OnRenderImage', {
    t 'private void OnRenderImage(RenderTexture src, RenderTexture dest) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnRenderObject is called after camera has rendered the scene.
  s('OnRenderObject', {
    t 'private void OnRenderObject() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Used to customize synchronization of variables in a script watched by a network view.
  s('OnSerializeNetworkView', {
    t 'private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on the server whenever a Network.InitializeServer was invoked and has completed.
  s('OnServerInitialized', {
    t 'private void OnServerInitialized() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called when the list of children of the transform of the GameObject has changed.
  s('OnTransformChildrenChanged', {
    t 'private void OnTransformChildrenChanged() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called when the parent property of the transform of the GameObject has changed.
  s('OnTransformParentChanged', {
    t 'private void OnTransformParentChanged() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnTriggerEnter is called when the Collider other enters the trigger.
  s('OnTriggerEnter', {
    t 'private void OnTriggerEnter(Collider other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Sent when another object enters a trigger collider attached to this object (2D physics only).
  s('OnTriggerEnter2D', {
    t 'private void OnTriggerEnter2D(Collider2D other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnTriggerExit is called when the Collider other has stopped touching the trigger.
  s('OnTriggerExit', {
    t 'private void OnTriggerExit(Collider other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Sent when another object leaves a trigger collider attached to this object (2D physics only).
  s('OnTriggerExit2D', {
    t 'private void OnTriggerExit2D(Collider2D other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnTriggerStay is called once per frame for every Collider other that is touching the trigger.
  s('OnTriggerStay', {
    t 'private void OnTriggerStay(Collider other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Sent each frame where another object is within a trigger collider attached to this object (2D physics only).
  s('OnTriggerStay2D', {
    t 'private void OnTriggerStay2D(Collider2D other) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called when the script is loaded or a value is changed in the inspector (Called in the editor only).
  s('OnValidate', {
    t 'private void OnValidate() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnWillRenderObject is called for each camera if the object is visible.
  s('OnWillRenderObject', {
    t 'private void OnWillRenderObject() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time.
  s('Reset', {
    t 'private void Reset() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
  s('Start', {
    t 'private void Start() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Update is called every frame, if the MonoBehaviour is enabled.
  s('Update', {
    t 'private void Update() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on the first Update frame when a statemachine evaluate this state.
  s('OnStateEnter', {
    t 'private void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on the last update frame when a statemachine evaluate this state.
  s('OnStateExit', {
    t 'private void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called right after MonoBehaviour.OnAnimatorIK.
  s('OnStateIK', {
    t 'private void OnStateIK(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called right after MonoBehaviour.OnAnimatorMove.
  s('OnStateMove', {
    t 'private void OnStateMove(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called on the first Update frame when a statemachine evaluate this state.
  s('OnStateUpdate', {
    t 'private void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {',
    t '	',
    i(0),
    t '}',
  }),

  -- Enables the Editor to handle an event in the scene view.
  s('OnSceneGUI', {
    t 'private void OnSceneGUI() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called when the window gets keyboard focus.
  s('OnFocus', {
    t 'private void OnFocus() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Handler for message that is sent when an object or group of objects in the hierarchy changes.
  s('OnHierarchyChange', {
    t 'private void OnHierarchyChange() {',
    t '	',
    i(0),
    t '}',
  }),

  -- OnInspectorUpdate is called at 10 frames per second to give the inspector a chance to update.
  s('OnInspectorUpdate', {
    t 'private void OnInspectorUpdate() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called when the window loses keyboard focus.
  s('OnLostFocus', {
    t 'private void OnLostFocus() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Handler for message that is sent whenever the state of the project changes.
  s('OnProjectChange', {
    t 'private void OnProjectChange() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Called whenever the selection has changed.
  s('OnSelectionChange', {
    t 'private void OnSelectionChange() {',
    t '	',
    i(0),
    t '}',
  }),

  -- This is called when the user clicks on the Create button.
  s('OnWizardCreate', {
    t 'private void OnWizardCreate() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Allows you to provide an action when the user clicks on the other button.
  s('OnWizardOtherButton', {
    t 'private void OnWizardOtherButton() {',
    t '	',
    i(0),
    t '}',
  }),

  -- This is called when the wizard is opened or whenever the user changes something in the wizard.
  s('OnWizardUpdate', {
    t 'private void OnWizardUpdate() {',
    t '	',
    i(0),
    t '}',
  }),

  -- Logs message to the Unity Console.
  s('Log', {
    t 'Debug.Log(',
    i(0),
    t ');',
  }),

  -- A variant of Debug.Log that logs an error message to the console.
  s('LogError', {
    t 'Debug.LogError(',
    i(0),
    t ');',
  }),

  -- A variant of Debug.Log that logs a warning message to the console.
  s('LogWarning', {
    t 'Debug.LogWarning(',
    i(0),
    t ');',
  }),

  -- A variant of Debug.Log that logs an error message from an exception to the console.
  s('LogException', {
    t 'Debug.LogException(',
    i(0),
    t ');',
  }),

  -- Logs a formatted message to the Unity Console.
  s('LogFormat', {
    t 'Debug.LogFormat(',
    i(0),
    t ');',
  }),

  -- Logs a formatted error message to the Unity console.
  s('LogErrorFormat', {
    t 'Debug.LogErrorFormat(',
    i(0),
    t ');',
  }),

  -- Logs a formatted warning message to the Unity Console.
  s('LogWarningFormat', {
    t 'Debug.LogWarningFormat(',
    i(0),
    t ');',
  }),

  -- Draws a line between specified start and end points.
  s('DrawLine', {
    t 'Debug.DrawLine(Vector3 ',
    i(1, 'start'),
    t ', Vector3 ',
    i(2, 'end'),
    t ', Color ',
    i(3, 'color'),
    t ' = Color.white, float ',
    i(4, 'duration'),
    t ' = 0.0f, bool ',
    i(5, 'depthTest'),
    t ' = true);',
  }),

  -- Draws a line from start to start + dir in world coordinates.
  s('DrawRay', {
    t 'Debug.DrawRay(Vector3 ',
    i(1, 'start'),
    t ', Vector3 ',
    i(2, 'dir'),
    t ', Color ',
    i(3, 'color'),
    t ' = Color.white, float ',
    i(4, 'duration'),
    t ' = 0.0f, bool ',
    i(5, 'depthTest'),
    t ' = true);',
  }),

  -- Force Unity to serialize a private field.
  s('sfield', {
    t '[SerializeField] private ',
    i(0),
    t ';',
  }),

  -- Automatically adds required components as dependencies.
  s('RequireComponent', {
    t '[RequireComponent(typeof(',
    i(0),
    t '))]',
  }),
})
return ls
