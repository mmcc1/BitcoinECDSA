using System;

namespace MarxML
{
    public class ActivationFunctions
    {
        public double[] Step(double[] x, double min, double max)
        {
            double[] rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < ((max - min) / 2) + min)
                    rX[i] = min;
                else
                    rX[i] = max;
            }

            return rX;
        }

        public double[] Sigmoid(double[] x)
        {
            double[] rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                rX[i] = 1.0f / (1.0f + (float)Math.Exp(-x[i]));
            }

            return rX;
        }

        public double[] TanSigmoid(double[] x)
        {
            double[] rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
                rX[i] = 2 / (1 + Math.Exp(-2 * x[i])) - 1;

            return rX;
        }

        public double[] LogSigmoid(double[] x)
        {
            double[] rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
                rX[i] = 1 / (1 + Math.Exp(-x[i]));

            return rX;
        }

        public double[] TruncatedLogSigmoid(double[] x)
        {
            double[] rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < -45.0) rX[i] = 0.0;
                else if (x[i] > 45.0) rX[i] = 1.0;
                else rX[i] = 1.0 / (1.0 + Math.Exp(-x[i]));
            }

            return rX;
        }

        public double[] TruncatedHyperTanFunction(double[] x)
        {
            double[] rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < -45.0) rX[i] = -1.0;
                else if (x[i] > 45.0) rX[i] = 1.0;
                else rX[i] = Math.Tanh(x[i]);
            }

            return rX;
        }

        public double[] ByteOutputWithMidPoint(double[] x, long midPoint)
        {
            double[] _rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < midPoint)
                    _rX[i] = 0;
                else
                    _rX[i] = 1;
            }

            return _rX;
        }
    }
}
