using System;
using System.Security.Cryptography;

namespace MarxMLL2.WeightsGenerators
{
    public class WeightsGeneratorRNGCSPScaledELU : IWeightsGenerator
    {
        private readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public WeightsGeneratorRNGCSPScaledELU()
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

                ActivationFunctions af = new ActivationFunctions();
                c = af.ScaledELU(c);

                if (c < 1 && c > -1)
                    return c;
            }

            return 9;  //Never reached
        }

    }
}
