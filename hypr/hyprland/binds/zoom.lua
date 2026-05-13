local defs = require("defs")

hl.bind(defs.main_mod .. "+ mouse_down", hl.dsp.exec_cmd(
    "hyprctl -q keyword cursor:zoom_factor $(hyprctl getoption cursor:zoom_factor -j | jq '.float * 1.1')"))

hl.bind(defs.main_mod .. "+ mouse_up",   hl.dsp.exec_cmd(
    "hyprctl -q keyword cursor:zoom_factor $(hyprctl getoption cursor:zoom_factor -j | jq '(.float * 0.9) | if . < 1 then 1 else . end')"))

hl.bind(defs.main_mod .. "+ equal",      hl.dsp.exec_cmd(
    "hyprctl -q keyword cursor:zoom_factor $(hyprctl getoption cursor:zoom_factor -j | jq '.float * 1.5')"),
    { repeating = true })

hl.bind(defs.main_mod .. "+ minus",      hl.dsp.exec_cmd(
    "hyprctl -q keyword cursor:zoom_factor $(hyprctl getoption cursor:zoom_factor -j | jq '(.float * 0.5) | if . < 1 then 1 else . end')"),
    { repeating = true })
