import bpy
op = bpy.context.active_operator

op.filepath = '/home/nnra/Data/Projects/Unity/AD185-DZ05-Mateja-Popovic-5962/Assets/Models/Axe.fbx'
op.use_selection = True
op.use_visible = False
op.use_active_collection = False
op.collection = ''
op.global_scale = 1.0
op.apply_unit_scale = True
op.apply_scale_options = 'FBX_SCALE_UNITS'
op.use_space_transform = True
op.bake_space_transform = False
op.object_types = {'MESH', 'EMPTY', 'LIGHT', 'ARMATURE', 'OTHER', 'CAMERA'}
op.use_mesh_modifiers = True
op.use_mesh_modifiers_render = True
op.mesh_smooth_type = 'OFF'
op.colors_type = 'SRGB'
op.prioritize_active_color = False
op.use_subsurf = False
op.use_mesh_edges = False
op.use_tspace = False
op.use_triangles = False
op.use_custom_props = False
op.add_leaf_bones = True
op.primary_bone_axis = 'Y'
op.secondary_bone_axis = 'X'
op.use_armature_deform_only = False
op.armature_nodetype = 'NULL'
op.bake_anim = True
op.bake_anim_use_all_bones = True
op.bake_anim_use_nla_strips = True
op.bake_anim_use_all_actions = True
op.bake_anim_force_startend_keying = True
op.bake_anim_step = 1.0
op.bake_anim_simplify_factor = 1.0
op.path_mode = 'AUTO'
op.embed_textures = False
op.batch_mode = 'OFF'
op.use_batch_own_dir = True
op.axis_forward = 'Z'
op.axis_up = 'Y'
