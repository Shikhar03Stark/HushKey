#!/usr/bin/env bash
# Get the next semver version for a Docker image on Docker Hub
# Usage: ./next-docker-version.sh <dockerhub-username> <image-name>
# Example: ./next-docker-version.sh myuser hushkey-backend

set -euo pipefail

USERNAME="$1"
IMAGE="$2"

# Fetch all tags from Docker Hub (handle pagination)
TAGS=""
PAGE=1
while :; do
  RESPONSE=$(curl -s "https://hub.docker.com/v2/repositories/$USERNAME/$IMAGE/tags/?page_size=100&page=$PAGE")
  PAGE_TAGS=$(echo "$RESPONSE" | jq -r '.results[].name')
  TAGS="$TAGS
$PAGE_TAGS"
  NEXT=$(echo "$RESPONSE" | jq -r '.next')
  if [[ "$NEXT" == "null" || -z "$NEXT" ]]; then
    break
  fi
  PAGE=$((PAGE+1))
done

# Filter semver tags, sort, and get the latest
LATEST=$(echo "$TAGS" | sort -V | tail -n1)

if [[ -z "$LATEST" ]]; then
  NEXT="1.0.0"
else
  IFS='.' read -r MAJOR MINOR PATCH <<< "$LATEST"
  PATCH=$((PATCH+1))
  NEXT="$MAJOR.$MINOR.$PATCH"
fi

echo "$NEXT"
