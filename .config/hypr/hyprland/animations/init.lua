local defs = require("defs")
require("hyprland.animations.curves")

hl.config({
    animations = {
        enabled              = defs.animations.enabled,
        workspace_wraparound = defs.animations.workspace_wraparound
    }
})

hl.animation(defs.animations.windows)
hl.animation(defs.animations.layers)
hl.animation(defs.animations.fade)
hl.animation(defs.animations.border)
hl.animation(defs.animations.border_angle)
hl.animation(defs.animations.workspaces)
hl.animation(defs.animations.zoom_factor)
