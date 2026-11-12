#!/usr/bin/env bash
# Builds the ODC External Library upload package (IbanCheckerOS_Asset.zip) from this folder (macOS/Linux).
set -euo pipefail
cd "$(dirname "$0")"
rm -rf ./publish IbanCheckerOS_Asset.zip
dotnet publish ./IbanCheckerOS.csproj -c Release -r linux-x64 --self-contained false -o ./publish
(cd ./publish && zip -qr ../IbanCheckerOS_Asset.zip .)
echo "Created $(pwd)/IbanCheckerOS_Asset.zip"
