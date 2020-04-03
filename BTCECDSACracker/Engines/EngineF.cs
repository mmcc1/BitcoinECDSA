/*
 * Engine F
 * 
 * Using WeightsGeneratorRNGCSP for weights generation.
 * 
 * Produces byte probabilities of  0.065  in a reasonable time.
 * 
 * Test if reducing the input layer results
 * in better probabilities.
 */
using BTCECDSACracker.Helpers;
using BTCLib;
using MarxMLL2;
using MarxMLL2.WeightsGenerators;
using System;
using System.Collections.Generic;

namespace BTCECDSACracker.Engines
{


    public class EngineF
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
        private int deathRate;
        private int currentDeathRate;
        private List<double[]> parentWeights;
        CircularBuffer cb;
        private double oldMetric;

        #endregion

        public EngineF()
        {
            parentWeights = new List<double[]>();
            Init();
        }

        #region Initialisation

        private void Init()
        {
            //Init classes
            scalingFunction = new ScalingFunction();
            weightsGenerator = new WeightsGeneratorRNGCSP();
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

            deathRate = 100;  //If too high, then chance plays an increasing role and skews the result.

            cb = new CircularBuffer(deathRate);
            oldMetric = double.MaxValue;

            GenerateValidationDataset();
        }

        #endregion

        #region Deep Learning Network Design

        private void DesignNN()
        {
            //Layer 0
            for (int i = 0; i < 20; i++)
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
                    Weights = weightsGenerator.CreateRandomWeights(40)
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
            Console.WriteLine(string.Format("Engine F Starting..."));
            DesignNN();
            bool shouldRun = true;

            while (shouldRun)
            {
                GenerateDataset();
                
                Train();
            }
        }

        #endregion

        #region Train

        private void Train()
        {
            for (int i = 0; i < dataSet.Count; i++)
            {
                //Layer 0
                List<NeuralNetwork> hiddenLayer1 = neuralNetwork.FindAll(x => x.LayerNumber == 0);
                double[] weightedSum1 = new double[20];
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

        #endregion

        #region Assess

        private void Assess(double[] outputlayer, int index)
        {
            outputlayer = ConvertFromBinaryToDouble(outputlayer);
            double[] attemptstats = new double[32];
            double[] overallstats = new double[32];

            //Add to buffer
            cb.Create(new CircleBufferEntry() { PrivateKey = dataSet[index].PrivateKey, PrivateKeyAttempt = outputlayer });

            //Calculate stats for current attempt
            for (int i = 0; i < 32; i++)
            {
                if ((int)outputlayer[i] == (int)dataSet[index].PrivateKey[i])
                    attemptstats[i]++;
            }

            //Check if there is any byte match between output and private key
            bool match = false;
            for(int i = 0; i < 32; i++)
            {
                if (attemptstats[i] > 0)
                    match = true;
            }

            //if there is a match then we are justified in broader assessment
            if (match)
            {
                //Calculate stats since last population crash
                CircleBufferEntry[] cbes = cb.Read();

                for (int i = 0; i < cbes.Length; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if (cbes[i] != null)
                        {
                            if ((int)cbes[i].PrivateKey[j] == (int)cbes[i].PrivateKeyAttempt[j])
                                overallstats[j]++;
                        }
                        else  //if the buffer is not full yet, abort.
                        {
                            UpdateWeights();
                            return;
                        }
                    }
                }

                //Create a metric out of these stats
                double metric = 0;
                for (int i = 0; i < 32; i++)
                {
                    metric += overallstats[i];
                }

                //If there is no previous metric, create it, update weights and return
                if (oldMetric == double.MaxValue)
                {
                    oldMetric = metric;
                    UpdateWeights();
                    return;
                }

                //If the metric is less than the previous metric, either crash or increment deathrate
                if (metric < oldMetric)
                {
                    if (currentDeathRate > deathRate)
                    {
                        parentWeights.Clear();
                        currentDeathRate = 0;
                        UpdateWeights();
                        cb.Clear();
                        return;
                    }
                    else
                    {
                        currentDeathRate++;
                        UpdateWeights();
                        return;
                    }
                }
                else if (metric > oldMetric) //If it does exceed previous metric, we have an improvement, so print it out to the screen
                {
                    oldMetric = metric;
                    parentWeights.Clear();
                    parentWeights.Add(new double[] { 1, 2, 3 });

                    Console.WriteLine(string.Format("Metric: {32} - {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}", overallstats[0], overallstats[1], overallstats[2], overallstats[3], overallstats[4], overallstats[5], overallstats[6], overallstats[7], overallstats[8], overallstats[9], overallstats[10], overallstats[11], overallstats[12], overallstats[13], overallstats[14], overallstats[15], overallstats[16], overallstats[17], overallstats[18], overallstats[19], overallstats[20], overallstats[21], overallstats[22], overallstats[23], overallstats[24], overallstats[25], overallstats[26], overallstats[27], overallstats[28], overallstats[29], overallstats[30], overallstats[31], metric));
                    
                    //Validation
                    //For validation we require fairly high numbers across all byte positions since the last crash.

                    
                    return;
                }
                else //No change
                {
                    currentDeathRate++;
                    UpdateWeights();
                    return;
                }
            }
            else  //No match, so increment death rate
                currentDeathRate++;


            if (currentDeathRate > deathRate)
            {
                parentWeights.Clear();
                currentDeathRate = 0;
                UpdateWeights();
                cb.Clear();
                return;
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
                    if (neuralNetwork[i].LayerNumber == 0)
                        neuralNetwork[i].Weights = weightsGenerator.CreateRandomWeights(20);

                for (int i = 0; i < neuralNetwork.Count; i++)
                    if (neuralNetwork[i].LayerNumber == 1)
                        neuralNetwork[i].Weights = weightsGenerator.CreateRandomWeights(20);

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
