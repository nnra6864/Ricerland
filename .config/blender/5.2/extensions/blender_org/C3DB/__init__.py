# SPDX-License-Identifier: GPL-3.0-or-later

# https://extensions.blender.org/add-ons/c3db/

import bpy
from bpy_extras.keyconfig_utils import addon_keymap_register, addon_keymap_unregister
from typing import Tuple, Optional, List
from bl_ui.space_view3d import VIEW3D_PT_view3d_cursor
from bpy.props import BoolProperty, StringProperty

# Core data contracts and utilities (decoupled state + behavior helpers)

# Guard flag to prevent recursive update/operator calls
C3DB_INDEX_UPDATING = False


def _ensure_valid_index(index: int, size: int) -> bool:
    return 0 <= index < size


def _copy_rotation_from_cursor(scene_cursor, target_cursor3d) -> None:
    target_cursor3d.rotation_mode = scene_cursor.rotation_mode
    if scene_cursor.rotation_mode == "QUATERNION":
        target_cursor3d.rotation_quaternion = scene_cursor.rotation_quaternion
    elif scene_cursor.rotation_mode == "AXIS_ANGLE":
        target_cursor3d.rotation_axis_angle = scene_cursor.rotation_axis_angle
    else:
        target_cursor3d.rotation_euler = scene_cursor.rotation_euler


def _apply_rotation_to_cursor(src_cursor3d, scene_cursor) -> None:
    scene_cursor.rotation_mode = src_cursor3d.rotation_mode
    if src_cursor3d.rotation_mode == "QUATERNION":
        scene_cursor.rotation_quaternion = src_cursor3d.rotation_quaternion
    elif src_cursor3d.rotation_mode == "AXIS_ANGLE":
        scene_cursor.rotation_axis_angle = src_cursor3d.rotation_axis_angle
    else:
        scene_cursor.rotation_euler = src_cursor3d.rotation_euler


def _make_unique_name(base: str, existing_names: List[str]) -> str:
    if base not in existing_names:
        return base
    counter = 1
    while True:
        candidate = f"{base}.{counter:03d}"
        if candidate not in existing_names:
            return candidate
        counter += 1


def _c3db_cursor3d_needs_update(cursor, cursor3d) -> bool:
    if not all(a == b for a, b in zip(cursor.location, cursor3d.location)):
        return True
    if cursor3d.rotation_mode != cursor.rotation_mode:
        return True
    if cursor3d.rotation_mode == "QUATERNION" and not all(
        a == b for a, b in zip(cursor.rotation_quaternion, cursor3d.rotation_quaternion)
    ):
        return True
    if cursor3d.rotation_mode == "AXIS_ANGLE" and not all(
        a == b for a, b in zip(cursor.rotation_axis_angle, cursor3d.rotation_axis_angle)
    ):
        return True
    if cursor3d.rotation_mode not in ("QUATERNION", "AXIS_ANGLE") and not all(
        a == b for a, b in zip(cursor.rotation_euler, cursor3d.rotation_euler)
    ):
        return True
    return False


# Data model


class C3DB_PG_properties(bpy.types.PropertyGroup):
    selected: BoolProperty(name="Selected", default=False)
    location: bpy.props.FloatVectorProperty(name="Location")
    rotation_quaternion: bpy.props.FloatVectorProperty(name="Rotation Quaternion", size=4)
    rotation_axis_angle: bpy.props.FloatVectorProperty(name="Rotation Axis Angle", size=4)
    rotation_euler: bpy.props.FloatVectorProperty(name="Rotation euler", size=3)
    rotation_mode: bpy.props.StringProperty(name="Rotation Mode")


# Domain services (logic + navigation, independent of the UI)


