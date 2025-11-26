namespace AbstractRendering
{
    public class Scene
    {
        private readonly List<Element> elements = new List<Element>();
        private readonly RenderTarget renderTarget;
        public Scene(RenderTarget renderTarget)
        {
            this.renderTarget = renderTarget;
        }

        /// <summary>
        /// Adds an already-created element instance to the scene.
        /// </summary>
        public T Add<T>(T element) where T: Element
        {
            elements.Add(element);
            return element;
        }

        /// <summary>
        /// Main scene execution function.
        /// </summary>
        public void Go(int seconds)
        {
            renderTarget.AddElementFrames(elements, seconds * renderTarget.TargetFPS);

            foreach (var element in elements)
            {
                element.ApplyNext();
            }
        }
    }
}