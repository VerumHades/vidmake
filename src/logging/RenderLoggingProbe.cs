using Vidmake.src.rendering;
using Vidmake.src.rendering.writers;

namespace Vidmake.src.logging
{
    public class RenderLoggingProbe: LoggingProbe, IRenderStateProbe
    {
        private int totalFrames;
        private int renderedFrames;

        public RenderLoggingProbe(IReporter reporter) : base(reporter)
        {
            
        }

        public void RenderBegin(IVideoWriter writer)
        {
            Message("Rendering Started");
        }

        public void RenderStop()
        {
            Message("Rendering Stopped");
        }

        public void RenderSequenceBegin(int framesTotal)
        {
            totalFrames = framesTotal;
            renderedFrames = 0;

            Message($"Render sequence begins. Total frames: {framesTotal}");
        }

        public void RenderSequenceEnd()
        {
            Message("Render sequence completed.");
        }

        public void FrameChunkRendered(int chunkIndex, int framesRenderedCount)
        {
            renderedFrames += framesRenderedCount;
            Message($"Chunk {chunkIndex} rendered ({framesRenderedCount} frames). Total rendered: {renderedFrames}/{totalFrames}");
        }
    }
}