class C3DB_CursorService:
    @staticmethod
    def save_current_cursor(scene, name: Optional[str] = None) -> int:
        coll = scene.C3DB_3Dcursors_collection
        names = [cursor3d.name for cursor3d in coll]
        chosen_name = None
        if name:
            chosen_name = _make_unique_name(name, names)
        else:
            chosen_name = _make_unique_name("3D Cursor", names)
        cursor3d = coll.add()
        cursor3d.name = chosen_name
        cur = scene.cursor
        cursor3d.location = cur.location[:]
        _copy_rotation_from_cursor(cur, cursor3d)
        index = len(coll) - 1
        scene.C3DB_3Dcursors_index = index
        scene.C3DB_loaded_index = index
        return index

    @staticmethod
    def restore_index(scene, index: Optional[int] = None, center: bool = False) -> Optional[int]:
        coll = scene.C3DB_3Dcursors_collection
        if not coll:
            return None
        if index is None or not _ensure_valid_index(index, len(coll)):
            index = scene.C3DB_3Dcursors_index
        if not _ensure_valid_index(index, len(coll)):
            return None
        scene.C3DB_3Dcursors_index = index
        cursor3d = coll[index]
        cur = scene.cursor
        cur.location = cursor3d.location[:]
        _apply_rotation_to_cursor(cursor3d, cur)
        prefs = _get_prefs(bpy.context)
        if center or (prefs.auto_center_view_checkbox and not prefs.auto_load):
            try:
                bpy.ops.view3d.view_center_cursor()
            except RuntimeError:
                pass
        scene.C3DB_loaded_index = scene.C3DB_3Dcursors_index
        return index

    @staticmethod
    def get_selected_indices(scene):
        return [idx for idx, cursor3d in enumerate(scene.C3DB_3Dcursors_collection) if cursor3d.selected]

    @staticmethod
    def delete_selected(scene) -> Optional[int]:
        selected_indices = C3DB_CursorService.get_selected_indices(scene)
        if not selected_indices:
            return C3DB_CursorService.remove_at_index(scene, scene.C3DB_3Dcursors_index)

        loaded_index = scene.C3DB_loaded_index
        deleted_before_loaded = sum(1 for idx in selected_indices if idx < loaded_index)
        loaded_deleted = loaded_index in selected_indices

        for index in reversed(selected_indices):
            scene.C3DB_3Dcursors_collection.remove(index)

        if scene.C3DB_3Dcursors_collection:
            scene.C3DB_3Dcursors_index = min(
                scene.C3DB_3Dcursors_index, len(scene.C3DB_3Dcursors_collection) - 1
            )
        else:
            scene.C3DB_3Dcursors_index = 0

        if loaded_deleted:
            scene.C3DB_loaded_index = -1
        else:
            scene.C3DB_loaded_index = max(-1, loaded_index - deleted_before_loaded)

        return scene.C3DB_3Dcursors_index

    @staticmethod
    def convert_selected_to_empty(context) -> int:
        scene = context.scene
        selected_indices = C3DB_CursorService.get_selected_indices(scene)
        if not selected_indices:
            if not _ensure_valid_index(scene.C3DB_3Dcursors_index, len(scene.C3DB_3Dcursors_collection)):
                return 0
            selected_indices = [scene.C3DB_3Dcursors_index]
        count = 0
        for index in selected_indices:
            C3DB_CursorService.convert_cursor3d_to_empty(context, scene.C3DB_3Dcursors_collection[index])
            count += 1
        return count

    @staticmethod
    def update_selected(scene) -> Optional[int]:
        coll = scene.C3DB_3Dcursors_collection
        index = scene.C3DB_3Dcursors_index
        if not _ensure_valid_index(index, len(coll)):
            return None
        cursor3d = coll[index]
        cur = scene.cursor
        cursor3d.location = cur.location[:]
        _copy_rotation_from_cursor(cur, cursor3d)
        return index

    @staticmethod
    def delete_all(scene) -> None:
        scene.C3DB_3Dcursors_collection.clear()

    @staticmethod
    def remove_at_index(scene, index: int) -> Optional[int]:
        coll = scene.C3DB_3Dcursors_collection
        if not _ensure_valid_index(index, len(coll)):
            return None
        loaded_index = scene.C3DB_loaded_index
        coll.remove(index)
        new_index = min(index, len(coll) - 1) if coll else 0
        scene.C3DB_3Dcursors_index = new_index

        if loaded_index == index:
            scene.C3DB_loaded_index = -1
        elif loaded_index > index:
            scene.C3DB_loaded_index = loaded_index - 1

        return new_index

    @staticmethod
    def move_up(scene) -> Optional[int]:
        index = scene.C3DB_3Dcursors_index
        coll = scene.C3DB_3Dcursors_collection
        if index <= 0:
            return None
        loaded = scene.C3DB_loaded_index
        coll.move(index, index - 1)
        scene.C3DB_3Dcursors_index = index - 1
        if loaded == index:
            scene.C3DB_loaded_index = index - 1
        elif loaded == index - 1:
            scene.C3DB_loaded_index = index
        return scene.C3DB_3Dcursors_index

    @staticmethod
    def move_down(scene) -> Optional[int]:
        index = scene.C3DB_3Dcursors_index
        coll = scene.C3DB_3Dcursors_collection
        if index >= len(coll) - 1:
            return None
        loaded = scene.C3DB_loaded_index
        coll.move(index, index + 1)
        scene.C3DB_3Dcursors_index = index + 1
        if loaded == index:
            scene.C3DB_loaded_index = index + 1
        elif loaded == index + 1:
            scene.C3DB_loaded_index = index
        return scene.C3DB_3Dcursors_index

    @staticmethod
    def convert_cursor3d_to_empty(context, cursor3d) -> bpy.types.Object:
        empty = bpy.data.objects.new(cursor3d.name, None)
        empty.empty_display_type = "PLAIN_AXES"
        empty.location = cursor3d.location[:]
        empty.rotation_mode = cursor3d.rotation_mode
        if cursor3d.rotation_mode == "QUATERNION":
            empty.rotation_quaternion = cursor3d.rotation_quaternion
        elif cursor3d.rotation_mode == "AXIS_ANGLE":
            empty.rotation_axis_angle = cursor3d.rotation_axis_angle
        else:
            empty.rotation_euler = cursor3d.rotation_euler
        context.collection.objects.link(empty)
        return empty

    @staticmethod
    def convert_all_to_empties(context) -> int:
        count = 0
        for cursor3d in context.scene.C3DB_3Dcursors_collection:
            C3DB_CursorService.convert_cursor3d_to_empty(context, cursor3d)
            count += 1
        return count


