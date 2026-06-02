if hl.plugin.csgo_vulkan_fix ~= nil then
    local p = hl.plugin.csgo_vulkan_fix

    p.vkfix_app({ app = "hl_linux",      w = 1280, h = 1024 })
    p.vkfix_app({ app = "csgo_linux64",  w = 1280, h = 1024 })
    p.vkfix_app({ app = "steam_app_730", w = 1280, h = 1024 })
end
