hl.window_rule({ match = { class = "cs2" },           tag = "+game" })
hl.window_rule({ match = { class = "csgo_linux64" },  tag = "+game" })
hl.window_rule({ match = { class = "steam_app_730" }, tag = "+game" })
hl.window_rule({ match = { class = "hl_linux" },      tag = "+game" })
hl.window_rule({ match = { class = "osu! # Osu" },    tag = "+game" })
hl.window_rule({ match = { class = "Beat Saber" },    tag = "+game" })

hl.window_rule({
    name            = "games",
    match           = { tag = "game" },
    fullscreen      = true,
    opaque          = true,
    immediate       = true,
    confine_pointer = true
})
