/*
 * Engine A
 * 
 * Using RNGCryptoServiceProvider for weights generation.
 * 
 * Produces byte probabilities of 0.007 in a reasonable time.
 */
using BTCLib;
using MarxMLL2;
using MarxMLL2.WeightsGenerators;
using System;
using System.Collections.Generic;

namespace BTCECDSACracker.Engines
{
    

    public class EngineA
    {
        #region Variables

        private ScalingFunction scalingFunction;
        private ActivationFunctions activationFunctions;
        private GeneticAlgorithm geneticAlgorithm;
        private Perceptron perceptron;
        private IWeightsGenerator weightsGenerator;

        private List<BTCKeyStore> keyStore;
        private List<DataSet> dataSet;
        private List<BTCKeyStore> valkeyStore;
        private List<DataSet> valdataSet;
        private List<NeuralNetwork> neuralNetwork;

        private int currentMaxBytes;
        private int deathRate;
        private int currentDeathRate;

        private List<double[]> parentWeights;
        double maxStat;

        #endregion

        public EngineA()
        {
            parentWeights = new List<double[]>();
            Init();
        }

        #region Initialisation

        private void Init()
        {
            //Init classes
            scalingFunction = new ScalingFunction();
            //weightsGenerator = new WeightsGeneratorURNGFibonacci();  //inject a different weight generator
            weightsGenerator = new WeightsGeneratorRNGCSP();
            //weightsGenerator = new WeightsGeneratorPRNGSHA512(); 
            activationFunctions = new ActivationFunctions();
            geneticAlgorithm = new GeneticAlgorithm(weightsGenerator);
            neuralNetwork = new List<NeuralNetwork>();
            perceptron = new Perceptron();

            //Init Lists
            keyStore = new List<BTCKeyStore>();
            dataSet = new List<DataSet>();
            valkeyStore = new List<BTCKeyStore>();
            valdataSet = new List<DataSet>();
            neuralNetwork = new List<NeuralNetwork>();

            currentMaxBytes = 0;
            deathRate = 100;  //If too high, then chance plays an increasing role and skews the result.
        }

        #endregion

        #region Deep Learning Network Design

        private void DesignNN()
        {
            //Layer 0
            for (int i = 0; i < 32; i++)
            {
                NeuralNetwork nn = new NeuralNetwork()
                {
                    LayerNumber = 0,
                    NetworkNumber = i,
                    Bias = 0,
                    Weights = weightsGenerator.CreateRandomWeights(20)
                };
                neuralNetwork.Add(nn);
            }

            //Layer 1
            for (int i = 0; i < 64; i++)
            {
                NeuralNetwork nn = new NeuralNetwork()
                {
                    LayerNumber = 1,
                    NetworkNumber = i,
                    Bias = 0,
                    Weights = weightsGenerator.CreateRandomWeights(32)
                };
                neuralNetwork.Add(nn);
            }

            //Layer 2
            for (int i = 0; i < 128; i++)
            {
                NeuralNetwork nn = new NeuralNetwork()
                {
                    LayerNumber = 2,
                    NetworkNumber = i,
                    Bias = 0,
                    Weights = weightsGenerator.CreateRandomWeights(64)
                };
                neuralNetwork.Add(nn);
            }

            //Output Layer
            for (int i = 0; i < 256; i++)
            {
                NeuralNetwork nn = new NeuralNetwork()
                {
                    LayerNumber = 3,
                    NetworkNumber = i,
                    Bias = 0,
                    Weights = weightsGenerator.CreateRandomWeights(128)
                };
                neuralNetwork.Add(nn);
            }
        }

        #endregion

        #region Generate Dataset and validation set.

        internal void GenerateDataset()
        {
            keyStore.Clear();
            dataSet.Clear();

            Console.WriteLine("Generating Dataset...");

            for (int i = 0; i < 100000; i++)
            {
                keyStore.Add(BTCBasicFunctions.CreateKeyPair());
            }

            Console.WriteLine("Converting Dataset...");

            for (int i = 0; i < keyStore.Count; i++)
            {
                DataSet ds = new DataSet() { PublicAddressDouble = new double[20], PrivateKey = keyStore[i].PrivateKeyByteArray, PublicAddress = keyStore[i].PublicAddress };
                byte[] pap = BTCPrep.PrepareAddress(keyStore[i].PublicAddress);

                if (pap.Length != 20)
                    continue;

                for (int j = 0; j < pap.Length; j++)
                    ds.PublicAddressDouble[j] = pap[j];

                ds.PublicAddressDouble = scalingFunction.LinearScaleToRange(ds.PublicAddressDouble, scalingFunction.FindMinMax(ds.PublicAddressDouble), new MinMax() { min = 0, max = 0.01 });

                dataSet.Add(ds);
            }
        }

