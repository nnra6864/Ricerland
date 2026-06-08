-- Original: https://github.com/stax76/mpv-scripts/blob/main/delete_current_file.lua
-- Modified by nnra+Gemini

local key_bindings = {}

function file_exists(name)
	if not name or name == "" then
		return false
	end
	local f = io.open(name, "r")
	if f ~= nil then
		io.close(f)
		return true
	else
		return false
	end
end

function is_protocol(path)
	return type(path) == "string" and (path:match("^%a[%a%d_-]+://"))
end

function delete_file(path)
	local is_windows = package.config:sub(1, 1) == "\\"
	if is_protocol(path) or not file_exists(path) then
		return
	end
	if is_windows then
		local ps_code = [[
            Add-Type -AssemblyName Microsoft.VisualBasic
            [Microsoft.VisualBasic.FileIO.FileSystem]::DeleteFile('__path__', 'OnlyErrorDialogs', 'SendToRecycleBin')
        ]]
		local escaped_path = string.gsub(path, "'", "''")
		escaped_path = string.gsub(escaped_path, "’", "’’")
		escaped_path = string.gsub(escaped_path, "%%", "%%%%")
		ps_code = string.gsub(ps_code, "__path__", escaped_path)
		mp.command_native({
			name = "subprocess",
			playback_only = false,
			detach = true,
			args = { "powershell", "-NoProfile", "-Command", ps_code },
		})
	else
		mp.command_native({ name = "subprocess", playback_only = false, detach = true, args = { "trash", path } })
	end
end

function remove_current_file()
	local pos = mp.get_property_number("playlist-pos")
	if pos > -1 then
		mp.command("playlist-next")
		mp.command("playlist-remove " .. pos)
	end
end

function handle_confirm_key()
	local path = mp.get_property("path")
	if _G.file_to_delete == path then
		mp.commandv("show-text", "")
		delete_file(_G.file_to_delete)
		remove_current_file()
		cleanup()
	end
end

function handle_cancel_key()
	mp.commandv("show-text", "Deletion cancelled")
	mp.add_timeout(1, cleanup)
end

function cleanup()
	remove_bindings()
	_G.file_to_delete = nil
	mp.commandv("show-text", "")
end

function get_bindings()
	return {
		{ "y", handle_confirm_key },
		{ "n", handle_cancel_key },
	}
end

function add_bindings()
	if #key_bindings > 0 then
		return
	end
	local script_name = mp.get_script_name()
	local bindings_to_add = get_bindings()
	for _, bind in ipairs(bindings_to_add) do
		local name = script_name .. "_key_" .. bind[1]
		key_bindings[#key_bindings + 1] = name
		mp.add_forced_key_binding(bind[1], name, bind[2], { repeatable = false })
	end
end

function remove_bindings()
	if #key_bindings == 0 then
		return
	end
	for _, name in ipairs(key_bindings) do
		mp.remove_key_binding(name)
	end
	key_bindings = {}
end

function client_message(event)
	if event.args[1] == "delete-file" and #event.args == 1 then
		local path = mp.get_property("path")
		if _G.file_to_delete then
			mp.commandv("show-text", "Delete confirmation already pending!")
			return
		end
		_G.file_to_delete = path
		add_bindings()
		local confirm_message = "Trash '" .. mp.get_property("filename") .. "'? (Y/N)"
		mp.commandv("show-text", confirm_message, "10000")
		mp.add_timeout(10, cleanup)
	end
end

mp.register_event("client-message", client_message)
