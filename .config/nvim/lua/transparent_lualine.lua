local M = {}
M.theme = function()
	return {
		inactive = {
			a = { fg = "#A3C5CC", bg = "#081D26", gui = "bold" },
			b = { fg = "#A3C5CC", bg = "#0C2E3F" },
			c = { fg = "#A3C5CC", bg = nil },
		},
		visual = {
			a = { fg = "#A3C5CC", bg = "#1F5A66", gui = "bold" },
			b = { fg = "#A3C5CC", bg = "#0C2E3F" },
			c = { fg = "#A3C5CC", bg = nil },
		},
		replace = {
			a = { fg = "#A3C5CC", bg = "#661F1F", gui = "bold" },
			b = { fg = "#A3C5CC", bg = "#0C2E3F" },
			c = { fg = "#A3C5CC", bg = nil },
		},
		normal = {
			a = { fg = "#A3C5CC", bg = "#081D26", gui = "bold" },
			b = { fg = "#A3C5CC", bg = "#0C2E3F" },
			c = { fg = "#A3C5CC", bg = nil },
		},
		insert = {
			a = { fg = "#081D26", bg = "#1F6628", gui = "bold" },
			b = { fg = "#A3C5CC", bg = "#0C2E3F" },
			c = { fg = "#A3C5CC", bg = nil },
		},
		command = {
			a = { fg = "#081D26", bg = "#66661F", gui = "bold" },
			b = { fg = "#A3C5CC", bg = "#0C2E3F" },
			c = { fg = "#A3C5CC", bg = nil },
		},
	}
end
return M
