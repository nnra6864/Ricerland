#!/usr/bin/env bash
# Updates the entire system

# Clear cache to avoid issues and update the pacman and AUR packages
yes | paru -Scc

# Update packages and hyprpm
paru -Syu --devel --sudoloop \
    aur/hyprland-protocols-git \
    aur/hyprwayland-scanner-git \
    aur/hyprutils-git \
    aur/hyprgraphics-git \
    aur/hyprlang-git \
    aur/hyprcursor-git \
    aur/aquamarine-git \
    aur/xdg-desktop-portal-hyprland-git \
    aur/hyprwire-git \
    aur/hyprtoolkit-git \
    aur/hyprland-git \
    aur/hypridle-git \
    aur/hyprland-guiutils-git \
    aur/hyprland-qt-support-git \
    aur/hyprlock-git \
    aur/hyprpaper-git \
    aur/hyprpicker-git \
    aur/hyprpolkitagent-git \
    aur/hyprpwcenter-git \
    aur/hyprshutdown-git \
    aur/hyprsunset-git \
    aur/hyprsysteminfo-git \
    aur/hyprshade-git \
    && hyprpm update

# Update yazi pkgs
ya pkg upgrade

# Update flatpak
flatpak update -y

# Update fonts
"$HOME/Projects/Fonts/maple-font/update.sh"

# Clear the cache once again
yes | paru -Scc

# Notify the user system has been updated
if [ -n "$DISPLAY" ] || [ -n "$WAYLAND_DISPLAY" ]; then
    notify-send "Update" "Update complete"
    paplay /usr/share/sounds/freedesktop/stereo/message.oga &
else
    echo "Update complete"
fi
