#!/bin/bash

# Fixes inconsistent logo width
export LC_ALL=en_US.UTF-8

# Make text not bold
printf "\033[22m"

# Variables
LOGO="$(dirname "$0")/eastern_orthodox_logo"
LOGO_WIDTH=$(wc -L < "$LOGO")
COLOR_SYMBOL="፠፠፠"
COLOR_WIDTH=$(($(printf $COLOR_SYMBOL | wc -m) * 8))

# Padding
if [ $LOGO_WIDTH -gt $COLOR_WIDTH ]; then
    LOGO_PADDING=0
    COLOR_PADDING=$(( (LOGO_WIDTH - COLOR_WIDTH) / 2 ))
elif [ $COLOR_WIDTH -gt $LOGO_WIDTH ]; then
    LOGO_PADDING=$(( (COLOR_WIDTH - LOGO_WIDTH) / 2 ))
    COLOR_PADDING=0
else
    LOGO_PADDING=0
    COLOR_PADDING=0
fi

# Spaces
LOGO_SPACES=$(printf "%*s" $LOGO_PADDING "")
COLOR_SPACES=$(printf "%*s" $COLOR_PADDING "")

# Logo
sed "s/^/$LOGO_SPACES/" "$LOGO"
echo ""

# Regular colors
echo -e "${COLOR_SPACES}\033[30m$COLOR_SYMBOL\033[31m$COLOR_SYMBOL\033[32m$COLOR_SYMBOL\033[33m$COLOR_SYMBOL\033[34m$COLOR_SYMBOL\033[35m$COLOR_SYMBOL\033[36m$COLOR_SYMBOL\033[37m$COLOR_SYMBOL\033[0m"
# Bright colors
echo -e "${COLOR_SPACES}\033[90m$COLOR_SYMBOL\033[91m$COLOR_SYMBOL\033[92m$COLOR_SYMBOL\033[93m$COLOR_SYMBOL\033[94m$COLOR_SYMBOL\033[95m$COLOR_SYMBOL\033[96m$COLOR_SYMBOL\033[97m$COLOR_SYMBOL\033[0m"
