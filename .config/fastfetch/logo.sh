#!/bin/bash

# Ensure UTF-8 locale so wc -L uses wcwidth() for 0-width combining marks
export LC_ALL=C.UTF-8

# Make text not bold
printf '\033[22m'

# Resolve logo path without subshells
SCRIPT_DIR="${0%/*}"
LOGO_FILE="${SCRIPT_DIR}/eastern_orthodox_logo"

# Read logo into an array and get the width
mapfile -t LOGO_LINES < "$LOGO_FILE"
LOGO_WIDTH=$(wc -L < "$LOGO_FILE")

# Variables
COLOR_SYMBOL="🟍🟍🟍"
COLOR_SYMBOL_WIDTH=$(wc -L <<< "$COLOR_SYMBOL")
COLOR_WIDTH=$(( COLOR_SYMBOL_WIDTH * 8 ))

# Padding calculation
if (( LOGO_WIDTH > COLOR_WIDTH )); then
    LOGO_PADDING=0
    COLOR_PADDING=$(( (LOGO_WIDTH - COLOR_WIDTH) / 2 ))
elif (( COLOR_WIDTH > LOGO_WIDTH )); then
    LOGO_PADDING=$(( (COLOR_WIDTH - LOGO_WIDTH) / 2 ))
    COLOR_PADDING=0
else
    LOGO_PADDING=0
    COLOR_PADDING=0
fi

# Padding string slicing (fastest way in Bash)
PAD='                                                                                                                                '
LOGO_SPACES=${PAD:0:LOGO_PADDING}
COLOR_SPACES=${PAD:0:COLOR_PADDING}

# Logo — single printf call
printf '%s\n' "${LOGO_LINES[@]/#/$LOGO_SPACES}"
printf '\n'

# Regular colors
printf '%s\033[30m%s\033[31m%s\033[32m%s\033[33m%s\033[34m%s\033[35m%s\033[36m%s\033[37m%s\033[0m\n' \
    "$COLOR_SPACES" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL"

# Bright colors
printf '%s\033[90m%s\033[91m%s\033[92m%s\033[93m%s\033[94m%s\033[95m%s\033[96m%s\033[97m%s\033[0m\n' \
    "$COLOR_SPACES" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL" "$COLOR_SYMBOL"
