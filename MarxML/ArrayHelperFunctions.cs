using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarxML
{
    public struct TwoArrays
    {
        public double[] arrayA;
        public double[] arrayB;
    }

    public class ArrayHelperFunctions
    {
        public ArrayHelperFunctions()
        {
        }

        public double[] ReorderArrayAsInterleave(double[] input, int groupSize)
        {
            int numberOfGroups = input.Length / groupSize;
            double[] reorderedArray = new double[input.Length];
            int index = 0;
            int index2 = 0;
            int bp = 0;

            for (int j = 0; j < numberOfGroups; j++)
            {
                for (int i = 0; i < groupSize; i++)
                {
                    reorderedArray[index] = input[bp + index2];
                    index++;
                    bp += numberOfGroups;

                    if (bp == input.Length - numberOfGroups)
                        bp = 0;
                }
                index2++;
            }

            return reorderedArray;
        }

        public double[] MergeArraysSequentially(double[] firstArray, double[] secondArray)
        {
            double[] mergedArray = new double[firstArray.Length + secondArray.Length];

            Array.Copy(firstArray, mergedArray, firstArray.Length);
            Array.Copy(secondArray, 0, mergedArray, firstArray.Length, secondArray.Length);

            return mergedArray;
        }

        public TwoArrays SeparateMergedArrays(double[] input, int splitPoint) 
        {
            TwoArrays ta = new TwoArrays();
            ta.arrayA = new double[splitPoint];
            ta.arrayB = new double[input.Length - splitPoint];

            Array.Copy(input, 0, ta.arrayA, 0, splitPoint);
            Array.Copy(input, splitPoint, ta.arrayB, 0, ta.arrayB.Length);

            return ta;
        }

        public double[] ReorderArrayByColum(double[] theArray, int numInputs, int numNetworks)
        {
            double[] reOrderedArray = new double[theArray.Length];

            int index = 0;
            int index2 = 0;

            for (int j = 0; j < numNetworks; j++)
            {
                index2 = j;
                for (int i = 0; i < numInputs; i++)
                {
                    reOrderedArray[index++] = theArray[index2];
                    index2 += numNetworks;
                }
            }

            return reOrderedArray;
        }
    }
}
