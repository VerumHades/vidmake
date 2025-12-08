using Vidmake.src.rendering;
using Vidmake.src.rendering.writers;

namespace Vidmake.src.logging
{
    public class RenderLoggingProbe: IReportable, IRenderStateProbe
    {
        private int totalFrames;
        private int renderedFrames;

        public IReporter Reporter { get; set; } = NullReporter.Instance;

        public void RenderBegin(IVideoWriter writer)
        {
            Reporter.Message("Rendering Started");
        }

        public void RenderStop()
        {
            Reporter.Message("Rendering Stopped");
        }

        public void RenderSequenceBegin(int framesTotal)
        {
            totalFrames = framesTotal;
            renderedFrames = 0;

            Reporter.Message($"Render sequence begins. Total frames: {framesTotal}");
        }

        public void RenderSequenceEnd()
        {
            Reporter.Message("Render sequence completed.");
        }

        public void FrameChunkRendered(int chunkIndex, int framesRenderedCount)
        {
            renderedFrames += framesRenderedCount;
            Reporter.Message($"Chunk {chunkIndex} rendered ({framesRenderedCount} frames). Total rendered: {renderedFrames}/{totalFrames}");
        }
    }
}
