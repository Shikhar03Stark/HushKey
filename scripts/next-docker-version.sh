#!/usr/bin/env bash
# Get the next semver version for a Docker image on GitHub Container Registry (ghcr.io)
# Usage: ./next-docker-version.sh <github-username> <image-name>
# Example: ./next-docker-version.sh myuser hushkeyapi

set -euo pipefail

# Trap to print last executed command and its exit code on error
trap 'echo "Command failed: $BASH_COMMAND (exit code: $?)"' ERR

USERNAME="$1"
IMAGE="$2"

REPO="ghcr.io/$USERNAME/$IMAGE"

# Fetch tags using a fake token (NOOP)
TAGS=$(curl -s -H "Authorization: Bearer NOOP" "https://ghcr.io/v2/$USERNAME/$IMAGE/tags/list" | jq -r '.tags[]?')

# Filter only tags that match semver (e.g., 1.2.3)
SEMVER_TAGS=$(echo "$TAGS" | grep -E '^[0-9]+\.[0-9]+\.[0-9]+$' || true)

# Sort and get the latest semver tag
LATEST=$(echo "$SEMVER_TAGS" | sort -V | tail -n1)

if [[ -z "$LATEST" ]]; then
  NEXT="1.0.0"
else
  IFS='.' read -r MAJOR MINOR PATCH <<< "$LATEST"
  PATCH=$((PATCH+1))
  NEXT="$MAJOR.$MINOR.$PATCH"
fi

echo "$NEXT"
