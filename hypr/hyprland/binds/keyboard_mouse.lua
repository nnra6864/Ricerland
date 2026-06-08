local defs = require("defs")

hl.bind(defs.main_mod "+ M", hl.dsp.submap("keyboard_mouse"))

hl.define_submap("keyboard_mouse", function()

    hl.bind("escape", hl.dsp.submap("reset"))
end)
