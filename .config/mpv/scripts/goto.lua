-- Original code: https://github.com/occivink/mpv-scripts/blob/master/scripts/seek-to.lua
-- Rewritten by GPT for my needs

local assdraw = require("mp.assdraw")
local active = false
local input = ""
local timer = nil
local timer_duration = 3

local ass_begin = mp.get_property("osd-ass-cc/0")
local ass_end = mp.get_property("osd-ass-cc/1")

local function show_goto()
	mp.osd_message("Go to: " .. ass_begin .. input .. ass_end, timer_duration)
end

local function parse_time(time_str)
	local h, m, s = 0, 0, 0
	local parts = {}
	for part in time_str:gmatch("%d+") do
		table.insert(parts, tonumber(part))
	end
	if #parts == 1 then
		s = parts[1]
	elseif #parts == 2 then
		m, s = parts[1], parts[2]
	elseif #parts == 3 then
		h, m, s = parts[1], parts[2], parts[3]
	else
		return nil
	end
	return h * 3600 + m * 60 + s
end

local function go_to()
	local seconds = parse_time(input)
	if seconds then
		mp.commandv("osd-bar", "seek", seconds, "absolute")
	end
	set_inactive()
end

local function set_active()
	if not mp.get_property("seekable") then
		return
	end
	input = ""
	show_goto()
	for i = 0, 9 do
		local num = tostring(i)
		mp.add_forced_key_binding(num, "goto-" .. num, function()
			input = input .. num
			show_goto()
		end)
	end
	mp.add_forced_key_binding(":", "goto-colon", function()
		if not input:find(":$") then -- Prevent duplicate colons
			input = input .. ":"
			show_goto()
		end
	end)
	mp.add_forced_key_binding("BS", "goto-bs", function()
		input = input:sub(1, -2)
		show_goto()
	end)
	mp.add_forced_key_binding("ESC", "goto-esc", set_inactive)
	mp.add_forced_key_binding("ENTER", "goto-enter", go_to)
	timer = mp.add_periodic_timer(timer_duration, show_goto)
	active = true
end

function set_inactive()
	mp.osd_message("")
	for i = 0, 9 do
		mp.remove_key_binding("goto-" .. i)
	end
	mp.remove_key_binding("goto-colon")
	mp.remove_key_binding("goto-bs")
	mp.remove_key_binding("goto-esc")
	mp.remove_key_binding("goto-enter")
	if timer then
		timer:kill()
	end
	active = false
end

mp.add_key_binding(nil, "toggle-goto", function()
	if active then
		set_inactive()
	else
		set_active()
	end
end)
