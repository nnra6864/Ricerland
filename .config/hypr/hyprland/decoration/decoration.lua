local defs = require("defs")

hl.config({
    decoration = {
        -- Rounding
        rounding       = defs.border.rounding,
        rounding_power = defs.border.rounding_power,

        -- Opacity
        active_opacity   = defs.opacity.active,
        inactive_opacity = defs.opacity.inactive,

        -- Dimming
        dim_inactive = defs.dimming.inactive,
        dim_strength = defs.dimming.strength,
        dim_special  = defs.dimming.special,
        dim_around   = defs.dimming.around,
    }
})
