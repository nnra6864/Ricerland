if hl.plugin.hyprglass then
    local hg = hl.plugin.hyprglass

    hg.config({
        enabled        = true,
        default_theme  = "dark",
        default_preset = "default",

        blur_strength        = 4,
        blur_iterations      = 2,
        refraction_strength  = 3,
        chromatic_aberration = 0.5,
        fresnel_strength     = 0.25,
        specular_strength    = 0.5,
        glass_opacity        = 1,
        edge_thickness       = 0.15,
        tint_color           = 0x081D2680,
        lens_distortion      = 2,
        brightness           = 0.9,
        contrast             = 1.2,
        saturation           = 1,
        vibrancy             = 0.15,
        vibrancy_darkness    = 0,
        adaptive_dim         = 0.2,
        adaptive_boost       = 0
    })

    hg.layer("dunst")
    hg.layer("vicinae")
    hg.layer("wlr_which_key")
end
