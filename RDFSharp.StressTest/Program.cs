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

            QueryTesting test  = new QueryTesting();

            test.Run();
        }
    }
}