#!/usr/bin/env bash
# Get the next semver version for a Docker image on GitHub Container Registry (ghcr.io)
# Usage: ./next-docker-version.sh <github-username> <image-name>
# Example: ./next-docker-version.sh myuser hushkeyapi

set -euo pipefail

USERNAME="$1"
IMAGE="$2"

# GitHub API requires a token for private images, but public images can be accessed anonymously
# The repo must be in the format: ghcr.io/<owner>/<image>
# We'll use the GitHub API to fetch tags

REPO="ghcr.io/$USERNAME/$IMAGE"

# GitHub API: https://api.github.com/v2/packages/container/<image>/versions
# But for public images, we use the REST API for container packages
# API: https://ghcr.io/v2/<owner>/<image>/tags/list

TAGS=$(curl -s "https://ghcr.io/v2/$USERNAME/$IMAGE/tags/list" | jq -r '.tags[]?')

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
