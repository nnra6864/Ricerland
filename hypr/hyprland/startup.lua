local defs = require("defs")

hl.on("hyprland.start", function()
    hl.exec_cmd("sunshine")
    hl.exec_cmd("pypr")
    hl.exec_cmd("vicinae server")
    hl.exec_cmd("ydotoold")
    hl.exec_cmd("lan-mouse daemon")
    hl.exec_cmd("kdeconnectd")
    hl.exec_cmd("kdeconnect-indicator")
    hl.exec_cmd("openrgb --startminimized --profile 'Off' --server")
    hl.exec_cmd("hyprpaper")
    hl.exec_cmd("fish -c 'track_app_usage'")

    -- Main
    local main_workspace = "1"
    hl.exec_cmd(defs.apps.browser, { workspace = main_workspace })

    -- Social
    local social_workspace = "2"
    hl.exec_cmd("mumble",                              { workspace = social_workspace })
    hl.exec_cmd("sleep 1 && flatpak run im.riot.Riot", { workspace = social_workspace })

    -- Special
    local special_workspace = "special"
    hl.exec_cmd("keepassxc",                                        { workspace = special_workspace })
    hl.exec_cmd("sleep 1 && flatpak run md.obsidian.Obsidian",      { workspace = special_workspace })
    hl.exec_cmd("xdg-terminal-exec ncmpcpp",                        { workspace = special_workspace })
    hl.exec_cmd("obs --startreplaybuffer --disable-shutdown-check", { workspace = special_workspace })

    hl.exec_cmd("hyprlock")
end)
