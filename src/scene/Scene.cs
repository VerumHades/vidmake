using Vidmake.src.rendering;
using Vidmake.src.scene.elements;

namespace Vidmake.src.scene
{
    /// <summary>
    /// Represents a collection of elements that are rendered over time.
    /// Acts as the central controller for animations and layer management.
    /// </summary>
    public class Scene
    {
        // Elements are organized by z-index; higher keys are drawn later (on top).
        private readonly SortedDictionary<int, HashSet<Element>> elements = new();

        /// <summary>
        /// The target responsible for rendering frames of elements.
        /// </summary>
        private readonly RenderTarget renderTarget;

        /// <summary>
        /// Initializes a new <see cref="Scene"/> with a given <see cref="RenderTarget"/>.
        /// </summary>
        /// <param name="renderTarget">
        /// The target responsible for generating frames and rendering elements.
        /// </param>
        public Scene(RenderTarget renderTarget)
        {
            this.renderTarget = renderTarget;
        }

        /// <summary>
        /// Retrieves the set of elements at a specific z-index layer, creating the layer if it does not exist.
        /// </summary>
        /// <param name="number">The z-index of the layer.</param>
        /// <returns>A <see cref="HashSet{Element}"/> representing all elements at that layer.</returns>
        private HashSet<Element> GetOrCreateLayer(int number)
        {
            if (!elements.TryGetValue(number, out var layer))
            {
                layer = new HashSet<Element>();
                elements[number] = layer;
            }

            return layer;
        }

        /// <summary>
        /// Adds an element instance to the scene.
        /// Automatically tracks changes to the element's z-index and moves it between layers.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Element"/> being added.</typeparam>
        /// <param name="element">The element instance to add.</param>
        /// <returns>The element that was added, for convenience.</returns>
        public T Add<T>(T element) where T : Element
        {
            var layer = GetOrCreateLayer(element.zIndex.Value);
            layer.Add(element);

            element.zIndex.AddChangedListener((nextZIndex) =>
            {
                if (nextZIndex == element.zIndex.Value) return;

                elements[element.zIndex.Value].Remove(element);
                GetOrCreateLayer(nextZIndex).Add(element);
            });

            return element;
        }

        /// <summary>
        /// Enumerates all elements in the scene in ascending z-index order.
        /// </summary>
        /// <returns>An enumerable of all elements in the scene.</returns>
        private IEnumerable<Element> GetElementEnumerator()
        {
            foreach (var (index, batch) in elements)
            {
                foreach (var element in batch)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Advances the scene to the next animation frame over a specified duration.
        ///
        /// This generates frames for all elements based on their current and next states,
        /// then commits each element's next state as the new current state.
        /// </summary>
        /// <param name="seconds">The duration of this animation frame, in seconds.</param>
        public void Go(int seconds)
        {
            renderTarget.AddElementFrames(() => GetElementEnumerator(), seconds);

            foreach (var element in GetElementEnumerator())
            {
                element.ApplyAnimationState();
            }
        }
    }
}
