#!/bin/bash
set -e

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

VERBOSE=""
FILTER=""

while [[ $# -gt 0 ]]; do
    case $1 in
        --verbose|-v)
            VERBOSE="--verbosity detailed"
            shift
            ;;
        --filter|-f)
            FILTER="--filter $2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

echo "Running tests..."
dotnet test --configuration Release $VERBOSE $FILTER

echo "Tests completed."
