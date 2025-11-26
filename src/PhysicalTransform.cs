namespace AbstractRendering
{
    public struct PhysicalTransform
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public PhysicalTransform(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"PhysicalTransform {{ X = {X}, Y = {Y}, Width = {Width}, Height = {Height} }}";
        }
    }
}