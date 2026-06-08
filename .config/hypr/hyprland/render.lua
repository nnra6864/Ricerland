hl.config({
    render = {
        -- Sends frames directly to the GPU instead of Hyprland
        -- Reduces latency but can introduce issues
        -- 0 - off, 1 - on, 2 - auto (on with content type ‘game’)
        direct_scanout = 0,
    }
})
