hl.window_rule({ match = { class = "waydroid.com.*" },                          tag = "+float" })
hl.window_rule({ match = { class = "hyprland-share-picker" },                   tag = "+float" })
hl.window_rule({ match = { class = "org.freedesktop.impl.portal.desktop.kde" }, tag = "+float" })
hl.window_rule({ match = { class = "espanso" },                                 tag = "+float" })

hl.window_rule({ match = { class = "org.keepassxc.KeePassXC", title = "KeePassXC - Access Request" },  tag = "+float" })
hl.window_rule({ match = { class = "org.keepassxc.KeePassXC", title = "Unlock Database - KeePassXC" }, tag = "+float" })
hl.window_rule({ match = { class = "Material Maker",          title = "Alert!" },                      tag = "+float" })

hl.window_rule({
    name   = "float",
    match  = { tag = "float" },
    center = true,
    float  = true,
})
