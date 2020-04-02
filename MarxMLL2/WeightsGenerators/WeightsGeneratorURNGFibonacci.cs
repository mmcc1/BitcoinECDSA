using System;
using System.Collections.Generic;
using System.Text;

namespace MarxMLL2.WeightsGenerators
{
    public class WeightsGeneratorURNGFibonacci : IWeightsGenerator
    {
        private readonly URNGFibonacci urngf;

        public WeightsGeneratorURNGFibonacci()
        {
            urngf = new URNGFibonacci(1);
        }

        public double[] CreateRandomWeights(int numElements)
        {
            double[] weights = new double[numElements];

            for (int i = 0; i < numElements; i++)
                weights[i] = RandomNumberURNGFibonacci();

            return weights;
        }

        private double RandomNumberURNGFibonacci()
        {
            bool inRange = false;

            while (!inRange)
            {
                double c = urngf.GetNextDouble();

                if (c < 1 && c > -1)
                    return c;
            }

            return 9;  //Never reached
        }
    }
}
