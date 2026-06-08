---@module 'lazy'
---@type LazySpec
return {
  'NeogitOrg/neogit',
  lazy = true,
  dependencies = {
    'nvim-lua/plenary.nvim',
    'esmuellert/codediff.nvim', -- optional
    'm00qek/baleia.nvim', -- optional
    'nvim-telescope/telescope.nvim', -- optional
  },
  cmd = 'Neogit',
  keys = {
    { '<leader>gg', '<cmd>Neogit<cr>', desc = 'Show Neogit UI' },
  },
}
