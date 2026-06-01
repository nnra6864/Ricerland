local layers = {
    "notifications",
    "wlr_which_key",
    "vicinae",
    "hyprlauncher",
    "rofi",
    "swaync-control-center"
}

for _, layer in ipairs(layers) do
    hl.layer_rule({
        match = {
            namespace = layer
        },
        blur         = true,
        xray         = true,
        blur_popups  = true,
        ignore_alpha = 0.1
    })
end

-- Notifications should slide from top
hl.layer_rule({
    match = {
        namespace = "notifications",
    },
    animation = "slide top"
})
