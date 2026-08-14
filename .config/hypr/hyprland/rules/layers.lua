local layers = {
    "notifications",
    "wlr_which_key",
    "vicinae",
    "hyprlauncher",
    "rofi",
    "swaync-control-center",
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

local no_anim = {
    "selection",
    "hyprpicker",
}

for _, layer in ipairs(no_anim) do
    hl.layer_rule({
        match = {
            namespace = layer
        },
        no_anim = true
    })
end

-- Notifications should slide from top
hl.layer_rule({
    match = {
        namespace = "notifications",
    },
    animation = "slide top",
})

-- GPU Screen Recorder
local gsr = {
    "gsr-ui",
    "gsr-notify",
}

for _, layer in ipairs(gsr) do
    hl.layer_rule({
        match = {
            namespace = layer
        },
        blur         = true,
        blur_popups  = true,
        ignore_alpha = 0.4,
        animation    = "fade",
    })
end
