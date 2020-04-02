using System;
using System.Collections.Generic;
using System.Text;

namespace MarxMLL2.WeightsGenerators
{
    public class WeightsGeneratorPRNGSHA512 : IWeightsGenerator
    {
        private readonly PRNGSHA512 prng;

        public WeightsGeneratorPRNGSHA512()
        {
            prng = new PRNGSHA512(double.Epsilon);
        }

        public double[] CreateRandomWeights(int numElements)
        {
            double[] weights = new double[numElements];

            for (int i = 0; i < numElements; i++)
                weights[i] = RandomNumberPRNGSHA512();

            return weights;
        }

        private double RandomNumberPRNGSHA512()
        {
            bool inRange = false;
            byte[] b = new byte[8];

            while (!inRange)
            {
                byte[] d = prng.GetNextBytes();
                Array.Copy(d, 0, b, 0, b.Length);
                double c = BitConverter.ToDouble(b, 0);

                if (c < 1 && c > -1)
                    return c;
            }

            return 9;  //Never reached
        }
    }
}
