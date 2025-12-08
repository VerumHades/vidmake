import os
import subprocess
import sys
import zipfile
from debug_run import find_ffmpeg_executable, find_file  # import your helper function
from pathlib import Path

# -----------------------------
# Constants
# -----------------------------
CONFIGURATION = "Release"
FRAMEWORK = "net9.0"
RUNTIME = "win-x64"  # adjust if targeting another runtime

TEMPLATE_RELEASE_DIR = os.path.join("Vidmake","template", "release")
FFMPEG_DOWNLOAD_PATH = os.path.join("Vidmake","template", "ffmpeg")
BUILD_OUTPUT_DIR = os.path.join("bin", CONFIGURATION, FRAMEWORK, RUNTIME)
ZIP_OUTPUT_FILE = "sample.zip"

INCLUDE_FFMPEG = True

# -----------------------------
# Helper Functions
# -----------------------------
def build_project():
    """Builds the .NET project in Release mode for the specified runtime."""
    print(f"Publishing project: dotnet publish -c {CONFIGURATION} -r {RUNTIME}")
    result = subprocess.run([
            "dotnet",
            "publish",
            "-c", CONFIGURATION,
            "-r", RUNTIME,
            "--self-contained", "true",
            "/p:PublishSingleFile=true",
            #"/p:PublishTrimmed=true"
        ],
        stdout=subprocess.PIPE,
        stderr=subprocess.PIPE,
        text=True
    )
    if result.returncode != 0:
        print("Build failed:")
        print(result.stdout)
        print(result.stderr)
        sys.exit(1)
    else:
        print("Build succeeded.")


def find_ffmpeg_license(ffmpeg_dir: Path) -> Path:
    """
    Finds the FFmpeg LICENSE file in the given directory using `find_file`.
    Checks common names like LICENSE, LICENSE.txt, COPYING.
    """
    return find_file(ffmpeg_dir, ["LICENSE", "LICENSE.txt", "COPYING"], recursive=True)

def create_release_zip():
    """Creates a zip archive containing the executable in 'bin/' and template files in the root.
    Includes only the FFmpeg executable and its LICENSE file inside ffmpeg/ in the zip.
    """
    zip_file = os.path.abspath(ZIP_OUTPUT_FILE)

    with zipfile.ZipFile(zip_file, 'w', zipfile.ZIP_DEFLATED) as zipf:
        # 1. Add build output (executable) into bin/
        build_dir = os.path.abspath(BUILD_OUTPUT_DIR)
        for root, dirs, files in os.walk(build_dir):
            for file in files:
                file_path = os.path.join(root, file)
                # path inside zip: bin/<filename> or bin/<subdirs>/<filename>
                rel_path = os.path.relpath(file_path, build_dir)
                zipf.write(file_path, os.path.join("bin", rel_path))

        # 2. Add ffmpeg executable and LICENSE
        if INCLUDE_FFMPEG:
            # Find ffmpeg executable
            ffmpeg_path = find_ffmpeg_executable(FFMPEG_DOWNLOAD_PATH)
            zipf.write(ffmpeg_path, os.path.join("ffmpeg", ffmpeg_path.name))

            # Find LICENSE file using the new function
            try:
                license_path = find_ffmpeg_license(FFMPEG_DOWNLOAD_PATH)
                zipf.write(license_path, os.path.join("ffmpeg", license_path.name))
            except FileNotFoundError:
                print("Warning: FFmpeg LICENSE file not found, skipping.")

        # 3. Add other template files
        template_dir = os.path.abspath(TEMPLATE_RELEASE_DIR)
        if os.path.exists(template_dir):
            for root, dirs, files in os.walk(template_dir):
                for file in files:
                    file_path = os.path.join(root, file)
                    rel_path = os.path.relpath(file_path, template_dir)
                    zipf.write(file_path, rel_path)

    print(f"Created zip archive: {zip_file}")


# -----------------------------
# Main
# -----------------------------
if __name__ == "__main__":
    build_project()
    create_release_zip()
