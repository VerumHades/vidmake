using System;
using System.Threading;

namespace AbstractRendering
{
    public class ConsoleProgressReporter : IRenderStateReporter
    {
        int totalFrames;
        int renderedFrames;

        // Cursor positions for the two bars
        int globalBarLine;
        int chunkBarLine;
        public void RenderBegin(IVideoWriter writer)
        {
            Console.WriteLine("Rendering Started");
        }

        public void RenderStop()
        {
            Console.WriteLine("\nRendering Stopped");
        }

        public void RenderSequenceBegin(int framesTotal)
        {
            totalFrames = framesTotal;
            renderedFrames = 0;

            Console.WriteLine($"Total Frames: {framesTotal}");

            Console.WriteLine("Global Progress:");
            globalBarLine = Console.CursorTop;
            Console.WriteLine(); // actual bar line

            Console.WriteLine("Current Chunk Progress:");
            chunkBarLine = Console.CursorTop;
            Console.WriteLine(); // bar line

            DrawGlobalBar(0);
            DrawChunkBar(0, 1);
        }

        public void RenderSequenceEnd()
        {
            DrawGlobalBar(totalFrames);
            Console.WriteLine("\nSequence Completed\n");
        }

        public void FrameChunkRendered(int chunkIndex, int framesRenderedCount)
        {
            renderedFrames += framesRenderedCount;

            // Draw full chunk bar for chunk completion
            DrawChunkBar(1, 1);
            DrawGlobalBar(renderedFrames);
        }

        /*public void FrameRendered(int chunkRelativeFrameIndex, int globalFrameIndex)
        {
            
        }*/

        // ---------------- Drawing ----------------

        void DrawGlobalBar(int value)
        {
            value = Math.Clamp(value, 0, totalFrames);
            double pct = totalFrames == 0 ? 0 : (double)value / totalFrames;

            string bar = BuildBar(pct, 40);
            int oldTop = Console.CursorTop;

            Console.SetCursorPosition(0, globalBarLine);
            Console.Write($"{bar} {pct*100:0.0}%");

            Console.SetCursorPosition(0, oldTop);
        }

        void DrawChunkBar(int chunkIndex, int chunkSize)
        {
            double pct = chunkSize <= 0 ? 0 : Math.Clamp((double)chunkIndex / chunkSize, 0, 1);

            string bar = BuildBar(pct, 40);
            int oldTop = Console.CursorTop;

            Console.SetCursorPosition(0, chunkBarLine);
            Console.Write($"{bar} {(pct*100):0.0}%");

            Console.SetCursorPosition(0, oldTop);
        }

        static string BuildBar(double pct, int width)
        {
            int filled = (int)(pct * width);
            return "[" + new string('#', filled) + new string('-', width - filled) + "]";
        }
    }
}