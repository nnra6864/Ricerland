local defs = require("defs")

hl.config({
    misc = {
        disable_hyprland_logo    = true,
        disable_splash_rendering = true,

        font_family        = defs.font.family,
        splash_font_family = defs.font.family,

        col = { splash     = defs.theme.color.foreground },
        background_color   = defs.theme.color.background,

        animate_manual_resizes       = defs.animations.animate_manual_resizes,
        animate_mouse_windowdragging = defs.animations.animate_mouse_windowdragging,

        vrr                        = 0,
        focus_on_activate          = false,
        session_lock_xray          = false,
        close_special_on_empty     = false,
        initial_workspace_tracking = 0,
        middle_click_paste         = false,
        enable_anr_dialog          = false,
    }
})
