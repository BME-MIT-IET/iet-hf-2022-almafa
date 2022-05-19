using RDFSharp.Model;
using System.Diagnostics;
using RDFSharp.Query;

namespace RDFSharp.StressTest
{
    public class TestModels
    {
        private static RDFGraph Graph;

        public void Run()
        {
            Console.WriteLine("Started testing loading graphs.");
            allActorQuery();
            rembrandtQuery();
            Console.WriteLine("Finished testing loading graphs.");
        }

        public void allActorQuery()
        {
            Console.WriteLine("All Actor Query started\n");
            DateTime start;
            DateTime end;
            TimeSpan total_time = TimeSpan.Zero;
            Console.WriteLine(total_time);

            Process myProcess = Process.GetCurrentProcess();
            Console.WriteLine($"Start processor time            : {myProcess.TotalProcessorTime}");
            Console.WriteLine($"Start physical memory usage     : {myProcess.WorkingSet64}");

            var actor = new RDFVariable("?actor");

            var selectQuery =
                new RDFSelectQuery().AddPatternGroup(
                    new RDFPatternGroup("pg1").AddPattern(
                        new RDFPattern(
                            actor,
                            RDFVocabulary.RDFS.DATATYPE,
                            new RDFPlainLiteral("ecrm:E39_Actor")
                        )
                    )
                );

            start = DateTime.Now;
            var queryResult = selectQuery.ApplyToGraph(Graph);
            end = DateTime.Now;

            total_time = (end - start);

            Console.WriteLine($"Total loading time ms           : {total_time.TotalMilliseconds} ms");
            Console.WriteLine($"End processor time              : {myProcess.TotalProcessorTime}");
            Console.WriteLine($"End physical memory usage       : {myProcess.WorkingSet64} \n");
            Console.WriteLine("All Actor Query ended\n");
        }

        public void rembrandtQuery()
        {
            Console.WriteLine("RembrandtQuery started\n");
            DateTime start;
            DateTime end;
            TimeSpan total_time = TimeSpan.Zero;
            Console.WriteLine(total_time);

            Process myProcess = Process.GetCurrentProcess();
            Console.WriteLine($"Start processor time            : {myProcess.TotalProcessorTime}");
            Console.WriteLine($"Start physical memory usage     : {myProcess.WorkingSet64}");

            var actor = new RDFVariable("?actor");
            var description = new RDFVariable("?description");

            var selectQuery =
                new RDFSelectQuery().AddPatternGroup(
                    new RDFPatternGroup("pg1").AddPattern(
                        new RDFPattern(
                            actor,
                            RDFVocabulary.RDFS.LABEL,
                            new RDFPlainLiteral("Rembrandt", "en")
                        )
                    )
                    .AddPattern(
                        new RDFPattern(
                            actor,
                            new RDFResource(string.Concat("http://dbpedia.org/ontology/", "abstract")),
                            description
                        )
                    )
                    .AddFilter(
                        new RDFLangMatchesFilter(description, "en")
                    )
                )
                .AddModifier(new RDFLimitModifier(1))
                .AddProjectionVariable(description);

            
            start = DateTime.Now;
            var queryResult = selectQuery.ApplyToGraph(Graph);
            end = DateTime.Now;

            total_time = (end - start);

            Console.WriteLine($"Total loading time ms           : {total_time.TotalMilliseconds} ms");
            Console.WriteLine($"End processor time              : {myProcess.TotalProcessorTime}");
            Console.WriteLine($"End physical memory usage       : {myProcess.WorkingSet64} \n");
            Console.WriteLine("RembrandtQuery ended\n");
        }
    }
}
