using BTCECDSACracker.Engines;
using BTCECDSACracker.Analysers;
using System;

namespace BTCCracker
{
    class Program
    {
        static void Main(string[] args)
        {
            //EngineH a = new EngineH();
            //a.Execute();

            AnalyserA aa = new AnalyserA();
            aa.Execute();
        }
    }
}
