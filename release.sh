#!/bin/bash
set -e

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

# Parse arguments
DRY_RUN=false
VERSION=""
INCREMENT="patch"
MESSAGE=""

while [[ $# -gt 0 ]]; do
    case $1 in
        -p|--patch) INCREMENT="patch"; shift ;;
        -n|--minor) INCREMENT="minor"; shift ;;
        -M|--major) INCREMENT="major"; shift ;;
        -m|--message) MESSAGE="$2"; shift 2 ;;
        --dry-run) DRY_RUN=true; shift ;;
        -h|--help)
            echo "Usage: ./release.sh [VERSION] [OPTIONS]"
            echo ""
            echo "Arguments:"
            echo "  VERSION              Explicit version (e.g., 1.2.3)"
            echo ""
            echo "Options:"
            echo "  -p, --patch          Auto-increment patch version (default)"
            echo "  -n, --minor          Auto-increment minor version"
            echo "  -M, --major          Auto-increment major version"
            echo "  -m, --message TEXT   Release notes/message"
            echo "  --dry-run            Preview without executing"
            echo "  -h, --help           Show this help"
            exit 0
            ;;
        -*) echo "Unknown option: $1"; exit 1 ;;
        *) VERSION="$1"; shift ;;
    esac
done

# Check for uncommitted changes
if ! git diff --quiet || ! git diff --staged --quiet; then
    echo "Error: Uncommitted changes. Commit or stash first."
    exit 1
fi

# Check for unpushed commits
LOCAL=$(git rev-parse @)
REMOTE=$(git rev-parse @{u} 2>/dev/null || echo "")
if [ -n "$REMOTE" ] && [ "$LOCAL" != "$REMOTE" ]; then
    echo "Error: Unpushed commits. Push first."
    exit 1
fi

# Get latest version from tags
get_latest_version() {
    git tag -l 'v*.*.*' | sort -V | tail -n1 | sed 's/^v//' || echo "0.0.0"
}

# Increment version
increment_version() {
    local v=$1 part=$2
    IFS='.' read -r major minor patch <<< "$v"
    major=${major:-0}
    minor=${minor:-0}
    patch=${patch:-0}
    case $part in
        major) echo "$((major + 1)).0.0" ;;
        minor) echo "$major.$((minor + 1)).0" ;;
        patch) echo "$major.$minor.$((patch + 1))" ;;
    esac
}

# Determine version
if [ -z "$VERSION" ]; then
    LATEST=$(get_latest_version)
    VERSION=$(increment_version "$LATEST" "$INCREMENT")
fi

TAG="v$VERSION"

# Check tag doesn't exist
if git tag -l "$TAG" | grep -q "$TAG"; then
    echo "Error: Tag $TAG already exists"
    exit 1
fi

echo "Creating release: $TAG"

if [ "$DRY_RUN" = true ]; then
    echo "[DRY RUN] Would create and push tag: $TAG"
    exit 0
fi

# Create and push tag
if [ -n "$MESSAGE" ]; then
    git tag -a "$TAG" -m "$MESSAGE"
else
    git tag "$TAG"
fi
git push origin "$TAG"

echo "Release $TAG created! GitHub Actions will publish to NuGet.org"
