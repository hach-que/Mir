#!/bin/bash
cd "${0%/*}"

echo "Compiling assets..."
cd assets
mono ../../Protogame/ProtogameAssetTool/bin/Debug/ProtogameAssetTool.exe \
    -o ../compiled \
    -p Linux
cd ..
