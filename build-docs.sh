#!/bin/bash
set -euo pipefail

dotnet tool restore
cd site
python3 - <<'PY'
import shutil
from pathlib import Path

build_dir = Path(".lunet/build")
if build_dir.exists():
    shutil.rmtree(build_dir)
PY
dotnet tool run lunet --stacktrace build
