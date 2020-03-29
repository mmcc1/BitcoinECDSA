using System;

namespace MarxML
{
    public class GeneticAlgorithm
    {
        public GeneticAlgorithm()
        {
        }

        public double[] GeneratePopulation(int amount, int type)
        {
            WeightsGenerator _wg = new WeightsGenerator();

            return _wg.CreateWeights(type, amount);
        }

        //Check for all true and no solution - if so, start entire algorithm again from the beginning.
        //_minFactor and _maxFactor are arbitrary, but -0.2 and 0.2 is a good start.
        public bool[] EvaluateFitness(double[] parent1Weights, double[] parent2Weights, double minFactor, double maxFactor)
        {
            bool[] shouldKeep = new bool[parent1Weights.Length];

            for (int i = 0; i < parent1Weights.Length; i++)
            {
                if (parent1Weights[i] - parent2Weights[i] < maxFactor && parent1Weights[i] - parent2Weights[i] > minFactor)
                    shouldKeep[i] = true;
                else
                    shouldKeep[i] = false;
            }

            return shouldKeep;
        }

        //Actually worked quite well.
        public bool[] EvaluateFitness(double[] parentWeights)
        {
            bool[] shouldKeep = new bool[parentWeights.Length];

            for (int i = 0; i < parentWeights.Length; i++)
            {
                if (parentWeights[i] > 0.3)
                    shouldKeep[i] = true;
                else
                    shouldKeep[i] = false;
            }

            return shouldKeep;
        }

        //Better.  Prover evolution
        public bool[] EvaluateFitness2(double[] parentWeights)
        {
            WeightsGenerator wg = new WeightsGenerator();
            bool[] shouldKeep = new bool[parentWeights.Length];
            double k = wg.CreateRandomWeightsPositive(1)[0];

            for (int i = 0; i < parentWeights.Length; i++)
            {
                if (k > 0.025)  //Change this value to determine evolution rate.
                    shouldKeep[i] = true;
                else
                    shouldKeep[i] = false;
            }

            return shouldKeep;
        }

        public double[] CrossOverAndMutation(bool[] evaluateFitnessResult, double[] weights)
        {
            WeightsGenerator wg = new WeightsGenerator();

            double[] _childWeights = new double[weights.Length];

            for (int i = 0; i < weights.Length; i++)
            {
                if (evaluateFitnessResult[i])
                    _childWeights[i] = weights[i];
                else
                {
                    _childWeights[i] = wg.CreateRandomWeightsPositive(1)[0];
                }
            }

            return _childWeights;
        }
    }
}
