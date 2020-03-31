/*
 * Engine B
 * 
 * A proof of concept Engine which validates the probability of discovering Bitcoin's ECDSA 
 * private key is about 1.104427674243920646305299201e-69.
 * 
 * This engine makes use of PRNG, a custom SHA-512 based RNG.  Engine B employs a parallel stage
 * for the main training stage.  This means certain values are written indeterministically. Change
 * the Parallel.For to a for loop for deterministic operation.  The seed can be found in the
 * WeightsGenerator class.
 * 
 * To use RNGCryptoServiceProvider replace CreateRandomWeightsPositivePRNG with CreateRandomWeightsPositive.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarxML;

namespace BTCECDSAAnalyser
{
    public class EngineB : EngineBase
    {
        private List<double[]> parentWeights;
        double min;
        double max;
        double maxStat;

        public EngineB()
        {
            parentWeights = new List<double[]>();
            min = double.MaxValue;
            max = double.MinValue;
        }

        #region Deep Learning network design

        internal override void DesignNN()
        {
            NeuralNetworkLayerDesign nnld1 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 20, NumberOfNetworks = 32 };
            nnld1.Weights = weightsGenerator.CreateRandomWeightsPositivePRNG(nnld1.NumberOfInputs * nnld1.NumberOfNetworks);  //generate random weights

            NeuralNetworkLayerDesign nnld2 = new NeuralNetworkLayerDesign() { LayerNumber = 1, NumberOfInputs = nnld1.NumberOfNetworks, NumberOfNetworks = 64 };
            nnld2.Weights = weightsGenerator.CreateRandomWeightsPositivePRNG(nnld2.NumberOfInputs * nnld2.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld3 = new NeuralNetworkLayerDesign() { LayerNumber = 2, NumberOfInputs = nnld2.NumberOfNetworks, NumberOfNetworks = 128 };
            nnld3.Weights = weightsGenerator.CreateRandomWeightsPositivePRNG(nnld3.NumberOfInputs * nnld3.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld4 = new NeuralNetworkLayerDesign() { LayerNumber = 3, NumberOfInputs = nnld3.NumberOfNetworks, NumberOfNetworks = 256 };
            nnld4.Weights = weightsGenerator.CreateRandomWeightsPositivePRNG(nnld4.NumberOfInputs * nnld4.NumberOfNetworks);

            //Adding biases to the first layer
            nnld1.Biases = weightsGenerator.CreateRandomWeightsPositivePRNG(nnld1.Weights.Length);  // new double[nnld1.Weights.Length]; //  //Set biases to zero.
            nnld2.Biases = new double[nnld2.Weights.Length];
            nnld3.Biases = new double[nnld3.Weights.Length];
            nnld4.Biases = new double[nnld4.Weights.Length];

            nnld.Add(nnld1);
            nnld.Add(nnld2);
            nnld.Add(nnld3);
            nnld.Add(nnld4);
        }

        #endregion

        #region Execute

        public override void Execute()
        {
            Console.WriteLine(string.Format("Engine B Starting..."));
            DesignNN();
            bool shouldRun = true;

            while (shouldRun)
            {
                GenerateDataset();
                GenerateValidationDataset();
                Train();
            }
        }

        #endregion

        #region Train and assess

        private void Train()
        {
            //for (int i = 0; i < dataSet.Count; i++)
            //{
            Parallel.For(0, dataSet.Count, i => { 
                //Rescaling output of hidden layers to normalise and obtain bettwe distribution.
                double[] hiddenLayer1 = neuralNetwork.PerceptronLayer(nnld[0].NumberOfNetworks, dataSet[i].PublicAddressDouble, nnld[0].Weights, nnld[0].NumberOfInputs, nnld[0].Biases);
                //hiddenLayer1 = scalingFunction.LinearScaleToRange(hiddenLayer1, scalingFunction.FindMinMax(hiddenLayer1), new MinMax() { min = 0, max = 1 });
                hiddenLayer1 = activationFunctions.Step(hiddenLayer1, 0, 1);
                double[] hiddenLayer2 = neuralNetwork.PerceptronLayer(nnld[1].NumberOfNetworks, hiddenLayer1, nnld[1].Weights, nnld[1].NumberOfInputs, nnld[1].Biases);
                hiddenLayer2 = scalingFunction.LinearScaleToRange(hiddenLayer2, scalingFunction.FindMinMax(hiddenLayer2), new MinMax() { min = 0, max = 1 });
                hiddenLayer2 = activationFunctions.Step(hiddenLayer2, 0, 1);
                double[] hiddenLayer3 = neuralNetwork.PerceptronLayer(nnld[2].NumberOfNetworks, hiddenLayer2, nnld[2].Weights, nnld[2].NumberOfInputs, nnld[2].Biases);
                hiddenLayer3 = scalingFunction.LinearScaleToRange(hiddenLayer3, scalingFunction.FindMinMax(hiddenLayer3), new MinMax() { min = 0, max = 1 });
                hiddenLayer3 = activationFunctions.Step(hiddenLayer3, 0, 1);
                double[] outputlayer = neuralNetwork.PerceptronLayer(nnld[3].NumberOfNetworks, hiddenLayer3, nnld[3].Weights, nnld[3].NumberOfInputs, nnld[3].Biases);
                outputlayer = scalingFunction.LinearScaleToRange(outputlayer, scalingFunction.FindMinMax(outputlayer), new MinMax() { min = 0, max = 1 });
                outputlayer = activationFunctions.Step(outputlayer, 0, 1);

                Assess(outputlayer, i);
            });
        }

        private void Assess(double[] outputlayer, int index)
        {
            int matchCount = 0;
            outputlayer = ConvertFromBinaryToDouble(outputlayer);

            for (int i = 0; i < dataSet[index].PrivateKey.Length; i++)
            {
                if (dataSet[index].PrivateKey[i] == (int)outputlayer[i])
                    matchCount++;
            }

            if (matchCount > currentMaxBytes)
            {
                currentMaxBytes = matchCount;
                parentWeights.Clear();
                parentWeights.Add(nnld[0].Weights);
                parentWeights.Add(nnld[1].Weights);
                parentWeights.Add(nnld[2].Weights);
                parentWeights.Add(nnld[3].Weights);

                currentDeathRate = 0;

                Validate();
            }
            else if (matchCount > 0)
            {
                Validate();
                currentDeathRate++;
            }
            else
                currentDeathRate++;



            if (currentDeathRate >= deathRate)
            {
                currentDeathRate = 0;
                parentWeights.Clear();
                currentMaxBytes = 0;
            }

            UpdateWeights();
        }

        #endregion

        #region Validation of generalisation

        private void Validate()
        {
            bool shouldSave = false;

            WeightStore ws = new WeightStore() { Statistics = new double[32], WeightsHL0 = nnld[0].Weights, WeightsHL1 = nnld[1].Weights, WeightsHL2 = nnld[2].Weights, WeightsOL = nnld[3].Weights };

            for (int i = 0; i < valdataSet.Count; i++)
            {
                double[] hiddenLayer1 = neuralNetwork.PerceptronLayer(nnld[0].NumberOfNetworks, valdataSet[i].PublicAddressDouble, nnld[0].Weights, nnld[0].NumberOfInputs, nnld[0].Biases);
                hiddenLayer1 = scalingFunction.LinearScaleToRange(hiddenLayer1, scalingFunction.FindMinMax(hiddenLayer1), new MinMax() { min = 0, max = 1 });
                hiddenLayer1 = activationFunctions.Step(hiddenLayer1, 0, 1);
                double[] hiddenLayer2 = neuralNetwork.PerceptronLayer(nnld[1].NumberOfNetworks, hiddenLayer1, nnld[1].Weights, nnld[1].NumberOfInputs, nnld[1].Biases);
                hiddenLayer2 = scalingFunction.LinearScaleToRange(hiddenLayer2, scalingFunction.FindMinMax(hiddenLayer2), new MinMax() { min = 0, max = 1 });
                hiddenLayer2 = activationFunctions.Step(hiddenLayer2, 0, 1);
                double[] hiddenLayer3 = neuralNetwork.PerceptronLayer(nnld[2].NumberOfNetworks, hiddenLayer2, nnld[2].Weights, nnld[2].NumberOfInputs, nnld[2].Biases);
                hiddenLayer3 = scalingFunction.LinearScaleToRange(hiddenLayer3, scalingFunction.FindMinMax(hiddenLayer3), new MinMax() { min = 0, max = 1 });
                hiddenLayer3 = activationFunctions.Step(hiddenLayer3, 0, 1);
                double[] outputlayer = neuralNetwork.PerceptronLayer(nnld[3].NumberOfNetworks, hiddenLayer3, nnld[3].Weights, nnld[3].NumberOfInputs, nnld[3].Biases);
                outputlayer = scalingFunction.LinearScaleToRange(outputlayer, scalingFunction.FindMinMax(outputlayer), new MinMax() { min = 0, max = 1 });
                outputlayer = activationFunctions.Step(outputlayer, 0, 1);

                ValidateTest(outputlayer, i, ref ws);
            }

            for (int i = 0; i < ws.Statistics.Length; i++)
                if (ws.Statistics[i] > 50)
                {
                    shouldSave = true;

                    if (ws.Statistics[i] > maxStat)
                    {
                        maxStat = ws.Statistics[i];
                        Console.WriteLine(string.Format("Current highest stat is: {0}, Probability: {1}", maxStat, maxStat / valdataSet.Count));
                    }
                }

            if (shouldSave)
            {
                //Console.WriteLine(Environment.NewLine);
                //SerialiseWeightsAndSaveToDB(ws.Statistics);  //Save to DB using entity framework - uncomment to enable - remember to set connection string in WeightsDBContext
            }
        }

        private void ValidateTest(double[] outputlayer, int index, ref WeightStore ws)
        {
            outputlayer = ConvertFromBinaryToDouble(outputlayer);

            for (int i = 0; i < valdataSet[index].PrivateKey.Length; i++)
            {
                if (valdataSet[index].PrivateKey[i] == (int)outputlayer[i])
                    ws.Statistics[i]++;
            }
        }

        #endregion

        #region Convert Output layer from binary to double array

        private double[] ConvertFromBinaryToDouble(double[] data)
        {
            double[] value = new double[32];
            int index = 0;
            for (int i = 0; i < 32; i++)
            {
                if (data[index++] == 1)
                    value[i] += 1;
                if (data[index++] == 1)
                    value[i] += 2;
                if (data[index++] == 1)
                    value[i] += 4;
                if (data[index++] == 1)
                    value[i] += 8;
                if (data[index++] == 1)
                    value[i] += 16;
                if (data[index++] == 1)
                    value[i] += 32;
                if (data[index++] == 1)
                    value[i] += 64;
                if (data[index++] == 1)
                    value[i] += 128;
            }

            return value;
        }

        #endregion

        #region Update Weights

        private void UpdateWeights()
        {
            if (parentWeights.Count == 0)
            {
                nnld[0].Weights = weightsGenerator.CreateRandomWeightsPositivePRNG(nnld[0].NumberOfInputs * nnld[0].NumberOfNetworks);
                nnld[1].Weights = weightsGenerator.CreateRandomWeightsPositivePRNG(nnld[1].NumberOfInputs * nnld[1].NumberOfNetworks);
                nnld[2].Weights = weightsGenerator.CreateRandomWeightsPositivePRNG(nnld[2].NumberOfInputs * nnld[2].NumberOfNetworks);
                nnld[3].Weights = weightsGenerator.CreateRandomWeightsPositivePRNG(nnld[3].NumberOfInputs * nnld[3].NumberOfNetworks);
            }
            else
            {
                nnld[0].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[0].Weights), nnld[0].Weights);
                nnld[1].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[1].Weights), nnld[1].Weights);
                nnld[2].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[2].Weights), nnld[2].Weights);
                nnld[3].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[3].Weights), nnld[3].Weights);
            }
        }

        #endregion
    }
}
