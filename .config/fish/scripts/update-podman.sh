#!/usr/bin/env bash
set -euo pipefail

dirs=$(podman ps --format json | jq -r '.[].Labels["com.docker.compose.project.working_dir"] // empty' | sort -u)

for dir in $dirs; do
  name=$(basename "$dir")
  echo "- $name ($dir)"
  cd "$dir"

  compose_file=$(ls compose.yaml compose.yml docker-compose.yaml docker-compose.yml 2>/dev/null | head -n1)

  if [ -z "$compose_file" ]; then
    echo "    No compose file found, skipping..."
    cd - >/dev/null
    continue
  fi

  echo "    Stopping..."
  podman-compose up -d

  echo ""
  if yq -e '.services[] | select(has("build"))' "$compose_file" >/dev/null 2>&1; then
    echo "    Building..."
    podman-compose build --no-cache
  else
    echo "    Pulling..."
    podman-compose pull
  fi

  echo ""
  echo "    Starting..."
  podman-compose up -d
  cd - >/dev/null
done
