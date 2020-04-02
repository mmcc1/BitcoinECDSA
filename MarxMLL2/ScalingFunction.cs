namespace MarxMLL2
{
    public struct MinMax
    {
        public double min;
        public double max;
    }

    //Be careful with scaling.  This will force two values to be upper and lower, so can skew results.
    public class ScalingFunction
    {
        public ScalingFunction()
        {
        }

        public MinMax FindMinMax(double[] input)
        {
            MinMax minMax = new MinMax();
            minMax.min = double.MaxValue;
            minMax.max = double.MinValue;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] > minMax.max)
                    minMax.max = input[i];

                if (input[i] < minMax.min)
                    minMax.min = input[i];
            }

            return minMax;
        }

        public double[] LinearScaleToRange(double[] input, MinMax oldMinMax, MinMax newMinMax)
        {
            double[] scaled = new double[input.Length];
            double oldMinMaxDiff = oldMinMax.max - oldMinMax.min;

            for (int i = 0; i < input.Length; i++)
            {
                double _inputOldMinDiff = input[i] - oldMinMax.min;
                scaled[i] = (newMinMax.min * (1 - (_inputOldMinDiff / oldMinMaxDiff))) + (newMinMax.max * (_inputOldMinDiff / oldMinMaxDiff));
            }

            return scaled;
        }
    }
}
