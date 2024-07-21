if status is-interactive
    if test (tty) = /dev/tty1
        if not set -q WAYLAND_DISPLAY
            dbus-run-session Hyprland
        end
    end
end

alias doas='sudo'
alias nlear='clear; neofetch'
alias py='python'

function rice
    py ~/Data/Projects/Ricer/Ricer.py $argv
    
    nwg-look -a
    killall glava
    nohup glava &
end

function 8k
    ffmpeg -i $argv[1] -vf scale=7680:4320 -c:v libx265 -crf 23 -c:a copy $argv[2]
end

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

function nvimf
    set file (fzf --preview="bat --color=always {}")
    if test -n "$file"
        nvim "$file"
    end
end

function nvf
    set file (fzf --preview="bat --color=always {}")
    if test -n "$file"
        nv "$file"
    end
end

function spc
    read -P "Enter input file path - " input_file
    set input_file (string trim -- $input_file)
    set input_file_name (basename $input_file)
    set duration (ffprobe -v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 $input_file)
    set duration (date -u -d "1970-01-01 $duration seconds" +%H:%M:%S)
    
    read -P "Enter output file path(Default: $input_file_name) - " output_file
    if test -z $output_file
        set output_file $input_file_name
    end
    
    function parse_time
        set input $argv[1]
        set default $argv[2]
        if test -z $input
            echo $default
        else
            set parts (string split ':' $input)
            switch (count $parts)
                case 1
                    printf "%02d:%02d:%02d" 0 0 $parts[1]
                case 2
                    printf "%02d:%02d:%02d" 0 $parts[1] $parts[2]
                case 3
                    printf "%02d:%02d:%02d" $parts[1] $parts[2] $parts[3]
            end
        end
    end
    
    read -P "Enter start time (format: HH:MM:SS, Default: 0) - " start_time
    set start_time (parse_time $start_time "00:00:00")
    
    read -P "Enter end time (format: HH:MM:SS, Default: $duration) - " end_time
    set end_time (parse_time $end_time $duration)
    
    read -P "CQP(Default: 20) - " cqp_quality
    if test -z $cqp_quality
        set cqp_quality 20
    end
    
    set audio_count (ffmpeg -i "$input_file" 2>&1 | grep "Stream #" | grep -c "Audio")
    
    ffmpeg -i "$input_file" -ss "$start_time" -to "$end_time" -c:v hevc_nvenc -rc vbr -cq $cqp_quality -c:a copy -map 0 "$output_file"
    
    set delete_original "y"
    read -P "Delete original? (y/N): " delete_original
    if test "$delete_original" = "y"
        rm -rf $input_file
    end
end

function ytdl
    yt-dlp -f bestvideo+bestaudio --merge-output-format mkv "$argv"
end

zoxide init fish | source
fish_vi_key_bindings
