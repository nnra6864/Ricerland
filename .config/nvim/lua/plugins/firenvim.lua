---@module 'lazy'
---@type LazySpec
return {
    'glacambre/firenvim',
    build = ":call firenvim#install(0)",

    init = function()
        vim.g.firenvim_config = {
            globalSettings = { alt = "all" },
            localSettings = {
                [".*"] = {
                    cmdline  = "neovim",
                    content  = "text",
                    priority = 0,
                    selector = "textarea",
                    takeover = "never"
                }
            }
        }
    end,

    config = function()
        if vim.g.started_by_firenvim then
            -- System clipboard
            vim.keymap.set({ 'i', 'c', 't' }, '<C-S-v>', '<C-r>+', { desc = "Paste from system clipboard" })
            vim.keymap.set('n',               '<C-S-v>', '"+p',    { desc = "Paste from system clipboard" })

            vim.cmd([[
                highlight Normal guibg=NONE ctermbg=NONE
                highlight NonText guibg=NONE ctermbg=NONE
                highlight SignColumn guibg=NONE ctermbg=NONE
            ]])

            -- Resized the window to a usable size
            local min_height = 10
            local max_height = 30
            local id = vim.api.nvim_create_augroup("ExpandLinesOnTextChanged", { clear = true })

            local function resize_firenvim_window()
                local height = vim.api.nvim_win_text_height(0, {}).all
                if height > vim.o.lines then
                    if height < max_height then
                        vim.o.lines = height
                    else
                        vim.o.lines = max_height
                    end
                else
                    if height > min_height then
                        vim.o.lines = height
                    else
                        vim.o.lines = min_height
                    end
                end
            end

            -- Resizes when typing
            vim.api.nvim_create_autocmd({"TextChanged", "TextChangedI"}, {
                group = id,
                callback = resize_firenvim_window
            })
        end
    end
}
