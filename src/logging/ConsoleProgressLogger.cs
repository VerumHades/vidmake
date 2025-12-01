using System;

namespace AbstractRendering
{
    public class ConsoleProgressReporter : IRenderStateReporter
    {
        int totalFrames;
        int renderedFrames;

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

        // ---------------- Drawing (now simplified to nothing) ----------------

        void DrawGlobalBar(int value)
        {
            // simplified: just log the progress
            Console.WriteLine($"Global progress: {value}/{totalFrames}");
        }

        void DrawChunkBar(int chunkIndex, int chunkSize)
        {
            // simplified: just log the chunk update
            Console.WriteLine($"Chunk progress: {chunkIndex}/{chunkSize}");
        }

        static string BuildBar(double pct, int width)
        {
            return ""; // unused now
        }
    }
}
