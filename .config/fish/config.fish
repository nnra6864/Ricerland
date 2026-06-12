# Customize prompt (Obsolete, using omp now)
# set -g tide_left_prompt_items time os vi_mode pwd git
# set -g tide_right_prompt_items
# set -g tide_right_prompt_suffix ''
# set -g tide_time_format '%T'

# Load user scripts
fish_add_path ~/.config/fish/scripts

# Aliases
alias doas='sudo'
alias nlear='clear; fastfetch; echo ""'
alias py='python'
alias timeshit='timeshift'
alias hdr='ENABLE_HDR_WSI=1 mpv --vo=gpu-next --target-colorspace-hint --gpu-api=vulkan --gpu-context=waylandvk --target-colorspace-hint-mode=source'
alias lg='lazygit'
alias lnw='sh ~/.config/hypr/HyprlandUnityFix/ListNewWindows.sh'
alias tm='tmux new -As'
alias ls='zl -la --dir-grouping=before --no-permissions --no-group -C ~/.config/zlist/zlist.zon'

# Paths
alias cfg='cd ~/.config/'
alias hypr='cd ~/.config/hypr/; nvim ./'
alias fsh='cd ~/.config/fish/; nvim ./'
alias nisu='cd ~/.config/Nisualizer/; nvim ./'
alias ricer='cd ~/.config/Ricer/; nvim ./'

# Rices the system with the provided config
function rice
    # Start the Shifter transition
    #python ~/Data/Projects/Shifter/Shifter.py > /dev/null 2>&1 &
    #set pid $last_pid
    #disown
    #sleep 0.2

    # Remove all the CS2 fonts(can cause crashes)
    #rm -rf ~/Data/SteamLibrary/steamapps/common/Counter-Strike\ Global\ Offensive/game/csgo/panorama/fonts/*

    # Execute Ricer
    python ~/Data/Projects/Ricer/Ricer.py $argv &&

    # Reload terminal cfg
    systemctl reload --user app-com.mitchellh.ghostty.service
    #kill -SIGUSR1 (pgrep kitty)

    # Remove all the gtk files to avoid conflicts
    rm -rf ~/.gtkrc-2.0 ~/.config/gtk-3.0/settings.ini ~/.config/gtk-4.0/ ~/.icons/default/index.theme

    # Generate Themix theme and icons
    #/opt/oomox/plugins/theme_oomox/change_color.sh ~/.config/oomox/colors/Ricer -o Ricer
    /opt/oomox/plugins/icons_suruplus_aspromauros/change_color.sh ~/.config/oomox/colors/Ricer -o Ricer
    themix-multi-export ~/.config/oomox/export_config/multi_export_Ricer.json ~/.config/oomox/colors/Ricer

    # Apply the theme with nwg-look
    nwg-look -a

    # Generate Steam theme
    adwaita-steam-gtk -i

    # Update Flatpak env vars
    flatpak override --user \
        --env=XCURSOR_THEME=$XCURSOR_THEME \
        --env=XCURSOR_SIZE=$XCURSOR_SIZE \
        --env=GDK_BACKEND=$GDK_BACKEND \
        --env=GDK_SCALE=$GDK_SCALE \
        --env=GTK_THEME=$GTK_THEME \
        --env=QT_QPA_PLATFORM=$QT_QPA_PLATFORM \
        --env=QT_QPA_PLATFORMTHEME=$QT_QPA_PLATFORMTHEME \
        --env=QT_QUICK_CONTROLS_STYLE=$QT_QUICK_CONTROLS_STYLE \
        --env=QT_AUTO_SCREEN_SCALE_FACTOR=$QT_AUTO_SCREEN_SCALE_FACTOR \
        --env=QT_SCALE_FACTOR=$QT_SCALE_FACTOR \
        --env=QT_WAYLAND_DISABLE_WINDOWDECORATION=$QT_WAYLAND_DISABLE_WINDOWDECORATION \
        --env=CLUTTER_BACKEND=$CLUTTER_BACKEND \
        --env=ELECTRON_OZONE_PLATFORM_HINT=$ELECTRON_OZONE_PLATFORM_HINT

    # Reload Dunst
    dunstctl reload

    # Reload Hyprland
    hyprctl reload

    # Nlear the console, sleep and kill Shifter
    #nlear
    #sleep 0.5
    #kill -SIGUSR1 $pid
