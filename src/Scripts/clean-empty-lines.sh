#!/bin/bash

echo "Cleaning extra empty lines at EOF for all .cs files..."

find . -type f -name "*.cs" | while read -r file; do
    before=$(wc -l < "$file")
    perl -0777 -i -pe 's/(\n)*$/\n/' "$file"
    after=$(wc -l < "$file")

    if [ "$before" -ne "$after" ]; then
        echo "[MODIFIED] $file (removed extra newlines)"
    else
        echo "[OK]       $file"
    fi
done

echo "Cleanup complete!"