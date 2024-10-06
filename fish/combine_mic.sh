#!/bin/bash

# Create a new virtual source that other apps can use as input
SOURCE_NAME="combined_mic"

# First create a null sink that will serve as our mixing point
pactl load-module module-null-sink sink_name=${SOURCE_NAME}_mix sink_properties=device.description="${SOURCE_NAME}_mix"

# Create a virtual source that monitors our null sink
# This makes the mixed audio available as an input
pactl load-module module-virtual-source source_name=$SOURCE_NAME master=${SOURCE_NAME}_mix.monitor source_properties=device.description="Combined Microphones"

# Get a list of all audio input sources
SOURCES=$(pactl list sources short | awk '/input/ {print $2}')

# Loop through each audio input source and create a loopback to our mixing sink
for SOURCE in $SOURCES; do
    # Skip any virtual sources we just created to avoid feedback loops
    if [[ $SOURCE != *"$SOURCE_NAME"* && $SOURCE != *"monitor"* ]]; then
        pactl load-module module-loopback source=$SOURCE sink=${SOURCE_NAME}_mix latency_msec=1
    fi
done

echo "All microphone inputs have been combined into the virtual source '$SOURCE_NAME'"
echo "You can now select '$SOURCE_NAME' as an input device in your applications"
