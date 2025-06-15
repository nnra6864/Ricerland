# Customize prompt (Obsolete, using omp now)
# set -g tide_left_prompt_items time os vi_mode pwd git 
# set -g tide_right_prompt_items
# set -g tide_right_prompt_suffix ''
# set -g tide_time_format '%T'

# Perforce Morph
set -Ux P4IGNORE .p4ignore

# Aliases
alias doas='sudo'
alias nlear='clear; neofetch'
alias py='python'
alias timeshit='timeshift'
alias hdr='ENABLE_HDR_WSI=1 mpv --vo=gpu-next --target-colorspace-hint --gpu-api=vulkan --gpu-context=waylandvk' 
alias lg='lazygit'
alias update='paru -Syu --noconfirm --sudoloop'

# Paths
alias cfg='cd ~/.config/'
alias hypr='cd ~/.config/hypr/; nvim ./'
alias fsh='cd ~/.config/fish/; nvim ./'
alias nisu='cd ~/.config/Nisualizer/; nvim ./'
alias ricer='cd ~/.config/Ricer/; nvim ./'

# ls
alias ls='eza -lah --icons=always --no-quotes --group-directories-first --no-permissions'
# Detailed ls
alias lsd='eza -lah --icons=always --no-quotes --group-directories-first --no-permissions -muU'
# Link ls
alias lsl='eza -lah --icons=always --no-quotes --group-directories-first --no-permissions --hyperlink'

# Starts Monero Wallet GUI
function monero
    echo "Enter your password:"
    read -s password
    echo $password | nohup sudo -E QT_QPA_PLATFORM=wayland monero-wallet-gui >/dev/null 2>&1 & disown
    exit
end

# Rices the system with the provided config
function rice
    #Start the Shifter transition
    #python ~/Data/Projects/Shifter/Shifter.py > /dev/null 2>&1 &
    #set pid $last_pid
    #disown
    #
    #sleep 0.2

    #Execute Ricer
    python ~/Data/Projects/Ricer/Ricer.py $argv # > /dev/null 2>&1

    #Reload Kitty cfg
    kill -SIGUSR1 (pgrep kitty)

    #Generate Oomox theme and icons and update nwg-look
    /opt/oomox/plugins/theme_oomox/change_color.sh ~/.config/OomoxRicer -o Ricer > /dev/null 2>&1 &
    /opt/oomox/plugins/icons_suruplus_aspromauros/change_color.sh ~/.config/OomoxRicer -o Ricer > /dev/null 2>&1 &
    nwg-look -a > /dev/null 2>&1 &
    disown

    #Generate Steam theme
    adwaita-steam-gtk -i > /dev/null 2>&1 &
    disown
    
    #Restart Dunst
    killall dunst > /dev/null 2>&1
    dunst >/dev/null 2>&1 &
    disown

    #Update Unity Hub(requires sudo)
    #sudo python .config/RicerHub.py /opt/unityhub/

    #Nlear the console, sleep and kill Shifter
    #nlear
    #sleep 1
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

