require("hyprland.env.hyprland")
require("hyprland.env.applications")
require("hyprland.env.appearance")
-- TODO: Only load if nvidia gpu is detected
require("hyprland.env.nvidia")

-- Export env
hl.on("hyprland.start", function()
    hl.exec_cmd("dbus-update-activation-environment --systemd --all")
end)