class C3DB_NavigationService:
    @staticmethod
    def next(scene) -> Optional[int]:
        count = len(scene.C3DB_3Dcursors_collection)
        if count <= 1:
            return None
        scene.C3DB_3Dcursors_index = (scene.C3DB_3Dcursors_index + 1) % count
        return scene.C3DB_3Dcursors_index

    @staticmethod
    def previous(scene) -> Optional[int]:
        count = len(scene.C3DB_3Dcursors_collection)
        if count <= 1:
            return None
        scene.C3DB_3Dcursors_index = (scene.C3DB_3Dcursors_index - 1) % count
        return scene.C3DB_3Dcursors_index


# Menus, lists, panels (UI)


class C3DB_MT_menu_specials(bpy.types.Menu):
    bl_label = "3D Cursor Specials"
    bl_idname = "C3DB_MT_menu_specials"

    def draw(self, context):
        layout = self.layout
        prefs = _get_prefs(context)
        layout.prop(prefs, "auto_load", text="Auto Load", icon="AUTO")
        layout.separator()
        layout.operator("view3d.c3db_restore_previous", icon="TRIA_UP_BAR")
        layout.operator("view3d.c3db_restore_next", icon="TRIA_DOWN_BAR")
        layout.separator()
        index = context.scene.C3DB_3Dcursors_index
        cursors = context.scene.C3DB_3Dcursors_collection
        if cursors and index < len(cursors):
            cursor3d = cursors[index]
            cursor = context.scene.cursor
            if _c3db_cursor3d_needs_update(cursor, cursor3d):
                layout.operator("view3d.c3db_update", icon="FILE_REFRESH")
        layout.separator()
        layout.operator("view3d.c3db_delete_all", icon="TRASH")
        layout.separator()
        layout.operator("view3d.c3db_convert_selected_to_empty", icon="OUTLINER")
        layout.operator("view3d.c3db_convert_all_to_empties")
        layout.separator()
        layout.prop(
            prefs,
            "show_icons",
            icon="CHECKBOX_HLT" if prefs.show_icons else "CHECKBOX_DEHLT",
        )
        layout.prop(
            prefs,
            "show_selection_checkboxes",
            icon="CHECKBOX_HLT" if prefs.show_selection_checkboxes else "CHECKBOX_DEHLT",
        )
        layout.operator("view3d.c3db_show_popup", icon="CURSOR")
        layout.separator()
        layout.operator("c3db.open_preferences", icon="PREFERENCES")


