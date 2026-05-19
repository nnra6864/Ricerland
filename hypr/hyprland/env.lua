local defs = require("defs")

-- Hyprland
hl.env("XDG_CURRENT_DESKTOP", "Hyprland")
hl.env("XDG_SESSION_TYPE", "wayland")
hl.env("XDG_SESSION_DESKTOP", "Hyprland")

-- Applications
hl.env("TERMINAL", defs.apps.terminal)
hl.env("ELECTRON_OZONE_PLATFORM_HINT", "wayland")
hl.env("SDL_VIDEODRIVER", "wayland")
hl.env("CLUTTER_BACKEND", "wayland")

-- QT
hl.env("QT_QPA_PLATFORM", "wayland;xcb")
hl.env("QT_QPA_PLATFORMTHEME", defs.theme.qt.platform_theme)
hl.env("QT_QUICK_CONTROLS_STYLE", defs.theme.qt.quick_controls_style)
hl.env("QT_SCALE_FACTOR", defs.theme.qt.scaling)
hl.env("QT_AUTO_SCREEN_SCALE_FACTOR", defs.theme.qt.screen_scaling)
hl.env("QT_WAYLAND_DISABLE_WINDOWDECORATION", defs.theme.qt.disable_window_decoration)

-- GTK
hl.env("GDK_BACKEND", "wayland,x11,*")
hl.env("GTK_THEME", defs.theme.gtk.name)
hl.env("GDK_SCALE", defs.theme.gtk.scaling)

-- Cursor
hl.env("XCURSOR_THEME", defs.cursor.name)
hl.env("XCURSOR_SIZE", defs.cursor.size)
hl.env("HYPRCURSOR_THEME", defs.cursor.name)
hl.env("HYPRCURSOR_SIZE", defs.cursor.size)

-- Mozilla
hl.env("MOZ_ENABLE_WAYLAND", "1")
hl.env("MOZ_WAYLAND_USE_VAAPI", "1")
hl.env("MOZ_DBUS_REMOTE", "1")
hl.env("MOZ_ACCELERATED", "1")
hl.env("MOZ_WEBRENDERER", "1")

-- Makes WiVRn recentering work
hl.env("OXR_RECENTER_STAGE", "1")

-- Java
hl.env("_JAVA_AWT_WM_NONREPARENTING", "1")

-- Required for gparted and some other x11 apps
hl.env("XAUTHORITY", os.getenv("HOME") .. "/.Xauthority")

-- Term colors
hl.env("LS_COLORS", defs.theme.color.ls)

-- Nvidia
hl.env("NVD_BACKEND", "direct")
hl.env("GBM_BACKEND", "nvidia-drm")
hl.env("LIBVA_DRIVER_NAME", "nvidia")
hl.env("__GLX_VENDOR_LIBRARY_NAME", "nvidia")
hl.env("__GL_GSYNC_ALLOWED", "1")
hl.env("__GL_VRR_ALLOWED", "0")
