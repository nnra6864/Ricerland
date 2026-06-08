local defs = require("defs")

hl.config({
    decoration = {
        blur = {
            enabled                   = defs.blur.enabled,
            size                      = defs.blur.size,
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
            input_methods_ignorealpha = defs.blur.input_methods_ignore_alpha
        }
    }
})
