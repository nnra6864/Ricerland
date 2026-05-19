local defs = require("defs")

hl.config({
    general = {
        layout = "scrolling",
        allow_tearing = true,

        -- Border
        border_size = defs.border.size,
        resize_on_border = true,
        extend_border_grab_area = 10,

        -- Border colors
        col = {
            inactive_border       = defs.border.inactive_col,
            active_border         = defs.border.active_col,
            nogroup_border        = defs.border.inactive_col,
            nogroup_border_active = defs.border.active_col
        },

        -- Gaps
        gaps_in  = defs.gaps.inner,
        gaps_out = defs.gaps.outer,

        -- Snap
        snap = {
            enabled = true,
            window_gap = 10,
            monitor_gap = 10,
            respect_gaps = true
        }
    }
})
