namespace AbstractRendering
{
    /// <summary>
    /// Represents a collection of elements that are rendered over time.
    /// Acts as the central controller for animations.
    /// </summary>
    public class Scene
    {
        private readonly List<Element> elements = new List<Element>();
        private readonly RenderTarget renderTarget;

        /// <summary>
        /// Initializes a new Scene with a given RenderTarget.
        /// </summary>
        /// <param name="renderTarget">The target that will elements to render single animations over a given timespan.</param>
        public Scene(RenderTarget renderTarget)
        {
            this.renderTarget = renderTarget;
        }

        /// <summary>
        /// Adds an already-created element instance to the scene.
        /// </summary>
        /// <typeparam name="T">The type of Element being added.</typeparam>
        /// <param name="element">The element instance to add.</param>
        /// <returns>The element that was added, for convenience.</returns>
        public T Add<T>(T element) where T: Element
        {
            elements.Add(element);
            return element;
        }

        /// <summary>
        /// Advances the scene to the next animation over a specified duration in seconds.
        /// 
        /// This function tells the RenderTarget to generate frames for all elements
        /// based on their current and next states, then commits each element's next state.
        /// </summary>
        /// <param name="seconds">The duration of this animation frame, in seconds.</param>
        public void Go(int seconds)
        {
            renderTarget.AddElementFrames(elements, seconds);

            foreach (var element in elements)
            {
                element.ApplyNext();
            }
        }
    }
}
