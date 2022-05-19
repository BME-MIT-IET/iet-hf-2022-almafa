using RDFSharp.Model;
using RDFSharp.Query;
using RDFSharp.StressTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSharp.ManualTest
{
    public class Program
    {

        static void Main(string[] args)
        {
            TestModels asd = new TestModels();

            asd.Run();

            Console.WriteLine("All tests are finished.");
        }
    }
}