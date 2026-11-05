local defs       = require("defs")
local resolution = require("hyprland.monitors.resolution")

-- Resolution
hl.bind(defs.main_mod .. "+ bracketright", function() resolution.next() end, {
    locked = true,
    description = "Switch to next resolution preset",
})
hl.bind(defs.main_mod .. "+ bracketleft",  function() resolution.prev() end, {
    locked = true,
    description = "Switch to previous resolution preset",
})

-- Window
hl.bind(defs.main_mod .. "+ F",         hl.dsp.window.fullscreen("fullscreen"),
    { description = "Toggle window fullscreen" })
hl.bind(defs.main_mod .. "+ ALT + F",   hl.dsp.window.fullscreen("maximized"),
    { description = "Toggle window maximized" })
hl.bind(defs.main_mod .. "+ V",         hl.dsp.window.float(),
    { description = "Toggle window float" })
hl.bind(defs.main_mod .. "+ Z",         hl.dsp.window.pseudo(),
    { description = "Toggle window pseudo" })
hl.bind(defs.main_mod .. "+ O",         hl.dsp.window.set_prop({ prop = "opaque", value = "toggle" }),
    { description = "Toggle window opaque" })
hl.bind(defs.main_mod .. "+ X",         hl.dsp.window.center(),
    { description = "Center window" })
hl.bind(defs.main_mod .. "+ P",         hl.dsp.layout("promote"),
    { description = "Promote window" })
hl.bind(defs.main_mod .. "+ ALT + P",   hl.dsp.window.pin(),
    { description = "Pin window" })
hl.bind(defs.main_mod .. "+ C",         hl.dsp.window.close(),
    { description = "Close window" })
hl.bind(defs.main_mod .. "+ SHIFT + C", hl.dsp.window.kill(),
    { description = "Kill window" })

-- Directional
local resize_step = 100
local directional_keys = {
    Left = "l", Right = "r", Up = "u", Down = "d",
    H    = "l", L     = "r", K  = "u", J    = "d"
}

for k, d in pairs(directional_keys) do
    -- Focus
    hl.bind(defs.main_mod .. "+" .. k, hl.dsp.focus({ direction = d }),
        { description = "Focus window " .. d })

    -- Move
    hl.bind(defs.main_mod .. "+ ALT +".. k, hl.dsp.window.move({ direction = d }),
        { description = "Move window " .. d })

    -- Swap
    hl.bind(defs.main_mod .. "+ SHIFT +".. k, hl.dsp.window.swap({ direction = d }),
        { description = "Swap window " .. d })

    -- Resize
    local x, y = 0, 0
    if     d == "l" then x = -resize_step elseif d == "r" then x = resize_step
    elseif d == "u" then y = -resize_step elseif d == "d" then y = resize_step end
    hl.bind(defs.main_mod .. "+ CTRL +" .. k,
        hl.dsp.window.resize({ x = x, y = y, relative = true }), {
            repeating = true,
            description = "Resize window " .. x .. " " .. y,
        }
    )
end

-- Toggle Tiled/Floating Focus
hl.bind(defs.main_mod .. "+ CTRL + F", function()
    local window = hl.get_active_window()
    if not window then return end
    hl.dispatch(hl.dsp.window.cycle_next({
        floating = not window.floating,
        tiled = window.floating,
    }))
end, { description = "Switch focus between tiled and floating windows" })

-- Focus Last/Urgent
hl.bind(defs.main_mod .. "+ period", hl.dsp.focus({ last = true }),
    { description = "Focus last" })
hl.bind(defs.main_mod .. "+ comma",  hl.dsp.focus({ urgent_or_last = true }),
    { description = "Focus urgent" })

-- Resize Column (Scrolling)
hl.bind(defs.main_mod .. "+ S",       hl.dsp.layout("colresize +conf"),
    { description = "Resize column +" })
hl.bind(defs.main_mod .. "+ ALT + S", hl.dsp.layout("colresize -conf"),
    { description = "Resize column -" })

-- Mouse
hl.bind(defs.main_mod .. "+ ALT + mouse:272", hl.dsp.window.drag(), {
    mouse = true,
    description = "Drag window",
})
hl.bind(defs.main_mod .. "+ ALT + mouse:273", hl.dsp.window.resize(), {
    mouse = true,
    description = "Resize window",
})

-- Workspace
for i = 1, 10 do
    local key = i % 10

    -- Switch
    hl.bind(defs.main_mod .. "+" .. key, hl.dsp.focus({ workspace = i }),
        { description = "Focus workspace " .. i })

    -- Move Window
    hl.bind(defs.main_mod .. "+ SHIFT +" .. key, hl.dsp.window.move({ workspace = i }),
        { description = "Move to workspace " .. i })
end

-- Special (thanks ergon)
hl.bind(defs.main_mod .. "+ TAB", hl.dsp.workspace.toggle_special("special"),
    { description = "Toggle special workspace" })
hl.bind(defs.main_mod .. "+ SHIFT + TAB", function()
    if hl.get_active_monitor().active_special_workspace == nil then
        hl.dispatch(hl.dsp.window.move({ workspace = "special" }))
    else
        hl.dispatch(hl.dsp.window.move({ workspace = hl.get_active_workspace() }))
    end
end, { description = "Move to special workspace" })
