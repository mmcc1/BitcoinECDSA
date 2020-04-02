
using BTCECDSAAnalyser;
using System;

namespace BTCECDSAATest
{
    class Program
    {
        static void Main(string[] args)
        {
            EngineC eng = new EngineC();  //Change this if you derive a new engine
            eng.Execute();

            Console.ReadLine();
        }
    }
}
