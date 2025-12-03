import os
import sys
import subprocess
from pathlib import Path
from ffmpeg_setup import download_ffmpeg

FFMPEG_DOWNLOAD_PATH = Path("template") / "ffmpeg"
TEMPLATE_RELEASE_DIR = Path("template") / "release"

def find_file(directory: Path, filenames: list[str], recursive: bool = True) -> Path:
    """
    Searches for the first matching file in `directory` with a name in `filenames`.
    If `recursive` is True, scans all subdirectories; otherwise, only scans the top-level.
    """
    directory = Path(directory)
    if not directory.exists():
        raise FileNotFoundError(f"Directory does not exist: {directory}")

    # Check top-level files first
    for name in filenames:
        candidate = directory / name
        if candidate.exists() and os.access(candidate, os.X_OK if "exe" in name else os.R_OK):
            return candidate

    # If recursive, scan subdirectories
    if recursive:
        for item in directory.rglob("*"):
            if item.name in filenames and os.access(item, os.X_OK if "exe" in item.name else os.R_OK):
                return item

    raise FileNotFoundError(f"No file {filenames} found in {directory}")


def find_ffmpeg_executable(ffmpeg_dir: Path) -> Path:
    """
    Finds the ffmpeg executable in the given directory using `find_file`.
    Works on Windows and Unix-based systems.
    """
    return find_file(ffmpeg_dir, ["ffmpeg", "ffmpeg.exe"], recursive=True)


# Download ffmpeg into template/release/ffmpeg
download_ffmpeg(str(FFMPEG_DOWNLOAD_PATH))
ffmpeg_path = find_ffmpeg_executable(FFMPEG_DOWNLOAD_PATH)

def run_debug():
    """Runs the .NET project in Debug mode."""
    print("Running...")
    subprocess.run(
        [
            "dotnet",
            "run",
            "--",
            "--config", str(TEMPLATE_RELEASE_DIR / "config.json"),
            "--script", str(TEMPLATE_RELEASE_DIR / "animation.csx"),
            "--ffmpeg-path", str(ffmpeg_path),
            "--output", str(Path("output") / "video.mp4")
        ],
        text=True
    )


if __name__ == "__main__":
    run_debug()