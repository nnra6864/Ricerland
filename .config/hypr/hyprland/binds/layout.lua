local defs = require("defs")

-- Window
hl.bind(defs.main_mod .. "+ F",         hl.dsp.window.fullscreen("fullscreen"))
hl.bind(defs.main_mod .. "+ ALT + F",   hl.dsp.window.fullscreen("maximized"))
hl.bind(defs.main_mod .. "+ V",         hl.dsp.window.float())
hl.bind(defs.main_mod .. "+ X",         hl.dsp.window.center())
hl.bind(defs.main_mod .. "+ Z",         hl.dsp.window.pseudo())
hl.bind(defs.main_mod .. "+ P",         hl.dsp.layout("promote"))
hl.bind(defs.main_mod .. "+ ALT + P",   hl.dsp.window.pin())
hl.bind(defs.main_mod .. "+ O",         hl.dsp.window.set_prop({ prop = "opaque", value = "toggle" }))
hl.bind(defs.main_mod .. "+ C",         hl.dsp.window.close())
hl.bind(defs.main_mod .. "+ SHIFT + C", hl.dsp.window.kill())

-- Directional
local resize_step = 100
local directional_keys = {
    Left = "l", Right = "r", Up = "u", Down = "d",
    H    = "l", L     = "r", K  = "u", J    = "d"
}

for k, d in pairs(directional_keys) do
    -- Focus
    hl.bind(defs.main_mod .. "+" ..k, hl.dsp.focus({ direction = d }))

    -- Move
    hl.bind(defs.main_mod .. "+ ALT +"..k, hl.dsp.window.move({ direction = d }))

    -- Swap
    hl.bind(defs.main_mod .. "+ SHIFT +"..k, hl.dsp.window.swap({ direction = d }))

    -- Resize
    local x, y = 0, 0
    if     d == "l" then x = -resize_step elseif d == "r" then x = resize_step
    elseif d == "u" then y = -resize_step elseif d == "d" then y = resize_step end
    hl.bind(defs.main_mod .. "+ CTRL +" .. k,
        hl.dsp.window.resize({ x = x, y = y, relative = true }), { repeating = true })
end

-- Focus Last/Urgent
hl.bind(defs.main_mod .. "+ period", hl.dsp.focus({ last = true }))
hl.bind(defs.main_mod .. "+ comma",  hl.dsp.focus({ urgent_or_last = true }))

-- Resize Column (Scrolling)
hl.bind(defs.main_mod .. "+ S",       hl.dsp.layout("colresize +conf"))
hl.bind(defs.main_mod .. "+ ALT + S", hl.dsp.layout("colresize -conf"))

-- Mouse
hl.bind(defs.main_mod .. "+ ALT + mouse:272", hl.dsp.window.drag(),   { mouse = true })
hl.bind(defs.main_mod .. "+ ALT + mouse:273", hl.dsp.window.resize(), { mouse = true })

-- Workspace
for i = 1, 10 do
    local key = i % 10

    -- Switch
    hl.bind(defs.main_mod .. "+" .. key, hl.dsp.focus({ workspace = i }))

    -- Move Window
    hl.bind(defs.main_mod .. "+ SHIFT +" .. key, hl.dsp.window.move({ workspace = i }))
end

-- Special (thanks ergon)
hl.bind(defs.main_mod .. "+ TAB", hl.dsp.workspace.toggle_special("special"))
hl.bind(defs.main_mod .. "+ SHIFT + TAB", function()
    if hl.get_active_monitor().active_special_workspace == nil then
        hl.dispatch(hl.dsp.window.move({ workspace = "special" }))
    else
        hl.dispatch(hl.dsp.window.move({ workspace = hl.get_active_workspace() }))
    end
end)
