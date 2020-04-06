/*
 * Engine H
 * 
 * Improving generalisation
 * 
 */
using BTCECDSACracker.DAL;
using BTCECDSACracker.DAL.Tables;
using BTCECDSACracker.Helpers;
using BTCLib;
using MarxMLL2;
using MarxMLL2.WeightsGenerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace BTCECDSACracker.Engines
{


    public class EngineH
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
        private double[] attemptstats;

        #endregion

        public EngineH()
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

            deathRate = 10;  //If too high, then chance plays an increasing role and skews the result.

            cb = new CircularBuffer(deathRate);
            oldMetric = double.MaxValue;
            attemptstats = new double[32];

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
                    Weights = weightsGenerator.CreateRandomWeights(20)
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

        private void GenerateDataset()
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
                    ds.PublicAddressDouble[j] = pap[j] != 0 ? pap[j] / 256.0 : 0;

                dataSet.Add(ds);
            }
        }

        private void GenerateValidationDataset()
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
                    ds.PublicAddressDouble[j] = pap[j] != 0 ? pap[j] / 256.0 : 0;

                //Added for Engine B
                //ds.PublicAddressDouble = scalingFunction.LinearScaleToRange(ds.PublicAddressDouble, scalingFunction.FindMinMax(ds.PublicAddressDouble), new MinMax() { min = 0, max = 0.01 });

                valdataSet.Add(ds);
            }
        }

        #endregion

        #region Execute

        public void Execute()
        {
            Console.WriteLine(string.Format("Engine H Starting..."));
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
            //The assessment stage is now replaced with an execution across the
            //validation set.
            //
            //We're using the peak probability as a means of evolving the GA.

            outputlayer = ConvertFromBinaryToDouble(outputlayer);

            List<NeuralNetwork> layer1 = neuralNetwork.FindAll(x => x.LayerNumber == 0);
            List<NeuralNetwork> layer2 = neuralNetwork.FindAll(x => x.LayerNumber == 1);
            List<NeuralNetwork> layer3 = neuralNetwork.FindAll(x => x.LayerNumber == 2);
            List<NeuralNetwork> layer4 = neuralNetwork.FindAll(x => x.LayerNumber == 3);
            List<double[]> weights1 = new List<double[]>();
            List<double[]> weights2 = new List<double[]>();
            List<double[]> weights3 = new List<double[]>();
            List<double[]> weights4 = new List<double[]>();

            for (int i = 0; i < 20; i++)
            {
                weights1.Add(layer1[i].Weights);
            }

            for (int i = 0; i < 64; i++)
            {
                weights2.Add(layer2[i].Weights);
            }

            for (int i = 0; i < 128; i++)
            {
                weights3.Add(layer3[i].Weights);
            }

            for (int i = 0; i < 256; i++)
            {
                weights4.Add(layer4[i].Weights);
            }

            attemptstats = new double[32];

            double metric = Validate(weights1, weights2, weights3, weights4);

            if (oldMetric == double.MaxValue)
            {
                oldMetric = metric;
                UpdateWeights();
                return;
            }

            if (metric <= oldMetric)
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

                return;
            }
        }

        #endregion

        #region Validate

        private double Validate(List<double[]> weights1, List<double[]> weights2, List<double[]> weights3, List<double[]> weights4)
        {
            double probability = 0;

            for (int i = 0; i < valdataSet.Count; i++)
            {
                //Layer 0
                List<NeuralNetwork> hiddenLayer1 = neuralNetwork.FindAll(x => x.LayerNumber == 0);
                double[] weightedSum1 = new double[20];
                for (int j = 0; j < hiddenLayer1.Count; j++)
                    weightedSum1[j] = perceptron.Execute(weights1[j], valdataSet[i].PublicAddressDouble, hiddenLayer1[j].Bias);

                for (int k = 0; k < weightedSum1.Length; k++)
                    weightedSum1[k] = activationFunctions.LeakyReLU(weightedSum1[k]);

                //Layer 1
                List<NeuralNetwork> hiddenLayer2 = neuralNetwork.FindAll(x => x.LayerNumber == 1);
                double[] weightedSum2 = new double[64];
                for (int j = 0; j < hiddenLayer2.Count; j++)
                    weightedSum2[j] = perceptron.Execute(weights2[j], weightedSum1, hiddenLayer2[j].Bias);

                for (int k = 0; k < weightedSum2.Length; k++)
                    weightedSum2[k] = activationFunctions.LeakyReLU(weightedSum2[k]);

                //Layer 2
                List<NeuralNetwork> hiddenLayer3 = neuralNetwork.FindAll(x => x.LayerNumber == 2);
                double[] weightedSum3 = new double[128];
                for (int j = 0; j < hiddenLayer3.Count; j++)
                    weightedSum3[j] = perceptron.Execute(weights3[j], weightedSum2, hiddenLayer3[j].Bias);

                for (int k = 0; k < weightedSum3.Length; k++)
                    weightedSum3[k] = activationFunctions.LeakyReLU(weightedSum3[k]);

                //Output Layer
                List<NeuralNetwork> outputLayer = neuralNetwork.FindAll(x => x.LayerNumber == 3);
                double[] weightedSum4 = new double[256];
                for (int j = 0; j < outputLayer.Count; j++)
                    weightedSum4[j] = perceptron.Execute(weights4[j], weightedSum3, outputLayer[j].Bias);

                for (int k = 0; k < weightedSum4.Length; k++)
                    weightedSum4[k] = activationFunctions.BinaryStep(weightedSum4[k]);

                probability = ValidationAssess(weightedSum4, i); 
            }
            
            if(probability >= 0.005)
                SerialiseWeightsAndSaveToDB(attemptstats, weights1, weights2, weights3, weights4);

            return probability;
        }

        #endregion

        #region Validation Assessment

        private double ValidationAssess(double[] outputlayer, int index)
        {
            //Running total for these set of weights
            for (int i = 0; i < 32; i++)
            {
                if ((int)outputlayer[i] == (int)valdataSet[index].PrivateKey[i])
                    attemptstats[i]++;
            }

            double probability = double.MinValue;
            double[] overallstats = attemptstats;

            if (valdataSet.Count - 1 == index)
            {
                //Grab highest temp
                for (int i = 0; i < overallstats.Length; i++)
                    if (overallstats[i] > probability)
                        probability = overallstats[i];

                probability = probability / valdataSet.Count;

                Console.WriteLine(Environment.NewLine);
                Console.WriteLine(string.Format("Probability: {0}", probability));
                Console.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}", overallstats[0], overallstats[1], overallstats[2], overallstats[3], overallstats[4], overallstats[5], overallstats[6], overallstats[7], overallstats[8], overallstats[9], overallstats[10], overallstats[11], overallstats[12], overallstats[13], overallstats[14], overallstats[15], overallstats[16], overallstats[17], overallstats[18], overallstats[19], overallstats[20], overallstats[21], overallstats[22], overallstats[23], overallstats[24], overallstats[25], overallstats[26], overallstats[27], overallstats[28], overallstats[29], overallstats[30], overallstats[31]));
            }

            return probability;
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

        #region Serialise weights and save to DB

        //Weights are stored as serialised strings rather than objects - lazy me.
        private void SerialiseWeightsAndSaveToDB(double[] weightStatistics, List<double[]> weights1, List<double[]> weights2, List<double[]> weights3, List<double[]> weights4)
        {
            BTCCrackerDBContext wdbc = new BTCCrackerDBContext();
            WeightLog ws = new WeightLog();

            ws.WeightsHL0 = SerialiseWeights(weights1);
            ws.WeightsHL1 = SerialiseWeights(weights2);
            ws.WeightsHL2 = SerialiseWeights(weights3);
            ws.WeightsOL = SerialiseWeights(weights4);

            ws.Byte0 = (int)weightStatistics[0];
            ws.Byte1 = (int)weightStatistics[1];
            ws.Byte2 = (int)weightStatistics[2];
            ws.Byte3 = (int)weightStatistics[3];
            ws.Byte4 = (int)weightStatistics[4];
            ws.Byte5 = (int)weightStatistics[5];
            ws.Byte6 = (int)weightStatistics[6];
            ws.Byte7 = (int)weightStatistics[7];
            ws.Byte8 = (int)weightStatistics[8];
            ws.Byte9 = (int)weightStatistics[9];
            ws.Byte10 = (int)weightStatistics[10];
            ws.Byte11 = (int)weightStatistics[11];
            ws.Byte12 = (int)weightStatistics[12];
            ws.Byte13 = (int)weightStatistics[13];
            ws.Byte14 = (int)weightStatistics[14];
            ws.Byte15 = (int)weightStatistics[15];
            ws.Byte16 = (int)weightStatistics[16];
            ws.Byte17 = (int)weightStatistics[17];
            ws.Byte18 = (int)weightStatistics[18];
            ws.Byte19 = (int)weightStatistics[19];
            ws.Byte20 = (int)weightStatistics[20];
            ws.Byte21 = (int)weightStatistics[21];
            ws.Byte22 = (int)weightStatistics[22];
            ws.Byte23 = (int)weightStatistics[23];
            ws.Byte24 = (int)weightStatistics[24];
            ws.Byte25 = (int)weightStatistics[25];
            ws.Byte26 = (int)weightStatistics[26];
            ws.Byte27 = (int)weightStatistics[27];
            ws.Byte28 = (int)weightStatistics[28];
            ws.Byte29 = (int)weightStatistics[29];
            ws.Byte30 = (int)weightStatistics[30];
            ws.Byte31 = (int)weightStatistics[31];

            wdbc.Add(ws);
            wdbc.SaveChanges();

        }

        private string SerialiseWeights(List<double[]> weights)
        {
            StringBuilder w = new StringBuilder();

            for (int i = 0; i < weights.Count; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    if(j < weights[i].Length - 1)
                        w.Append(weights[i][j].ToString() + ",");
                    else
                        w.Append(weights[i][j].ToString() + ":");
                }

                w.Append(";");
            }

            return w.ToString();
        }
        #endregion
    }
}
