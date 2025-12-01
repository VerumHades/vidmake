using System;

namespace AbstractRendering
{
    /// <summary>
    /// Represents a collection of elements that are rendered over time.
    /// Acts as the central controller for animations.
    /// </summary>
    public class Scene
    {
        private readonly SortedDictionary<int, HashSet<Element>> elements = new();
        private readonly RenderTarget renderTarget;

        /// <summary>
        /// Initializes a new Scene with a given RenderTarget.
        /// </summary>
        /// <param name="renderTarget">The target that will elements to render single animations over a given timespan.</param>
        public Scene(RenderTarget renderTarget)
        {
            this.renderTarget = renderTarget;
        }

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
        /// Adds an already-created element instance to the scene.
        /// </summary>
        /// <typeparam name="T">The type of Element being added.</typeparam>
        /// <param name="element">The element instance to add.</param>
        /// <returns>The element that was added, for convenience.</returns>
        public T Add<T>(T element) where T: Element
        {   
            var layer = GetOrCreateLayer(element.zIndex.Value);
            layer.Add(element);


            element.zIndex.AddChangedListener((nextZIndex) => {
                if(nextZIndex == element.zIndex.Value) return;

                elements[element.zIndex.Value].Remove(element);
                GetOrCreateLayer(nextZIndex).Add(element);
            });

            return element;
        }

        private IEnumerable<Element> GetElementEnumerator()
        {
            foreach(var (index, batch) in elements)
            {
                foreach(var element in batch)
                {
                    yield return element;
                }
            }
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
            renderTarget.AddElementFrames(() => GetElementEnumerator(), seconds);

            foreach (var element in GetElementEnumerator())
            {
                element.ApplyNext();
            }
        }
    }
}
