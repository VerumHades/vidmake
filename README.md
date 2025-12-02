# Vidmake - A C# Animation Rendering CLI

A lightweight C# framework for programmatic video creation, inspired by [Manim](https://github.com/ManimCommunity/manim). Supports frame-by-frame rendering of drawable elements, animations via interpolators, and output to video through **FFmpeg**.

## Warning

**Rendering scripts are not sandboxed.**  

Users are responsible for the content they render. Do **not** run scripts, render scenes, or process files that you do not understand or trust, as they may execute unsafe operations, modify your system, or access sensitive data.  
Use at your own risk. The author is not liable for any damage, data loss, or security issues resulting from its use.

## Runinning the app

You can find the [latest release here](https://github.com/VerumHades/vidmake/releases/latest)
When you are ready check out what you can do in the video [scripts](#video-scripting)
### Running the sample project

The quickest way to get setup. Includes the standalone binary, ffmpeg and a sample script with a config.

1. Find `sample` in the latest release, download and unpack it.
It should look like something like this:

```
unzipped_sample/
│
├── bin/
│   ├── Vidmake.exe
│   ├── Vidmake.dll
│   ├── Vidmake.pdb
│   ├── dependencies/
│   │   ├── SomeLibrary.dll
│   │   ├── AnotherLibrary.dll
│   │   └── ...
│   └── ...
│
├── config.json
└── animation.csx
```

2. Navigate to the root folder (one with `config.json` in it), open a console;

You can render the sample video using:
```bash
bin\\Vidmake --config config.json
```

### Getting just the standalone build

You can download it in the latest releases under `standalone`.

---

## Configuration

You can configure the rendering pipeline using **command-line arguments** or a **JSON config file**.

### Command-Line Arguments

| Option          | Description |
|-----------------|-------------|
| `--width`       | Width of the video in pixels |
| `--height`      | Height of the video in pixels |
| `--fps`         | Frames per second |
| `--output-file` | Output video file path |
| `--ffmpeg-path` | Path to the FFmpeg executable |
| `--script` | Path to the C# script (`.csx`) defining the scene and elements |
| `--config` | Path a [json](#json-config) configuration file |

Example:

```bash
Vidmake --width 1920 --height 1080 --fps 60 --output-file video.mp4 --ffmpeg-path ffmpeg.exe --script myscene.csx 
```
Or simply:

```bash
Vidmake --config config.json
```

### JSON Config

Instead of repeating arguments, you can provide a JSON configuration file. Example:
```json
{
    "width": 1920,
    "height": 1080,
    "fps": 60,
    "outputFile": "video.mp4",
    "ffmpegPath": "ffmpeg\\win-x86\\ffmpeg.exe",
    "scriptFile": "myscene.csx"
}
```

Load the configuration in your CLI program, and it will override default values and command-line options.

---

## Video scripting

In vidmake videos are written in C# scripts. See the available elements [here](#elements)

### Quickstart Example

Here’s a minimal scene script demonstrating how to animate rectangles using the framework:

```csharp
var rect = scene.Add(new Rectangle(100, 100)); // 1
var rect2 = scene.Add(new Rectangle(10, 100)); // 1

rect.Move(100, 100); // 2
rect2.Move(1000, 0); // 2

Go(1); // 3

rect.Move(100, 200); // 4
Go(1); // 5
```

Lets go trough it step by step:

1. Two rectangles of sizes `(100,100)` and `(10,100)` are added to the scene
2. Their next positions are set to `(100,100)` and `(1000, 0)` respectively
3. The `Go` command is called with the duration of `1 second`, thus frames are rendered such that in 1 second both the rectangles have moved to their next positions.
4. Only one rectangles next position is set (the other one will remain static during the next animation sequence)
5. The `Go` command is called again, the one rectangle that is not in its designated position moves to it in the 1 second time frame.

You can go find out more about elements here: [See the elements guide](doc/ELEMENTS.md)

## Third-Party Software

This project uses [FFmpeg](https://ffmpeg.org/) for video processing.

- FFmpeg is licensed under the GNU General Public License (GPL) version 3.
- The FFmpeg executable included in this distribution comes with its license.
- You can find the full FFmpeg license as `ffmpeg/win32-x64.LICENSE` file included in this every sample release that includes it.

Please note that FFmpeg is developed and maintained by the FFmpeg team. Our project does not modify FFmpeg.