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

function 8k
	ffmpeg -i $argv[1] -vf scale=7680:4320 -c:v libx265 -crf 23 -c:a copy $argv[2]
end

function nv
    set current_window (hyprctl activewindow -j | jq -r .address 2>/dev/null)
    
    neovide &
    set neovide_pid $last_pid
    
    hyprctl dispatch movetoworkspacesilent "special:nv,address:$current_window" >/dev/null 2>&1
    while kill -0 $neovide_pid 2>/dev/null
        sleep 1
    end
    set current_workspace (hyprctl activeworkspace -j | jq -r .id 2>/dev/null)
    hyprctl dispatch movetoworkspacesilent "$current_workspace,address:$current_window" >/dev/null 2>&1
    hyprctl dispatch focusurgentorlast >/dev/null 2>&1
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
	
	read -P "Enter start time (format: HH:MM:SS, Default: 00:00:00) - " start_time
	if test -z $start_time
		set start_time 00:00:00
	end
	
	read -P "Enter end time (format: HH:MM:SS, Default: $duration) - " end_time
	if test -z $end_time
		set end_time $duration
	end

	read -P "CQP(Default: 10) - " cqp_quality
	if test -z $cqp_quality
		set cqp_quality 10
	end

	set audio_count (ffmpeg -i "$input_file" 2>&1 | grep "Stream #" | grep -c "Audio")
	
	#ffmpeg -i "$input_file" -ss "$start_time" -to "$end_time" -c:v copy -c:a copy -map 0 "$output_file" #Leaves the video as it is
	ffmpeg -i "$input_file" -ss "$start_time" -to "$end_time" -c:v hevc_nvenc -rc vbr_hq -cq $cqp_quality -c:a copy -map 0 "$output_file" #Uses CQP which greatly reducec the file size while keeping the quality
	#ffmpeg -i "$input_file" -ss "$start_time" -to "$end_time" -c:v hevc_nvenc -rc vbr_hq -cq <cq_value> -filter_complex "[0:a]amerge=inputs=$(ffprobe -v error -select_streams a:0 -show_entries stream=index -of csv=p=0 "$input_file" | wc -l)[a]" -map "[a]" -c:a aac -map 0 "$output_file"
end
zoxide init fish | source
fish_vi_key_bindings
