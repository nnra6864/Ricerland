if status is-interactive
  	if test (tty) = /dev/tty1
    	if not set -q WAYLAND_DISPLAY
      	dbus-run-session Hyprland
    	end
 	end
end
alias nv='~/.local/share/applications/neovide.AppImage'
fish_vi_key_bindings
