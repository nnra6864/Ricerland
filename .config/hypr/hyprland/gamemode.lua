hl.bind("SUPER + F1", function ()
    local game_mode = (hl.get_config("animations.enabled") == false)

    if game_mode then
        hl.exec_cmd("hyprctl reload")
        return
    end

    hl.config({
        general = {
            gaps_in = 0, gaps_out = 0,
            border_size = 0,
        },

        animations = {
            enabled = false,
        },

        decoration = {
            shadow = { enabled = false },
            glow = { enabled = false },
            blur = { enabled = false },
            rounding = 0,
        }
    })
end)
