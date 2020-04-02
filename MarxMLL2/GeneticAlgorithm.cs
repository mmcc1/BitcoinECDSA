namespace MarxMLL2
{
    public class GeneticAlgorithm
    {
        IWeightsGenerator wg;

        public GeneticAlgorithm(IWeightsGenerator wg)
        {
            this.wg = wg;
        }

        public bool[] EvaluateFitness(double[] parentWeights)
        {
            bool[] shouldKeep = new bool[parentWeights.Length];
            double k = wg.CreateRandomWeights(1)[0];

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

            double[] _childWeights = new double[weights.Length];

            for (int i = 0; i < weights.Length; i++)
            {
                if (evaluateFitnessResult[i])
                    _childWeights[i] = weights[i];
                else
                {
                    _childWeights[i] = wg.CreateRandomWeights(1)[0];
                }
            }

            return _childWeights;
        }
    }
}
