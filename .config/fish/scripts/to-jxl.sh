#!/usr/bin/env bash
# to_jxl.sh — Losslessly convert images to JPEG XL
# Usage: to_jxl.sh [-j JOBS] [-p PATH]
#
# All inputs use: -d 0 -e 10
#   -d 0: mathematically lossless for non-JPEG inputs.
#         For JPEG inputs, cjxl ignores -d 0 and uses lossless JPEG
#         transcoding by default (distance > 0 would disable it, but 0 doesn't).
#   -e 10: maximum compression effort. Requires --allow_expert_options,
#          without it the cap is 9.
#
# RAW camera formats (CR2, CR3, NEF, ARW, DNG, RAF, ORF, RW2, etc.) are
# intentionally excluded — cjxl cannot represent RAW sensor data losslessly.

set -euo pipefail

# ── Defaults ──────────────────────────────────────────────────────────────────
TOTAL_CORES=$(nproc 2>/dev/null || echo 2)
TARGET_DIR="."

# ── Argument parsing ──────────────────────────────────────────────────────────
usage() {
    cat >&2 <<EOF
Usage: $(basename "$0") [-j JOBS] [-p PATH]

  -j JOBS    Number of parallel jobs (default: half of the CPU cores)
  -c CORES   Number of cores cjxl uses (default: 1 or 2, depending on the CPU)
  -p PATH    Directory to process (default: current directory)
  -h         Show this help
EOF
    exit 1
}

while getopts ":j:c:p:h" opt; do
    case "$opt" in
        j) JOBS="$OPTARG" ;;
        c) CORES="$OPTARG" ;;
        p) TARGET_DIR="$OPTARG" ;;
        h) usage ;;
        :) echo "Error: -$OPTARG requires an argument." >&2; usage ;;
        \?) echo "Error: Unknown option -$OPTARG." >&2; usage ;;
    esac
done

# ── Threads ───────────────────────────────────────────────────────────────────
# If JOBS wasn't set by getopts, default to half the cores
if [[ "${JOBS:-}" == "" ]]; then
    JOBS=$(( TOTAL_CORES / 2 ))
    [[ $JOBS -lt 1 ]] && JOBS=1
fi

# Dynamically calculate threads per job so you never oversubscribe the CPU
if [[ "${CORES:-}" == "" ]]; then
    CORES=$(( TOTAL_CORES / JOBS ))
    [[ $CORES -lt 1 ]] && CORES=1
    
    # Cap it at 2 threads max per cjxl instance to keep RAM usage low for big PNGs
    [[ $CORES -gt 2 ]] && CORES=2
fi

# ── Sanity checks ─────────────────────────────────────────────────────────────
if ! command -v cjxl &>/dev/null; then
    echo "Error: cjxl not found." >&2
    exit 1
fi

if ! command -v trash &>/dev/null && ! command -v trash-put &>/dev/null; then
    echo "Error: trash-cli not found." >&2
    exit 1
fi

TRASH_CMD=$(command -v trash || command -v trash-put)

if ! [[ "$JOBS" =~ ^[1-9][0-9]*$ ]]; then
    echo "Error: -j must be a positive integer." >&2
    exit 1
fi

if ! [[ "$CORES" =~ ^[1-9][0-9]*$ ]]; then
    echo "Error: -c must be a positive integer." >&2
    exit 1
fi

if [[ ! -d "$TARGET_DIR" ]]; then
    echo "Error: '$TARGET_DIR' is not a directory." >&2
    exit 1
fi

# ── Supported formats ─────────────────────────────────────────────────────────
EXTS=(
    "jpg" "jpeg" "jpe" "jif" "jfif"
    "png" "apng"
    "gif"
    "bmp" "dib"
    "tif" "tiff"
    "webp"
    "pnm" "ppm" "pgm" "pbm" "pfm" "pam"
    "pgx"
    "exr"
    "qoi"
)

# ── Build find -iname pattern list ────────────────────────────────────────────
build_find_args() {
    local args=()
    local first=1
    for ext in "${EXTS[@]}"; do
        if [[ $first -eq 1 ]]; then
            args+=( -iname "*.${ext}" )
            first=0
        else
            args+=( -o -iname "*.${ext}" )
        fi
    done
    printf '%s\n' "${args[@]}"
}

mapfile -t FIND_ARGS < <(build_find_args)

# ── Conversion function ───────────────────────────────────────────────────────
convert_to_jxl() {
    local src="$1"
    local dst="${src%.*}.jxl"

    if [[ -e "$dst" ]]; then
        echo "[skip] Already exists: $dst"
        return 0
    fi

    echo "[→jxl] $src"
    # -d 0: lossless. For JPEG inputs cjxl ignores this and uses lossless
    #       JPEG transcoding instead (only -d > 0 would disable that path).
    # -e 10: max effort
    if cjxl -d 0 -e 10 --num_threads="$CORES" "$src" "$dst" 2>/dev/null; then
        "$TRASH_CMD" "$src"
        echo "[done] $src → $dst"
    else
        echo "[error] Failed: $src" >&2
        rm -f "$dst"
    fi
}

export -f convert_to_jxl
export CORES
export TRASH_CMD

# ── Main ──────────────────────────────────────────────────────────────────────
echo "Processing: $(realpath "$TARGET_DIR")"
echo "Jobs:       $JOBS"
echo "Cores:      $CORES"
echo ""

find "$TARGET_DIR" -type f \( "${FIND_ARGS[@]}" \) -print0 \
    | xargs -0 -P "$JOBS" -I{} bash -c 'convert_to_jxl "$@"' _ {}

echo ""
echo "Done."
