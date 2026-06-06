local defs = require("defs")

hl.on("hyprland.start", function()
    hl.exec_cmd("dunst")
    hl.exec_cmd("sunshine")
    hl.exec_cmd("udiskie")
    hl.exec_cmd("vicinae server")
    hl.exec_cmd("ydotoold")
    hl.exec_cmd("lan-mouse daemon")
    hl.exec_cmd("kdeconnectd")
    hl.exec_cmd("kdeconnect-indicator")
    hl.exec_cmd("openrgb --startminimized --profile 'Off' --server")
    hl.exec_cmd("hyprpaper")
    hl.exec_cmd("fish -c 'track_app_usage'")

    -- Main
    local main_workspace = "1 silent"
    hl.exec_cmd(defs.apps.browser, { workspace = main_workspace })

    -- Social
    local social_workspace = "2 silent"
    hl.exec_cmd("mumble",                              { workspace = social_workspace })
    hl.exec_cmd("sleep 2 && flatpak run im.riot.Riot", { workspace = social_workspace })

    -- Special
    local special_workspace = "special"
    hl.exec_cmd("keepassxc",                                        { workspace = special_workspace .. " silent" })
    hl.exec_cmd("obs --startreplaybuffer --disable-shutdown-check", { workspace = special_workspace .. " silent" })
    hl.exec_cmd("flatpak run md.obsidian.Obsidian",                 { workspace = special_workspace .. " silent" })
    hl.exec_cmd("xdg-terminal-exec ncmpcpp",                        { workspace = special_workspace .. " silent" })

    -- Move broken apps manually
    hl.exec_cmd(string.format(
        "sleep 2 && hyprctl dispatch 'hl.dsp.window.move({ workspace = \"%s\", follow = false, window = \"class:obsidian\" })'",
        special_workspace))
    hl.exec_cmd(string.format(
        "sleep 2.5 && hyprctl dispatch 'hl.dsp.window.move({ workspace = \"%s\", follow = false, window = \"title:ncmpcpp\" })'",
        special_workspace))

    hl.exec_cmd("sleep 3 && hyprlock")
end)
