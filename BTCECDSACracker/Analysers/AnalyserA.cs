using BTCECDSACracker.DAL;
using BTCECDSACracker.DAL.Tables;
using BTCLib;
using MarxMLL2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BTCECDSACracker.Analysers
{
    #region Analyser Entry

    public class AnalyserEntry
    {
        public List<double[]> Layer1Weights { get; set; }
        public List<double[]> Layer2Weights { get; set; }
        public List<double[]> Layer3Weights { get; set; }
        public List<double[]> Layer4Weights { get; set; }
        public int Byte0 { get; set; }
        public int Byte1 { get; set; }
        public int Byte2 { get; set; }
        public int Byte3 { get; set; }
        public int Byte4 { get; set; }
        public int Byte5 { get; set; }
        public int Byte6 { get; set; }
        public int Byte7 { get; set; }
        public int Byte8 { get; set; }
        public int Byte9 { get; set; }
        public int Byte10 { get; set; }
        public int Byte11 { get; set; }
        public int Byte12 { get; set; }
        public int Byte13 { get; set; }
        public int Byte14 { get; set; }
        public int Byte15 { get; set; }
        public int Byte16 { get; set; }
        public int Byte17 { get; set; }
        public int Byte18 { get; set; }
        public int Byte19 { get; set; }
        public int Byte20 { get; set; }
        public int Byte21 { get; set; }
        public int Byte22 { get; set; }
        public int Byte23 { get; set; }
        public int Byte24 { get; set; }
        public int Byte25 { get; set; }
        public int Byte26 { get; set; }
        public int Byte27 { get; set; }
        public int Byte28 { get; set; }
        public int Byte29 { get; set; }
        public int Byte30 { get; set; }
        public int Byte31 { get; set; }
        
    }

    #endregion

    public class AnalyserA
    {
        #region Variables

        private ScalingFunction scalingFunction;
        private ActivationFunctions activationFunctions;
        private Perceptron perceptron;
        private string publicAddress = "1CounterpartyXXXXXXXXXXXXXXXUWLpVr";
        private double[] publicAddressDouble;
        private List<NeuralNetwork> neuralNetwork;
        private AnalyserEntry analyserEntry;
        private int byteNum;
        private int[] rowCountsPerByte;
        private int rowSkipCount;
        private double[,] votes;
        private int prob;

        #endregion

        public AnalyserA()
        {
            Init();
        }

        #region Initialisation

        private void Init()
        {
            //Init classes
            scalingFunction = new ScalingFunction();
            activationFunctions = new ActivationFunctions();
            neuralNetwork = new List<NeuralNetwork>();
            perceptron = new Perceptron();
            analyserEntry = new AnalyserEntry();
            

            //Init Lists
            neuralNetwork = new List<NeuralNetwork>();
            rowCountsPerByte = new int[32];
            byteNum = 0;
            rowSkipCount = 0;
            votes = new double[32, 256];
            publicAddressDouble = new double[20];
            prob = 40;
        }

        #endregion

        #region Execute

        public void Execute()
        {
            Console.WriteLine("BTC Analyser A starting...");
            Prep();

            while (true)
            {
                if(rowSkipCount == rowCountsPerByte[byteNum]  && rowCountsPerByte[byteNum] != 0)
                {
                    if (byteNum == 31)
                        break;

                    rowSkipCount = 0;
                    byteNum++;

                    Console.WriteLine("Analysing Byte " + byteNum);
                }
                else if (rowSkipCount == rowCountsPerByte[byteNum] && rowCountsPerByte[byteNum] == 0)
                    Console.WriteLine("Analysing Byte " + byteNum);

                InitValue();
                Train();
            }

            Console.WriteLine("The results of the vote are as follows:");
            for(int i = 0; i < 32; i++)
            {
                Console.WriteLine(Environment.NewLine);

                for(int j = 0; j < 256; j++)
                {
                    Console.Write(votes[i, j] + ", ");
                }
            }

            Console.WriteLine("Press ENTER key to exit...");
            Console.ReadLine();
        }

        #endregion

        #region Public Address Prep

        public void Prep()
        {
            byte[] pap = BTCPrep.PrepareAddress(publicAddress);

            for (int j = 0; j < pap.Length; j++)
                publicAddressDouble[j] = pap[j] != 0 ? pap[j] / 256.0 : 0;
        }

        #endregion

        #region InitValue

        private void InitValue()
        {
            BTCCrackerDBContext btcDB = new BTCCrackerDBContext();

            if (byteNum == 0)
            {
                rowCountsPerByte[0] = btcDB.WeightLogs.Count(x => x.Byte0 >= prob);
                rowCountsPerByte[1] = btcDB.WeightLogs.Count(x => x.Byte1 >= prob);
                rowCountsPerByte[2] = btcDB.WeightLogs.Count(x => x.Byte2 >= prob);
                rowCountsPerByte[3] = btcDB.WeightLogs.Count(x => x.Byte3 >= prob);
                rowCountsPerByte[4] = btcDB.WeightLogs.Count(x => x.Byte4 >= prob);
                rowCountsPerByte[5] = btcDB.WeightLogs.Count(x => x.Byte5 >= prob);
                rowCountsPerByte[6] = btcDB.WeightLogs.Count(x => x.Byte6 >= prob);
                rowCountsPerByte[7] = btcDB.WeightLogs.Count(x => x.Byte7 >= prob);
                rowCountsPerByte[8] = btcDB.WeightLogs.Count(x => x.Byte8 >= prob);
                rowCountsPerByte[9] = btcDB.WeightLogs.Count(x => x.Byte9 >= prob);
                rowCountsPerByte[10] = btcDB.WeightLogs.Count(x => x.Byte10 >= prob);
                rowCountsPerByte[11] = btcDB.WeightLogs.Count(x => x.Byte11 >= prob);
                rowCountsPerByte[12] = btcDB.WeightLogs.Count(x => x.Byte12 >= prob);
                rowCountsPerByte[13] = btcDB.WeightLogs.Count(x => x.Byte13 >= prob);
                rowCountsPerByte[14] = btcDB.WeightLogs.Count(x => x.Byte14 >= prob);
                rowCountsPerByte[15] = btcDB.WeightLogs.Count(x => x.Byte15 >= prob);
                rowCountsPerByte[16] = btcDB.WeightLogs.Count(x => x.Byte16 >= prob);
                rowCountsPerByte[17] = btcDB.WeightLogs.Count(x => x.Byte17 >= prob);
                rowCountsPerByte[18] = btcDB.WeightLogs.Count(x => x.Byte18 >= prob);
                rowCountsPerByte[19] = btcDB.WeightLogs.Count(x => x.Byte19 >= prob);
                rowCountsPerByte[20] = btcDB.WeightLogs.Count(x => x.Byte20 >= prob);
                rowCountsPerByte[21] = btcDB.WeightLogs.Count(x => x.Byte21 >= prob);
                rowCountsPerByte[22] = btcDB.WeightLogs.Count(x => x.Byte22 >= prob);
                rowCountsPerByte[23] = btcDB.WeightLogs.Count(x => x.Byte23 >= prob);
                rowCountsPerByte[24] = btcDB.WeightLogs.Count(x => x.Byte24 >= prob);
                rowCountsPerByte[25] = btcDB.WeightLogs.Count(x => x.Byte25 >= prob);
                rowCountsPerByte[26] = btcDB.WeightLogs.Count(x => x.Byte26 >= prob);
                rowCountsPerByte[27] = btcDB.WeightLogs.Count(x => x.Byte27 >= prob);
                rowCountsPerByte[28] = btcDB.WeightLogs.Count(x => x.Byte28 >= prob);
                rowCountsPerByte[29] = btcDB.WeightLogs.Count(x => x.Byte29 >= prob);
                rowCountsPerByte[30] = btcDB.WeightLogs.Count(x => x.Byte30 >= prob);
                rowCountsPerByte[31] = btcDB.WeightLogs.Count(x => x.Byte31 >= prob);
            }

            //Get next weights from database
            switch(byteNum)
            {
                case 0:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[0])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte0 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 1:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[1])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte1 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 2:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[2])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte2 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 3:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[3])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte3 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 4:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[4])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte4 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 5:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[5])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte5 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 6:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[6])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte6 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 7:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[7])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte7 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 8:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[8])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte8 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 9:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[9])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte9 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 10:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[10])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte10 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 11:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[11])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte11 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 12:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[12])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte12 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 13:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[13])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte13 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 14:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[14])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte14 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 15:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[15])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte15 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 16:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[16])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte16 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 17:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[17])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte17 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 18:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[18])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte18 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 19:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[19])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte19 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 20:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[20])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte20 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 21:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[21])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte21 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 22:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[22])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte22 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 23:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[23])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte23 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 24:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[24])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte24 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 25:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[25])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte25 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 26:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[26])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte26 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 27:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[27])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte27 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 28:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[28])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte28 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 29:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[29])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte29 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 30:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[30])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte30 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
                case 31:
                    {
                        List<WeightLog> wl = new List<WeightLog>();

                        if (rowSkipCount < rowCountsPerByte[31])
                            wl = btcDB.WeightLogs.OrderByDescending(x => x.Byte31 >= prob).Skip(rowSkipCount++).Take(1).ToList();

                        DeserialiseWeights(wl[0]);
                        ParseStats(wl[0]);

                        break;
                    }
            }
        }

        #endregion

        #region Deserialise Bytes

        private void DeserialiseWeights(WeightLog wl)
        {
            analyserEntry.Layer1Weights = new List<double[]>();
            analyserEntry.Layer2Weights = new List<double[]>();
            analyserEntry.Layer3Weights = new List<double[]>();
            analyserEntry.Layer4Weights = new List<double[]>();

            //Split the entire string
            string[] w0 = wl.WeightsHL0.Split(';');

            //Layer 1
            for(int i = 0; i < 20; i++)
            {
                string[] w1 = w0[i].Split(':');
                 
                for(int j = 0; j < w1.Length - 1; j++)
                {
                    string[] w2 = w1[j].Split(',');
                    double[] weights = new double[w2.Length];

                    for (int k = 0; k < w2.Length; k++)
                    {
                        weights[k] = double.Parse(w2[k]);
                    }

                    analyserEntry.Layer1Weights.Add(weights);
                }
            }

            w0 = wl.WeightsHL1.Split(';');

            //Layer 2
            for (int i = 0; i < 64; i++)
            {
                string[] w1 = w0[i].Split(':');

                for (int j = 0; j < w1.Length - 1; j++)
                {
                    string[] w2 = w1[j].Split(',');
                    double[] weights = new double[w2.Length];

                    for (int k = 0; k < w2.Length; k++)
                    {
                        weights[k] = double.Parse(w2[k]);
                    }

                    analyserEntry.Layer2Weights.Add(weights);
                }
            }

            w0 = wl.WeightsHL2.Split(';');

            //Layer 3
            for (int i = 0; i < 128; i++)
            {
                string[] w1 = w0[i].Split(':');

                for (int j = 0; j < w1.Length - 1; j++)
                {
                    string[] w2 = w1[j].Split(',');
                    double[] weights = new double[w2.Length];

                    for (int k = 0; k < w2.Length; k++)
                    {
                        weights[k] = double.Parse(w2[k]);
                    }

                    analyserEntry.Layer3Weights.Add(weights);
                }
            }

            w0 = wl.WeightsOL.Split(';');

            //Layer 4
            for (int i = 0; i < 256; i++)
            {
                string[] w1 = w0[i].Split(':');

                for (int j = 0; j < w1.Length - 1; j++)
                {
                    string[] w2 = w1[j].Split(',');
                    double[] weights = new double[w2.Length];

                    for (int k = 0; k < w2.Length; k++)
                    {
                        weights[k] = double.Parse(w2[k]);
                    }

                    analyserEntry.Layer4Weights.Add(weights);
                }
            }
        }

        #endregion

        #region ParseStats

        private void ParseStats(WeightLog wl)
        {
            analyserEntry.Byte0 = wl.Byte0;
            analyserEntry.Byte1 = wl.Byte1;
            analyserEntry.Byte2 = wl.Byte2;
            analyserEntry.Byte3 = wl.Byte3;
            analyserEntry.Byte4 = wl.Byte4;
            analyserEntry.Byte5 = wl.Byte5;
            analyserEntry.Byte6 = wl.Byte6;
            analyserEntry.Byte7 = wl.Byte7;
            analyserEntry.Byte8 = wl.Byte8;
            analyserEntry.Byte9 = wl.Byte9;
            analyserEntry.Byte10 = wl.Byte10;
            analyserEntry.Byte11 = wl.Byte11;
            analyserEntry.Byte12 = wl.Byte12;
            analyserEntry.Byte13 = wl.Byte13;
            analyserEntry.Byte14 = wl.Byte14;
            analyserEntry.Byte15 = wl.Byte15;
            analyserEntry.Byte16 = wl.Byte16;
            analyserEntry.Byte17 = wl.Byte17;
            analyserEntry.Byte18 = wl.Byte18;
            analyserEntry.Byte19 = wl.Byte19;
            analyserEntry.Byte20 = wl.Byte20;
            analyserEntry.Byte21 = wl.Byte21;
            analyserEntry.Byte22 = wl.Byte22;
            analyserEntry.Byte23 = wl.Byte23;
            analyserEntry.Byte24 = wl.Byte24;
            analyserEntry.Byte25 = wl.Byte25;
            analyserEntry.Byte26 = wl.Byte26;
            analyserEntry.Byte27 = wl.Byte27;
            analyserEntry.Byte28 = wl.Byte28;
            analyserEntry.Byte29 = wl.Byte29;
            analyserEntry.Byte30 = wl.Byte30;
            analyserEntry.Byte31 = wl.Byte31;
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
                    Bias = -1.0,
                    Weights = analyserEntry.Layer1Weights[i]
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
                    Weights = analyserEntry.Layer2Weights[i]
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
                    Weights = analyserEntry.Layer3Weights[i]
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
                    Weights = analyserEntry.Layer4Weights[i]
                };
                neuralNetwork.Add(nn);
            }
        }

        #endregion

        #region Train

        private void Train()
        {
            //Layer 0
            List<NeuralNetwork> hiddenLayer1 = neuralNetwork.FindAll(x => x.LayerNumber == 0);
            double[] weightedSum1 = new double[20];
            for (int j = 0; j < hiddenLayer1.Count; j++)
                weightedSum1[j] = perceptron.Execute(analyserEntry.Layer1Weights[j], publicAddressDouble, hiddenLayer1[j].Bias);

            for (int k = 0; k < weightedSum1.Length; k++)
                weightedSum1[k] = activationFunctions.LeakyReLU(weightedSum1[k]);

            //Layer 1
            List<NeuralNetwork> hiddenLayer2 = neuralNetwork.FindAll(x => x.LayerNumber == 1);
            double[] weightedSum2 = new double[64];
            for (int j = 0; j < hiddenLayer2.Count; j++)
                weightedSum2[j] = perceptron.Execute(analyserEntry.Layer2Weights[j], weightedSum1, hiddenLayer2[j].Bias);

            for (int k = 0; k < weightedSum2.Length; k++)
                weightedSum2[k] = activationFunctions.LeakyReLU(weightedSum2[k]);

            //Layer 2
            List<NeuralNetwork> hiddenLayer3 = neuralNetwork.FindAll(x => x.LayerNumber == 2);
            double[] weightedSum3 = new double[128];
            for (int j = 0; j < hiddenLayer3.Count; j++)
                weightedSum3[j] = perceptron.Execute(analyserEntry.Layer3Weights[j], weightedSum2, hiddenLayer3[j].Bias);

            for (int k = 0; k < weightedSum3.Length; k++)
                weightedSum3[k] = activationFunctions.LeakyReLU(weightedSum3[k]);

            //Output Layer
            List<NeuralNetwork> outputLayer = neuralNetwork.FindAll(x => x.LayerNumber == 3);
            double[] weightedSum4 = new double[256];
            for (int j = 0; j < outputLayer.Count; j++)
                weightedSum4[j] = perceptron.Execute(analyserEntry.Layer4Weights[j], weightedSum3, outputLayer[j].Bias);

            for (int k = 0; k < weightedSum4.Length; k++)
                weightedSum4[k] = activationFunctions.BinaryStep(weightedSum4[k]);

            Assess(weightedSum4);
        }

        #endregion

        #region Assess

        private void Assess(double[] outputLayer)
        {
            outputLayer = ConvertFromBinaryToDouble(outputLayer);
            
            switch (byteNum)
            {
                case 0:
                    {
                        votes[0, (int)outputLayer[0]] += (double)((double)analyserEntry.Byte0 / (double)10000);  //weighted vote  
                        break;
                    }
                case 1:
                    {
                        votes[1, (int)outputLayer[1]] += (double)((double)analyserEntry.Byte1 / (double)10000);  //weighted vote  
                        break;
                    }
                case 2:
                    {
                        votes[2, (int)outputLayer[2]] += (double)((double)analyserEntry.Byte2 / (double)10000);  //weighted vote  
                        break;
                    }
                case 3:
                    {
                        votes[3, (int)outputLayer[3]] += (double)((double)analyserEntry.Byte3 / (double)10000);  //weighted vote  
                        break;
                    }
                case 4:
                    {
                        votes[4, (int)outputLayer[4]] += (double)((double)analyserEntry.Byte4 / (double)10000);  //weighted vote  
                        break;
                    }
                case 5:
                    {
                        votes[5, (int)outputLayer[5]] += (double)((double)analyserEntry.Byte5 / (double)10000);  //weighted vote  
                        break;
                    }
                case 6:
                    {
                        votes[6, (int)outputLayer[6]] += (double)((double)analyserEntry.Byte6 / (double)10000);  //weighted vote  
                        break;
                    }
                case 7:
                    {
                        votes[7, (int)outputLayer[7]] += (double)((double)analyserEntry.Byte7 / (double)10000);  //weighted vote  
                        break;
                    }
                case 8:
                    {
                        votes[8, (int)outputLayer[8]] += (double)((double)analyserEntry.Byte8 / (double)10000);  //weighted vote  
                        break;
                    }
                case 9:
                    {
                        votes[9, (int)outputLayer[9]] += (double)((double)analyserEntry.Byte9 / (double)10000);  //weighted vote  
                        break;
                    }
                case 10:
                    {
                        votes[10, (int)outputLayer[10]] += (double)((double)analyserEntry.Byte10 / (double)10000);  //weighted vote  
                        break;
                    }
                case 11:
                    {
                        votes[11, (int)outputLayer[11]] += (double)((double)analyserEntry.Byte11 / (double)10000);  //weighted vote  
                        break;
                    }
                case 12:
                    {
                        votes[12, (int)outputLayer[12]] += (double)((double)analyserEntry.Byte12 / (double)10000);  //weighted vote  
                        break;
                    }
                case 13:
                    {
                        votes[13, (int)outputLayer[13]] += (double)((double)analyserEntry.Byte13 / (double)10000);  //weighted vote  
                        break;
                    }
                case 14:
                    {
                        votes[14, (int)outputLayer[14]] += (double)((double)analyserEntry.Byte14 / (double)10000);  //weighted vote  
                        break;
                    }
                case 15:
                    {
                        votes[15, (int)outputLayer[15]] += (double)((double)analyserEntry.Byte15 / (double)10000);  //weighted vote  
                        break;
                    }
                case 16:
                    {
                        votes[16, (int)outputLayer[16]] += (double)((double)analyserEntry.Byte16 / (double)10000);  //weighted vote  
                        break;
                    }
                case 17:
                    {
                        votes[17, (int)outputLayer[17]] += (double)((double)analyserEntry.Byte17 / (double)10000);  //weighted vote  
                        break;
                    }
                case 18:
                    {
                        votes[18, (int)outputLayer[18]] += (double)((double)analyserEntry.Byte18 / (double)10000);  //weighted vote  
                        break;
                    }
                case 19:
                    {
                        votes[19, (int)outputLayer[19]] += (double)((double)analyserEntry.Byte19 / (double)10000);  //weighted vote  
                        break;
                    }
                case 20:
                    {
                        votes[20, (int)outputLayer[20]] += (double)((double)analyserEntry.Byte20 / (double)10000);  //weighted vote  
                        break;
                    }
                case 21:
                    {
                        votes[21, (int)outputLayer[21]] += (double)((double)analyserEntry.Byte21 / (double)10000);  //weighted vote  
                        break;
                    }
                case 22:
                    {
                        votes[22, (int)outputLayer[22]] += (double)((double)analyserEntry.Byte22 / (double)10000);  //weighted vote  
                        break;
                    }
                case 23:
                    {
                        votes[23, (int)outputLayer[23]] += (double)((double)analyserEntry.Byte23 / (double)10000);  //weighted vote  
                        break;
                    }
                case 24:
                    {
                        votes[24, (int)outputLayer[24]] += (double)((double)analyserEntry.Byte24 / (double)10000);  //weighted vote  
                        break;
                    }
                case 25:
                    {
                        votes[25, (int)outputLayer[25]] += (double)((double)analyserEntry.Byte25 / (double)10000);  //weighted vote  
                        break;
                    }
                case 26:
                    {
                        votes[26, (int)outputLayer[26]] += (double)((double)analyserEntry.Byte26 / (double)10000);  //weighted vote  
                        break;
                    }
                case 27:
                    {
                        votes[27, (int)outputLayer[27]] += (double)((double)analyserEntry.Byte27 / (double)10000);  //weighted vote  
                        break;
                    }
                case 28:
                    {
                        votes[28, (int)outputLayer[28]] += (double)((double)analyserEntry.Byte28 / (double)10000);  //weighted vote  
                        break;
                    }
                case 29:
                    {
                        votes[29, (int)outputLayer[29]] += (double)((double)analyserEntry.Byte29 / (double)10000);  //weighted vote  
                        break;
                    }
                case 30:
                    {
                        votes[30, (int)outputLayer[30]] += (double)((double)analyserEntry.Byte30 / (double)10000);  //weighted vote  
                        break;
                    }
                case 31:
                    {
                        votes[31, (int)outputLayer[31]] += (double)((double)analyserEntry.Byte31 / (double)10000);  //weighted vote  
                        break;
                    }
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
    }
}
