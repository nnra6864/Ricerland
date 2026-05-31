local defs = require("defs")


---@param offset number
---@return nil
local function zoom(offset)
    local current = hl.get_config("cursor.zoom_factor")
    if offset ~= nil then
        current = current + offset
    elseif current ~= defs.zoom.min then
        current = defs.zoom.min
    else
        current = defs.zoom.toggle_factor
    end
    current = math.max(defs.zoom.min, math.min(defs.zoom.max, current))
    hl.config({ cursor = { zoom_factor = current } })
end

hl.bind(defs.main_mod .. "+ mouse_up", function()
    zoom(defs.zoom.step)
end)

hl.bind(defs.main_mod .. "+ mouse_down", function()
    zoom(-defs.zoom.step)
end)
