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

        //Custom Output Functions

        public double[] ByteOutput(double[] x)
        {
            double[] _rX = new double[x.Length];          
            
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < 230000)
                    _rX[i] = 0;
                else
                    _rX[i] = 1;
            }

            return _rX;
        }//27551969.5

        public double[] ByteOutputB0(double[] x)
        {
            double[] _rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < 28163400) //27551970)
                    _rX[i] = 0;
                else
                    _rX[i] = 1;
            }

            return _rX;
        }

        public double[] ByteOutputE(double[] x)
        {
            double[] _rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < 29180215) //27551970)
                    _rX[i] = 0;
                else
                    _rX[i] = 1;
            }

            return _rX;
        }

        public double[] ByteOutputE3(double[] x, MinMax mm)
        {
            double[] _rX = new double[x.Length];

            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < ((mm.max - mm.min) / 2) + mm.min)
                    _rX[i] = 0;
                else
                    _rX[i] = 1;
            }

            return _rX;
        }
    }
}
