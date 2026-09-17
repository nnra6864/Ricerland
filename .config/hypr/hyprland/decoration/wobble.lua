local defs = require("defs")

hl.config({
    decoration = {
        wobble = {
            enabled          = defs.wobble.enabled,
            mesh             = defs.wobble.mesh,
            stiffness        = defs.wobble.stiffness,
            damping          = defs.wobble.damping,
            mass             = defs.wobble.mass,
            intensity        = defs.wobble.intensity,
            value_epsilon    = defs.wobble.value_epsilon,
            velocity_epsilon = defs.wobble.velocity_epsilon,
        }
    }
})