# Used to cut shadowplays
function spc
    # Set to " " if you want to keep the spaces
    set default_space_replacement "_"

    # Appended to output name if extension is not found
    set default_ext ".mkv"

    type -q ffmpeg
    or begin
        echo "Error: ffmpeg not found" >&2
        return 1
    end

    type -q trash
    or begin
        echo "Error: trash CLI not found" >&2
        return 1
    end

    # Get the input file and its duration
    read -P "Input file path - " input_file
    set input_file (string trim -- "$input_file")
    set input_file_name (basename "$input_file")
    set base_name (string replace -r '\.[^.]*$' '' (basename "$input_file"))
    set extension (string match -r '\.[^.]*$' (basename "$input_file"))
    set default_output "$base_name Remuxed$extension"
    set duration_seconds (ffprobe -v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 "$input_file")
    set duration (printf "%02d:%02d:%09.6f" (math "floor($duration_seconds / 3600)") (math "floor($duration_seconds % 3600 / 60)") (math "$duration_seconds % 60"))
    set modified_date (stat -c "%y" $input_file | cut -d'.' -f1 | sed 's/ /T/')

    # Get and set output name
    read -P "Output file path(Default: $default_output) - " output_file
    if test -z "$output_file"
        set output_file "$default_output"
    end

    # Replace spaces in the name with the space replacement
    read -P "Space replacement character(Default: '$default_space_replacement') - " space_replacement
    if test -z "$space_replacement"
        set space_replacement "$default_space_replacement"
    end
    set output_file (string replace -a ' ' "$space_replacement" -- "$output_file")

    # Add the default extension if one is not found
    set file_ext (string match -r '\.[^.]+$' -- "$output_file")
    if test -z "$file_ext"
        set output_file "$output_file$default_ext"
    end

    # Parse time
    function parse_time
        set input "$argv[1]"
        set default "$argv[2]"
        if test -z "$input"
            echo "$default"
        else
            # Handle different input formats
            if string match -qr '^\d+(\.\d+)?$' "$input"
                # Just seconds (e.g., "30.250")
                printf "%02d:%02d:%09.6f" 0 0 "$input"
            else if string match -qr '^\d+:\d+(\.\d+)?$' "$input"
                # MM:SS or MM:SS.mmm format
                set parts (string split ':' "$input")
                printf "%02d:%02d:%09.6f" 0 "$parts[1]" "$parts[2]"
            else if string match -qr '^\d+:\d+:\d+(\.\d+)?$' "$input"
                # HH:MM:SS or HH:MM:SS.mmm format
                set parts (string split ':' "$input")
                printf "%02d:%02d:%09.6f" "$parts[1]" "$parts[2]" "$parts[3]"
            else
                echo "$default"
            end
        end
    end

    # Get and parse the start time
    read -P "Start time (format: HH:MM:SS.MS, Default: 0) - " start_time
    set start_time (parse_time "$start_time" "00:00:00")

    # Get and parse the end time
    read -P "End time (format: HH:MM:SS.MS, Default: $duration) - " end_time
    set end_time (parse_time "$end_time" "$duration")

    # Get the CQP quality
    read -P "CQP(Default: 20) - " cqp_quality
    if test -z "$cqp_quality"
        set cqp_quality 20
    end

    # Process the file with ffmpeg
    ffmpeg -i "$input_file" -ss "$start_time" -to "$end_time" \
    -c:v hevc_nvenc -preset p7 -profile:v main10 -rc vbr -cq "$cqp_quality" -b:v 0 \
    -spatial_aq 1 -temporal_aq 1 -b_ref_mode middle -rc-lookahead 32 -multipass 2 \
    -c:a copy -map 0 \
    -metadata creation_time="$modified_date" \
    "$output_file"

    # Return if ffmpeg failed
    if [ "$status" -ne 0 ]
        echo ""
        notify-send "SPC" "Remuxing failed: $status"
        paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
        return $status
    end

    # Print info
    echo ""
    echo "Finished processing the file:"
    echo "Input:   '$input_file' ("(du -h "$input_file" | cut -f1)")"
    echo "Output:  '$output_file' ("(du -h "$output_file" | cut -f1)")"
    echo "Start:   '$start_time'"
    echo "End:     '$end_time'"
    echo "Quality: '$cqp_quality'"
    echo ""

    # Send the notification
    notify-send "SPC" "Remuxing finished"
    paplay /usr/share/sounds/freedesktop/stereo/message.oga

    # Trash the original file
    set trash_original "y"
    read -P "Trash original? (Y/n): " trash_original
    if test -z "$trash_original" -o (string lower -- "$trash_original") = "y"
        trash "$input_file"
        echo "Trashed the original file"
    end
end

function to_intermediate -d "Transcodes a single MKV file to AV1+FLAC intermediate format"
    if test (count $argv) -ne 1
        echo "Usage: to_av1_flac <input_file.mkv>"
        return 1
    end

    set -l input_file "$argv[1]"

    if not test -f "$input_file"
        echo "Error: Input file '$input_file' not found."
        return 1
    end

    set -l input_dir (dirname "$input_file")
    set -l input_base (basename "$input_file" .mkv)
    set -l output_base "$input_base"_intermediate
    set -l output_file "$input_dir/$output_base.mkv"

    echo ""
    echo "Processing: $input_file"
    echo "Output: $output_file"

    if [ "$input_file" = "$output_file" ]
        echo "Error: Input and output file paths are the same. Skipping to prevent overwriting source."
        return 1
    end

    ffmpeg -i "$input_file" -map 0:v:0 -map 0:a -c:v libsvtav1 -crf 18 -c:a flac "$output_file"

    if [ "$status" -ne 0 ]
        echo "Error transcoding $input_file (FFmpeg exit status: $status)"
        return $status
    else
        echo "Successfully transcoded $input_file"
        return 0
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
alias dlv 'yt-dlp -f bestvideo+bestaudio --merge-output-format mkv'
# Downloads audio
alias dla 'yt-dlp -f bestaudio --extract-audio'

