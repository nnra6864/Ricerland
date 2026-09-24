#!/bin/bash

if [ -z "$1" ]; then
    echo "Usage: $0 <target-ip>"
    echo "Example: $0 192.168.0.20"
    exit 1
fi

TARGET_IP="$1"

MODULE_ID=$(pactl list modules short | grep "module-tunnel-sink.*server=${TARGET_IP}" | awk '{print $1}')

if [ -n "$MODULE_ID" ]; then
    echo "Tunnel sink exists, removing it..."
    pactl unload-module "$MODULE_ID"
    echo "Disconnected from audio server"
else
    echo "Creating tunnel sink..."
    pactl load-module module-tunnel-sink server="$TARGET_IP"
	sleep 0.5
	SINK_NAME=$(pactl list sinks short | grep -i tunnel | awk '{print $2}' | head -n1)
    if [ -n "$SINK_NAME" ]; then
        pactl set-default-sink "$SINK_NAME"
        echo "Connected to audio server at $TARGET_IP (sink: $SINK_NAME)"
    else
        echo "Connected to audio server at $TARGET_IP (couldn't auto-set as default)"
    fi
fi   
