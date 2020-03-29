using System;

namespace MarxML
{
    public class NeuralNetworkLayerDesign
    {
        public int LayerNumber { get; set; }
        public int NumberOfInputs { get; set; }
        public int NumberOfNetworks { get; set; }
        public double[] Weights { get; set; }
        public double[] Biases { get; set; }
    }

    public class NeuralNetwork
    {
        public double[] PerceptronLayer(int numberOfNetworks, double[] input, double[] weights, int numberOfInputs, double[] bias)
        {
            double[] sum = new double[numberOfNetworks * (input.Length / numberOfInputs)];

            int wIndex = 0;
            int sIndex = 0;
            int iIndex = 0;
            bool shouldLoopWeights = (input.Length > weights.Length == true) ? true : false;

            for (int k = 0; k < numberOfNetworks; k++)
            {
                for (int i = 0; i < input.Length / numberOfInputs; i++)
                {
                    for (int j = 0; j < numberOfInputs; j++)
                    {
                        sum[sIndex] += (input[iIndex] * weights[wIndex]);
                        wIndex++;
                        iIndex++;
                    }

                    sum[sIndex] += bias[sIndex];

                    if (shouldLoopWeights)
                        wIndex = 0;

                    sIndex++;
                }
                iIndex = 0;
            }

            return sum;
        }
    }
}