# Downloads a video with metadata
function ytdli
    # Downloads the video
    yt-dlp -f bestvideo+bestaudio --merge-output-format mkv --write-info-json --write-thumbnail --write-subs -o "%(title)s/%(title)s.%(ext)s" "$argv"

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

function rub
    set dependencies curl wget grep ssh sshpass notify-send paplay pv rsync

    function find-project-dir
      for dir in */
        if test -d "$dir/Assets" -a -d "$dir/ProjectSettings" -a -d "$dir/Packages"
          echo (string trim -r -- $dir)
          return
        end
      end
    end

    function get_unity_url
        set unity_version $argv[1]
        set base_version (string replace -r 'f.*$' '' $unity_version)

        curl -s "https://unity.com/releases/editor/whats-new/$base_version#installs" | \
            grep -oE 'https://download\.unity3d\.com/download_unity/[a-f0-9]{12}/LinuxEditorInstaller/Unity-[^"]*\.tar\.xz' | \
        head -1
    end

    # Check all dependencies
    echo "Checking dependencies..."
    if not check_dependencies $dependencies
        paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
        return 1
    end
    echo "Success"
    echo ""
    echo ""

    # Gather project info
    echo "Gathering project info..."
    set root_dir (pwd)
    set project_dir (find-project-dir)
    if test -z "$project_dir"
        echo "No valid Unity project found in: $root_dir"
        paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
        return 1
    end
    set project_name (basename "$project_dir")
    
    # Get the Unity version
    set version_file "$project_dir/ProjectSettings/ProjectVersion.txt"
    if not test -f "$version_file"
        echo "ProjectVersion.txt not found in $project_dir/ProjectSettings/"
        paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
        return 1
    end
    set unity_version (grep "m_EditorVersion:" "$version_file" | cut -d' ' -f2)
    if test -z "$unity_version"
        echo "Could not parse Unity version from ProjectVersion.txt"
        paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
        return 1
    end

    # Print all the info
    echo "Successfully gathered project info:"
    echo "Name: $project_name" 
    echo "Path: $project_dir"
    echo "Unity Version: $unity_version"
    echo ""
    echo ""
    
    # Get ssh info
    set default_remote_machine "nn@192.168.0.23"
    read -P "Enter remote machine ($default_remote_machine): " remote_machine
    if test -z "$remote_machine"
        set remote_machine "$default_remote_machine"
    end
    read -sP "Enter SSH password: " remote_pass

    # Test the SSH connection
    echo "Testing SSH connection..."
    if not sshpass -p "$remote_pass" ssh -o StrictHostKeyChecking=no -q "$remote_machine" exit
        echo "Can't SSH into $remote_machine with provided password"
        paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
        return 1
    end
    echo "Connection to '$remote_machine' successful"
    echo ""
    echo ""
    
    # Check remote dependencies
    echo "Checking remote dependencies..."
    if not check_remote_dependencies "$remote_machine" "$remote_pass" $dependencies
        paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
        return 1
    end
    echo "Success"
    echo ""
    echo ""

    # Gather remote info
    echo "Gathering remote info..."
    set remote_unity_dir "~/Unity/Hub/Editor/"
    set remote_projects_dir "~/Projects/Unity/"
    set remote_project_dir "$remote_projects_dir/$project_name/"
    set remote_builds_dir "$remote_project_dir/Builds/"
    set remote_unity_project_dir "$remote_project_dir/$project_name/"
    set unity_editor "$remote_unity_dir$unity_version/Editor/Unity"
    echo "Successfully gathered remote info"
    echo ""
    echo ""
    
    # Create the Unity directory if not present on the remote machine
    if not sshpass -p "$remote_pass" ssh "$remote_machine" test -d "$remote_unity_dir"
        echo "Unity dir not found on the remote machine: '$remote_unity_dir', creating..."
        sshpass -p "$remote_pass" ssh "$remote_machine" "mkdir -p $remote_unity_dir"
        echo "Successfully created '$remote_unity_dir' dir on the remote machine"
        echo ""
        echo ""
    end

    # Verify this version exists on the remote machine
    if not sshpass -p "$remote_pass" ssh "$remote_machine" test -d "$remote_unity_dir$unity_version"
        echo "Unity version $unity_version not found on remote machine"
        paplay /usr/share/sounds/freedesktop/stereo/dialog-warning.oga
        
        read -n 1 -P "Install Unity $unity_version on the remote machine? [Y/n]: " download_choice

        if test "$download_choice" = "n" -o "$download_choice" = "N"
            echo "Unity version $unity_version not found on the remote machine"
            paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
            return 1
        end

        # Get the Unity download link
        echo "Getting the Unity $unity_version download link..."
        set download_url (get_unity_url $unity_version)
        if test -z "$download_url"
            echo "Could not find download URL for Unity $unity_version"
            paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
            return 1
        end
        echo "Successfully found the Unity $unity_version download link: $download_url"
        echo ""
        echo ""

        # Install Unity
        echo "Installing Unity $unity_version on remote machine..."
        sshpass -p "$remote_pass" ssh "$remote_machine" "
            cd ~/.cache &&
            echo 'Downloading Unity $unity_version...' &&
            wget --progress=bar:force:noscroll '$download_url' -O 'Unity-$unity_version.tar.xz' &&
            echo 'Download complete' &&
            mkdir -p ~/Unity/Hub/Editor/$unity_version &&
            echo 'Extracting Unity-$unity_version.tar.xz...' &&
            pv Unity-$unity_version.tar.xz | tar --xz -x -C ~/Unity/Hub/Editor/$unity_version &&
            echo 'Extraction complete' &&
            rm -rf 'Unity-$unity_version.tar.xz' &&
            echo 'Marking Unity as executable' &&
            chmod +x ~/Unity/Hub/Editor/$unity_version/Editor/Unity &&
            echo 'Marked' &&
            cd -
            "

        if test $status -ne 0
            notify-send "RUB" "Failed to install Unity $unity_version on remote machine"
            paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
            return 1
        end

        echo "Unity $unity_version successfully installed on remote machine"
        paplay /usr/share/sounds/freedesktop/stereo/message.oga
    end
    
    if sshpass -p "$remote_pass" ssh "$remote_machine" "test -f $unity_editor"
        echo "Unity Editor found on the remote machine"
    else
        echo "Unity Editor not found on the remote machine: $unity_editor"
        paplay /usr/share/sounds/freedesktop/stereo/dialog-error.oga
        return 1
    end
    echo ""
    echo ""
    echo ""

    # rsync files to the remote machine
    echo "Syncing project..."
    sshpass -p "$remote_pass" ssh "$remote_machine" mkdir -p "$remote_unity_project_dir"
    sshpass -p "$remote_pass" rsync -az --delete \
                --exclude 'Library' \
                --exclude 'Logs' \
                --exclude 'Temp' \
                --exclude 'UserSettings' \
                "$project_dir/" "$remote_machine:$remote_unity_project_dir"
    echo "Project synced"
    echo ""
    echo ""

    # Make builds
    echo "Building..."
    sshpass -p "$remote_pass" ssh "$remote_machine" mkdir -p "$remote_builds_dir"
    if not sshpass -p "$remote_pass" ssh "$remote_machine" "$unity_editor -batchmode -logfile - -quit -projectPath \"$remote_unity_project_dir\" -executeMethod BuildScript.PerformBuild"
        echo "Unity build failed"
        notify-send --urgency=critical --expire-time=0 "RUB" "Unity build failed"
        paplay /usr/share/sounds/freedesktop/stereo/message.oga
        return 1
    end
    echo "Unity build successful"
    echo ""
    echo ""

    # rsync builds
    echo "Syncing builds..."
    mkdir -p Builds
    sshpass -p "$remote_pass" rsync -az --delete "$remote_machine:$remote_builds_dir" "./Builds/"
    echo "Builds synced"
    notify-send --urgency=critical --expire-time=0 "RUB" "Builds complete"
    paplay /usr/share/sounds/freedesktop/stereo/message.oga
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

# Fish config
zoxide init fish | source
fish_vi_key_bindings

# Start a Hyprland session
if status is-interactive
    if test (tty) = /dev/tty1
        if not set -q WAYLAND_DISPLAY
            # Remove any stale keyring files
            rm -rf /run/user/1000/keyring/*

            # Start dbus session
            eval (dbus-launch --sh-syntax)

            # Start gnome-keyring-daemon
            /usr/bin/gnome-keyring-daemon --start --components=secrets,ssh

            # Setup and combine_input and output_input
            combine_input
            # output_input

            # Start Hyprland
            Hyprland
        end
    end
end

oh-my-posh init fish --config ~/.config/oh-my-posh/Ricer.json | source

# Created by `pipx` on 2025-02-06 06:22:34
set PATH $PATH /home/nnra/.local/bin
