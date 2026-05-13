local M = {}

-- Apps
M.main_mod = "SUPER"
M.terminal = "ghostty"
M.browser = "zen-browser"
M.file_manager = "yazi"

-- Appearance
M.active_opacity = 1.0
M.inactive_opacity = 1.0
M.inactive_dimming = 0.6
M.gaps_in = 5
M.gaps_out = 10
M.blur = true
M.blur_size = 20
M.blur_passes = 4
M.shadow_col = "#081D2680"
M.shadow_inactiveCol = "#081D2680"
M.rounding = 10
M.rounding_power = 2

-- Border
M.border_size = 2
M.border_active_rotation = 90
M.border_inactive_rotation = -90
M.border_active_col = { colors = { "#266F7F", "#A3C5CC" }, M.borderActiveRotation }
M.border_inactive_col = { colors = { "#1E444C", "#899699" }, M.borderInactiveRotation }
M.border_fade_animation = 1
M.border_fade_duration = 1
M.border_fade_bezier = "quadOut"
M.border_rotation_animation = 1
M.border_rotation_duration = 30
M.border_rotation_bezier = "linear"
M.border_rotation_type = "loop"

-- Font
M.font = "Maple Mono NF CN"
M.fontSize = 13

-- Cursor
M.cursor = "Bibata-Modern-Ice"
M.cursor_size = 24
M.cursor_rot = 21

-- Theme
M.theme = "oomox-Ricer"
M.background = os.getenv("HOME") .. "/.config/Backgrounds/Kyanos/Misty_Cloudy_Mountain.jpg"
M.bg_col = "#081D26"
M.fg_col = "#A3C5CC"
M.bg_col_a = "#081D2680"
M.termColors = "di=#A3C5CC:fi=#A3C5CC:ln=#267F6F:pi=#267F6F:so=#266F7F:bd=#266F7F:cd=#266F7F:or=#7F2626:mi=#7F2626:ex=#267F32"

-- Animation
M.animation_duration_multiplier = 1

-- Sound
M.screenshot_sound = os.getenv("HOME") .. "/.config/SFX/Select.wav"

-- Overrides
local overrides_path = package.searchpath("overrides", package.path)
if overrides_path then
    local ok, overrides = pcall(require, "overrides")
    if ok and type(overrides) == "table" then
        for key, value in pairs(overrides) do
            M[key] = value
        end
    end
end

return M