end

# Turns video resolution into 8K
function 8k
    ffmpeg -i $argv[1] -vf scale=7680:4320 -c:v libx265 -crf 23 -c:a copy $argv[2]
end

# Opens neovide and moves terminal to the nv workspace(Hyprland only)
function nv
    set current_window (hyprctl activewindow -j | jq -r .address 2>/dev/null)

    neovide $argv &
    set neovide_pid $last_pid
    #Sleep is needed to avoid window rearangement when opening
    #Adjust as needed per system, the slower the system the bigger the pause
    sleep 0.3

    hyprctl dispatch movetoworkspacesilent "special:nv,address:$current_window" >/dev/null 2>&1
    while kill -0 $neovide_pid 2>/dev/null
    #Lowering this number reduces the pause between checks
    #May lead to a very slight performance increase at the cost of seamlessness
        sleep 0.1
    end
    set current_workspace (hyprctl activeworkspace -j | jq -r .id 2>/dev/null)
    hyprctl dispatch movetoworkspace "$current_workspace,address:$current_window" >/dev/null 2>&1
end

# Opens fzf result with nvim
function nvimf
    set file (fzf --preview="bat --color=always {}")
    if test -n "$file"
        nvim "$file"
    end
end

# Opens fzf result with neovide
function nvf
    set file (fzf --preview="bat --color=always {}")
    if test -n "$file"
        nv "$file"
    end
end

function dir_to_intermediate -d "Transcodes all MKV files in a directory (recursively) to AV1+FLAC"
    set -l target_dir (count $argv > /dev/null; and echo $argv[1]; or echo .)

    if not test -d "$target_dir"
        echo "Error: '$target_dir' is not a valid directory."
        return 1
    end

    for file in (find $target_dir -type f -iname '*.mkv')
        to_av1_flac "$file"
    end
end

function cleanup_intermediate_dir -d "Deletes all the original files and removes _intermediate from names"
    for file in (find . -type f -not -name '*_intermediate.*')
        rm "$file"
    end
    for file in (find . -type f -name '*_intermediate.*')
        mv "$file" (string replace '_intermediate' '' "$file")
    end
end

# Downloads video
alias dlv 'yt-dlp -f bestvideo+bestaudio --merge-output-format mkv --exec \'du -sh {}\''
# Downloads audio
alias dla 'yt-dlp -f bestaudio --extract-audio --exec \'du -sh {}\''

