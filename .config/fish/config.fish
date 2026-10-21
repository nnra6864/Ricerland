# Load user scripts
fish_add_path ~/.config/fish/scripts

# Aliases
alias cip='curl ifconfig.me --silent | wl-copy'
alias doas='sudo'
alias hdr='ENABLE_HDR_WSI=1 mpv --vo=gpu-next --target-colorspace-hint --gpu-api=vulkan --gpu-context=waylandvk --target-colorspace-hint-mode=source'
alias lg='lazygit'
alias lnw='sh ~/.config/hypr/HyprlandUnityFix/ListNewWindows.sh'
alias ls='zl -la --dir-grouping=before --no-permissions --no-group -C ~/.config/zlist/zlist.zon'
alias nlear='clear; fastfetch; echo ""'
alias py='python'
alias t='tuios n'
alias timeshit='timeshift'
alias tm='tmux new -As'
alias wip='curl ifconfig.me --silent'
alias ytdla 'yt-dlp -f bestaudio --extract-audio --exec \'du -sh {}\''
alias ytdlv 'yt-dlp -f bestvideo+bestaudio --merge-output-format mkv --exec \'du -sh {}\''

# Path aliases
alias cfg='cd ~/.config/'
alias fsh='cd ~/.config/fish/; nvim ./config.fish'
alias hypr='cd ~/.config/hypr/; nvim ./'
alias nisu='cd ~/.config/Nisualizer/; nvim ./'
alias ricer='cd ~/.config/Ricer/; nvim ./'

# Paths
fish_add_path $HOME/.local/bin
fish_add_path $HOME/.zvm/bin

# Environment
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
            start-hyprland
        end
    end
end
