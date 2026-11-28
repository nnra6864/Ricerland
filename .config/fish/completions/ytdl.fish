# Modes for ytdl (see ~/.config/fish/scripts/ytdl)
set -l modes v vs a as all
complete -c ytdl -f
complete -c ytdl -n "not __fish_seen_subcommand_from $modes" -a v -d 'Video → mkv'
complete -c ytdl -n "not __fish_seen_subcommand_from $modes" -a vs -d 'Video + embedded subs'
complete -c ytdl -n "not __fish_seen_subcommand_from $modes" -a a -d 'Audio'
complete -c ytdl -n "not __fish_seen_subcommand_from $modes" -a as -d 'Audio → mka + embedded subs'
complete -c ytdl -n "not __fish_seen_subcommand_from $modes" -a all -d 'Archive: video, metadata, comments, subs'

# After the mode, hand everything to yt-dlp's own completions
function __ytdl_complete_ytdlp
    set -l tokens (commandline -xpc)
    set -l rest $tokens[3..]
    complete -C (string join ' ' -- yt-dlp (string escape -- $rest) (commandline -ct))
end
complete -c ytdl -n "__fish_seen_subcommand_from $modes" -a '(__ytdl_complete_ytdlp)'
