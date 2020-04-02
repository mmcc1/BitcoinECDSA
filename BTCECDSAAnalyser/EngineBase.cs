/*
 * EngineBase
 * 
 * Used as a base object to capture common stuff across any derived engine.
 * 
 * Now includes DB.
 */
using BTCECDSAAnalyser.Entity;
using BTCECDSAAnalyser.Entity.Tables;
using BTCLib;
using MarxML;
using System;
using System.Collections.Generic;
using System.Text;

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
                DataSet ds = new DataSet() { PublicAddressDouble = new double[20], PrivateKey = valkeyStore[i].PrivateKeyByteArray, PublicAddress = valkeyStore[i].PublicAddress } ;
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

        #region Serialise weights and save to DB

        //Weights are stored as serialised strings rather than objects - lazy me.
        internal void SerialiseWeightsAndSaveToDB(double[] weightStatistics)
        {
            WeightsDBContext wdbc = new WeightsDBContext();
            WeightStatistics ws = new WeightStatistics();

            ws.WeightsHL0 = SerialiseWeights(nnld[0].Weights);
            ws.WeightsHL1 = SerialiseWeights(nnld[1].Weights);
            ws.WeightsHL2 = SerialiseWeights(nnld[2].Weights);
            ws.WeightsOL = SerialiseWeights(nnld[3].Weights);

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

        private string SerialiseWeights(double[] w)
        {
            StringBuilder weights = new StringBuilder();

            for (int i = 0; i < w.Length; i++)
            {
                weights.Append(w[i].ToString() + ",");
            }

            return weights.ToString();
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
