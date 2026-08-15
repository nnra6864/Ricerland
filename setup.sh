#!/usr/bin/env bash

# Exit on error
set -e

clear

aur_helpers=("paru" "yay")
pre_install=("git" "television" "ripgrep", "zvm", "podman")
config_packages=(
    "bat"
    "brightnessctl"
    "btop"
    "dunst"
    "eza"
    "fastfetch"
    "fish"
    "flatpak"
    "fzf"
    "git"
    "grim"
    "less"
    "nvim"
    "reflector"
    "trash-cli"
    "tree-sitter-cli"
    "zoxide"
    "zip"
    "unzip"
    "xdg-terminal-exec"
    "oh-my-posh-bin"
)
kernels=("linux" "linux-zen")
cachy_packages=("cachyos-gaming-application" "cachyos-gaming-meta" "proton-cachyos" "proton-cachyos-slr")
nvidia_packages=("cuda")
qol_packages=(
    "7zip"
    "appimagelauncher"
    "blueman"
    "bluez-utils"
    "croc"
    "curl"
    "downgrade"
    "duf"
    "dust"
    "dysk"
    "fd"
    "glow"
)
terminals=("ghostty" "foot" "kitty")
file_managers=("yazi" "dolphin" "nautilus")
browsers=("zen-browser-bin" "helium-browser-bin" "brave-bin" "qutebrowser")
socials=("mumble" "element-desktop" "equibop")
media_packages=("mpv" "mpd" "ncmpcpp" "vlc")
dev_packages=(
    "bear"
    "unity-hub"
    "blender-bin"
    "curl"
    "git"
    "kdenlive"
    "krita"
)
hypr_packages=(
    "hyprland-protocols-git"
    "hyprwayland-scanner-git"
    "hyprutils-git"
    "hyprgraphics-git"
    "hyprlang-git"
    "hyprcursor-git"
    "aquamarine-git"
    "xdg-desktop-portal-hyprland-git"
    "hyprwire-git"
    "hyprtoolkit-git"
    "hyprland-git"
    "hypridle-git"
    "hyprland-guiutils-git"
    "hyprland-qt-support-git"
    "hyprlock-git"
    "hyprpaper-git"
    "hyprpicker-git"
    "hyprpolkitagent-git"
    "hyprpwcenter-git"
    "hyprshutdown-git"
    "hyprsunset-git"
    "hyprsysteminfo-git"
    "hyprshade-git"
    "hyprshot-git"
)
applications=(
    "kdeconnect"
    "keepassxc"
    "ktailctl"
    "lan-mouse-git"
    "libreoffice-fresh"
)
ricing_packages=("adwsteamgtk" "breeze5" "kvantum")


declare -A package_dependencies
package_dependencies=(
    ["git"]="git-lfs lazygit git-filter-repo gitleaks"
    ["yazi"]="ffmpeg 7zip jq poppler fd ripgrep fzf zoxide resvg imagemagick wl-clipboard git glow"
    ["dolphin"]="dolphin-plugins ffmpegthumbs filelight kde-cli-tools kdegraphics-thumbnailers kio-admin kompare"
    ["kdeconnect"]="sshfs qt6-tools"
    ["kdenlive"]="bigsh0t dvgrab kimageformats noise-suppression-for-voice opencv python-openai-whisper qt6-imageformats"
)

package_preview=("pacman -Si {1} | sed -e '/^$/q'")

space() {
    echo
    echo
    echo
}

multi_select() {
    local prompt="$1"
    local preview="$2"
    shift 2
    local options=("$@")

    printf "%s\n" "${options[@]}" | tv --input-header "$prompt" --preview-command "$preview"
}

