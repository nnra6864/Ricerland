local utils = require("utils")

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
    inactive = 1.0
}

-- Dimming
M.dimming = {
    modal    = false,
    inactive = false,
    strength = 0.2,
    special  = 0.2,
    around   = 0.4
}

-- Gaps
M.gaps = {
    inner = 5,
    outer = 10
}

-- Border
M.border = {
    rounding       = 10,
    rounding_power = 2,
    size           = 1,

    -- Color
    active_col   = { colors = { "#2E8799", "#2E8799" } },
    inactive_col = { colors = { "#2E9990", "#2E9990" } }
}

-- Shadow
M.shadow = {
    enabled = true,
    sharp   = false,

    range        = 10,
    render_power = 4,
    scale        = 1,

    color          = "#2E8799",
    color_inactive = "#2E9990",

    offset = { 0, 0 }
}

-- Glow
M.glow = {
    enabled = true,

    range        = 30,
    render_power = 4,

    color          = "#2E8799",
    color_inactive = "#2E9990"
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

    variant           = "frost",
    size              = 20,
    passes            = 4,
    noise             = 0.02,
    contrast          = 1,
    brightness        = 0.6,
    vibrancy          = 1,
    vibrancy_darkness = 0,

    glass = {
        refraction = 20,
        size       = 50,
        roughness  = 0.75,
    },

    ripple = {
        strength = 32,
        radius   = 400,
        width    = 64,
        duration = 0.45,
    },

    drops = {
        speed = 3,
    },

    water = {
        strength = 32,
        radius   = 10,
        speed    = 0.76,
        damping  = 0.95,
        duration = 12,
    },

    fluid_jar = {
        color       = "#1F5A66CC",
        speed       = 5,
        fill_amount = 0.5,
        mass        = 5.4,
        precision   = 1,
        turbulence  = 1.2,
        distortion  = 8,
    },

    heat_shimmer = {
        speed = 1,
    },

    acrylic = {
        refraction = 24,
        bulb       = 48,
        clarity    = 0.05,
        aberration = 0.025,
        tint       = "#EEF5FF14",
    },

    aurora = {
        speed     = 1,
        intensity = 1,
        color1    = "#2E8799AA",
        color2    = "#4D1F66AA",
    },

    haze = {
        intensity   = 1,
        iridescence = 1,
    },
}

-- Motion Blur
M.motion_blur = {
    enabled = false,
    samples = 7
}

-- Font
M.font = {
    family = "Maple Mono Normal NFP",
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
        platform_theme            = "qt6ct",
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

        ls = "di=#267180:fi=#A3C5CC:ln=#267F6F:pi=#267F6F:so=#266F7F:bd=#266F7F:cd=#266F7F:or=#7F2626:mi=#7F2626:ex=#267F32"
    }
}

-- Animation
M.animation_settings = {
    enabled = true,
    speed   = 3,
    bezier  = "expo_out"
}

M.animations = {
    enabled                      = true,
    workspace_wraparound         = false,
    animate_manual_resizes       = true,
    animate_mouse_windowdragging = true,
    duration_multiplier          = 1,

    windows = (function()
        local s = utils.copy(M.animation_settings)
        s.leaf  = "windows"
        s.style = "slide bottom"
        return s
    end)(),

    layers = (function()
        local s = utils.copy(M.animation_settings)
        s.leaf  = "layers"
        s.style = "slide bottom"
        return s
    end)(),

    workspaces = (function()
        local s = utils.copy(M.animation_settings)
        s.leaf  = "workspaces"
        s.style = "slidevert"
        return s
    end)(),

    fade = (function()
        local s = utils.copy(M.animation_settings)
        s.leaf  = "fade"
        return s
    end)(),

    border = (function()
        local s = utils.copy(M.animation_settings)
        s.leaf  = "border"
        return s
    end)(),

    border_angle = (function()
        local s = utils.copy(M.animation_settings)
        s.leaf   = "borderangle"
        s.speed  = 10
        s.bezier = "quad_out"
        s.style  = "once"
        return s
    end)(),

    zoom_factor = (function()
        local s = utils.copy(M.animation_settings)
        s.leaf  = "zoomFactor"
        return s
    end)()
}

-- Zoom
M.zoom = {
    max           = 10,
    min           = 1,
    toggle_factor = 1.5,
    step          = 0.25
}

-- Sound
M.sound = {
    play_cmd       = "pw-play --media-role=Notification ",
    screenshot     = os.getenv("HOME") .. "/.config/SFX/Select.wav",
    instant_replay = os.getenv("HOME") .. "/.config/SFX/Select.wav"
}

-- Overrides
local overrides_path = package.searchpath("def_overrides", package.path)
if overrides_path then
    local ok, overrides = pcall(require, "def_overrides")
    if ok and type(overrides) == "table" then
        utils.deep_merge(M, overrides)
    end
end

return M
