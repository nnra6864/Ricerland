---@module 'lazy'
---@type LazySpec
return {
  'khoido2003/roslyn-filewatch.nvim',
  dependencies = { 'seblyng/roslyn.nvim' },
  build = 'nvim -l build.lua --', -- Compiles or downloads the Native Rust module fallback
  opts = {
    client_names = { 'roslyn' },
  },
}