class C3DB_UL_list(bpy.types.UIList):
    def draw_item(self, context, layout, data, cursor3d, icon, active_data, active_propname, index):
        row = layout.row(align=True)
        prefs = _get_prefs(context)
        if prefs.show_selection_checkboxes:
            icon_name = "CHECKBOX_HLT" if cursor3d.selected else "CHECKBOX_DEHLT"
            row.prop(cursor3d, "selected", text="", emboss=False, icon=icon_name)
        sub = row.row()
        prefs_view = context.preferences.view
        if prefs_view.show_developer_ui:
            sub.alignment = "LEFT"
            sub.label(text=str(index))

        row.prop(cursor3d, "name", text="", emboss=False)
        sub = row.row()
        scene = context.scene
        if index == scene.C3DB_3Dcursors_index:
            cursor = scene.cursor
            if _c3db_cursor3d_needs_update(cursor, cursor3d):
                sub.alignment = "RIGHT"
                sub.operator("view3d.c3db_update", text="", icon="FILE_REFRESH", emboss=False)

        prefs = _get_prefs(context)
        if prefs.auto_load:
            is_loaded = index == scene.C3DB_3Dcursors_index
        else:
            is_loaded = index == scene.C3DB_loaded_index

        if is_loaded and not prefs.show_icons:
            sub.label(icon="DOT")

        if prefs.show_icons:
            # Hide the screen icon when auto-center is on
            if not prefs.auto_center_view_checkbox:
                icon_screen = "FULLSCREEN_EXIT"
                op_screen = sub.operator("c3db.restore_screen", text="", icon=icon_screen, emboss=False)
                op_screen.index = index
                op_screen.center = True
            icon_eye = "HIDE_OFF" if is_loaded else "HIDE_ON"
            op_eye = sub.operator("c3db.restore_eye", text="", icon=icon_eye, emboss=False)
            op_eye.index = index


class C3DB_PT_panel_base:
    """Base class for panel drawing logic"""

    def _draw_panel_content(self, context):
        layout = self.layout
        row = layout.row()
        col = row.column()
        col.template_list(
            "C3DB_UL_list",
            "",
            context.scene,
            "C3DB_3Dcursors_collection",
            context.scene,
            "C3DB_3Dcursors_index",
            rows=4,
        )
        col = row.column(align=True)
        col.operator("view3d.c3db_save", icon="ADD", text="")
        col.operator("view3d.c3db_remove_from_list", icon="REMOVE", text="")
        col.separator()
        col.menu("C3DB_MT_menu_specials", icon="DOWNARROW_HLT", text="")
        col.separator()
        col.operator("view3d.c3db_move_up_list", icon="TRIA_UP", text="")
        col.operator("view3d.c3db_move_down_list", icon="TRIA_DOWN", text="")
        prefs = _get_prefs(context)
        if not prefs.show_icons and not prefs.auto_load:
            row = layout.row()
            prefs_view = context.preferences.view
            if prefs_view.show_developer_ui:
                op = row.operator("view3d.c3db_restore", text="Load")
                op.index = -1
            else:
                row.operator("view3d.c3db_restore_selected", text="Load")
            row.operator("view3d.view_center_cursor", text="Center View")
            row.prop(prefs, "auto_center_view_checkbox", text="")


class C3DB_PT_panel(C3DB_PT_panel_base, bpy.types.Panel):
    bl_label = "3D Cursors Briefcase"
    bl_idname = "C3DB_PT_panel"
    bl_space_type = "VIEW_3D"
    bl_region_type = "UI"
    bl_category = "View"
    bl_parent_id = "VIEW3D_PT_view3d_cursor"

    @classmethod
    def poll(cls, context):
        prefs = _get_prefs(context)
        # Only show this panel when show_in_own_tab is False
        return not prefs.show_in_own_tab

    def draw(self, context):
        self._draw_panel_content(context)


class C3DB_PT_panel_tab(C3DB_PT_panel_base, bpy.types.Panel):
    bl_label = "3D Cursors Briefcase"
    bl_idname = "C3DB_PT_panel_tab"
    bl_space_type = "VIEW_3D"
    bl_region_type = "UI"
    bl_category = "3D Cursors"

    @classmethod
    def poll(cls, context):
        prefs = _get_prefs(context)
        # Only show this panel when show_in_own_tab is True
        return prefs.show_in_own_tab

    def draw(self, context):
        self._draw_panel_content(context)


# Operators


