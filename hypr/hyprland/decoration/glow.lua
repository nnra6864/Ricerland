local defs = require("defs")

hl.config({
    decoration = {
        glow ={
            enabled        = defs.glow.enabled,
            range          = defs.glow.range,
            render_power   = defs.glow.render_power,
            color          = defs.glow.color,
            color_inactive = defs.glow.color_inactive,
        }
    }
})
