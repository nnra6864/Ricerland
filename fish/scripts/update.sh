#!/usr/bin/env bash
# Updates the entire system

# Clear cache to avoid issues and update the pacman and AUR packages
yes | paru -Scc

# Update packages and hyprpm
paru -Syu --sudoloop \
    && hyprpm update

# Update flatpak
flatpak update -y

# Clear the cache once again
yes | paru -Scc

# Notify the user system has been updated
if [ -n "$DISPLAY" ] || [ -n "$WAYLAND_DISPLAY" ]; then
    notify-send "Update" "Update complete"
    paplay /usr/share/sounds/freedesktop/stereo/message.oga &
else
    echo "Update complete"
fi
