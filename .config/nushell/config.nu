$env.config.show_banner = false
$env.config.history.file_format = "sqlite"
$env.config.buffer_editor = "nvim"
$env.config.use_kitty_protocol = true
$env.config.ls.clickable_links = false
$env.config.table.mode = "default"

$env.config.edit_mode = "vi"
$env.config.cursor_shape.emacs = "inherit"
$env.config.cursor_shape.vi_insert = "block"
$env.config.cursor_shape.vi_normal = "block"

$env.PROMPT_COMMAND = null
$env.PROMPT_COMMAND_RIGHT = null

$env.PROMPT_INDICATOR = null
$env.PROMPT_INDICATOR_VI_NORMAL = ""
$env.PROMPT_INDICATOR_VI_INSERT = ""
$env.PROMPT_MULTILINE_INDICATOR = ""

# Paths
# Changes to directory and opens file/dir in nvim
def --env nvim_path [path: string] {
    if ($path | path type) == "dir" {
        cd $path
        nvim .
    } else {
        cd ($path | path dirname)
        nvim ($path | path basename)
    }
}

alias cfg = cd ~/.config/
alias hypr = nvim_path ~/.config/hypr/
alias huf = nvim_path ~/.config/hypr/HyprlandUnityFix/UnityFix.conf
alias fsh = nvim_path ~/.config/fish/config.fish
alias nisu = nvim_path ~/.config/Nisualizer/
alias ricer = nvim_path ~/.config/Ricer/

# General aliases
alias doas = sudo
alias py = python
alias timeshit = timeshift
alias ytdl = yt-dlp -f bestvideo+bestaudio --merge-output-format mkv
alias ytdla = yt-dlp -f bestaudio --extract-audio --audio-format mp3

def nlear [] {
    clear
    neofetch
}

def hdr [...args] {
    $env.ENABLE_HDR_WSI = "1"
    mpv --vo=gpu-next --target-colorspace-hint --gpu-api=vulkan --gpu-context=waylandvk --fullscreen ...$args
}

# Takes textures exported from Substance Painter in the format:
# FBXName_ObjectName_MapType.*
# And turns it into:
# MaterialNameObjectName_MapType.*
def rename_substance_textures [material_name: string] {
    ls *.png | each { |it| 
        let old_name = $it.name
        let new_name = (
            $old_name 
            | str replace -r '^[^_]*_' $"($material_name)_" 
            | str replace -r $"($material_name)_" $material_name
        )
        mv $old_name $new_name
        {from: $old_name, to: $new_name}
    }
}

source ~/.oh-my-posh.nu
nlear
