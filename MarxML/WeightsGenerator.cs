using System;
using System.Security.Cryptography;

namespace MarxML
{
    //Updated to be Cryptographically secure.
    public class WeightsGenerator
    {
        private readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public WeightsGenerator()
        {
        }

        public double[] CreateWeights(int type, int amount)
        {
            switch (type)
            {
                case 0:
                    return CreateRandomWeightsPositive(amount);
                case 1:
                    return CreateRandomWeightsNegative(amount);
                case 3:
                    return CreateRandomWeightsPositiveAndNegative(amount);
                default:
                    return CreateRandomWeightsPositiveAndNegative(amount);
            }
        }

        public double[] CreateRandomWeightsPositive(int numElements)
        {
            double[] weights = new double[numElements];

            for (int i = 0; i < numElements; i++)
                weights[i] = NextDoubleBetween0and1();

            return weights;
        }

        public double[] CreateRandomWeightsIntPositive(int numElements)
        {
            double[] weights = new double[numElements];

            for (int i = 0; i < numElements; i++)
                weights[i] = RandomInteger(0, 255);

            return weights;
        }

        public double[] CreateRandomWeightsNegative(int numElements)
        {
            double[] weights = new double[numElements];

            for (int i = 0; i < numElements; i++)
                weights[i] = NextDouble();

            return weights;
        }

        public double[] CreateRandomWeightsPositiveAndNegative(int numElements)
        {
            double[] weights = new double[numElements];

            for (int i = 0; i < numElements; i++)
            {
                if (RandomInteger(1, 3) == 1)
                    weights[i] = NextDouble();
                else
                    weights[i] = -NextDouble();
            }

            return weights;
        }

        private double NextDouble()
        {
            byte[] doubleBlock = new byte[8];
            rng.GetBytes(doubleBlock);

            return Math.Abs(BitConverter.ToDouble(doubleBlock, 0));
        }

        private double NextDoubleBetween0and1()
        {
            // Step 1: fill an array with 8 random bytes
            var rng = new RNGCryptoServiceProvider();
            var bytes = new Byte[8];
            rng.GetBytes(bytes);
            // Step 2: bit-shift 11 and 53 based on double's mantissa bits
            var ul = BitConverter.ToUInt64(bytes, 0) / (1 << 11);
            return ul / (Double)(1UL << 53);
        }

        private int RandomInteger(int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                // Get four random bytes.
                byte[] four_bytes = new byte[4];
                rng.GetBytes(four_bytes);

                // Convert that into an uint.
                scale = BitConverter.ToUInt32(four_bytes, 0);
            }

            // Add min to the scaled difference between max and min.
            return (int)(min + (max - min) *
                (scale / (double)uint.MaxValue));
        }
    }
}
