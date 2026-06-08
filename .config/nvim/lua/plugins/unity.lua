---@module 'lazy'
---@type LazySpec
return {
  'apyra/nvim-unity-sync',
  lazy = false,
  config = function() require('unity.plugin').setup() end,
}
