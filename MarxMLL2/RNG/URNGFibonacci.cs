/*
 * URNGFibonacci
 * 
 * An experimental unrandom number generator which uses the first byte
 * of each number in the fibonacci series.
 */

using System;

namespace MarxMLL2
{
    public class URNGFibonacci
    {
        private double currentPosition;
        private readonly Random rng;

        public URNGFibonacci(double startPosition)
        {
            rng = new Random((int)DateTime.UtcNow.Ticks);
            currentPosition = startPosition;
        }

        public double GetNextDouble()
        {
            return FillBuffer();
        }

        private double FillBuffer()
        {
            double[] f = new double[8];

            for (int i = 0; i < 8; i++)
                f[i] = Fib(currentPosition++);
            ScalingFunction sf = new ScalingFunction();
            sf.LinearScaleToRange(f, sf.FindMinMax(f), new MinMax() { min = -1, max = 1 });

            return f[rng.Next(8)];
        }

        private double GetNthFibonacci_Rec(double n)
        {
            if ((n == 0) || (n == 1))
            {
                return n;
            }
            else
                return GetNthFibonacci_Rec(n - 1) + GetNthFibonacci_Rec(n - 2);
        }

        private ulong Fib(double n)
        {
            double sqrt5 = Math.Sqrt(5);
            double p1 = (1 + sqrt5) / 2;
            double p2 = -1 * (p1 - 1);


            double n1 = Math.Pow(p1, n + 1);
            double n2 = Math.Pow(p2, n + 1);
            return (ulong)((n1 - n2) / sqrt5);
        }
    }
}
