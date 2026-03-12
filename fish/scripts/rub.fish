# Unity license must still be manually activated on the build machine
# You can achieve this by:
# 1. Opening Unity Hub
# 2. Navigating to Preferences -> Licenses -> Add -> Get a free personal license
# This step can't be avoided now thanks to highly competent Unity devs
function rub
    set dependencies curl wget grep ssh sshpass notify-send paplay pv rsync tee

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
    set remote_unity_dir (sshpass -p "$remote_pass" ssh "$remote_machine" "echo \$HOME/Unity/Hub/Editor/")
    set remote_projects_dir (sshpass -p "$remote_pass" ssh "$remote_machine" "echo \$HOME/Projects/Unity/")
    set remote_project_dir "$remote_projects_dir$project_name/"
    set remote_builds_dir "$remote_project_dir""Builds/"
    set remote_unity_project_dir "$remote_project_dir$project_name/"
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
            pv -f Unity-$unity_version.tar.xz | tar --xz -x -C ~/Unity/Hub/Editor/$unity_version &&
            echo 'Extraction complete' &&
            rm -rf 'Unity-$unity_version.tar.xz' &&
            echo 'Marking Unity as executable' &&
            chmod +x ~/Unity/Hub/Editor/$unity_version/Editor/Unity &&
            echo 'Marked' &&
            cd -"

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
    set build_result (sshpass -p "$remote_pass" ssh "$remote_machine" \
        "mkdir -p '$remote_builds_dir' && \
        \"$unity_editor\" \
        -batchmode -nographics -quit -log - \
        -projectPath \"$remote_unity_project_dir\" \
        -executeMethod BuildScript.Build 2>&1 | tee /dev/stderr")

    if test $status -ne 0
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
    sshpass -p "$remote_pass" rsync -az --delete \
        "$remote_machine:$remote_builds_dir/" "./Builds/"
    echo "Builds synced"
    notify-send --urgency=critical --expire-time=0 "RUB" "Builds complete"
    paplay /usr/share/sounds/freedesktop/stereo/message.oga
end

