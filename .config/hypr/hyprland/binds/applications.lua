local defs = require("defs")

-- Runners
hl.bind(defs.main_mod .. "+ R",     hl.dsp.exec_cmd("vicinae toggle"))
hl.bind(defs.main_mod .. "+ SPACE", hl.dsp.exec_cmd("pkill wlr-which-key || wlr-which-key"))
hl.bind(defs.main_mod .. "+ B",     hl.dsp.exec_cmd("pkill wlr-which-key || wlr-which-key --initial-keys 'a b'"))

-- Apps
hl.bind(defs.main_mod .. "+ Q",                hl.dsp.exec_cmd("xdg-terminal-exec"))
hl.bind(defs.main_mod .. "+ E",                hl.dsp.exec_cmd("xdg-terminal-exec yazi"))
hl.bind(defs.main_mod .. "+ ESCAPE",           hl.dsp.exec_cmd("xdg-terminal-exec btop"))
hl.bind(defs.main_mod .. "+ CTRL + P",         hl.dsp.exec_cmd("pkill hyprpicker || hyprpicker -a"))

-- Lock
hl.bind(defs.main_mod .. "+ CTRL + SHIFT + L", hl.dsp.exec_cmd("hyprlock"))
hl.bind("switch:off:Lid Switch",               hl.dsp.exec_cmd("hyprlock"))

-- Global
hl.bind("F10", function()
    hl.dispatch(hl.dsp.pass({ window = "class:com\\.obsproject\\.Studio" }))
end)

hl.bind("F10", function()
    hl.dispatch(hl.dsp.exec_cmd("notify-send -t 1000 'OBS' 'Instant Replay Saved'"))
    hl.dispatch(hl.dsp.exec_cmd(defs.sound.play_cmd .. defs.sound.instant_replay))
end)
