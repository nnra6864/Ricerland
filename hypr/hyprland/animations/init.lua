require("hyprland.animations.curves")
local defs = require("defs")

hl.config({
    animations = {
        enabled = defs.animations.enabled,
        workspace_wraparound = defs.animations.workspace_wraparound
    }
})

hl.animation({
    leaf = "windowsIn",
    enabled = true,
    speed = 5,
    spring = "spring",
    style = "slide bottom"
})
