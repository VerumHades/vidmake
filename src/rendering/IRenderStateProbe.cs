using Vidmake.src.rendering.writers;

namespace Vidmake.src.rendering
{
    public interface IRenderStateProbe
    {
        /// <summary>
        /// Gets when the rendering begins.
        /// param name="writer">The target output.</param>
        /// </summary>
        public void RenderBegin(IVideoWriter writer);

        /// <summary>
        /// Gets when the rendering stops.
        /// </summary>
        public void RenderStop();

        /// <summary>
        /// Gets called when a rendering sequence starts.
        /// </summary>
        /// <param name="framesTotal">The full count of all frames in the sequence</param>
        public void RenderSequenceBegin(int framesTotal);

        /// <summary>
        /// Gets called when a rendering sequence stops.
        /// </summary>
        public void RenderSequenceEnd();

        /// <summary>
        /// Gets called for every chunk of frames that is rendered.
        /// </summary>
        /// <param name="chunkIndex">Index of the chunk in the whole sequence.</param>
        /// <param name="framesRenderedCount">The total count of frames rendered in that chunk.</param>
        public void FrameChunkRendered(int chunkIndex, int framesRenderedCount);

        /// <summary>
        /// USE WITH CAUTION AND PROPER SYCHRONIZATION DIRECTIVES!
        ///
        /// Gets called for every frame that is rendered. Can and probably will be called asynchronously.
        /// </summary>
        /// <param name="chunkRelativeFrameIndex">If the frames are rendered in chunk, this is the relative index of the frame in said chunks otherwise 0.</param>
        /// <param name="globalFrameIndex">The frames index relative to the whole rendered sequence, may be an animation or the whole video.</param>
        /// public void FrameRendered(int chunkRelativeFrameIndex, int globalFrameIndex);
    }
}