/*
 * Engine C
 * 
 * Engine C splits up the Public Address into adjacent pairs and feeds these pairs into
 * the first layer of the network.  The output is combined and fed into subsequent layers.
 * 
 * The performance of this network is similar to other versions.
 */

using BTCLib;
using MarxML;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BTCECDSAAnalyser
{
    public class DataSetPair
    {
        public double[] PublicAddressDouble { get; set; }
        public int PairStartPosition { get; set; }
        public string PublicAddress { get; set; }
        public byte[] PrivateKey { get; set; }
    }

    public class EngineC
    {
        private List<double[]> parentWeights;
        double min;
        double max;
        double maxStat;
        #region Variables

        internal ScalingFunction scalingFunction;
        internal ActivationFunctions activationFunctions;
        internal GeneticAlgorithm geneticAlgorithm;
        internal NeuralNetwork neuralNetwork;
        internal WeightsGenerator weightsGenerator;

        private List<BTCKeyStore> keyStore;
        internal List<DataSet> dataSet;
        internal List<DataSetPair> dataSetPair;
        internal List<BTCKeyStore> valkeyStore;
        internal List<DataSet> valdataSet;
        internal List<DataSetPair> valdataSetPair;
        internal List<NeuralNetworkLayerDesign> nnld;

        internal int currentMaxBytes;
        internal int deathRate;
        internal int currentDeathRate;

        #endregion

        public EngineC()
        {
            parentWeights = new List<double[]>();
            min = double.MaxValue;
            max = double.MinValue;

            Console.WriteLine("Cracking Bitcoin ECDSA...");
            Init();
        }

        private void Init()
        {
            //Init classes
            scalingFunction = new ScalingFunction();
            activationFunctions = new ActivationFunctions();
            geneticAlgorithm = new GeneticAlgorithm();
            neuralNetwork = new NeuralNetwork();
            weightsGenerator = new WeightsGenerator();

            //Init Lists
            keyStore = new List<BTCKeyStore>();
            dataSet = new List<DataSet>();
            dataSetPair = new List<DataSetPair>();
            valkeyStore = new List<BTCKeyStore>();
            valdataSet = new List<DataSet>();
            valdataSetPair = new List<DataSetPair>();
            nnld = new List<NeuralNetworkLayerDesign>();

            currentMaxBytes = 0;
            deathRate = 10;  //If too high, then chance plays an increasing role and skews the result.
        }

        public void Execute()
        {
            Console.WriteLine(string.Format("Engine C Starting..."));
            DeepLearningNetworkDesign();
            bool shouldRun = true;

            while (shouldRun)
            {
                GenerateDataset();
                GenerateValidationDataset();
                Train();
            }
        }

        #region Generate Dataset

        public void GenerateDataset()
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

                //Added for Engine B.
                ds.PublicAddressDouble = scalingFunction.LinearScaleToRange(ds.PublicAddressDouble, scalingFunction.FindMinMax(ds.PublicAddressDouble), new MinMax() { min = 0, max = 0.01 });

                dataSet.Add(ds);
            }

            //Break into pairs
            for(int i = 0; i < dataSet.Count; i++)
            {
                for(int j = 0; j < dataSet[i].PublicAddressDouble.Length;)
                {
                    DataSetPair dsp = new DataSetPair();
                    dsp.PairStartPosition = j;
                    dsp.PublicAddressDouble = new double[2];
                    dsp.PrivateKey = dataSet[i].PrivateKey;
                    dsp.PublicAddress = dataSet[i].PublicAddress;
                    dsp.PublicAddressDouble[0] = dataSet[i].PublicAddressDouble[j++];
                    dsp.PublicAddressDouble[1] = dataSet[i].PublicAddressDouble[j++];

                    dataSetPair.Add(dsp);
                }
            }
        }

        #endregion

        #region Generate Validation Dataset

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

            //Break into pairs
            for (int i = 0; i < valdataSet.Count; i++)
            {
                for (int j = 0; j < valdataSet[i].PublicAddressDouble.Length;)
                {
                    DataSetPair dsp = new DataSetPair();
                    dsp.PairStartPosition = j;
                    dsp.PublicAddressDouble = new double[2];
                    dsp.PrivateKey = dataSet[i].PrivateKey;
                    dsp.PublicAddress = dataSet[i].PublicAddress;
                    dsp.PublicAddressDouble[0] = dataSet[i].PublicAddressDouble[j++];
                    dsp.PublicAddressDouble[1] = dataSet[i].PublicAddressDouble[j++];

                    valdataSetPair.Add(dsp);
                }
            }
        }

        #endregion

        public void DeepLearningNetworkDesign()
        {
            #region Layer 1

            NeuralNetworkLayerDesign nnld1_1 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_1.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_1.NumberOfInputs * nnld1_1.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld1_2 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_2.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_2.NumberOfInputs * nnld1_2.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld1_3 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_3.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_3.NumberOfInputs * nnld1_3.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld1_4 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_4.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_4.NumberOfInputs * nnld1_4.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld1_5 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_5.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_5.NumberOfInputs * nnld1_5.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld1_6 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_6.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_6.NumberOfInputs * nnld1_6.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld1_7 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_7.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_7.NumberOfInputs * nnld1_7.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld1_8 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_8.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_8.NumberOfInputs * nnld1_8.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld1_9 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_9.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_9.NumberOfInputs * nnld1_9.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld1_10 = new NeuralNetworkLayerDesign() { LayerNumber = 0, NumberOfInputs = 2, NumberOfNetworks = 4 };
            nnld1_10.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld1_10.NumberOfInputs * nnld1_10.NumberOfNetworks);

            #endregion

            NeuralNetworkLayerDesign nnld2 = new NeuralNetworkLayerDesign() { LayerNumber = 2, NumberOfInputs = 40, NumberOfNetworks = 64 };
            nnld2.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld2.NumberOfInputs * nnld2.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld3 = new NeuralNetworkLayerDesign() { LayerNumber = 3, NumberOfInputs = nnld2.NumberOfNetworks, NumberOfNetworks = 128 };
            nnld3.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld3.NumberOfInputs * nnld3.NumberOfNetworks);

            NeuralNetworkLayerDesign nnld4 = new NeuralNetworkLayerDesign() { LayerNumber = 4, NumberOfInputs = nnld3.NumberOfNetworks, NumberOfNetworks = 256 };
            nnld4.Weights = weightsGenerator.CreateRandomWeightsPositive(nnld4.NumberOfInputs * nnld4.NumberOfNetworks);

            //Adding biases to the first layer
            nnld1_1.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_1.Weights.Length);
            nnld1_2.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_2.Weights.Length);
            nnld1_3.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_3.Weights.Length);
            nnld1_4.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_4.Weights.Length);
            nnld1_5.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_5.Weights.Length);
            nnld1_6.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_6.Weights.Length);
            nnld1_7.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_7.Weights.Length);
            nnld1_8.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_8.Weights.Length);
            nnld1_9.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_9.Weights.Length);
            nnld1_10.Biases = weightsGenerator.CreateRandomWeightsPositive(nnld1_10.Weights.Length);

            nnld2.Biases = new double[nnld2.Weights.Length];
            nnld3.Biases = new double[nnld3.Weights.Length];
            nnld4.Biases = new double[nnld4.Weights.Length];

            nnld.Add(nnld1_1);
            nnld.Add(nnld1_2);
            nnld.Add(nnld1_3);
            nnld.Add(nnld1_4);
            nnld.Add(nnld1_5);
            nnld.Add(nnld1_6);
            nnld.Add(nnld1_7);
            nnld.Add(nnld1_8);
            nnld.Add(nnld1_9);
            nnld.Add(nnld1_10);

            nnld.Add(nnld2);
            nnld.Add(nnld3);
            nnld.Add(nnld4);
        }

        public void Train()
        {
            //for (int i = 0; i < dataSetPair.Count; i++)
            //{
            Parallel.For(0, dataSetPair.Count, i =>
            { 
                List<double[]> hl1 = new List<double[]>();
                //string publicAddress = dataSetPair[i].PublicAddress;
                byte[] privateKey = dataSetPair[i].PrivateKey;

                double[] hiddenLayer1_1 = neuralNetwork.PerceptronLayer(nnld[0].NumberOfNetworks, dataSetPair[i++].PublicAddressDouble, nnld[0].Weights, nnld[0].NumberOfInputs, nnld[0].Biases);
                double[] hiddenLayer1_2 = neuralNetwork.PerceptronLayer(nnld[1].NumberOfNetworks, dataSetPair[i++].PublicAddressDouble, nnld[1].Weights, nnld[1].NumberOfInputs, nnld[1].Biases);
                double[] hiddenLayer1_3 = neuralNetwork.PerceptronLayer(nnld[2].NumberOfNetworks, dataSetPair[i++].PublicAddressDouble, nnld[2].Weights, nnld[2].NumberOfInputs, nnld[2].Biases);
                double[] hiddenLayer1_4 = neuralNetwork.PerceptronLayer(nnld[3].NumberOfNetworks, dataSetPair[i++].PublicAddressDouble, nnld[3].Weights, nnld[3].NumberOfInputs, nnld[3].Biases);
                double[] hiddenLayer1_5 = neuralNetwork.PerceptronLayer(nnld[4].NumberOfNetworks, dataSetPair[i++].PublicAddressDouble, nnld[4].Weights, nnld[4].NumberOfInputs, nnld[4].Biases);
                double[] hiddenLayer1_6 = neuralNetwork.PerceptronLayer(nnld[5].NumberOfNetworks, dataSetPair[i++].PublicAddressDouble, nnld[5].Weights, nnld[5].NumberOfInputs, nnld[5].Biases);
                double[] hiddenLayer1_7 = neuralNetwork.PerceptronLayer(nnld[6].NumberOfNetworks, dataSetPair[i++].PublicAddressDouble, nnld[6].Weights, nnld[6].NumberOfInputs, nnld[6].Biases);
                double[] hiddenLayer1_8 = neuralNetwork.PerceptronLayer(nnld[7].NumberOfNetworks, dataSetPair[i++].PublicAddressDouble, nnld[7].Weights, nnld[7].NumberOfInputs, nnld[7].Biases);
                double[] hiddenLayer1_9 = neuralNetwork.PerceptronLayer(nnld[8].NumberOfNetworks, dataSetPair[i++].PublicAddressDouble, nnld[8].Weights, nnld[8].NumberOfInputs, nnld[8].Biases);
                double[] hiddenLayer1_10 = neuralNetwork.PerceptronLayer(nnld[9].NumberOfNetworks, dataSetPair[i].PublicAddressDouble, nnld[9].Weights, nnld[9].NumberOfInputs, nnld[9].Biases);
                
                hiddenLayer1_1 = activationFunctions.Step(hiddenLayer1_1, 0, 1);
                hiddenLayer1_2 = activationFunctions.Step(hiddenLayer1_2, 0, 1);
                hiddenLayer1_3 = activationFunctions.Step(hiddenLayer1_3, 0, 1);
                hiddenLayer1_4 = activationFunctions.Step(hiddenLayer1_4, 0, 1);
                hiddenLayer1_5 = activationFunctions.Step(hiddenLayer1_5, 0, 1);
                hiddenLayer1_6 = activationFunctions.Step(hiddenLayer1_6, 0, 1);
                hiddenLayer1_7 = activationFunctions.Step(hiddenLayer1_7, 0, 1);
                hiddenLayer1_8 = activationFunctions.Step(hiddenLayer1_8, 0, 1);
                hiddenLayer1_9 = activationFunctions.Step(hiddenLayer1_9, 0, 1);
                hiddenLayer1_10 = activationFunctions.Step(hiddenLayer1_10, 0, 1);

                hl1.Add(hiddenLayer1_1);
                hl1.Add(hiddenLayer1_2);
                hl1.Add(hiddenLayer1_3);
                hl1.Add(hiddenLayer1_4);
                hl1.Add(hiddenLayer1_5);
                hl1.Add(hiddenLayer1_6);
                hl1.Add(hiddenLayer1_7);
                hl1.Add(hiddenLayer1_8);
                hl1.Add(hiddenLayer1_9);
                hl1.Add(hiddenLayer1_10);

                //Merge outputs
                int totalNumberOfOutputs = 0;

                for (int k = 0; k < hl1.Count; k++)
                    totalNumberOfOutputs += hl1[k].Length;

                double[] hiddenLayer1 = new double[totalNumberOfOutputs];

                int index = 0;
                for (int k = 0; k < hl1.Count; k++)
                {
                    for(int l = 0; l < hl1[k].Length; l++)
                    {
                        hiddenLayer1[index++] = hl1[k][l];
                    }
                }


                double[] hiddenLayer2 = neuralNetwork.PerceptronLayer(nnld[10].NumberOfNetworks, hiddenLayer1, nnld[10].Weights, nnld[10].NumberOfInputs, nnld[10].Biases);
                hiddenLayer2 = scalingFunction.LinearScaleToRange(hiddenLayer2, scalingFunction.FindMinMax(hiddenLayer2), new MinMax() { min = 0, max = 1 });
                hiddenLayer2 = activationFunctions.Step(hiddenLayer2, 0, 1);
                double[] hiddenLayer3 = neuralNetwork.PerceptronLayer(nnld[11].NumberOfNetworks, hiddenLayer2, nnld[11].Weights, nnld[11].NumberOfInputs, nnld[11].Biases);
                hiddenLayer3 = scalingFunction.LinearScaleToRange(hiddenLayer3, scalingFunction.FindMinMax(hiddenLayer3), new MinMax() { min = 0, max = 1 });
                hiddenLayer3 = activationFunctions.Step(hiddenLayer3, 0, 1);
                double[] outputlayer = neuralNetwork.PerceptronLayer(nnld[12].NumberOfNetworks, hiddenLayer3, nnld[12].Weights, nnld[12].NumberOfInputs, nnld[12].Biases);
                outputlayer = scalingFunction.LinearScaleToRange(outputlayer, scalingFunction.FindMinMax(outputlayer), new MinMax() { min = 0, max = 1 });
                outputlayer = activationFunctions.Step(outputlayer, 0, 1);

                Assess(outputlayer, privateKey);
            });
        }

        private void Assess(double[] outputlayer, byte[] privateKey)
        {
            int matchCount = 0;
            outputlayer = ConvertFromBinaryToDouble(outputlayer);

            for (int i = 0; i < privateKey.Length; i++)
            {
                if (privateKey[i] == (int)outputlayer[i])
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


        #region Validation of generalisation

        private void Validate()
        {
            bool shouldSave = false;

            WeightStore ws = new WeightStore() { Statistics = new double[32], WeightsHL0 = nnld[0].Weights, WeightsHL1 = nnld[1].Weights, WeightsHL2 = nnld[2].Weights, WeightsOL = nnld[3].Weights };

            for (int i = 0; i < valdataSetPair.Count; i++)
            {
                List<double[]> hl1 = new List<double[]>();
                //string publicAddress = dataSetPair[i].PublicAddress;
                byte[] privateKey = valdataSetPair[i].PrivateKey;

                double[] hiddenLayer1_1 = neuralNetwork.PerceptronLayer(nnld[0].NumberOfNetworks, valdataSetPair[i++].PublicAddressDouble, nnld[0].Weights, nnld[0].NumberOfInputs, nnld[0].Biases);
                double[] hiddenLayer1_2 = neuralNetwork.PerceptronLayer(nnld[1].NumberOfNetworks, valdataSetPair[i++].PublicAddressDouble, nnld[1].Weights, nnld[1].NumberOfInputs, nnld[1].Biases);
                double[] hiddenLayer1_3 = neuralNetwork.PerceptronLayer(nnld[2].NumberOfNetworks, valdataSetPair[i++].PublicAddressDouble, nnld[2].Weights, nnld[2].NumberOfInputs, nnld[2].Biases);
                double[] hiddenLayer1_4 = neuralNetwork.PerceptronLayer(nnld[3].NumberOfNetworks, valdataSetPair[i++].PublicAddressDouble, nnld[3].Weights, nnld[3].NumberOfInputs, nnld[3].Biases);
                double[] hiddenLayer1_5 = neuralNetwork.PerceptronLayer(nnld[4].NumberOfNetworks, valdataSetPair[i++].PublicAddressDouble, nnld[4].Weights, nnld[4].NumberOfInputs, nnld[4].Biases);
                double[] hiddenLayer1_6 = neuralNetwork.PerceptronLayer(nnld[5].NumberOfNetworks, valdataSetPair[i++].PublicAddressDouble, nnld[5].Weights, nnld[5].NumberOfInputs, nnld[5].Biases);
                double[] hiddenLayer1_7 = neuralNetwork.PerceptronLayer(nnld[6].NumberOfNetworks, valdataSetPair[i++].PublicAddressDouble, nnld[6].Weights, nnld[6].NumberOfInputs, nnld[6].Biases);
                double[] hiddenLayer1_8 = neuralNetwork.PerceptronLayer(nnld[7].NumberOfNetworks, valdataSetPair[i++].PublicAddressDouble, nnld[7].Weights, nnld[7].NumberOfInputs, nnld[7].Biases);
                double[] hiddenLayer1_9 = neuralNetwork.PerceptronLayer(nnld[8].NumberOfNetworks, valdataSetPair[i++].PublicAddressDouble, nnld[8].Weights, nnld[8].NumberOfInputs, nnld[8].Biases);
                double[] hiddenLayer1_10 = neuralNetwork.PerceptronLayer(nnld[9].NumberOfNetworks, valdataSetPair[i].PublicAddressDouble, nnld[9].Weights, nnld[9].NumberOfInputs, nnld[9].Biases);

                hiddenLayer1_1 = activationFunctions.Step(hiddenLayer1_1, 0, 1);
                hiddenLayer1_2 = activationFunctions.Step(hiddenLayer1_2, 0, 1);
                hiddenLayer1_3 = activationFunctions.Step(hiddenLayer1_3, 0, 1);
                hiddenLayer1_4 = activationFunctions.Step(hiddenLayer1_4, 0, 1);
                hiddenLayer1_5 = activationFunctions.Step(hiddenLayer1_5, 0, 1);
                hiddenLayer1_6 = activationFunctions.Step(hiddenLayer1_6, 0, 1);
                hiddenLayer1_7 = activationFunctions.Step(hiddenLayer1_7, 0, 1);
                hiddenLayer1_8 = activationFunctions.Step(hiddenLayer1_8, 0, 1);
                hiddenLayer1_9 = activationFunctions.Step(hiddenLayer1_9, 0, 1);
                hiddenLayer1_10 = activationFunctions.Step(hiddenLayer1_10, 0, 1);

                hl1.Add(hiddenLayer1_1);
                hl1.Add(hiddenLayer1_2);
                hl1.Add(hiddenLayer1_3);
                hl1.Add(hiddenLayer1_4);
                hl1.Add(hiddenLayer1_5);
                hl1.Add(hiddenLayer1_6);
                hl1.Add(hiddenLayer1_7);
                hl1.Add(hiddenLayer1_8);
                hl1.Add(hiddenLayer1_9);
                hl1.Add(hiddenLayer1_10);

                //Merge outputs
                int totalNumberOfOutputs = 0;

                for (int k = 0; k < hl1.Count; k++)
                    totalNumberOfOutputs += hl1[k].Length;

                double[] hiddenLayer1 = new double[totalNumberOfOutputs];
                int index = 0;

                for (int k = 0; k < hl1.Count; k++)
                {
                    for (int l = 0; l < hl1[k].Length; l++)
                    {
                        hiddenLayer1[index++] = hl1[k][l];
                    }
                }


                double[] hiddenLayer2 = neuralNetwork.PerceptronLayer(nnld[10].NumberOfNetworks, hiddenLayer1, nnld[10].Weights, nnld[10].NumberOfInputs, nnld[10].Biases);
                hiddenLayer2 = scalingFunction.LinearScaleToRange(hiddenLayer2, scalingFunction.FindMinMax(hiddenLayer2), new MinMax() { min = 0, max = 1 });
                hiddenLayer2 = activationFunctions.Step(hiddenLayer2, 0, 1);
                double[] hiddenLayer3 = neuralNetwork.PerceptronLayer(nnld[11].NumberOfNetworks, hiddenLayer2, nnld[11].Weights, nnld[11].NumberOfInputs, nnld[11].Biases);
                hiddenLayer3 = scalingFunction.LinearScaleToRange(hiddenLayer3, scalingFunction.FindMinMax(hiddenLayer3), new MinMax() { min = 0, max = 1 });
                hiddenLayer3 = activationFunctions.Step(hiddenLayer3, 0, 1);
                double[] outputlayer = neuralNetwork.PerceptronLayer(nnld[12].NumberOfNetworks, hiddenLayer3, nnld[12].Weights, nnld[12].NumberOfInputs, nnld[12].Biases);
                outputlayer = scalingFunction.LinearScaleToRange(outputlayer, scalingFunction.FindMinMax(outputlayer), new MinMax() { min = 0, max = 1 });
                outputlayer = activationFunctions.Step(outputlayer, 0, 1);

                ValidateTest(outputlayer, privateKey, ref ws);
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

        private void ValidateTest(double[] outputlayer, byte[] privateKey, ref WeightStore ws)
        {
            outputlayer = ConvertFromBinaryToDouble(outputlayer);

            for (int i = 0; i < privateKey.Length; i++)
            {
                if (privateKey[i] == (int)outputlayer[i])
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
                nnld[0].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[0].NumberOfInputs * nnld[0].NumberOfNetworks);
                nnld[1].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[1].NumberOfInputs * nnld[1].NumberOfNetworks);
                nnld[2].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[2].NumberOfInputs * nnld[2].NumberOfNetworks);
                nnld[3].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[3].NumberOfInputs * nnld[3].NumberOfNetworks);
                nnld[4].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[4].NumberOfInputs * nnld[4].NumberOfNetworks);
                nnld[5].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[5].NumberOfInputs * nnld[5].NumberOfNetworks);
                nnld[6].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[6].NumberOfInputs * nnld[6].NumberOfNetworks);
                nnld[7].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[7].NumberOfInputs * nnld[7].NumberOfNetworks);
                nnld[8].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[8].NumberOfInputs * nnld[8].NumberOfNetworks);
                nnld[9].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[9].NumberOfInputs * nnld[9].NumberOfNetworks);
                nnld[10].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[10].NumberOfInputs * nnld[10].NumberOfNetworks);
                nnld[11].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[11].NumberOfInputs * nnld[11].NumberOfNetworks);
                nnld[12].Weights = weightsGenerator.CreateRandomWeightsPositive(nnld[12].NumberOfInputs * nnld[12].NumberOfNetworks);
            }
            else
            {
                nnld[0].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[0].Weights), nnld[0].Weights);
                nnld[1].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[1].Weights), nnld[1].Weights);
                nnld[2].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[2].Weights), nnld[2].Weights);
                nnld[3].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[3].Weights), nnld[3].Weights);
                nnld[4].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[4].Weights), nnld[4].Weights);
                nnld[5].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[5].Weights), nnld[5].Weights);
                nnld[6].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[6].Weights), nnld[6].Weights);
                nnld[7].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[7].Weights), nnld[7].Weights);
                nnld[8].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[8].Weights), nnld[8].Weights);
                nnld[9].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[9].Weights), nnld[9].Weights);
                nnld[10].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[10].Weights), nnld[10].Weights);
                nnld[11].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[11].Weights), nnld[11].Weights);
                nnld[11].Weights = geneticAlgorithm.CrossOverAndMutation(geneticAlgorithm.EvaluateFitness(nnld[12].Weights), nnld[12].Weights);
            }
        }

        #endregion
    }
}
