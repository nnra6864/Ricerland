function local_backup
	set config_file ~/.config/LocalBackup.conf

	for line in (cat $config_file)
		if string match -qr '^\s*#|^\s*$' $line
			continue
		end

		set parts (string split "=>" $line)
		set source (eval echo $parts[1])
		set target (eval echo $parts[2])

		if test -z "$source" -o -z "$target"
			echo "Invalid entry: $line" >&2
			continue
		end

		mkdir -p "$target"
		rsync -a --info=progress2 "$source" "$target"
		and echo "$source => $target"
	end

	echo "Finished backup"
end
