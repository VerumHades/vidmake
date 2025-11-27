namespace AbstractRendering
{
    /// <summary>
    /// A RenderTarget implementation that generates raw frames in memory
    /// and streams them directly to a video writer.
    /// </summary>
    public class RawRenderTarget : RenderTarget
    {
        // Number of frames to process in a single parallel chunk.
        private const int chunkSize = 64;

        // Buffer to hold all frames for a chunk before sending to the video writer.
        private byte[] framesBuffer;

        // The video writer that receives raw frame data.
        private IVideoWriter videoWriter;

        /// <summary>
        /// Constructs a RawRenderTarget that writes to the given video writer.
        /// </summary>
        /// <param name="videoWriter">The video writer to output frames to.</param>
        public RawRenderTarget(IVideoWriter videoWriter)
        {
            this.videoWriter = videoWriter;
            framesBuffer = new byte[videoWriter.FrameSizeInBytes * chunkSize];
        }

        /// <summary>
        /// Adds frames for all elements over a specified duration in seconds.
        /// Generates frames in parallel chunks for efficiency and streams them directly to the video writer.
        /// </summary>
        /// <param name="elements">The scene elements to render.</param>
        /// <param name="seconds">Duration to render in seconds.</param>
        public override void AddElementFrames(IReadOnlyList<Element> elements, int seconds)
        {
            int frameCount = seconds * videoWriter.FPS; // total number of frames to render
            int totalChunks = (int)Math.Ceiling((double)frameCount / chunkSize); // number of chunks

            for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
            {
                Array.Clear(framesBuffer, 0, framesBuffer.Length);

                int startFrame = chunkIndex * chunkSize;
                int endFrame = Math.Min(startFrame + chunkSize, frameCount);
                int currentChunkSize = endFrame - startFrame;

                Parallel.For(0, currentChunkSize, i =>
                {
                    float animationPercentage = (startFrame + i) / (float)frameCount;

                    RenderFrame(
                        framesBuffer.AsSpan(i * videoWriter.FrameSizeInBytes, videoWriter.FrameSizeInBytes),
                        elements,
                        animationPercentage
                    );
                });

                videoWriter.Write(framesBuffer, currentChunkSize);
            }

            videoWriter.Flush();
        }

        /// <summary>
        /// Renders a single frame for all elements into the given target buffer.
        /// </summary>
        /// <param name="target">Span of bytes representing the frame buffer for one frame.</param>
        /// <param name="elements">The elements to render.</param>
        /// <param name="animationPercentage">Progress of the animation [0..1].</param>
        private void RenderFrame(Span<byte> target, IReadOnlyList<Element> elements, float animationPercentage)
        {
            foreach (var element in elements)
            {
                var transform = element.GetInterpolated(element.AnimationInterpolator, animationPercentage);

                try
                {
                    var area = new DrawableArea(
                        target,
                        videoWriter.Width,
                        transform.X,
                        transform.Y,
                        transform.Width,
                        transform.Height,
                        videoWriter.PixelFormat
                    );

                    element.Render(ref area, animationPercentage);

                }
                catch (ArgumentOutOfRangeException) // Happens when the area is out of bounds
                {

                }

            }
        }
    }
}
