#!/usr/bin/env bash
set -euo pipefail

dirs=$(podman ps --format json | jq -r '.[].Labels["com.docker.compose.project.working_dir"] // empty' | sort -u)

for dir in $dirs; do
  name=$(basename "$dir")
  echo "- $name ($dir)"
  cd "$dir"

  compose_file=""
  for candidate in compose.yaml compose.yml docker-compose.yaml docker-compose.yml; do
    if [ -f "$candidate" ]; then
      compose_file="$candidate"
      break
    fi
  done

  if [ -z "$compose_file" ]; then
    echo "    No compose file found, skipping..."
    cd - >/dev/null
    continue
  fi

  if yq -e '.services[] | select(has("build"))' "$compose_file" >/dev/null 2>&1; then
    echo "    Building..."
    podman-compose build --no-cache
  else
    echo "    Pulling..."
    podman-compose pull
  fi

  echo "    Restarting..."
  podman-compose down
  podman-compose up -d
  cd - >/dev/null

done

echo "Pruning..."
podman system prune -a
