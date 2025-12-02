#!/usr/bin/env python3
import json
import sys
import platform
import hashlib
import urllib.request
import zipfile
import tarfile
from pathlib import Path

# ============================================================
#                    CONSTANTS / CONFIG
# ============================================================

DEFAULT_TARGET_FOLDER = "ffmpeg_bin"
SCRIPT_DIR = Path(__file__).resolve().parent

CONFIG_FILE = Path(SCRIPT_DIR / "ffmpeg_source.json")


# Load JSON
with CONFIG_FILE.open("r", encoding="utf-8") as f:
    FFMPEG_SOURCES = json.load(f)


# ============================================================
#                    HELPER FUNCTIONS
# ============================================================

def sha256sum(path: Path) -> str:
    h = hashlib.sha256()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(8192), b""):
            h.update(chunk)
    return h.hexdigest()


def verify_hash(path: Path, expected: str) -> bool:
    if not path.exists():
        return False
    if not expected:
        return False
    return sha256sum(path).lower() == expected.lower()


def download_file(url: str, output_path: Path):
    def _progress(count, block_size, total_size):
        if total_size > 0:
            percent = count * block_size * 100 / total_size
            print(f"\r   {percent:5.1f}% ", end="")

    print(f"\n→ Downloading: {url}")
    try:
        urllib.request.urlretrieve(url, output_path, _progress)
        print("\n   Download complete.")
        return True
    except Exception as e:
        print(f"   Download failed: {e}")
        return False


def extract_archive(archive: Path, dest: Path):
    print(f"→ Extracting {archive.name} ...")
    suffix = archive.suffix.lower()

    if suffix == ".zip":
        with zipfile.ZipFile(archive, "r") as z:
            z.extractall(dest)
    elif suffix in [".xz", ".gz"]:
        with tarfile.open(archive, "r:*") as t:
            t.extractall(dest)
    else:
        raise RuntimeError(f"Unsupported archive type: {suffix}")

    print("   Extraction complete.")


def detect_platform() -> str:
    sysname = platform.system()
    if sysname not in FFMPEG_SOURCES:
        raise RuntimeError(f"Unsupported platform: {sysname}")
    return sysname


def save_sources():
    """Safely write updated hashes back to JSON."""
    tmp = CONFIG_FILE.with_suffix(".tmp")
    with tmp.open("w", encoding="utf-8") as f:
        json.dump(FFMPEG_SOURCES, f, indent=4)
    tmp.replace(CONFIG_FILE)


def ask_yes_no(question: str) -> bool:
    """Prompt a yes/no question."""
    while True:
        ans = input(question + " [y/n]: ").strip().lower()
        if ans in ("y", "yes"):
            return True
        if ans in ("n", "no"):
            return False

def get_zip_root_dir(zip_path: Path) -> Path:
    with zipfile.ZipFile(zip_path, 'r') as z:
        for name in z.namelist():
            parts = Path(name).parts
            if parts:
                return Path(parts[0])
    return Path(zip_path.stem)

def get_tar_root_dir(tar_path: Path) -> Path:
    with tarfile.open(tar_path, 'r:*') as t:
        for member in t:
            parts = Path(member.name).parts
            if parts:
                return Path(parts[0])
    return Path(tar_path.stem)

def get_archive_root_dir(archive_path: Path) -> Path:
    suffix = archive_path.suffix.lower()
    if suffix == ".zip":
        return get_zip_root_dir(archive_path)
    elif suffix in [".gz", ".xz"]:
        return get_tar_root_dir(archive_path)
    else:
        raise RuntimeError(f"Unsupported archive type: {suffix}")


# ============================================================
#                 PUBLIC FUNCTION (IMPORT SAFE)
# ============================================================

def download_ffmpeg(target_folder: str = DEFAULT_TARGET_FOLDER, check_hash=True) -> Path:
    """
    Downloads and extracts the correct standalone FFmpeg build for the
    current platform into target_folder.

    Returns:
        Path to the directory where FFmpeg was installed.

    Raises:
        RuntimeError on failure.
    """
    system = detect_platform()
    sources = FFMPEG_SOURCES[system]

    dest = Path(target_folder).expanduser().absolute()
    dest.mkdir(parents=True, exist_ok=True)

    print(f"\n=== FFmpeg Setup (Module) ===")
    print(f"Platform: {system}")
    print(f"Destination: {dest}\n")

    for entry in sources:
        url = entry["url"]
        expected_hash = entry.get("sha256", "")
        desc = entry.get("desc", url)
        temp_archive = dest / Path(url).name
        print(f"Checking source: {desc}")
                
        # If archive already exists, check if the extracted folder exists
        if temp_archive.exists():
            print("→ Checking existing archive")
            extracted_dir = dest / get_archive_root_dir(temp_archive)
            if extracted_dir.exists() and extracted_dir.is_dir():
                print(f"→ Already extracted directory exists: {extracted_dir}")
                return dest

        # If cached file matches hash, reuse it
        if expected_hash and temp_archive.exists() and verify_hash(temp_archive, expected_hash):
            print("→ Local archive matches hash. Skipping download.")
            extract_archive(temp_archive, dest)
            return dest

        # Attempt download
        if not download_file(url, temp_archive):
            continue

        current_hash = sha256sum(temp_archive)
        hash_is_empty = not expected_hash

        # Case 1: No expected hash → ask user if they want to trust this one
        if hash_is_empty:
            print(f"\n⚠ No expected hash for this source.")
            print(f"   Downloaded file hash: {current_hash}")
            if ask_yes_no("Would you like to trust this file and store its hash?"):
                entry["sha256"] = current_hash
                save_sources()
                print("→ Hash saved. Trusting this source.")
                extract_archive(temp_archive, dest)
                return dest
            else:
                print("→ Not trusting this file. Trying next source...")
                continue

        # Case 2: Hash mismatch
        if check_hash and not verify_hash(temp_archive, expected_hash):
            print("\n❌ Hash mismatch!")
            print("!! WARNING: File may be tampered with OR new upstream release.")
            print(f"   Expected: {expected_hash}")
            print(f"   Got:      {current_hash}")

            if ask_yes_no("Trust this file anyway and continue?"):
                print("→ Continuing with a potentially unsafe file.")
                extract_archive(temp_archive, dest)
                return dest

            if ask_yes_no("Update JSON to use the new hash instead?"):
                entry["sha256"] = current_hash
                save_sources()
                print("→ New hash saved. Using this source.")
                extract_archive(temp_archive, dest)
                return dest

            print("→ Rejecting this file. Trying next source...")
            continue

        # Case 3: Hash valid
        extract_archive(temp_archive, dest)
        print("FFmpeg successfully installed.\n")
        return dest

    raise RuntimeError("All FFmpeg sources failed — cannot install FFmpeg.")


# ============================================================
#                CLI ENTRY POINT (Script Mode)
# ============================================================

if __name__ == "__main__":
    target = sys.argv[1] if len(sys.argv) > 1 else DEFAULT_TARGET_FOLDER
    try:
        path = download_ffmpeg(target)
        print(f"FFmpeg installed at: {path}")
    except Exception as e:
        print(f"Error: {e}")
        sys.exit(1)