class C3DB_OT_show_popup(C3DB_PT_panel_base, bpy.types.Operator):
    """Open as a floating 3D Cursor popup panel"""

    bl_idname = "view3d.c3db_show_popup"
    bl_label = "3D Cursor Floating Popup"
    bl_options = {"REGISTER", "INTERNAL"}

    def draw(self, context):
        layout = self.layout
        title_row = layout.row()
        title_row.label(text="3D Cursors Briefcase", icon="CURSOR")
        layout.separator()
        layout.operator_context = "INVOKE_DEFAULT"
        self._draw_panel_content(context)
        layout.separator()
        prefs = _get_prefs(context)
        icon = "CHECKBOX_HLT" if prefs.show_default_cursor_panel else "CHECKBOX_DEHLT"
        row = layout.row(align=True)
        row.prop(prefs, "show_default_cursor_panel", text="", icon=icon)
        if prefs.show_default_cursor_panel:
            row.label(text="Show default 3D Cursor panel")
            VIEW3D_PT_view3d_cursor.draw(self, context)
        else:
            row.label(text="Show default 3D Cursor panel")

    def invoke(self, context, event):
        if context.area.type != "VIEW_3D":
            return {"CANCELLED"}
        return context.window_manager.invoke_popup(self, width=300)

    def execute(self, context):
        return {"FINISHED"}


class C3DB_OT_save(bpy.types.Operator):
    """Save 3D Cursor location, rotation and rotation mode"""

    bl_idname = "view3d.c3db_save"
    bl_label = "Save 3D Cursor"
    bl_options = {"REGISTER", "UNDO"}
    name: bpy.props.StringProperty(name="Name", default="")

    def execute(self, context):
        name = self.name if getattr(self, "name", "") else None
        C3DB_CursorService.save_current_cursor(context.scene, name=name)
        context.area.tag_redraw()
        return {"FINISHED"}


class C3DB_OT_restore(bpy.types.Operator):
    """Load and optionally center the 3D Cursor"""

    bl_idname = "view3d.c3db_restore"
    bl_label = "Load 3D Cursor"
    bl_options = {
        "REGISTER"
    }  # "UNDO" do not work for 3D Cursor see https://projects.blender.org/blender/blender/issues/131422

    index: bpy.props.IntProperty(default=-1)
    center: bpy.props.BoolProperty(default=False)

    @classmethod
    def poll(cls, context):
        scene = context.scene
        return bool(scene.C3DB_3Dcursors_collection) and scene.C3DB_3Dcursors_index < len(
            scene.C3DB_3Dcursors_collection
        )

    def execute(self, context):
        index = self.index if self.index >= 0 else None
        C3DB_CursorService.restore_index(context.scene, index, center=self.center)
        context.area.tag_redraw()
        return {"FINISHED"}


class C3DB_OT_restore_selected(bpy.types.Operator):
    """Restore selected 3D Cursor"""

    bl_idname = "view3d.c3db_restore_selected"
    bl_label = "Restore Selected 3D Cursor"

    @classmethod
    def poll(cls, context):
        scene = context.scene
        return bool(scene.C3DB_3Dcursors_collection) and scene.C3DB_3Dcursors_index < len(
            scene.C3DB_3Dcursors_collection
        )

    def execute(self, context):
        return bpy.ops.view3d.c3db_restore("INVOKE_DEFAULT", index=-1)


class C3DB_OT_restore_eye(bpy.types.Operator):
    """Load the 3D Cursor"""

    bl_idname = "c3db.restore_eye"
    bl_label = "Load Cursor"
    bl_options = {"INTERNAL"}

    index: bpy.props.IntProperty(default=-1)

    @classmethod
    def poll(cls, context):
        scene = context.scene
        return bool(scene.C3DB_3Dcursors_collection) and scene.C3DB_3Dcursors_index < len(
            scene.C3DB_3Dcursors_collection
        )

    def execute(self, context):
        bpy.ops.view3d.c3db_restore(index=self.index)
        return {"FINISHED"}


class C3DB_OT_restore_screen(bpy.types.Operator):
    """Load and Center view on the 3D Cursor"""

    bl_idname = "c3db.restore_screen"
    bl_label = "Center View on Cursor"
    bl_options = {"INTERNAL"}

    index: bpy.props.IntProperty(default=-1)
    center: bpy.props.BoolProperty(default=True)

    @classmethod
    def poll(cls, context):
        scene = context.scene
        return bool(scene.C3DB_3Dcursors_collection) and scene.C3DB_3Dcursors_index < len(
            scene.C3DB_3Dcursors_collection
        )

    def execute(self, context):
        bpy.ops.view3d.c3db_restore(index=self.index, center=self.center)
        try:
            bpy.ops.view3d.view_center_cursor()
        except RuntimeError:
            pass
        return {"FINISHED"}


