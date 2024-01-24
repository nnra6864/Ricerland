local names = { "Shutdown", "Reboot", "Logout", "Lock" }
local actions = {
  "loginctl poweroff",
  "loginctl reboot",
  "loginctl kill-user nnra" .. os.getenv("USER"),
  "waylock -init-color 0x11111b -input-color 0x11111b -fail-color 0x1e1e2e"
}

local str = ""
for _, v in ipairs(names) do
  str = str .. v .. "\n"
end

local fd = assert(io.popen("rofi -d > /tmp/rofi", "w"))
fd:write(str)
fd:close()
local out = assert(io.open("/tmp/rofi"))
local action = assert(out:read())
out:close()

for i in ipairs(names) do
  if names[i] == action then
    os.execute(actions[i])
  end
end
