local defs = require("defs")

hl.define_submap("passthru", function()
  hl.bind(defs.main_mod .. "+ Escape", hl.dsp.submap("reset"))
end)
hl.bind(defs.main_mod .. "+ CTRL + ALT + P", hl.dsp.submap("passthru"))
