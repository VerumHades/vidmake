import os
import subprocess
import sys
import zipfile

# -----------------------------
# Constants
# -----------------------------
CONFIGURATION = "Release"
FRAMEWORK = "net9.0"
RUNTIME = "win-x64"  # adjust if targeting another runtime

TEMPLATE_RELEASE_DIR = os.path.join("template", "release")
BUILD_OUTPUT_DIR = os.path.join("bin", CONFIGURATION, FRAMEWORK, RUNTIME)
ZIP_OUTPUT_FILE = "Vidmake_Release.zip"

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

def create_release_zip():
    """Creates a zip archive containing the executable in 'bin/' and template files in the root."""
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

        # 2. Add template files directly into the root of the zip
        template_dir = os.path.abspath(TEMPLATE_RELEASE_DIR)
        if os.path.exists(template_dir):
            for root, dirs, files in os.walk(template_dir):
                for file in files:
                    file_path = os.path.join(root, file)
                    # path inside zip: just relative to template_dir
                    rel_path = os.path.relpath(file_path, template_dir)
                    zipf.write(file_path, rel_path)

    print(f"Created zip archive: {zip_file}")

# -----------------------------
# Main
# -----------------------------
if __name__ == "__main__":
    build_project()
    create_release_zip()