class C3DB_OT_update(bpy.types.Operator):
    """Update selected 3D Cursor"""

    bl_idname = "view3d.c3db_update"
    bl_label = "Update 3D Cursor"
    bl_options = {"REGISTER", "UNDO"}

    @classmethod
    def poll(cls, context):
        scene = context.scene
        return bool(scene.C3DB_3Dcursors_collection) and scene.C3DB_3Dcursors_index < len(
            scene.C3DB_3Dcursors_collection
        )

    def execute(self, context):
        C3DB_CursorService.update_selected(context.scene)
        context.area.tag_redraw()
        return {"FINISHED"}


class C3DB_OT_restore_next(bpy.types.Operator):
    """Go to next 3D Cursor and Restore it"""

    bl_idname = "view3d.c3db_restore_next"
    bl_label = "Load next 3D Cursor"

    @classmethod
    def poll(cls, context):
        return len(context.scene.C3DB_3Dcursors_collection) > 1

    def execute(self, context):
        C3DB_NavigationService.next(context.scene)
        bpy.ops.view3d.c3db_restore()
        return {"FINISHED"}


class C3DB_OT_restore_previous(bpy.types.Operator):
    """Go to previous 3D Cursor and Restore it"""

    bl_idname = "view3d.c3db_restore_previous"
    bl_label = "Load previous 3D Cursor"

    @classmethod
    def poll(cls, context):
        return len(context.scene.C3DB_3Dcursors_collection) > 1

    def execute(self, context):
        C3DB_NavigationService.previous(context.scene)
        bpy.ops.view3d.c3db_restore()
        return {"FINISHED"}


class C3DB_OT_delete_all(bpy.types.Operator):
    """Delete all 3D Cursors"""

    bl_idname = "view3d.c3db_delete_all"
    bl_label = "Delete All 3D Cursors"
    bl_options = {"REGISTER", "UNDO"}

    @classmethod
    def poll(cls, context):
        return bool(context.scene.C3DB_3Dcursors_collection)

    def execute(self, context):
        C3DB_CursorService.delete_all(context.scene)
        context.area.tag_redraw()
        return {"FINISHED"}


class C3DB_OT_remove_from_list(bpy.types.Operator):
    """Delete selected 3D Cursor(s) from the list"""

    bl_idname = "view3d.c3db_remove_from_list"
    bl_label = "Delete 3D Cursor"
    bl_options = {"REGISTER", "UNDO"}

    @classmethod
    def poll(cls, context):
        return bool(context.scene.C3DB_3Dcursors_collection)

    def execute(self, context):
        C3DB_CursorService.delete_selected(context.scene)
        context.area.tag_redraw()
        return {"FINISHED"}


class C3DB_OT_move_up_list(bpy.types.Operator):
    """Move the selected 3D Cursor up in the list"""

    bl_idname = "view3d.c3db_move_up_list"
    bl_label = "Move 3D Cursor Up the list"

    @classmethod
    def poll(cls, context):
        return context.scene.C3DB_3Dcursors_index > 0

    def execute(self, context):
        C3DB_CursorService.move_up(context.scene)
        return {"FINISHED"}


class C3DB_OT_move_down_list(bpy.types.Operator):
    """Move the selected 3D Cursor down in the list"""

    bl_idname = "view3d.c3db_move_down_list"
    bl_label = "Move 3D Cursor Down the list"

    @classmethod
    def poll(cls, context):
        return context.scene.C3DB_3Dcursors_index < len(context.scene.C3DB_3Dcursors_collection) - 1

    def execute(self, context):
        C3DB_CursorService.move_down(context.scene)
        return {"FINISHED"}


class C3DB_OT_convert_all_to_empties(bpy.types.Operator):
    """Convert all 3D Cursors to empty objects"""

    bl_idname = "view3d.c3db_convert_all_to_empties"
    bl_label = "Convert All 3D Cursors to Empties"
    bl_options = {"REGISTER", "UNDO"}

    @classmethod
    def poll(cls, context):
        return bool(context.scene.C3DB_3Dcursors_collection)

    def execute(self, context):
        C3DB_CursorService.convert_all_to_empties(context)
        return {"FINISHED"}


