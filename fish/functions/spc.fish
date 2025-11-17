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

	# Get the input file creation date
	set creation_time (ffprobe -v quiet -show_entries format_tags=creation_time -of csv=p=0 "$input_file" 2>/dev/null)
	if test -n "$creation_time"
		set modified_date $creation_time
	else
		set modified_date (stat -c "%y" "$input_file" | cut -d'.' -f1 | sed 's/ /T/')
	end

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

