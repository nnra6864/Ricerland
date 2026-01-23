#!/bin/bash

if [ $# -eq 0 ]; then
    echo "Usage: $0 <text> [sleep_time] [delay]"
    echo "  text       - The text to type"
    echo "  sleep_time - Optional initial sleep before typing (default: 3)"
    echo "  delay      - Optional delay between characters (default: 0.1)"
    exit 1
fi

text="$1"
sleep_time="${2:-1}"
delay="${3:-0.01}"

sleep "$sleep_time"

for (( i=0; i<${#text}; i++ )); do
    char="${text:$i:1}"
    ydotool type "$char"
    sleep "$delay"
done
