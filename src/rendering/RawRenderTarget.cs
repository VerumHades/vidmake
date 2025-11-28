namespace AbstractRendering
{
    /// <summary>
    /// A RenderTarget implementation that generates raw frames in memory
    /// and streams them directly to a video writer.
    /// </summary>
    public class RawRenderTarget : RenderTarget
    {
        // Number of frames to process in a single parallel chunk.
        private readonly int maxParallelRenderFrameCount;

        // Buffer to hold all frames for a chunk before sending to the video writer.
        private byte[] framesBuffer;

        // The video writer that receives raw frame data.
        private IVideoWriter videoWriter;

        /// <summary>
        /// Constructs a RawRenderTarget that writes to the given video writer.
        /// </summary>
        /// <param name="videoWriter">The video writer to output frames to.</param>
        /// <param name="videoWriter">The video writer to output frames to.</param>
        public RawRenderTarget(IVideoWriter videoWriter, IRenderStateReporter? reporter = null, int maxParallelRenderFrameCount = 64)
        {
            RenderStateReporter = reporter;
            RenderStateReporter?.RenderBegin(videoWriter);

            this.maxParallelRenderFrameCount = maxParallelRenderFrameCount;
            this.videoWriter = videoWriter;
            framesBuffer = new byte[videoWriter.FrameSizeInBytes * maxParallelRenderFrameCount];
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
            int totalChunks = (int)Math.Ceiling((double)frameCount / maxParallelRenderFrameCount); // number of chunks

            RenderStateReporter?.RenderSequenceBegin(frameCount);
            
            for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
            {
                Array.Clear(framesBuffer, 0, framesBuffer.Length);

                int startFrame = chunkIndex * maxParallelRenderFrameCount;
                int endFrame = Math.Min(startFrame + maxParallelRenderFrameCount, frameCount);
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
                RenderStateReporter?.FrameChunkRendered(chunkIndex, currentChunkSize);
            }

            //videoWriter.Flush();
            RenderStateReporter?.RenderSequenceEnd();
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

                var area = new DrawableArea(
                    target,
                    videoWriter.Width,
                    videoWriter.Height,
                    transform.X,
                    transform.Y,
                    transform.Width,
                    transform.Height,
                    videoWriter.PixelFormat
                );

                element.Render(ref area, animationPercentage);
            }
        }
    }
}
