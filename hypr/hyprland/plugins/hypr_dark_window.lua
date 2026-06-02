local defs = require("defs")

if hl.plugin.darkwindow ~= nil then
      hl.bind(defs.main_mod .. " + I", hl.plugin.darkwindow.dsp_shade({
      shader = "invert",
  }))
end
