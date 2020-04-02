using System;
using System.Security.Cryptography;

namespace MarxMLL2.WeightsGenerators
{
    public class WeightsGeneratorRNGCSPInverse : IWeightsGenerator
    {
        private readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public WeightsGeneratorRNGCSPInverse()
        {
        }

        public double[] CreateRandomWeights(int numElements)
        {
            double[] weights = new double[numElements];

            for (int i = 0; i < numElements; i++)
                weights[i] = RandomNumber();

            return weights;
        }


        private double RandomNumber()
        {
            bool inRange = false;
            byte[] b = new byte[8];

            while (!inRange)
            {
                rng.GetBytes(b);
                double c = BitConverter.ToDouble(b, 0);
                c = -c;

                if (c < 1 && c > -1)
                    return c;
            }

            return 9;  //Never reached
        }

    }
}
