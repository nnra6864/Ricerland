---@module 'lazy'
---@type LazySpec
return {
  'L3MON4D3/LuaSnip',
  version = '2.*',
  build = (function()
    -- Build Step is needed for regex support in snippets.
    -- This step is not supported in many windows environments.
    -- Remove the below condition to re-enable on windows.
    if vim.fn.has 'win32' == 1 or vim.fn.executable 'make' == 0 then return end
    return 'make install_jsregexp'
  end)(),
  dependencies = {
    {
      'rafamadriz/friendly-snippets',
    },
  },
  config = function()
    local ls = require 'luasnip'
    require('luasnip.loaders.from_vscode').lazy_load()
    require('luasnip.loaders.from_lua').lazy_load {
      paths = { vim.fn.stdpath 'config' .. '/lua/snippets' },
    }
  end,
  opts = {},
}