class C3DB_OT_convert_selected_to_empty(bpy.types.Operator):
    """Convert selected 3D Cursor(s) to empty object(s)"""

    bl_idname = "view3d.c3db_convert_selected_to_empty"
    bl_label = "Convert Selected 3D Cursor(s) to Empty object(s)"
    bl_options = {"REGISTER", "UNDO"}

    @classmethod
    def poll(cls, context):
        return bool(context.scene.C3DB_3Dcursors_collection)

    def execute(self, context):
        C3DB_CursorService.convert_selected_to_empty(context)
        return {"FINISHED"}


# Preferences and keymap ops


class C3DB_OT_open_keymap_editor(bpy.types.Operator):
    """Open Keymap Editor to configure addon shortcuts"""

    bl_idname = "c3db.open_keymap_editor"
    bl_label = "Edit addon keymap shortcuts"
    bl_options = {"INTERNAL"}

    def execute(self, context):
        bpy.ops.screen.userpref_show("INVOKE_DEFAULT")
        prefs = context.preferences
        prefs.active_section = "KEYMAP"
        for window in context.window_manager.windows:
            for area in window.screen.areas:
                if area.type == "PREFERENCES":
                    space = area.spaces.active
                    space.filter_type = "NAME"
                    space.filter_text = "c3db_"
                    break
        return {"FINISHED"}


def _update_panel_tab_name(self, context):
    """Update panel bl_category when tab name changes."""
    try:
        # Unregister the old panel
        bpy.utils.unregister_class(C3DB_PT_panel_tab)

        # Update the bl_category
        C3DB_PT_panel_tab.bl_category = self.tab_name

        # Re-register with new category
        bpy.utils.register_class(C3DB_PT_panel_tab)

        # Trigger a full UI redraw
        for screen in bpy.data.screens:
            for area in screen.areas:
                area.tag_redraw()
    except (AttributeError, RuntimeError):
        pass


def _update_show_selection_checkboxes(self, context):
    if not self.show_selection_checkboxes:
        for scene in bpy.data.scenes:
            if hasattr(scene, "C3DB_3Dcursors_collection"):
                for cursor3d in scene.C3DB_3Dcursors_collection:
                    cursor3d.selected = False


class C3DB_AddonPreferences(bpy.types.AddonPreferences):
    bl_idname = __package__

    C3DB_addon_key: StringProperty(
        name="Addon Key",
        description="Unique identifier for this addon instance",
        default="test",
    )

    auto_center_view_checkbox: BoolProperty(name="Auto center view on 3D Cursor", default=False)

    auto_load: BoolProperty(
        name="Automatically load cursors when selecting them in the list",
        description="Automatically load/restore cursors when selecting them in the list",
        default=False,
    )

    show_icons: BoolProperty(
        name="Show Controls as Icons",
        description="Toggle between row text and inline icon display for controls",
        default=False,
    )

    show_selection_checkboxes: BoolProperty(
        name="Show Selection Checkboxes",
        description=("Use batch-select checkboxes\n" "for multi 3D Cursor delete/convert operations"),
        default=False,
        update=_update_show_selection_checkboxes,
    )

    show_default_cursor_panel: BoolProperty(
        name="Show default 3D Cursor panel",
        description="Show the default View3D Cursor panel inside the popup",
        default=False,
    )

    show_in_own_tab: BoolProperty(
        name="Show in its own tab",
        description="Display the 3D Cursors Briefcase in its own sidebar tab instead of under the 3D Cursor panel",
        default=False,
    )

    tab_name: StringProperty(
        name="Tab Name",
        description="Custom name for the sidebar tab",
        default="3D Cursors",
        update=_update_panel_tab_name,
    )

    def draw(self, context):
        layout = self.layout
        col = layout.column()
        col.prop(self, "show_in_own_tab", text="Show in its own tab")
        if self.show_in_own_tab:
            col.prop(self, "tab_name", text="Tab Name")
            col.label(text=f"Location: View3D > Sidebar > {self.tab_name} Tab")
        else:
            col.label(text="Location: View3D > Sidebar > View Tab > 3D Cursor Panel")
        layout.operator("c3db.open_keymap_editor", icon="CON_SIZELIKE")


def _get_prefs(context) -> C3DB_AddonPreferences:
    """Helper to get addon preferences."""
    return context.preferences.addons[__package__].preferences


