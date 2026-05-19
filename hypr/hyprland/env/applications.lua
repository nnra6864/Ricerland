local defs = require("defs")

-- Terminal
hl.env("TERMINAL", defs.apps.terminal)

-- Wayland
hl.env("QT_QPA_PLATFORM",              "wayland;xcb")
hl.env("GDK_BACKEND",                  "wayland,x11,*")
hl.env("ELECTRON_OZONE_PLATFORM_HINT", "wayland")
hl.env("SDL_VIDEODRIVER",              "wayland")
hl.env("CLUTTER_BACKEND",              "wayland")

-- Mozilla
hl.env("MOZ_ENABLE_WAYLAND",    "1")
hl.env("MOZ_WAYLAND_USE_VAAPI", "1")
hl.env("MOZ_DBUS_REMOTE",       "1")
hl.env("MOZ_ACCELERATED",       "1")
hl.env("MOZ_WEBRENDERER",       "1")

-- WiVRn recentering
hl.env("OXR_RECENTER_STAGE", "1")

-- Required for some x11 apps
hl.env("XAUTHORITY", os.getenv("HOME") .. "/.Xauthority")

-- Java
hl.env("_JAVA_AWT_WM_NONREPARENTING", "1")
