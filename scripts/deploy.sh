#!/usr/bin/env bash
# Update hushkey-api.yaml with the latest image tag from print-latest-tag.sh
# Usage: ./update_manifest.sh

set -euo pipefail

TENANTID=$1
UAMI_CLIENTID=$2

MANIFEST="infra/k8s/manifest.template.yaml"
OUTPUT="infra/k8s/deploy.yaml"
TAG=$(./scripts/print-latest-tag.sh shikhar03stark hushkeyapi)

if [[ -z "$TAG" ]]; then
  echo "Could not determine image tag."
  exit 1
fi

# Substitute __IMAGE_TAG__ and __TENANTID__ with the latest tag and provided tenant id
sed "s|__IMAGE_TAG__|$TAG|g; s|__TENANTID__|$TENANTID|g; s|__UAMI_CLIENTID__|$UAMI_CLIENTID|g" "$MANIFEST" > "$MANIFEST.tmp" && mv "$MANIFEST.tmp" "$OUTPUT"
echo "Updated $MANIFEST with image tag: $TAG and tenant id: $TENANTID"

kubectl apply -f ./infra/k8s/deploy.yaml
