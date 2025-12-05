using Vidmake.src.logging;
using Vidmake.src.rendering.writers;
using Vidmake.src.scene.elements;

namespace Vidmake.src.rendering
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
        public RawRenderTarget(
            IVideoWriter videoWriter,
            IRenderStateProbe? reporter = null,
            long maxBufferSizeInBytes = 256 * 1024 * 1024 // 256MB
        )
        {
            if (maxBufferSizeInBytes <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxBufferSizeInBytes),
                    "maxBufferSizeInBytes must be positive.");

            RenderStateReporter = reporter;
            RenderStateReporter?.RenderBegin(videoWriter);

            this.videoWriter = videoWriter;

            int frameSizeInBytes = videoWriter.Format.FrameSizeInBytes;
            long possibleFrames = maxBufferSizeInBytes / frameSizeInBytes;
            maxParallelRenderFrameCount = (int)Math.Min(possibleFrames, int.MaxValue);

            if(maxParallelRenderFrameCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxBufferSizeInBytes),
                    $"maxBufferSizeInBytes the size specified didnt amount to a single frame, at least one frame must fit into the specified size, currently: {frameSizeInBytes} bytes per frame.");
            }

            try
            {
                checked
                {
                    int totalBuffer = frameSizeInBytes * maxParallelRenderFrameCount;
                    framesBuffer = new byte[frameSizeInBytes * maxParallelRenderFrameCount];  
                }
            }
            catch (OverflowException ex)
            {
                throw new InvalidOperationException(
                    "Overflow occurred while allocating frames buffer. Total buffer size exceeds maximum supported array size.", ex);
            }
            catch (OutOfMemoryException ex)
            {
                throw new InvalidOperationException(
                    $"Cannot allocate frames buffer of size {frameSizeInBytes * maxParallelRenderFrameCount} bytes.", ex);
            }
        }

        /// <summary>
        /// Generates frames for a collection of elements over a specified duration.
        /// Each element may have "Current" and "Next" states, which should be
        /// interpolated or applied over the given time span.
        /// </summary>
        /// <param name="createElementEnumerator">Creates an enumerator that should be saved to use in multiple threads.</param>
        /// <param name="seconds">The duration (in seconds) to advance the scene.</param>
        public override void AddElementFrames(Func<IEnumerable<Element>> createElementEnumerator, int seconds)
        {
            int frameCount = seconds * videoWriter.Format.FPS; // total number of frames to render
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
                //for (int i = 0; i < currentChunkSize; i++)
                //{
                    float animationPercentage = (startFrame + i) / (float)frameCount;

                    RenderFrame(
                        framesBuffer.AsSpan(i * videoWriter.Format.FrameSizeInBytes, videoWriter.Format.FrameSizeInBytes),
                        createElementEnumerator(),
                        animationPercentage
                    );
                //}
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
        private void RenderFrame(Span<byte> target, IEnumerable<Element> elements, float animationPercentage)
        {
            foreach (var element in elements)
            {
                var transform = element.GetInterpolated(element.AnimationInterpolator, animationPercentage);

                var area = new DrawableArea(
                    target,
                    videoWriter.Format.Width,
                    videoWriter.Format.Height,
                    transform.X,
                    transform.Y,
                    transform.Width,
                    transform.Height,
                    videoWriter.Format.PixelFormat
                );

                element.Render(ref area, animationPercentage);
            }
        }
    }
}
