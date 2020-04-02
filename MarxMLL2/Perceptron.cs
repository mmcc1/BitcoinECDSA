using System;

namespace MarxMLL2
{
    public class Perceptron
    {
        public double Execute(double[] weights, double[] inputs, double bias)
        {
            double sum = 0;

            for (int i = 0; i < inputs.Length; i++)
                sum += weights[i] * inputs[i];

            return sum + bias;
        }
    }
}
