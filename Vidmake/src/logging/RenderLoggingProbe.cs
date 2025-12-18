using Vidmake.src.rendering;
using Vidmake.src.rendering.writers;

namespace Vidmake.src.logging
{
    public class RenderConsoleLoggingProbe: IRenderStateProbe
    {
        private int totalFrames;
        private int renderedFrames;

        public void RenderBegin(IVideoWriter writer)
        {
            Console.WriteLine("Rendering Started");
        }

        public void RenderStop()
        {
            Console.WriteLine("Rendering Stopped");
        }

        public void RenderSequenceBegin(int framesTotal)
        {
            totalFrames = framesTotal;
            renderedFrames = 0;

            Console.WriteLine($"Render sequence begins. Total frames: {framesTotal}");
        }

        public void RenderSequenceEnd()
        {
            Console.WriteLine("Render sequence completed.");
        }

        public void FrameChunkRendered(int chunkIndex, int framesRenderedCount)
        {
            renderedFrames += framesRenderedCount;
            Console.WriteLine($"Chunk {chunkIndex} rendered ({framesRenderedCount} frames). Total rendered: {renderedFrames}/{totalFrames}");
        }
    }
}
