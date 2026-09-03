local STATE_DIR      = (os.getenv("XDG_STATE_HOME") or (os.getenv("HOME") .. "/.local/state")) .. "/hypr"
local STATE_FILE     = STATE_DIR .. "/resolution.lua"
local GSR_STATE_FILE = STATE_DIR .. "/gsr.lua"
local GSR_CONFIG     = os.getenv("HOME") .. "/.config/gpu-screen-recorder/config_ui"

local profiles = {
    high = {
        ["replay.record_options.fps"] = "60",
        ["record.record_options.fps"] = "60"
    },
    wide = {
        ["replay.record_options.fps"] = "120",
        ["record.record_options.fps"] = "120"
    },
}

local chunk = loadfile(STATE_FILE)
if not chunk then return end

local ok, state = pcall(chunk)
if not (ok and type(state) == "table" and state.profile) then return end

local profile_name = state.profile
local profile = profiles[profile_name]
if not profile then return end

local gsr_chunk = loadfile(GSR_STATE_FILE)
if gsr_chunk then
    local gsr_ok, gsr_state = pcall(gsr_chunk)
    if gsr_ok and type(gsr_state) == "table" and gsr_state.profile == profile_name then
        return
    end
end

local cmds = { "gsr-ui-cli toggle-replay" }
for key, value in pairs(profile) do
    table.insert(cmds, string.format(
        "sed -i -E 's|^(%s )[^ ]+|\\1%s|' %q",
        key, value, GSR_CONFIG
    ))
end
table.insert(cmds, "gsr-ui-cli reload-config")
table.insert(cmds, "gsr-ui-cli toggle-replay")

hl.exec_cmd(table.concat(cmds, " && "))

local f = io.open(GSR_STATE_FILE, "w")
if f then
    f:write(string.format("return { profile = %q }\n", profile_name))
    f:close()
end
