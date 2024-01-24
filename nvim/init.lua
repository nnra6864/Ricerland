local vars = require("vars")
local vim = vim

-- sets the keymap prefixed by a leader key
local function set_keymap(mode, keymap, command, opts)
  vim.api.nvim_set_keymap(
    mode, "<leader>" .. keymap,
    command[1] .. command[2] .. command[3], opts
  )
end

-- small optimization
local to_disable = {
  "netrw", "netrwPlugin", "netrwSettings", "netrwFileHandlers", "gzip", "zip",
  "zipPlugin", "tar", "tarPlugin", "getscript", "getscriptPlugin", "vimball",
  "vimballPlugin", "2html_plugin", "logipat", "rrhelper", "spellfile_plugin",
  "matchit"
}
for _, plugin in ipairs(to_disable) do
  vim.g["loaded_" .. plugin] = 0
end

-- lazy.nvim configuration, also downloads it if not yet downloaded
local lazypath = vim.fn.stdpath("data") .. "/lazy/lazy.nvim"
if not vim.loop.fs_stat(lazypath) then
  vim.fn.system({
    "git", "clone", "--filter=blob:none", "--depth=1",
    "https://github.com/folke/lazy.nvim.git", "--branch=stable", lazypath
  })
end
vim.opt.rtp:prepend(lazypath)
require("lazy").setup(vars.packages)

vim.cmd("colorscheme " .. vars.colorscheme)

vim.defer_fn(function()
  require("autoclose").setup({})
  require("true-zen").setup({})
  require("nvim-tree").setup({})
  require("noice").setup({})
  require("lualine").setup({
    options = {
      component_separators = "",
      globalstatus = false
    },
    tabline = {
      lualine_b = { "buffers" }
    },
    sections = {
      lualine_a = { "mode" },
      lualine_b = { "branch", "diff" },
      lualine_c = {},
      lualine_x = { { "diagnostics", update_in_insert = false } },
      lualine_y = { "progress" },
      lualine_z = { "location" }
    },
    extensions = { "nvim-tree", "lazy", "trouble" }
  })
end, 0)

-- cmp setup
vim.defer_fn(function()
  local luasnip = require("luasnip")
  require("luasnip.loaders.from_vscode").lazy_load()
  local cmp = require("cmp")
  cmp.setup({
    sources = {
      { name = "nvim_lsp" },
      { name = "luasnip" },
      { name = "buffer" },
      { name = "path" },
    },
    window = { documentation = cmp.config.window.bordered() },
    formatting = {
      fields = { "menu", "abbr", "kind" },
      format = function(entry, item)
        item.menu = vars.autocomplete_icons[entry.source.name]
        return item
      end
    },
    mapping = {
      ["<Tab>"] = cmp.mapping(function(fallback)
        if cmp.visible() then
          cmp.select_next_item()
        elseif luasnip.expand_or_jumpable() then
          luasnip.expand_or_jump()
        else
          fallback()
        end
      end, { "i", "s" }),
      ["<S-Tab>"] = cmp.mapping(function(fallback)
        if cmp.visible() then
          cmp.select_prev_item()
        elseif luasnip.jumpable(-1) then
          luasnip.jump(-1)
        else
          fallback()
        end
      end, { "i", "s" }),
      ["<C-u>"]   = cmp.mapping.scroll_docs(-3),
      ["<C-d>"]   = cmp.mapping.scroll_docs(3),
      ["<C-e>"]   = cmp.mapping.abort(),
      ["<CR>"]    = cmp.mapping.confirm({ select = false }),
    },
    snippet = {
      expand = function(args) luasnip.lsp_expand(args.body) end
    }
  })
end, 0)

-- lsp setup
local lspconfig = require("lspconfig")
for _, name in ipairs(vars.lsps) do
  lspconfig[name].setup({
    capabilities = require("cmp_nvim_lsp").default_capabilities()
  })
end
vim.api.nvim_create_autocmd("LspAttach", {
  callback = function()
    for _, keymap in ipairs(vars.lsp_keymaps) do
      set_keymap(
        "n", keymap[1],
        { ":lua vim.", keymap[2], "()<cr>" }, {}
      )
    end
  end
})
for _, sign in ipairs(vars.lsp_signs) do
  vim.fn.sign_define(sign.name, {
    texthl = "DiagnosticSign" .. sign.name,
    text   = sign.text,
    numhl  = ""
  })
end

-- load keymaps
vim.defer_fn(function()
  for _, keymap in ipairs(vars.keymaps) do
    set_keymap(
      "n", keymap[1],
      { ":", keymap[2], "<cr>" }, { silent = true, noremap = true }
    )
  end
end, 0)
