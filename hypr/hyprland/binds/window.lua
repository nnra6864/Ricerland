local defs = require("defs")

-- Layout
hl.bind(defs.main_mod .. "+ F",       hl.dsp.window.fullscreen("fullscreen"))
hl.bind(defs.main_mod .. "+ ALT + F", hl.dsp.window.fullscreen("maximized"))
hl.bind(defs.main_mod .. "+ V",       hl.dsp.window.float())
hl.bind(defs.main_mod .. "+ X",       hl.dsp.window.center())
hl.bind(defs.main_mod .. "+ Z",       hl.dsp.window.pseudo())
hl.bind(defs.main_mod .. "+ P",       hl.dsp.layout("promote"))
hl.bind(defs.main_mod .. "+ ALT + P", hl.dsp.window.pin())
hl.bind(defs.main_mod .. "+ O",       hl.dsp.window.set_prop({ prop = "opaque", value = "toggle" }))

-- Close
hl.bind(defs.main_mod .. "+ C",         hl.dsp.window.close())
hl.bind(defs.main_mod .. "+ SHIFT + C", hl.dsp.window.kill())

-- Focus
hl.bind(defs.main_mod .. "+ ALT + P", hl.dsp.window.pin())

hl.bind(defs.main_mod .. "+ period", hl.dsp.focus({ last = true }))
hl.bind(defs.main_mod .. "+ comma",  hl.dsp.focus({ urgent_or_last = true }))
