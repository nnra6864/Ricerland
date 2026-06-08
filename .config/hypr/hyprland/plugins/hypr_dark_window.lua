if hl.plugin.darkwindow ~= nil then
      local defs = require("defs")

      hl.bind(defs.main_mod .. " + I", hl.plugin.darkwindow.dsp_shade({
      shader = "invert",
  }))
end
