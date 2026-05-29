local M = {}

-- Main mod
M.main_mod = "SUPER"

-- Apps
M.apps = {
    terminal     = "ghostty",
    browser      = "zen-browser",
    file_manager = "yazi"
}

-- Opacity
M.opacity = {
    active   = 1.0,
    inactive = 1.0,
}

-- Dimming
M.dimming = {
    modal    = true,
    inactive = true,
    strength = 0.6,
    special  = 0.2,
    around   = 0.4
}

-- Gaps
M.gaps = {
    inner = 5,
    outer = 10
}

-- Shadow
M.shadow = {
    col         = "#081D2680",
    inactiveCol = "#081D2680"
}

-- Border
M.border = {
    rounding       = 10,
    rounding_power = 2,
    size           = 2,

    -- Color
    active_col   = { colors = { "#266F7F", "#A3C5CC" }, M.borderActiveRotation },
    inactive_col = { colors = { "#1E444C", "#899699" }, M.borderInactiveRotation },

    -- Rotation
    active_rotation    = 90,
    inactive_rotation  = -90,
    rotation_animation = 1,
    rotation_duration  = 30,
    rotation_bezier    = "linear",
    rotation_type      = "loop",

    -- Fade
    fade_animation = 1,
    fade_duration  = 1,
    fade_bezier    = "quadOut"
}


-- Blur
M.blur = {
    enabled       = true,
    popups        = true,
    input_methods = true,
    special       = false,

    ignore_opacity             = true,
    popups_ignore_alpha        = true,
    input_methods_ignore_alpha = true,

    new_optimizations = true,
    xray              = true,

    size              = 20,
    passes            = 4,
    noise             = 0.02,
    contrast          = 1,
    brightness        = 0.6,
    vibrancy          = 1,
    vibrancy_darkness = 0
}

-- Shadow
M.shadow = {
    enabled = true,
    sharp   = false,

    range        = 4,
    render_power = 3,
    scale        = 1,

    color          = "#000000",
    color_inactive = "#000000",

    offset = { 0, 0 }
}

-- Glow
M.glow = {
    enabled = false,

    range        = 10,
    render_power = 3,

    color          = "#000000",
    color_inactive = "#000000",
}

-- Font
M.font = {
    family = "Maple Mono NF CN",
    size   = 13
}

-- Cursor
M.cursor = {
    name = "Bibata-Modern-Ice",
    size = 24
}

-- Theme
M.theme = {
    background = os.getenv("HOME") .. "/.config/Backgrounds/Kyanos/Misty_Cloudy_Mountain.jpg",

    qt = {
        platform_theme            = "gtk3",
        style                     = "breeze",
        quick_controls_style      = "org.kde.desktop",
        disable_window_decoration = 1,
        scaling                   = 1,
        screen_scaling            = 0
    },

    gtk = {
        name    = "oomox-Ricer",
        scaling = 1
    },

    color = {
        background       = "#081D26",
        foreground       = "#A3C5CC",
        background_alpha = "#081D2680",

        ls = "di=#A3C5CC:fi=#A3C5CC:ln=#267F6F:pi=#267F6F:so=#266F7F:bd=#266F7F:cd=#266F7F:or=#7F2626:mi=#7F2626:ex=#267F32"
    }
}

-- Animation
M.animations = {
    enabled = true,
    workspace_wraparound         = false,
    animate_manual_resizes       = true,
    animate_mouse_windowdragging = true,
    duration_multiplier          = 1
}

-- Sound
M.sound = {
    screenshot     = os.getenv("HOME") .. "/.config/SFX/Select.wav",
    instant_replay = os.getenv("HOME") .. "/.config/SFX/Select.wav"
}

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
