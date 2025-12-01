import os
import sys
import subprocess
from pathlib import Path
from ffmpeg_setup import download_ffmpeg


TEMPLATE_RELEASE_DIR = Path("template") / "release"


def find_ffmpeg_executable(ffmpeg_dir: Path) -> Path:
    """
    Scans a directory for an ffmpeg executable and returns the first valid one found.
    Works on Windows and Unix-based systems.
    """
    ffmpeg_dir = Path(ffmpeg_dir)

    if not ffmpeg_dir.exists():
        raise FileNotFoundError(f"FFmpeg directory does not exist: {ffmpeg_dir}")

    candidates = ["ffmpeg", "ffmpeg.exe"]

    # Search for both names
    for name in candidates:
        exe = ffmpeg_dir / name
        if exe.exists() and os.access(exe, os.X_OK):
            return exe

    # Fallback: scan recursively
    for item in ffmpeg_dir.rglob("*"):
        if item.name in candidates and os.access(item, os.X_OK):
            return item

    raise FileNotFoundError(f"No ffmpeg executable found in: {ffmpeg_dir}")


# Download ffmpeg into template/release/ffmpeg
ffmpeg_dir = TEMPLATE_RELEASE_DIR / "ffmpeg"
download_ffmpeg(str(ffmpeg_dir))

# Find the ffmpeg executable automatically
ffmpeg_path = find_ffmpeg_executable(ffmpeg_dir)

def run_debug():
    """Runs the .NET project in Debug mode."""
    print("Running...")
    result = subprocess.run(
        [
            "dotnet",
            "run",
            "--",
            "--config", str(TEMPLATE_RELEASE_DIR / "config.json"),
            "--script", str(TEMPLATE_RELEASE_DIR / "animation.csx"),
            "--ffmpeg-path", str(ffmpeg_path),
            "--output-file", str(Path("output") / "video.mp4")
        ],
        text=True
    )

    if result.returncode != 0:
        print("Build failed:")
        print(result.stdout)
        print(result.stderr)
        sys.exit(1)
    else:
        print("Build succeeded.")

if __name__ == "__main__":
    run_debug()