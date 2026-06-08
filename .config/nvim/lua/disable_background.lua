vim.cmd [[
   augroup MyColors
   autocmd!
   autocmd ColorScheme * highlight Normal guibg=none
   autocmd ColorScheme * highlight NonText guibg=none
   autocmd ColorScheme * highlight SignColumn guibg=none
   autocmd ColorScheme * highlight StatusLine guibg=none
   autocmd ColorScheme * highlight StatusLineNC guibg=none
   autocmd ColorScheme * highlight LineNr guibg=none
   autocmd ColorScheme * highlight CursorLineNr guibg=none
   autocmd ColorScheme * highlight VertSplit guibg=none
   autocmd ColorScheme * highlight Pmenu guibg=none
   autocmd ColorScheme * highlight PmenuSel guibg=none
   autocmd ColorScheme * highlight PmenuSbar guibg=none
   autocmd ColorScheme * highlight PmenuThumb guibg=none
   autocmd ColorScheme * highlight TabLine guibg=none
   autocmd ColorScheme * highlight TabLineFill guibg=none
   autocmd ColorScheme * highlight TabLineSel guibg=none
   autocmd ColorScheme * highlight Visual guibg=none
   augroup END
   augroup MyHighlightOverrides
   autocmd!
   autocmd ColorScheme * highlight CursorLine guibg=NONE
   autocmd ColorScheme * highlight StatusLine guibg=NONE
   augroup END
]]
