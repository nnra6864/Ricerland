local defs = require("defs")

hl.config({
    decoration = {
        blur = {
            enabled                   = defs.blur.enabled,
            size                      = defs.blur.size,
            variant                   = defs.blur.variant,
            passes                    = defs.blur.passes,
            ignore_opacity            = defs.blur.ignore_opacity,
            new_optimizations         = defs.blur.new_optimizations,
            xray                      = defs.blur.xray,
            noise                     = defs.blur.noise,
            contrast                  = defs.blur.contrast,
            brightness                = defs.blur.brightness,
            vibrancy                  = defs.blur.vibrancy,
            vibrancy_darkness         = defs.blur.vibrancy_darkness,
            special                   = defs.blur.special,
            popups                    = defs.blur.popups,
            popups_ignorealpha        = defs.blur.popups_ignore_alpha,
            input_methods             = defs.blur.input_methods,
            input_methods_ignorealpha = defs.blur.input_methods_ignore_alpha,

            glass = {
                refraction = defs.blur.glass.refraction,
                size       = defs.blur.glass.size,
                roughness  = defs.blur.glass.roughness,
            },

            acrylic = {
                refraction = defs.blur.acrylic.refraction,
                bulb       = defs.blur.acrylic.bulb,
                clarity    = defs.blur.acrylic.clarity,
                aberration = defs.blur.acrylic.aberration,
                tint       = defs.blur.acrylic.tint,
            },

            drops = {
                speed = defs.blur.drops.speed,
            },

            heat_shimmer = {
                speed = defs.blur.heat_shimmer.speed,
            },

            aurora = {
                speed     = defs.blur.aurora.speed,
                intensity = defs.blur.aurora.intensity,
                color1    = defs.blur.aurora.color1,
                color2    = defs.blur.aurora.color2,
            },

            haze = {
                intensity   = defs.blur.haze.intensity,
                iridescence = defs.blur.haze.iridescence,
            },

            ripple = {
                strength = defs.blur.ripple.strength,
                radius   = defs.blur.ripple.radius,
                width    = defs.blur.ripple.width,
                duration = defs.blur.ripple.duration,
            },

            water = {
                strength = defs.blur.water.strength,
                radius   = defs.blur.water.radius,
                speed    = defs.blur.water.speed,
                damping  = defs.blur.water.damping,
                duration = defs.blur.water.duration,
            },

            fluid_jar = {
                color       = defs.blur.fluid_jar.color,
                speed       = defs.blur.fluid_jar.speed,
                fill_amount = defs.blur.fluid_jar.fill_amount,
                mass        = defs.blur.fluid_jar.mass,
                precision   = defs.blur.fluid_jar.precision,
                turbulence  = defs.blur.fluid_jar.turbulence,
                distortion  = defs.blur.fluid_jar.distortion,
            },
        },
    }
})
