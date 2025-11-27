# Architecture

## Core Classes

- **DrawableArea**: Represents a section of a frame buffer; supports `SetPixel`, `SetRow`, `SetColumn`, and `Fill`.
- **Pixel**: Struct representing a color (R, G, B, A); predefined colors: `Black`, `White`, `Red`, `Green`, `Blue`.
- **Element (abstract)**: Implements `ISurface` and `TransitionalTransform`; stores current and next position/size for animation; must implement `Render(ref DrawableArea, float animationPercentage)`.
- **Rectangle**: Simple drawable rectangle element; supports customizable `BackgroundColor`; automatically animates size and position.
- **TransitionalTransform**: Stores `Current` and `Next` values for X, Y, Width, Height; provides methods for moving/resizing and interpolating.
- **IInterpolator**: Provides a contract for interpolating between two float values.
- **Scene**: Contains a list of `Element`s; main driver function `Go(int seconds)` renders frames to the target.
- **RenderTarget (abstract)**: Base class for all render targets; handles `AddElementFrames`.
- **RawRenderTarget**: Implements `RenderTarget` by rendering frames to an `IVideoWriter`; handles chunked parallel rendering for performance.
- **IVideoWriter (interface)**: Abstracts video output; exposes `PixelFormat`, `FPS`, `Width`, `Height`, `FrameSizeInBytes`; methods `Write` and `Flush`.
- **FfmpegVideoWriter**: Implements `IVideoWriter` by streaming raw frames to an FFmpeg process; supports `Grayscale`, `RGB`, `RGBA`.


## Building

dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true -o ./publish
