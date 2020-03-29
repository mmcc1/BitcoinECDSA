/*
 * EngineBase
 * 
 * Used as a base object to capture common stuff across any derived engine.
 */
using BTCLib;
using MarxML;
using System;
using System.Collections.Generic;

namespace BTCECDSAAnalyser
{
    //A dataset object records all info.  CrackedPrivateKey not used, for a different architecture.
    public class DataSet
    {
        public double[] PublicAddressDouble { get; set; }
        public string PublicAddress { get; set; }
        public byte[] PrivateKey { get; set; }
        public byte[] CrackedPrivateKey { get; set; }
    }

    //An object used in validation
    public class WeightStore
    {
        public double[] WeightsHL0 { get; set; }
        public double[] WeightsHL1 { get; set; }
        public double[] WeightsHL2 { get; set; }
        public double[] WeightsOL { get; set; }
        public double[] Statistics { get; set; }
    }

    public class EngineBase
    {
        #region Variables

        internal ScalingFunction scalingFunction;
        internal ActivationFunctions activationFunctions;
        internal GeneticAlgorithm geneticAlgorithm;
        internal NeuralNetwork neuralNetwork;
        internal WeightsGenerator weightsGenerator;

        private List<BTCKeyStore> keyStore;
        internal List<DataSet> dataSet;
        internal List<BTCKeyStore> valkeyStore;
        internal List<DataSet> valdataSet;
        internal List<NeuralNetworkLayerDesign> nnld;

        internal int currentMaxBytes;
        internal int deathRate;
        internal int currentDeathRate;

        #endregion

        public EngineBase()
        {
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
            valkeyStore = new List<BTCKeyStore>();
            valdataSet = new List<DataSet>();
            nnld = new List<NeuralNetworkLayerDesign>();

            currentMaxBytes = 0;
            deathRate = 10;  //If too high, then chance plays an increasing role and skews the result.
        }

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
                DataSet ds = new DataSet() { PublicAddressDouble = new double[20], PrivateKey = keyStore[i].PrivateKeyByteArray, PublicAddress = keyStore[i].PublicAddress } ;
                byte[] pap = BTCPrep.PrepareAddress(keyStore[i].PublicAddress);

                if (pap.Length != 20)
                    continue;

                for (int j = 0; j < pap.Length; j++)
                    ds.PublicAddressDouble[j] = pap[j];

                dataSet.Add(ds);
            }
        }

        internal void GenerateValidationDataset()
        {
            Console.WriteLine("Generating Validation Dataset...");

            for (int i = 0; i < 10000; i++)
            {
                valkeyStore.Add(BTCBasicFunctions.CreateKeyPair());
            }

            Console.WriteLine("Converting Validation Dataset...");

            for (int i = 0; i < valkeyStore.Count; i++)
            {
                DataSet ds = new DataSet() { PublicAddressDouble = new double[20], PrivateKey = valkeyStore[i].PrivateKeyByteArray, PublicAddress = valkeyStore[i].PublicAddress } ;
                byte[] pap = BTCPrep.PrepareAddress(valkeyStore[i].PublicAddress);

                if (pap.Length != 20)
                    continue;

                for (int j = 0; j < pap.Length; j++)
                    ds.PublicAddressDouble[j] = pap[j];

                valdataSet.Add(ds);
            }
        }

        #endregion

        internal virtual void DesignNN()
        {

        }

        public virtual void Execute()
        {

        }
    }
}
