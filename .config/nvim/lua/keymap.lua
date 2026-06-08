local map = vim.keymap.set

-- Diagnostics
map('n', '<leader>q', vim.diagnostic.setloclist, { desc = 'Open diagnostic [Q]uickfix list' })

-- Copy to global clipboard
map({ 'n', 'i', 'v' }, '<C-S-c>', '"+y', { desc = 'Copy to clipboard' })

-- Split
map('n', '<leader>|', '<C-w><C-v>', { desc = 'Split window vertically' })
map('n', '<leader>-', '<C-w><C-s>', { desc = 'Split window horizontally' })

-- Move focus
map('n', '<C-h>', '<C-w><C-h>', { desc = 'Move focus left' })
map('n', '<C-l>', '<C-w><C-l>', { desc = 'Move focus right' })
map('n', '<C-j>', '<C-w><C-j>', { desc = 'Move focus down' })
map('n', '<C-k>', '<C-w><C-k>', { desc = 'Move focus up' })

-- Move windows
map('n', '<A-h>', '<C-w>H', { desc = 'Move window left' })
map('n', '<A-l>', '<C-w>L', { desc = 'Move window right' })
map('n', '<A-j>', '<C-w>J', { desc = 'Move window down' })
map('n', '<A-k>', '<C-w>K', { desc = 'Move window up' })

-- Clear highlights on search when pressing <Esc> in normal mode
--  See `:help hlsearch`
map('n', '<Esc>', '<cmd>nohlsearch<CR>')

-- Exit terminal mode in the builtin terminal with a shortcut that is a bit easier
-- for people to discover. Otherwise, you normally need to press <C-\><C-n>, which
-- is not what someone will guess without a bit more experience.
--
-- NOTE: This won't work in all terminal emulators/tmux/etc. Try your own mapping
-- or just use <C-\><C-n> to exit terminal mode
map('t', '<Esc><Esc>', '<C-\\><C-n>', { desc = 'Exit terminal mode' })
