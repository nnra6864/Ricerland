-- TODO: Iterate instead of using regex
hl.layer_rule({
    match = {
        namespace =
            "notifications" ..
            "|wlr_which_key" ..
            "|vicinae"..
            "|hyprlauncher" ..
            "|rofi" ..
            "|swaync-control-center"
    },
    blur         = true,
    xray         = true,
    blur_popups  = true,
    ignore_alpha = 0.1
})