class C3DB_OT_open_preferences(bpy.types.Operator):
    """Open addon preferences"""

    bl_idname = "c3db.open_preferences"
    bl_label = "Preferences"

    def execute(self, context):
        bpy.ops.screen.userpref_show("INVOKE_DEFAULT")
        context.preferences.active_section = "ADDONS"
        context.window_manager.addon_search = "3d cursors briefcase"
        bpy.ops.preferences.addon_show(module=__package__)
        return {"FINISHED"}


# Registration and keymaps

classes = (
    C3DB_OT_show_popup,
    C3DB_PG_properties,
    C3DB_UL_list,
    C3DB_MT_menu_specials,
    C3DB_OT_save,
    C3DB_OT_restore,
    C3DB_OT_restore_selected,
    C3DB_OT_restore_eye,
    C3DB_OT_restore_screen,
    C3DB_OT_update,
    C3DB_OT_restore_next,
    C3DB_OT_restore_previous,
    C3DB_OT_delete_all,
    C3DB_OT_remove_from_list,
    C3DB_OT_move_up_list,
    C3DB_OT_move_down_list,
    C3DB_OT_convert_all_to_empties,
    C3DB_OT_convert_selected_to_empty,
    C3DB_PT_panel,
    C3DB_PT_panel_tab,
    C3DB_OT_open_keymap_editor,
    C3DB_AddonPreferences,
    C3DB_OT_open_preferences,
)

_c3db_keymap_data = [
    (
        "3D View",
        {"space_type": "VIEW_3D", "region_type": "WINDOW"},
        {
            "items": [
                ("view3d.c3db_save", {"type": "S", "value": "PRESS", "ctrl": True, "oskey": True}, None),
                (
                    "view3d.c3db_remove_from_list",
                    {"type": "D", "value": "PRESS", "ctrl": True, "shift": True},
                    None,
                ),
                ("view3d.c3db_restore_selected", {"type": "R", "value": "PRESS", "ctrl": True}, None),
                (
                    "view3d.c3db_update",
                    {"type": "U", "value": "PRESS", "ctrl": True, "shift": True},
                    None,
                ),
                (
                    "view3d.c3db_restore_next",
                    {"type": "R", "value": "PRESS", "ctrl": True, "shift": True},
                    None,
                ),
                (
                    "view3d.c3db_restore_previous",
                    {"type": "R", "value": "PRESS", "ctrl": True, "shift": True, "alt": True},
                    None,
                ),
                (
                    "view3d.c3db_delete_all",
                    {"type": "X", "value": "PRESS", "ctrl": True, "shift": True},
                    None,
                ),
                ("view3d.c3db_show_popup", {"type": "Q", "value": "PRESS", "shift": True}, None),
            ]
        },
    ),
]


def register():
    for cls in classes:
        bpy.utils.register_class(cls)

    # Initialize panel tab name from preferences
    try:
        prefs = _get_prefs(bpy.context)
        C3DB_PT_panel_tab.bl_category = prefs.tab_name
    except (RuntimeError, AttributeError):
        # If context is not available yet, use default
        C3DB_PT_panel_tab.bl_category = "3D Cursors"

    bpy.types.Scene.C3DB_3Dcursors_collection = bpy.props.CollectionProperty(type=C3DB_PG_properties)

    def index_update(self, context):
        global C3DB_INDEX_UPDATING
        if C3DB_INDEX_UPDATING:
            return
        prefs = _get_prefs(context)
        if prefs.auto_load:
            C3DB_INDEX_UPDATING = True
            try:
                bpy.ops.view3d.c3db_restore(index=self.C3DB_3Dcursors_index)
            except RuntimeError:
                pass
            finally:
                C3DB_INDEX_UPDATING = False

    bpy.types.Scene.C3DB_3Dcursors_index = bpy.props.IntProperty(update=index_update)
    bpy.types.Scene.C3DB_loaded_index = bpy.props.IntProperty(default=-1)

    addon_keymap_register(_c3db_keymap_data)


def unregister():
    for cls in classes:
        bpy.utils.unregister_class(cls)

    del bpy.types.Scene.C3DB_3Dcursors_collection
    del bpy.types.Scene.C3DB_3Dcursors_index
    del bpy.types.Scene.C3DB_loaded_index

    addon_keymap_unregister(_c3db_keymap_data)


if __name__ == "__main__":
    register()
