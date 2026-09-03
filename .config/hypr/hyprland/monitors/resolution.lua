local M = {}

local STATE_DIR  = (os.getenv("XDG_STATE_HOME") or (os.getenv("HOME") .. "/.local/state")) .. "/hypr"
local STATE_FILE = STATE_DIR .. "/resolution.lua"

local profiles = {
    high = {
        { output = "", mode = "highres@highrr", position = "0x0", scale = 1 }
    },
    wide = {
        { output = "", mode = "2560x1440@240", position = "0x0", scale = 1 }
    },
}

local profile_order   = { "high", "wide" }
local default_profile = "high"

os.execute("mkdir -p " .. STATE_DIR)

local function read_state()
    local chunk = loadfile(STATE_FILE)
    if chunk then
        local ok, state = pcall(chunk)
        if ok and type(state) == "table" and state.profile then
            return state
        end
    end
    return { profile = default_profile }
end

local function write_state(profile_name)
    local f = io.open(STATE_FILE, "w")
    if not f then return end
    f:write(string.format("return { profile = %q }\n", profile_name))
    f:close()
end

function M.get(output)
    output = output or ""
    local profile_name = read_state().profile
    local profile = profiles[profile_name] or profiles[default_profile]

    for _, m in ipairs(profile) do
        if m.output == output then
            return { mode = m.mode, position = m.position, scale = m.scale }
        end
    end

    return nil
end

function M.switch(profile_name)
    local profile = profiles[profile_name]
    if not profile then
        hl.notification.create({ text = "Resolution: unknown profile '" .. tostring(profile_name) .. "'", timeout = 3 })
        return
    end

    write_state(profile_name)
    hl.exec_cmd("hyprctl reload")
end

local function index_of(name)
    for i, n in ipairs(profile_order) do
        if n == name then return i end
    end
    return 1
end

function M.next()
    local idx = index_of(read_state().profile)
    M.switch(profile_order[(idx % #profile_order) + 1])
end

function M.prev()
    local idx = index_of(read_state().profile)
    M.switch(profile_order[((idx - 2) % #profile_order) + 1])
end

return M
