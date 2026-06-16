local defs = require("defs")

-- Switch XKB Layout
hl.bind(defs.main_mod .. "+ F2", function()
    hl.dispatch(hl.dsp.exec_cmd("hyprctl switchxkblayout current next"))
    hl.dispatch(hl.dsp.exec_cmd([[
        bash -c 'notify-send -t 1000 \
            --hint=string:x-dunst-stack-tag:keyboard_layout \
            "Switched Keyboard Layout" \
            "$(hyprctl devices -j | jq -r \
            ".keyboards[] | select(.main == true) | .active_keymap")"'
    ]]))
    hl.dispatch(hl.dsp.exec_cmd(defs.sound.play_cmd .. defs.sound.instant_replay))
end)