# Downloads a video with metadata
function dlvi
    # Downloads the video
    yt-dlp -f bestvideo+bestaudio --merge-output-format mkv --write-description --write-info-json --no-clean-info-json --write-comments --write-thumbnail --write-subs -o "%(title)s/%(title)s.%(ext)s" "$argv"

    # Makes the json human readable
    for file in **/*.info.json
        jq . "$file" > temp.json && mv temp.json "$file"
    end
end

# Turns a file into mp3
function mp3
    if test (count $argv) -ne 1
        echo "Usage: mp3 <input_file>"
        return 1
    end

    set input_file $argv[1]
    set output_file (string replace -r '\.[^.]+$' '.mp3' $input_file)

    if not test -f $input_file
        echo "Error: Input file '$input_file' does not exist."
        return 1
    end

    ffmpeg -i $input_file -acodec libmp3lame -b:a 320k $output_file

    if test $status -eq 0
        echo "Conversion successful: $output_file"
    else
        echo "Conversion failed"
    end
end

function timer
    if test -z "$argv[1]"
        echo "Usage: timer <duration>"
        echo "Example: timer 5m (for 5 minutes)"
        echo "Example: timer 30s (for 30 seconds)"
        return 1
    end

    echo "Timer set for $argv[1]..."
    sleep "$argv[1]"
    echo "Timer" "Timer for $argv[1] finished"
    notify-send "Timer" "Timer for $argv[1] finished"
    paplay /usr/share/sounds/freedesktop/stereo/message.oga
end

function check_dependency
    if not type -q $argv[1]
        echo "Error: $argv[1] not found" >&2
        return 1
    end
end

function check_dependencies
    set deps $argv
    set missing_deps

    for dep in $deps
        if not type -q $dep
            set -a missing_deps $dep
        end
    end

    if test (count $missing_deps) -gt 0
        echo "Error - Missing dependencies: $missing_deps" >&2
        return 1
    end
end

function check_remote_dependency
    set remote_machine $argv[1]
    set remote_pass $argv[2]
    set dependency $argv[3]

    if not sshpass -p "$remote_pass" ssh "$remote_machine" "command -v $dependency >/dev/null 2>&1"
        echo "Error: $dependency not found on remote machine" >&2
        return 1
    end
end

function check_remote_dependencies
    set remote_machine $argv[1]
    set remote_pass $argv[2]
    set deps $argv[3..-1]
    set missing_remote_deps

    for dep in $deps
        if not sshpass -p "$remote_pass" ssh "$remote_machine" "fish -c 'type -q $dep'"
            set -a missing_remote_deps $dep
        end
    end

    if test (count $missing_remote_deps) -gt 0
        echo "Error - Missing remote dependencies: $missing_remote_deps" >&2
        return 1
    end
end

# Combines all input devices into a new one called CombinedInput
function combine_input
    # Create a new virtual source that other apps can use as input
    set SOURCE_NAME "CombinedInput"

    # First create a null sink that will serve as our mixing point
    pactl load-module module-null-sink sink_name="$SOURCE_NAME"_mix sink_properties=device.description="$SOURCE_NAME"_mix

    # Create a virtual source that monitors our null sink
    # This makes the mixed audio available as an input
    pactl load-module module-virtual-source source_name=$SOURCE_NAME master="$SOURCE_NAME"_mix.monitor source_properties=device.description="Combined Input"

    # Get a list of all audio input sources
    set SOURCES (pactl list sources short | awk '/input/ {print $2}')

    # Loop through each audio input source and create a loopback to our mixing sink
    for SOURCE in $SOURCES
        # Skip any virtual sources we just created to avoid feedback loops
        if test "$SOURCE" != "*$SOURCE_NAME*" -a "$SOURCE" != "*monitor*"
            pactl load-module module-loopback source=$SOURCE sink="$SOURCE_NAME"_mix latency_msec=1
        end
    end

    echo "All inputs have been combined into the virtual source '$SOURCE_NAME'"
    echo "You can now select '$SOURCE_NAME' as an input device in your applications"
end

# Creates a new input and output device that mirrors all the system audio(used for Nisualizer)
function output_input
    # Create a new virtual source that other apps can use as input
    set SOURCE_NAME "OutputInput"

    # Create a null sink to serve as the mixing point
    pactl load-module module-null-sink sink_name="$SOURCE_NAME"_mix sink_properties=device.description="$SOURCE_NAME"_mix

    # Create a virtual source that monitors the null sink, making the mixed audio available as input
    pactl load-module module-virtual-source source_name=$SOURCE_NAME master="$SOURCE_NAME"_mix.monitor source_properties=device.description="OutputInput"

    # Get the name of the current default output sink
    set DEFAULT_SINK (pactl info | grep "Default Sink" | awk '{print $3}')

    # Create a loopback from the default sink's monitor to our null sink
    if test -n "$DEFAULT_SINK"
        pactl load-module module-loopback source="$DEFAULT_SINK.monitor" sink="$SOURCE_NAME"_mix latency_msec=1
    else
        echo "Error: Could not find default output sink."
    end
end

# Unloads pactl modules
function pactl_unload_modules_from
    set start_id $argv[1]
    while true
        # Attempt to unload the current module
        if not pactl unload-module $start_id
            break
        end
        # Increment the module ID
        set start_id (math "$start_id + 1")
    end
end

# Restarts the hyprland desktop portal
function xdph
    killall -e xdg-desktop-portal-hyprland
    killall -e xdg-desktop-portal-wlr
    killall xdg-desktop-portal
    /usr/lib/xdg-desktop-portal-hyprland &
    sleep 2
    /usr/lib/xdg-desktop-portal &
end

function track_app_usage
    set save_dir ~/TimeTracking
    mkdir -p $save_dir

    set interval 10

    while true
        sleep $interval

        set initial_class (hyprctl activewindow -j | jq -r '.initialClass // empty' 2>/dev/null)

        if test -n "$initial_class"
            set save_file "$save_dir/$initial_class"
            if test -f $save_file
                set seconds (cat $save_file)
            else
                set seconds 0
            end
            set seconds (math $seconds + 10)
            echo $seconds > $save_file
        end
    end
end

function get_app_usage
    set time_dir ~/TimeTracking

    if not test -d $time_dir
        echo "TimeTracking directory not found"
        return 1
    end

    # If no arguments, show all apps sorted by usage time
    if test (count $argv) -eq 0
        set apps
        set times

        # Collect all apps and their times
        for file in $time_dir/*
            if test -f $file
                set app (basename $file)
                set total_seconds (math --scale=0 (cat $file) 2>/dev/null || echo 0)
                set apps $apps $app
                set times $times $total_seconds
            end
        end

        if test (count $apps) -eq 0
            echo "No time tracking data found"
            return 1
        end

        # Find maximum app name length
        set max_width 0
        for app in $apps
            set name_length (string length $app)
            if test $name_length -gt $max_width
                set max_width $name_length
            end
        end
        set max_width (math $max_width + 1)

        # Create pairs and sort by time (descending)
        set pairs
        for i in (seq (count $apps))
            set pairs $pairs "$times[$i]:$apps[$i]"
        end

        # Sort by time (descending) and display
        for pair in (printf '%s\n' $pairs | sort -nr -t: -k1)
            set time_seconds (string split -m1 ':' $pair)[1]
            set app_name (string split -m1 ':' $pair)[2]

            set days (math --scale=0 "$time_seconds / 86400")
            set hours (math --scale=0 "($time_seconds % 86400) / 3600")
            set minutes (math --scale=0 "($time_seconds % 3600) / 60")
            set seconds (math --scale=0 "$time_seconds % 60")

            if test $days -gt 0
                printf "%-*s %dd %02d:%02d:%02d\n" $max_width $app_name $days $hours $minutes $seconds
            else
                printf "%-*s %02d:%02d:%02d\n" $max_width $app_name $hours $minutes $seconds
            end
        end

        return 0
    end

    if test (count $argv) -ne 1
        echo "Usage: get_time [AppName]"
        echo "       get_time        (show all apps sorted by usage time)"
        return 1
    end

    set app $argv[1]
    set save_file $time_dir/$app

    if not test -f $save_file
        echo "No data for $app"
        return 1
    end

    set file_content (cat $save_file 2>/dev/null | string trim)
    if test -z "$file_content"
        set total_seconds 0
    else
        set total_seconds (math --scale=0 $file_content 2>/dev/null || echo 0)
    end

    set days (math --scale=0 "$total_seconds / 86400")
    set hours (math --scale=0 "($total_seconds % 86400) / 3600")
    set minutes (math --scale=0 "($total_seconds % 3600) / 60")
    set seconds (math --scale=0 "$total_seconds % 60")

    if test $days -gt 0
        printf "%dd %02d:%02d:%02d\n" $days $hours $minutes $seconds
    else
        printf "%02d:%02d:%02d\n" $hours $minutes $seconds
    end
end

fish_add_path $HOME/.local/bin
fish_add_path $HOME/.zvm/bin

set -gx EDITOR nvim
set -gx VISUAL nvim

# Start the ssh-agent
if not pgrep -u $USER ssh-agent > /dev/null
    ssh-agent -c -a $XDG_RUNTIME_DIR/ssh-agent.socket | source
end
set -Ux SSH_AUTH_SOCK $XDG_RUNTIME_DIR/ssh-agent.socket

# Start a Hyprland session
if status is-interactive
    zoxide init fish | source
    tv init fish | source
    fish_vi_key_bindings

    oh-my-posh init fish --config ~/.config/oh-my-posh/Ricer.json | source
    #starship init fish | source

    nlear

    if test (tty) = /dev/tty1
        if not set -q WAYLAND_DISPLAY
            rm -rf ~/.config/Mumble/Mumble/mumble_settings.json.back
            #start-hyprland
        end
    end
end
