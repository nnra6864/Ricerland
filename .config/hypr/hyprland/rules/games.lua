hl.window_rule({ match = { class = "gamescope" },     tag = "+game" })
hl.window_rule({ match = { class = "cs2" },           tag = "+game" })
hl.window_rule({ match = { class = "csgo_linux64" },  tag = "+game" })
hl.window_rule({ match = { class = "steam_app_730" }, tag = "+game" })
hl.window_rule({ match = { class = "hl_linux" },      tag = "+game" })
hl.window_rule({ match = { class = "osu! # Osu" },    tag = "+game" })
hl.window_rule({ match = { class = "Beat Saber" },    tag = "+game" })

-- R.E.P.O.
hl.window_rule({ match = { class = "steam_app_3241660" }, tag = "+game" })

-- The Forest
hl.window_rule({ match = { class = "steam_app_242760" }, tag = "+game" })

hl.window_rule({
    name            = "games",
    match           = { tag = "game" },
    content         = "game",
    fullscreen      = true,
    opaque          = true,
    immediate       = true,
    confine_pointer = true,
    decorate        = false,
})
