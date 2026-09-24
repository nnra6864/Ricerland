#!/usr/bin/env bash

set -euo pipefail

TARGET_DIR="${1:-.}"

if ! command -v rsgain &>/dev/null; then
    echo "Error: rsgain is not installed." >&2
    exit 1
fi

THREADS=$(nproc 2>/dev/null || getconf _NPROCESSORS_ONLN 2>/dev/null || echo 4)

EXT_REGEX=$(rsgain --help | grep -A 3 "supports writing tags" | grep -v "rsgain" | grep -oP '\.\K[a-z0-9]+' | sort -u | paste -sd '|' -)

echo "Normalizing library: $TARGET_DIR ($THREADS threads)"
echo "Supported extensions: $EXT_REGEX"

find "$TARGET_DIR" -type f -regextype posix-extended \
    -iregex ".*\\.($EXT_REGEX)$" -print0 | \
    xargs -0 -r -P "$THREADS" -n 32 rsgain custom -s i -t -c a -m -1.0 -o t -I 3 -S 2> >(grep -v '^TagLib:' >&2)

echo "Done!"
