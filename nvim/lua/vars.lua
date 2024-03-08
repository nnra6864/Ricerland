local colorscheme = "nord"

local keymaps = {
    { "T", "NvimTreeToggle" },

    { "F", "Telescope" },
    { "ff", "Telescope find_files" },
    { "fg", "Telescope live_grep" },
    { "fb", "Telescope buffers" },
}

local lsp_keymaps = {
	{ "ld", "lsp.buf.declaration" },
	{ "li", "lsp.buf.implementation" },
	{ "lr", "lsp.buf.rename" },

	{ "d[", "diagnostics.goto_prev" },
	{ "d]", "diagnostics.goto_next" },
}

local lsps = { "clangd", "lua_ls", "omnisharp", "pylsp" }

local autocomplete_icons = {
	nvim_lsp = "L",
	luasnip = "S",
	buffer = "B",
	path = "F"
}

local lsp_signs = {
	{ name = "Error", text = "E" },
	{ name = "Warn", text = "W" },
	{ name = "Hint", text = "H" },
	{ name = "Info", text = "I" },
}

local packages = {
	-- Dependencies
	"nvim-treesitter/nvim-treesitter",
	"nvim-lua/plenary.nvim",
	"nvim-tree/nvim-web-devicons",
	"rafamadriz/friendly-snippets",
	"MunifTanjim/nui.nvim",
	"BurntSushi/ripgrep",

	-- Eyecandy
	"nvim-lualine/lualine.nvim",
  	"Pocco81/true-zen.nvim",
  	"shaunsingh/nord.nvim",
  	"folke/twilight.nvim",
	"folke/noice.nvim",

	-- LSP
  	"neovim/nvim-lspconfig",
  	"hrsh7th/cmp-nvim-lsp",
  	"hrsh7th/cmp-buffer",
  	"hrsh7th/cmp-path",
  	"hrsh7th/cmp-cmdline",
  	"hrsh7th/nvim-cmp",
  	"L3MON4D3/LuaSnip",
  	"saadparwaiz1/cmp_luasnip",
  	"folke/trouble.nvim",
	"python-lsp/python-lsp-server",

  	-- IDE
  	"nvim-telescope/telescope.nvim",
  	"nvim-tree/nvim-tree.lua",
  	"m4xshen/autoclose.nvim",
  	"folke/todo-comments.nvim"
}

-- gets rid of most warnings, don't remove
local vim = vim

-- neovim options
vim.opt.completeopt = { "menu", "menuone", "noselect" }
vim.opt.number = true
vim.opt.relativenumber = true
vim.opt.showmode = false
vim.opt.colorcolumn = "0"
vim.o.tabstop = 4
vim.o.shiftwidth = 4
vim.o.softtabstop = 0
vim.g.mapleader = "<Space>"
vim.api.nvim_set_keymap('n', '<C-c>', '"+y', { noremap = true, silent = true })
vim.api.nvim_set_keymap('v', '<C-c>', '"+y', { noremap = true, silent = true })
vim.keymap.set("n", "<leader>D", function ()
    vim.diagnostic.open_float()
end)

return {
  colorscheme = colorscheme,
  keymaps = keymaps,
  lsp_keymaps = lsp_keymaps,
  lsps = lsps,
  autocomplete_icons = autocomplete_icons,
  lsp_signs = lsp_signs,
  packages = packages
}