install_packages_and_dependencies() {
    local raw_targets=("$@")
    local targets=()
    local to_install=()

    # Split items at \n
    for item in "${raw_targets[@]}"; do
        while IFS= read -r line; do
            [[ -n "$line" ]] && targets+=("$line")
        done <<< "$item"
    done

    # return if nothing is selected
    if [[ ${#targets[@]} -eq 0 || -z "${targets[0]}" ]]; then
        echo "Nothing selected, skipping..."
        return 0
    fi

    # Select dependencies
    for pkg in "${targets[@]}"; do
        to_install+=("$pkg")
        local deps="${package_dependencies[$pkg]}"
        if [[ -n "$deps" ]]; then
            to_install+=($deps)
        fi
    done

    # Install
    if [[ ${#to_install[@]} -gt 0 ]]; then
        $aur_helper -S --needed "${to_install[@]}"
    fi
}

echo "***************"
echo "* ☦ Typikon ☦ *"
echo "***************"
space

mkdir -p "$HOME/.local/bin"

# Check if it's an Nvidia GPU
lspci -nn | rg -iq "10de:"
is_nvidia=$?

echo "Updating repositories..."
sudo pacman -Sy
space

echo "Installing required packages..."
sudo pacman -S --needed "$pre_install"
space

# Get/install the AUR helper
if command -v paru &> /dev/null; then
    aur_helper="paru"
elif command -v yay &> /dev/null; then
    aur_helper="yay"
else
    selected_aur=$(multi_select "Select AUR helper" "" "all" "${aur_helpers[@]}")

    if echo "$selected_aur" | grep -q "^all$"; then
        sudo pacman -S --needed $aur_helpers
        aur_helper="paru"
    elif [[ -n "$selected_aur" ]]; then
        to_install=$(echo "$selected_aur" | tr '\n' ' ')
        sudo pacman -S --needed $to_install
        aur_helper=$(echo "$selected_aur" | head -n 1)
    fi

    select opt in "${options[@]}"; do
        case $opt in
            "paru"|"yay")
                aur_helper=$opt
                echo "Installing $aur_helper..."
                sudo pacman -S --needed base-devel
                git clone https://aur.archlinux.org/$aur_helper.git /tmp/$aur_helper
                cd /tmp/$aur_helper && makepkg -si --noconfirm && cd -
                break
                ;;
            "None")
                echo "AUR helper is required for some parts of this config. Exiting..."
                exit 1
                ;;
        esac
    done

    space
fi

read -r -n 1 -p ":: Install CachyOS repositories? [Y/n] " response
if [[ -z "$response" || "$response" =~ ^[yY] ]]; then
    echo "Installing Cachy repositories..."
    curl https://mirror.cachyos.org/cachyos-repo.tar.xz -o cachyos-repo.tar.xz
    tar xvf cachyos-repo.tar.xz && cd cachyos-repo
    sudo ./cachyos-repo.sh
    rm -rf cachyos-repo.tar.xz
    rm -rf cachyos-repo
    kernels+=("linux-cachyos" "linux-cachyos-bore" "linux-cachyos-server")

    echo "Installing Cachy packages..."
    selected_cachy=$(multi_select "Select Cachy packages" "$package_preview" "${cachy_packages[@]}")
    install_packages_and_dependencies "${selected_cachy[@]}"
fi
space

echo "Installing config packages..."
install_packages_and_dependencies "${config_packages[@]}"
space

echo "Installing kernel packages..."
kernel_packages=()
selected_kernel=$(multi_select "Select kernels" "$package_preview" "${kernels[@]}")
for pkg in $selected_kernel; do
    kernel_packages+=("$pkg" "${pkg}-headers")
    if [[ $is_nvidia -eq 0 ]]; then
        if [[ "$pkg" == *"cachyos"* ]]; then
            kernel_packages+=("${pkg}-nvidia-open")
        fi
    fi
done

install_packages_and_dependencies "${kernel_packages[@]}"
space

# Install Nvidia packages
if [[ $is_nvidia -eq 0 ]]; then
    # Install drivers if none are present
    if ! paru -Qs nvidia-open > /dev/null; then
        read -r -n 1 -p ":: Install Nvidia drivers? [Y/n] " response
        if [[ -z "$response" || "$response" =~ ^[yY] ]]; then
            $aur_helper -S --needed nvidia-open-dkms
        fi
        space
    fi

    echo "Installing Nvidia packages..."
    selected_nvidia=$(multi_select "Select Nvidia packages" "$package_preview" "${nvidia_packages[@]}")
    install_packages_and_dependencies "${selected_nvidia[@]}"
fi
space

echo "Installing QOL packages..."
selected_qol=$(multi_select "Select QOL packages" "$package_preview" "${qol_packages[@]}")
install_packages_and_dependencies "${selected_qol[@]}"
space

echo "Installing terminals..."
selected_terminals=$(multi_select "Select terminals" "$package_preview" "${terminals[@]}")
install_packages_and_dependencies "${selected_terminals[@]}"
space

echo "Installing file managers..."
selected_file_managers=$(multi_select "Select file managers" "$package_preview" "${file_managers[@]}")
install_packages_and_dependencies "${selected_file_managers[@]}"
space

echo "Installing browsers..."
selected_browsers=$(multi_select "Select browsers" "$package_preview" "${browsers[@]}")
install_packages_and_dependencies "${selected_browsers[@]}"
space

echo "Installing media packages..."
selected_media=$(multi_select "Select media packages" "$package_preview" "${media_packages[@]}")
install_packages_and_dependencies "${selected_media[@]}"
space

echo "Installing socials..."
selected_socials=$(multi_select "Select socials" "$package_preview" "${socials[@]}")
install_packages_and_dependencies "${selected_socials[@]}"
space

echo "Installing dev packages..."
selected_dev=$(multi_select "Select dev packages" "$package_preview" "${dev_packages[@]}")
install_packages_and_dependencies "${selected_dev[@]}"
space

echo "Installing hypr packages..."
selected_hypr=$(multi_select "Select hypr packages" "$package_preview" "${hypr_packages[@]}")
install_packages_and_dependencies "${selected_hypr[@]}"
space

echo "Installing applications..."
selected_applications=$(multi_select "Select applications" "$package_preview" "${applications[@]}")
install_packages_and_dependencies "${selected_applications[@]}"
space

echo "Installing ricing packages..."
selected_ricing=$(multi_select "Select ricing packages" "$package_preview" "${ricing_packages[@]}")
install_packages_and_dependencies "${selected_ricing[@]}"
space

echo "Installing zig..."
zvm i master --zls
space

echo "Installing zlist..."
cd "$HOME/Projects/Zig/zlist"
zig build -Doptimize=ReleaseFast -Dtarget=native
ln -sf "$HOME/Projects/Zig/zlist/zig-out/zl" "$HOME/.local/bin/zl"
cd "$HOME"
space

echo "Installing Maple font..."
"$HOME/Projects/Fonts/maple-font/build.sh"
space
