if status is-interactive
  	if test (tty) = /dev/tty1
    	if not set -q WAYLAND_DISPLAY
      	dbus-run-session Hyprland
    	end
 	end
end
alias nv='neovide'
function 8k
  ffmpeg -i $argv[1] -vf scale=7680:4320 -c:v libx265 -crf 23 -c:a copy $argv[2]
end
fish_vi_key_bindings
zoxide init fish | source
