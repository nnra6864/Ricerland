hl.monitor({
    output   = "",
    mode     = "highres@highrr",
    position = "0x0",
    scale    = 1,

    bitdepth            = 10,
    supports_wide_color = true,
    supports_hdr        = true,

    sdr_min_luminance = 0.005,
    sdr_max_luminance = 250,

    min_luminance     = 0,
    max_luminance     = 2000,
    max_avg_luminance = 750
})
