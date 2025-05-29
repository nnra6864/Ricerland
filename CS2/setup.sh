#!/bin/bash

SOURCE_FILE="./autoexec.cfg"
CONFIG_NAME="autoexec.cfg"
PERSISTENCE_NAME="listenserver.cfg"
MOVEMENT_CONFIG_URL="https://github.com/1rubyrain/cfg/releases/download/v1.2/cfg.zip"

print_separator() {
    local COLUMNS
    COLUMNS=$(tput cols 2>/dev/null || echo 80)

    echo ""
    printf '%*s\n' "${COLUMNS}" '' | tr ' ' '-'
    echo ""
}

print_separator

echo "Movement config by ruby rain: https://steamcommunity.com/id/r_by"
echo "Manual installation: https://steamcommunity.com/sharedfiles/filedetails/?id=3313210014"

print_separator

if [ ! -f "$SOURCE_FILE" ]; then
    echo "Error: Source file '$SOURCE_FILE' not found in the same directory as this script"
    exit 1
fi
read -p "Enter the full path to your Steam Library directory (e.g., /home/user/.steam/steam): " STEAM_LIBRARY_DIR
TARGET_DIR="$STEAM_LIBRARY_DIR/steamapps/common/Counter-Strike Global Offensive/game/csgo/cfg"

if [ -z "$STEAM_LIBRARY_DIR" ]; then
    echo "Error: No directory entered, exiting"
    exit 1
fi
if [ ! -d "$TARGET_DIR" ]; then
    echo "Error: Target directory '$TARGET_DIR' does not exist"
    echo "Please ensure the Steam Library path is correct and CS2 is installed in that library"
    exit 1
fi

MOVEMENT_CONFIG_PATH="$TARGET_DIR/cfg.zip"
wget -O "$MOVEMENT_CONFIG_PATH" "$MOVEMENT_CONFIG_URL"
if [ $? -ne 0 ]; then
    echo "Error: Failed to download the file from '$MOVEMENT_CONFIG_URL'"
    exit 1
fi

echo "Successfully downloaded '$MOVEMENT_CONFIG_URL' to '$MOVEMENT_CONFIG_PATH'"

print_separator

unzip -q -o "$MOVEMENT_CONFIG_PATH" -d "$TARGET_DIR"
if [ $? -ne 0 ]; then
    echo "Error: Failed to unzip '$MOVEMENT_CONFIG_PATH'"
    exit 1
fi
echo "Successfully extracted the movement config into '$TARGET_DIR/movement'"

rm -rf "$MOVEMENT_CONFIG_PATH"
echo "Successfully deleted '$MOVEMENT_CONFIG_PATH'"

print_separator

echo "exec movement/setup" > "$TARGET_DIR/listenserver.cfg"
echo "Successfully created '$TARGET_DIR/listenserver.cfg'"

print_separator

FULL_LINK_PATH="$TARGET_DIR/$CONFIG_NAME"
if [ -e "$FULL_LINK_PATH" ]; then
    read -p "A file or link named '$CONFIG_NAME' already exists in '$TARGET_DIR'. Overwrite? (y/N): " confirm_overwrite
    if [[ ! "$confirm_overwrite" =~ ^[Yy]$ ]]; then
        echo "Operation cancelled, exiting"
        echo "Make sure to place 'exec movement/setup' at the end of your autoexec.cfg"
        exit 0
    fi
    rm "$FULL_LINK_PATH"
fi

ln -s "$(realpath "$SOURCE_FILE")" "$FULL_LINK_PATH"
if [ $? -eq 0 ]; then
    echo "Successfully linked '$(realpath "$SOURCE_FILE")' to '$FULL_LINK_PATH'"
else
    echo "Error: Failed to create symbolic link to config"
    exit 1
fi

print_separator

exit 0
