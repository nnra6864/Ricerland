local defs = require("defs")

hl.config({
    decoration = {
        shadow ={
            enabled        = defs.shadow.enabled,
            range          = defs.shadow.range,
            render_power   = defs.shadow.render_power,
            sharp          = defs.shadow.sharp,
            color          = defs.shadow.color,
            color_inactive = defs.shadow.color_inactive,
            offset         = defs.shadow.offset,
            scale          = defs.shadow.scale
        }
    }
})
