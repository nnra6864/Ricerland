require("hyprland.animations.curves")
local defs = require("defs")

hl.config({
    animations = {
        enabled = defs.animations.enabled,
        workspace_wraparound = defs.animations.workspace_wraparound
    }
})

hl.animation({
    leaf = "windows",
    enabled = true,
    speed = 3,
    bezier = "expo_out",
    style = "slide bottom"
})

hl.animation({
    leaf = "layers",
    enabled = true,
    speed = 3,
    bezier = "expo_out",
    style = "slide bottom"
})

hl.animation({
    leaf = "fade",
    enabled = true,
    speed = 3,
    bezier = "expo_out",
})

hl.animation({
    leaf = "border",
    enabled = true,
    speed = 3,
    bezier = "expo_out",
})

hl.animation({
    leaf = "borderangle",
    enabled = true,
    speed = 10,
    bezier = "quad_out",
    style = "once"
})

hl.animation({
    leaf = "workspaces",
    enabled = true,
    speed = 3,
    bezier = "expo_out",
    style = "slidevert"
})

hl.animation({
    leaf = "zoomFactor",
    enabled = true,
    speed = 3,
    bezier = "expo_out",
})
