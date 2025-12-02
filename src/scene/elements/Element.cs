using Vidmake.src;
using Vidmake.src.positioning;
using Vidmake.src.positioning.interpolators;
using Vidmake.src.rendering;

namespace Vidmake.src.scene.elements
{
    /// <summary>
    /// Base class for all renderable objects in a Scene.
    /// Combines a transitional transform (position/size animation) with a renderable surface.
    /// </summary>
    public abstract class Element : TransitionalTransform, ISurface
    {
        /// <summary>
        /// The interpolator used to calculate intermediate states
        /// between Current and Next values for smooth animations.
        /// Default is linear interpolation.
        /// </summary>
        public IInterpolator AnimationInterpolator { get; } = LinearInterpolator.Instance;

        public ObservableProperty<int> zIndex { get; } = new(0);

        public virtual void ApplyAnimationState()
        {
            ApplyNext();
        }

        /// <summary>
        /// Renders the element into a DrawableArea for a specific animation frame.
        /// This method must be implemented by derived classes to define
        /// how the element draws itself.
        /// </summary>
        /// <param name="area">The target drawable area in the frame buffer.</param>
        /// <param name="animationPercentage">
        /// Progress of the animation for this frame (0.0 = start, 1.0 = end).
        /// </param>
        public abstract void Render(ref DrawableArea area, float animationPercentage);
    }
}