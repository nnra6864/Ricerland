hl.window_rule({ match = { class = "blender", initial_title = "File Browser" }, tag = "+blender_min_size" })

hl.window_rule({
    name   = "blender_min_size",
    match  = { tag = "blender_min_size" },
    size   = { "monitor_h", "(monitor_h) * 0.5" }
})
