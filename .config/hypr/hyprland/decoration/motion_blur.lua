local defs = require("defs")

hl.config({
    decoration = {
        motion_blur = {
            enabled = defs.motion_blur.enabled,
            samples = defs.motion_blur.samples
        }
    }
})
