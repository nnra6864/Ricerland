local defs = require("defs")

-- GTK
hl.env("GTK_THEME", defs.theme.gtk.name)
hl.env("GDK_SCALE", defs.theme.gtk.scaling)

-- QT
hl.env("QT_QPA_PLATFORMTHEME",                defs.theme.qt.platform_theme)
hl.env("QT_STYLE_OVERRIDE",                   defs.theme.qt.style)
hl.env("QT_QUICK_CONTROLS_STYLE",             defs.theme.qt.quick_controls_style)
hl.env("QT_WAYLAND_DISABLE_WINDOWDECORATION", defs.theme.qt.disable_window_decoration)
hl.env("QT_SCALE_FACTOR",                     defs.theme.qt.scaling)
hl.env("QT_AUTO_SCREEN_SCALE_FACTOR",         defs.theme.qt.screen_scaling)

-- Cursor
hl.env("HYPRCURSOR_THEME", defs.cursor.name)
hl.env("XCURSOR_THEME",    defs.cursor.name)
hl.env("HYPRCURSOR_SIZE",  defs.cursor.size)
hl.env("XCURSOR_SIZE",     defs.cursor.size)

-- Term colors
--hl.env("LS_COLORS", defs.theme.color.ls)
