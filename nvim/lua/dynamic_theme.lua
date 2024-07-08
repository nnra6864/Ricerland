local M = {}

local function set_theme(theme_name)
	if type(theme_name) ~= "string" then
		--print("Invalid theme name. Expected string, got " .. type(theme_name))
		return
	end
	local ok, _ = pcall(vim.cmd.colorscheme, theme_name)
	if not ok then
		print("Theme not installed: " .. theme_name)
	end
end

function M.ReloadTheme()
	package.loaded["theme"] = nil
	local ok, theme_module = pcall(require, "theme")
	if ok then
		if type(theme_module) == "string" then
			set_theme(theme_module)
		else
			--print("Invalid theme returned. Expected string, got " .. type(theme_module))
		end
	else
		print("Failed to reload theme module: " .. tostring(theme_module))
	end
end

local theme_file = vim.fn.expand("~/.config/nvim/lua/theme.lua")
local watcher = vim.loop.new_fs_event()

local function start_watcher()
	if not watcher then
		watcher = vim.loop.new_fs_event()
	end

	if watcher then
		local function on_change(err, filename, status)
			if err then
				print("Error watching file: " .. err)
				return
			end

			vim.schedule(function()
				--print("Theme file changed. Reloading...")
				M.ReloadTheme()

				watcher:stop()
				start_watcher()
			end)
		end

		local ok, err = pcall(function()
			watcher:start(theme_file, {}, vim.schedule_wrap(on_change))
		end)

		if not ok then
			print("Failed to start watcher: " .. tostring(err))
			watcher = nil
		end
	else
		print("Failed to create fs_event watcher")
	end
end

function M.setup()
	local theme = require("theme")
	if type(theme) == "string" then
		set_theme(theme)
	else
		--print("Invalid initial theme. Expected string, got " .. type(theme))
	end

	vim.cmd([[command! ReloadTheme lua require('dynamic_theme').ReloadTheme()]])

	start_watcher()

	vim.api.nvim_create_autocmd("VimLeavePre", {
		callback = function()
			if watcher then
				watcher:stop()
			end
		end,
	})

	if not watcher then
		print("Using autocmd fallback for theme file watching")
		vim.api.nvim_create_autocmd({ "BufWritePost" }, {
			pattern = { theme_file },
			callback = function()
				M.ReloadTheme()
			end,
		})
	end
end

return M
