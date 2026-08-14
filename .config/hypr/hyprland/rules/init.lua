-- Ignore maximize requests from all apps
hl.window_rule({
    name  = "suppress-maximize-events",
    match = { class = ".*" },

    suppress_event = "maximize",
})

-- Fix some dragging issues with XWayland
hl.window_rule({
    name  = "fix-xwayland-drags",
    match = {
        class      = "^$",
        title      = "^$",
        xwayland   = true,
        float      = true,
        fullscreen = false,
        pin        = false,
    },

    no_focus = true,
})

-- Fix espanso config reload stealing focus
hl.window_rule({
    name  = "espanso",
    match = {
        class = "Espanso.SyncTool",
    },

    float             = true,
    opacity           = 0.001,
    size              = { "monitor_w", "monitor_h" },
    focus_on_activate = true,

    decorate = false,
    no_anim  = true,
    no_blur  = true,
    no_dim   = true,
})
require("hyprland.rules.float")
require("hyprland.rules.games")
require("hyprland.rules.layers")
require("hyprland.rules.transparent")