        internal void GenerateValidationDataset()
        {
            valkeyStore.Clear();
            valdataSet.Clear();

            Console.WriteLine("Generating Validation Dataset...");

            for (int i = 0; i < 10000; i++)
            {
                valkeyStore.Add(BTCBasicFunctions.CreateKeyPair());
            }

            Console.WriteLine("Converting Validation Dataset...");

            for (int i = 0; i < valkeyStore.Count; i++)
            {
                DataSet ds = new DataSet() { PublicAddressDouble = new double[20], PrivateKey = valkeyStore[i].PrivateKeyByteArray, PublicAddress = valkeyStore[i].PublicAddress };
                byte[] pap = BTCPrep.PrepareAddress(valkeyStore[i].PublicAddress);

                if (pap.Length != 20)
                    continue;

                for (int j = 0; j < pap.Length; j++)
                    ds.PublicAddressDouble[j] = pap[j];

                //Added for Engine B
                ds.PublicAddressDouble = scalingFunction.LinearScaleToRange(ds.PublicAddressDouble, scalingFunction.FindMinMax(ds.PublicAddressDouble), new MinMax() { min = 0, max = 0.01 });

                valdataSet.Add(ds);
            }
        }

        #endregion

        #region Execute

        public void Execute()
        {
            Console.WriteLine(string.Format("Engine A Starting..."));
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
            for (int i = 0; i < dataSet.Count; i++)
            {
                //Layer 0
                List<NeuralNetwork> hiddenLayer1 = neuralNetwork.FindAll(x => x.LayerNumber == 0);
                double[] weightedSum1 = new double[32];
                for (int j = 0; j < hiddenLayer1.Count; j++)
                    weightedSum1[j] = perceptron.Execute(hiddenLayer1[j].Weights, dataSet[i].PublicAddressDouble, hiddenLayer1[j].Bias);

                for (int k = 0; k < weightedSum1.Length; k++)
                    weightedSum1[k] = activationFunctions.LeakyReLU(weightedSum1[k]);

                //Layer 1
                List<NeuralNetwork> hiddenLayer2 = neuralNetwork.FindAll(x => x.LayerNumber == 1);
                double[] weightedSum2 = new double[64];
                for (int j = 0; j < hiddenLayer2.Count; j++)
                    weightedSum2[j] = perceptron.Execute(hiddenLayer2[j].Weights, weightedSum1, hiddenLayer2[j].Bias);

                for (int k = 0; k < weightedSum2.Length; k++)
                    weightedSum2[k] = activationFunctions.LeakyReLU(weightedSum2[k]);

                //Layer 2
                List<NeuralNetwork> hiddenLayer3 = neuralNetwork.FindAll(x => x.LayerNumber == 2);
                double[] weightedSum3 = new double[128];
                for (int j = 0; j < hiddenLayer3.Count; j++)
                    weightedSum3[j] = perceptron.Execute(hiddenLayer3[j].Weights, weightedSum2, hiddenLayer3[j].Bias);

                for (int k = 0; k < weightedSum3.Length; k++)
                    weightedSum3[k] = activationFunctions.LeakyReLU(weightedSum3[k]);

                //Output Layer
                List<NeuralNetwork> outputLayer = neuralNetwork.FindAll(x => x.LayerNumber == 3);
                double[] weightedSum4 = new double[256];
                for (int j = 0; j < outputLayer.Count; j++)
                    weightedSum4[j] = perceptron.Execute(outputLayer[j].Weights, weightedSum3, outputLayer[j].Bias);

                for (int k = 0; k < weightedSum4.Length; k++)
                    weightedSum4[k] = activationFunctions.BinaryStep(weightedSum4[k]);

                Assess(weightedSum4, i);
            }
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
                double[] placeholder = new double[2];
                parentWeights.Add(placeholder);

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

            WeightStore ws = new WeightStore() { Statistics = new double[32] };

            for (int i = 0; i < valdataSet.Count; i++)
            {
                //Layer 0
                List<NeuralNetwork> hiddenLayer1 = neuralNetwork.FindAll(x => x.LayerNumber == 0);
                double[] weightedSum1 = new double[32];
                for (int j = 0; j < hiddenLayer1.Count; j++)
                    weightedSum1[j] = perceptron.Execute(hiddenLayer1[j].Weights, valdataSet[i].PublicAddressDouble, hiddenLayer1[j].Bias);

                for (int k = 0; k < weightedSum1.Length; k++)
                    weightedSum1[k] = activationFunctions.LeakyReLU(weightedSum1[k]);

                //Layer 1
                List<NeuralNetwork> hiddenLayer2 = neuralNetwork.FindAll(x => x.LayerNumber == 1);
                double[] weightedSum2 = new double[64];
                for (int j = 0; j < hiddenLayer2.Count; j++)
                    weightedSum2[j] = perceptron.Execute(hiddenLayer2[j].Weights, weightedSum1, hiddenLayer2[j].Bias);

                for (int k = 0; k < weightedSum2.Length; k++)
                    weightedSum2[k] = activationFunctions.LeakyReLU(weightedSum2[k]);

                //Layer 2
                List<NeuralNetwork> hiddenLayer3 = neuralNetwork.FindAll(x => x.LayerNumber == 2);
                double[] weightedSum3 = new double[128];
                for (int j = 0; j < hiddenLayer3.Count; j++)
                    weightedSum3[j] = perceptron.Execute(hiddenLayer3[j].Weights, weightedSum2, hiddenLayer3[j].Bias);

                for (int k = 0; k < weightedSum3.Length; k++)
                    weightedSum3[k] = activationFunctions.LeakyReLU(weightedSum3[k]);

                //Output Layer
                List<NeuralNetwork> outputLayer = neuralNetwork.FindAll(x => x.LayerNumber == 3);
                double[] weightedSum4 = new double[256];
                for (int j = 0; j < outputLayer.Count; j++)
                    weightedSum4[j] = perceptron.Execute(outputLayer[j].Weights, weightedSum3, outputLayer[j].Bias);

                for (int k = 0; k < weightedSum4.Length; k++)
                    weightedSum4[k] = activationFunctions.BinaryStep(weightedSum4[k]);

                ValidateTest(weightedSum4, i, ref ws);
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
                for (int i = 0; i < neuralNetwork.Count; i++)
                    if(neuralNetwork[i].LayerNumber == 0)
                        neuralNetwork[i].Weights = weightsGenerator.CreateRandomWeights(20);

                for (int i = 0; i < neuralNetwork.Count; i++)
                    if (neuralNetwork[i].LayerNumber == 1)
                        neuralNetwork[i].Weights = weightsGenerator.CreateRandomWeights(32);

                for (int i = 0; i < neuralNetwork.Count; i++)
                    if (neuralNetwork[i].LayerNumber == 2)
                        neuralNetwork[i].Weights = weightsGenerator.CreateRandomWeights(128);

                for (int i = 0; i < neuralNetwork.Count; i++)
                    if (neuralNetwork[i].LayerNumber == 3)
                        neuralNetwork[i].Weights = weightsGenerator.CreateRandomWeights(256);

            }
            else
            {
                for (int i = 0; i < neuralNetwork.Count; i++)
                    if (neuralNetwork[i].LayerNumber == 0)
                        neuralNetwork[i].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(neuralNetwork[i].Weights), neuralNetwork[i].Weights);

                for (int i = 0; i < neuralNetwork.Count; i++)
                    if (neuralNetwork[i].LayerNumber == 1)
                        neuralNetwork[i].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(neuralNetwork[i].Weights), neuralNetwork[i].Weights);

                for (int i = 0; i < neuralNetwork.Count; i++)
                    if (neuralNetwork[i].LayerNumber == 2)
                        neuralNetwork[i].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(neuralNetwork[i].Weights), neuralNetwork[i].Weights);

                for (int i = 0; i < neuralNetwork.Count; i++)
                    if (neuralNetwork[i].LayerNumber == 3)
                        neuralNetwork[i].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(neuralNetwork[i].Weights), neuralNetwork[i].Weights);
            }
        }

        #endregion
    }
}
