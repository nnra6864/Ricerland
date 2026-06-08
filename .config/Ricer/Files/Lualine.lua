local M = {}
M.theme = function()
	return {
		inactive = {
			a = { fg = "#ricer.col.foreground", bg = "#ricer.col.background", gui = "bold" },
			b = { fg = "#ricer.col.foreground", bg = "#ricer.col.background_alt" },
			c = { fg = "#ricer.col.foreground", bg = nil },
		},
		visual = {
			a = { fg = "#ricer.col.foreground", bg = "#ricer.col.blue", gui = "bold" },
			b = { fg = "#ricer.col.foreground", bg = "#ricer.col.background_alt" },
			c = { fg = "#ricer.col.foreground", bg = nil },
		},
		replace = {
			a = { fg = "#ricer.col.foreground", bg = "#ricer.col.red", gui = "bold" },
			b = { fg = "#ricer.col.foreground", bg = "#ricer.col.background_alt" },
			c = { fg = "#ricer.col.foreground", bg = nil },
		},
		normal = {
			a = { fg = "#ricer.col.foreground", bg = "#ricer.col.background", gui = "bold" },
			b = { fg = "#ricer.col.foreground", bg = "#ricer.col.background_alt" },
			c = { fg = "#ricer.col.foreground", bg = nil },
		},
		insert = {
			a = { fg = "#ricer.col.background", bg = "#ricer.col.green", gui = "bold" },
			b = { fg = "#ricer.col.foreground", bg = "#ricer.col.background_alt" },
			c = { fg = "#ricer.col.foreground", bg = nil },
		},
		command = {
			a = { fg = "#ricer.col.background", bg = "#ricer.col.yellow", gui = "bold" },
			b = { fg = "#ricer.col.foreground", bg = "#ricer.col.background_alt" },
			c = { fg = "#ricer.col.foreground", bg = nil },
		},
	}
end
return M
