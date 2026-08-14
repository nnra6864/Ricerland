-- Transparent
hl.window_rule({ match = { class = "org.kde.dolphin" },             tag = "+transparent" })
hl.window_rule({ match = { class = "org.kde.kdeconnect.*" },        tag = "+transparent" })
hl.window_rule({ match = { class = "org.kde.partitionmanager" },    tag = "+transparent" })
hl.window_rule({ match = { class = "org.qbittorrent.qBittorrent" }, tag = "+transparent" })
hl.window_rule({ match = { class = "org.fkoehler.KTailctl" },       tag = "+transparent" })
hl.window_rule({ match = { class = "org.openrgb.OpenRGB" },         tag = "+transparent" })
hl.window_rule({ match = { class = "org.keepassxc.KeePassXC" },     tag = "+transparent" })
hl.window_rule({ match = { class = "PrismLauncher" },               tag = "+transparent" })
hl.window_rule({ match = { class = "steam" },                       tag = "+transparent" })
hl.window_rule({ match = { class = "info.mumble.Mumble" },          tag = "+transparent" })

hl.window_rule({
    name    = "transparent",
    match   = { tag = "transparent" },
    opacity = 0.8
})

-- Broken Opaque (some apps behave weirdly when opaque if a transparent theme is used)
hl.window_rule({ match = { class = "md.obsidian.Obsidian"}, tag = "+broken_opaque" })
--hl.window_rule({ match = { class = "info.mumble.Mumble"}, tag = "broken_opaque" })

hl.window_rule({
    name    = "broken_opaque",
    match   = { tag = "broken_opaque" },
    opacity = 0.99999
})
