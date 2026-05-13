-- Hyprland
hl.env("XDG_CURRENT_DESKTOP", "Hyprland")
hl.env("XDG_SESSION_TYPE", "wayland")
hl.env("XDG_SESSION_DESKTOP", "Hyprland")

-- QT
hl.env("QT_QPA_PLATFORM", "wayland;xcb")
hl.env("QT_QPA_PLATFORMTHEME", "qt6ct")
hl.env("QT_QUICK_CONTROLS_STYLE", "org.kde.desktop")
hl.env("QT_AUTO_SCREEN_SCALE_FACTOR", "1")
hl.env("QT_SCALE_FACTOR", "1")
hl.env("QT_WAYLAND_DISABLE_WINDOWDECORATION", "1")

-- Theming
hl.env("GTK_THEME", "$theme")
hl.env("GDK_SCALE", "1")
hl.env("CLUTTER_BACKEND", "wayland")
hl.env("LS_COLORS", "di=#A3C5CC:fi=#A3C5CC:ln=#267F6F:pi=#267F6F:so=#266F7F:bd=#266F7F:cd=#266F7F:or=#7F2626:mi=#7F2626:ex=#267F32")

-- Cursor
hl.env("XCURSOR_THEME", "$cursor")
hl.env("XCURSOR_SIZE", "$cursorSize")
hl.env("HYPRCURSOR_THEME", "$cursor")
hl.env("HYPRCURSOR_SIZE", "$cursorSize")

-- Applications
hl.env("TERMINAL", "$terminal")
hl.env("MOZ_WAYLAND_USE_VAAPI", "1")
hl.env("ELECTRON_OZONE_PLATFORM_HINT", "wayland")
hl.env("_JAVA_AWT_WM_NONREPARENTING", "1")
-- Required for gparted and some other x11 apps
hl.env("XAUTHORITY", os.getenv("HOME") .. "/.Xauthority")
-- Makes WiVRn recentering work
hl.env("OXR_RECENTER_STAGE", "1")

-- Nvidia
hl.env("LIBVA_DRIVER_NAME", "nvidia")
hl.env("GBM_BACKEND", "nvidia-drm")
hl.env("EGL_PLATFORM", "wayland")
hl.env("WLR_USE_LIBINPUT", "1")
hl.env("__GL_VRR_ALLOWED", "0")
hl.env("__GL_GSYNC_ALLOWED", "1")
hl.env("__GLX_VENDOR_LIBRARY_NAME", "nvidia")
hl.env("NVD_BACKEND", "direct")

-- Accessibility, needed for hints(https://github.com/AlfredoSequeida/hints)
hl.env("ACCESSIBILITY_ENABLED", "1")
hl.env("GTK_MODULES", "gail:atk-bridge")
hl.env("OOO_FORCE_DESKTOP", "gnome")
hl.env("GNOME_ACCESSIBILITY", "1")
hl.env("QT_ACCESSIBILITY", "1")
hl.env("QT_LINUX_ACCESSIBILITY_ALWAYS_ON", "1")
