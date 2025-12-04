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
        /// The interpolator used to compute intermediate states between
        /// Current and Next values for smooth animations.
        /// Defaults to linear interpolation.
        /// </summary>
        public IInterpolator AnimationInterpolator { get; } = LinearInterpolator.Instance;

        /// <summary>
        /// Determines the rendering layer of the element.
        /// Elements with higher z-index values are drawn on top of those with lower values.
        /// </summary>
        public ObservableProperty<int> ZIndex { get; } = new(0);

        /// <summary>
        /// Applies the next animation state to the current state.
        /// Can be extended by derived classes to incorporate additional state updates.
        /// </summary>
        public virtual void ApplyAnimationState()
        {
            ApplyNext();
        }

        /// <summary>
        /// Renders the element into a <see cref="DrawableArea"/> for a specific animation frame.
        /// Must be implemented by derived classes to define how the element draws itself.
        /// </summary>
        /// <param name="area">The target drawable area in the frame buffer.</param>
        /// <param name="animationPercentage">
        /// The progress of the animation for this frame (0.0 = start, 1.0 = end).
        /// </param>
        public abstract void Render(ref DrawableArea area, float animationPercentage);
    }
}