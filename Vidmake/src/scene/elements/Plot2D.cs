using Vidmake.src.positioning;
using Vidmake.src.positioning.constraints;
using Vidmake.src.positioning.interpolators;
using Vidmake.src.rendering;

namespace Vidmake.src.scene.elements
{
    /// <summary>
    /// A simple rectangle element that can be added to a Scene.
    /// Inherits animation properties from Element / TransitionalTransform.
    /// Supports a customizable background color.
    /// </summary>
    public class Plot2D : Element
    {
        /// <summary>
        /// The background color of the rectangle.
        /// </summary>
        public Pixel StrokeColor { get; set; } = Pixel.Blue;

        public TransitionalProperty<Func<double, double>> SampledFunction = new((x) => x);
        public TransitionalProperty<Interval<double>> SampleInterval { get; } = new TransitionalProperty<Interval<double>>(new Interval<double>(-100, 100));
        public TransitionalProperty<Interval<double>> ValueInterval { get; } = new TransitionalProperty<Interval<double>>(new Interval<double>(-100, 100));
        public TransitionalProperty<double> SampleStep { get; } = new TransitionalProperty<double>(0.01, new PositiveComparableConstraint<double>());

        public IInterpolator ValueIntervalInterpolator { get; } = LinearInterpolator.Instance;
        public IInterpolator SampleCountInterpolator { get; } = LinearInterpolator.Instance;
        public IInterpolator SampleInterpolator { get; } = LinearInterpolator.Instance;
        /// <summary>
        /// Default constructor initializes a 10x10 rectangle with default green color.
        /// </summary>
        public Plot2D(int width = 500, int height = 500)
        {
            Width.Next = width;
            Height.Next = height;
            ApplyNext();
        }

        public override void ApplyAnimationState()
        {
            base.ApplyAnimationState();

            SampleInterval.ApplyNext();
            SampleStep.ApplyNext();
            SampledFunction.ApplyNext();
            ValueInterval.ApplyNext();
        }

        private double GetIntervalLength(Interval<double> interval)
        {
            return interval.End - interval.Start;
        }
        private int GetSampleNumberInRange(Interval<double> interval, double step)
        {
            return (int)(GetIntervalLength(interval) / step);
        }
        /// <summary>
        /// Draws the rectangle into the provided drawable area.
        /// Fills the area with the rectangle's background color.
        /// </summary>
        public override void Render(ref DrawableArea area, float animationPercentage)
        {
            int currentSampleCount = GetSampleNumberInRange(SampleInterval.Current, SampleStep.Current);
            int futureSampleCount = GetSampleNumberInRange(SampleInterval.Next, SampleStep.Next);

            double valueDisplayRange = ValueIntervalInterpolator.Interpolate(GetIntervalLength(ValueInterval.Current), GetIntervalLength(ValueInterval.Next), animationPercentage);

            int sampleCount = (int)SampleCountInterpolator.Interpolate(currentSampleCount, futureSampleCount, animationPercentage);

            double currentSampleStep = GetIntervalLength(SampleInterval.Current) / currentSampleCount;
            double futureSampleStep = GetIntervalLength(SampleInterval.Next) / futureSampleCount;

            for (int i = 0; i < sampleCount; i++)
            {
                double currentSample = SampledFunction.Current(SampleInterval.Current.Start + i * currentSampleStep);
                double futureSample = SampledFunction.Next(SampleInterval.Next.Start + i * futureSampleStep);

                double intermidiateSample = SampleInterpolator.Interpolate(currentSample, futureSample, animationPercentage);

                double normalizedY = intermidiateSample / valueDisplayRange + 0.5;
                double normalizedX = (double)i / sampleCount;

                for (var j = -5; j < 5; j++)
                {
                    area.SetPixel((int)(normalizedX * area.Width), (int)(normalizedY * area.Height) + j, StrokeColor);
                }
            }
        }
    }
}
