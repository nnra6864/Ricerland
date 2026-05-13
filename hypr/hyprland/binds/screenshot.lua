local defs = require("defs");

-- Screen
hl.bind("Print", function()
    hl.dispatch(hl.dsp.exec_cmd(
        "grim - | wl-copy &&" ..
        "notify-send -t 2000 'Grim' 'Screenshot Taken!' &&" ..
        "paplay " .. defs.screenshot_sound))
end)

-- Regional
hl.bind("ALT + X", function()
    hl.dispatch(hl.dsp.exec_cmd(
        "hyprshot -m region -z -t 2000 --clipboard-only &&" ..
        "paplay " .. defs.screenshot_sound))
end)

-- Active Window
hl.bind(defs.main_mod .. "+ ALT + X", function()
    hl.dispatch(hl.dsp.exec_cmd(
        "hyprshot -m window -m active -t 2000 --clipboard-only &&" ..
        "paplay " .. defs.screenshot_sound))
end)

-- Window
hl.bind(defs.main_mod .. "+ CTRL + X", function()
    hl.dispatch(hl.dsp.exec_cmd(
        "hyprshot -m window -z -t 2000 --clipboard-only &&" ..
        "paplay " .. defs.screenshot_sound))
end)
