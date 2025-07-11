#!/bin/bash

# Fixes inconsistent logo width
export LC_ALL=en_US.UTF-8

# NN
cat "$(dirname "$0")/NN"
echo ""
echo ""

# Get the width of the first line of the logo
LOGO_WIDTH=$(head -n1 "$(dirname "$0")/NN" | wc -m)
# Color bar width is 32 characters (8 colors × 4 chars each including color codes)
COLOR_WIDTH=32

# Right
PADDING=$((LOGO_WIDTH - COLOR_WIDTH - 1))

# Center
#PADDING=$(((LOGO_WIDTH / 2) - (COLOR_WIDTH/ 2)))

# Add padding spaces before the color bars
SPACES=$(printf "%*s" $PADDING "")

# Regular colors
echo -e "${SPACES}\033[30m████\033[31m████\033[32m████\033[33m████\033[34m████\033[35m████\033[36m████\033[37m████\033[0m"
# Bright colors
echo -e "${SPACES}\033[90m████\033[91m████\033[92m████\033[93m████\033[94m████\033[95m████\033[96m████\033[97m████\033[0m"
