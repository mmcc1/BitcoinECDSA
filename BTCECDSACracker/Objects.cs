using System;
using System.Collections.Generic;
using System.Text;

namespace BTCECDSACracker
{
    #region Objects

    public class DataSet
    {
        public double[] PublicAddressDouble { get; set; }
        public string PublicAddress { get; set; }
        public byte[] PrivateKey { get; set; }
        public byte[] CrackedPrivateKey { get; set; }
    }

    public class WeightStore
    {
        public double[] Statistics { get; set; }
    }

    public class NeuralNetwork
    {
        public int LayerNumber { get; set; }
        public int NetworkNumber { get; set; }
        public double[] Weights { get; set; }
        public double Bias { get; set; }
    }

    #endregion
}
