using RDFSharp.Model;
using RDFSharp.Query;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace RDFSharp.StressTest
{
    public class QueryTesting
    {
        private static RDFGraph Graph;

        public void Run()
        {
            Console.WriteLine("Started query testing");
            init();
            initialTime();
            stress();
            allActorQuery();
            rembrandtQuery();
            giovanniQuery();
            Console.WriteLine("Finished query testing");
        }

        public void init()
        {
            Graph = new RDFGraph();
            Graph.SetContext(new Uri("http://dbpedia.org/ontology/"));
        }

        public void initialTime()
        {

            var actor = new RDFVariable("?actor");

            var selectQuery =
                new RDFSelectQuery().AddPatternGroup(
                    new RDFPatternGroup("pg1").AddPattern(
                        new RDFPattern(
                            actor,
                            RDFVocabulary.RDF.TYPE,
                            new RDFPlainLiteral("ecrm:E39_Actor")
                        )
                    )
                );

            var queryResult = selectQuery.ApplyToGraph(Graph);

        }
        public void allActorQuery()
        {
            Console.WriteLine("--------------------------------------\n");
            Console.WriteLine("All Actor Query started\n");
            DateTime start;
            DateTime end;
            Process myProcess = Process.GetCurrentProcess();

            DateTime startprocess = DateTime.Now;

            var actor = new RDFVariable("?actor");

            var selectQuery =
                new RDFSelectQuery().AddPatternGroup(
                    new RDFPatternGroup("pg1").AddPattern(
                        new RDFPattern(
                            actor,
                            RDFVocabulary.RDF.TYPE,
                            new RDFPlainLiteral("ecrm:E39_Actor")
                        )
                    )
                )
                .AddProjectionVariable(actor);


            start = DateTime.Now;
            var queryResult = selectQuery.ApplyToGraph(Graph);
            end = DateTime.Now;
            DateTime endprocess = DateTime.Now;

            long peakWorkingSet = myProcess.PeakWorkingSet64;

            Console.WriteLine($"Total query time                : {(end - start).TotalMilliseconds} ms");
            Console.WriteLine($"Total process time              : {(endprocess - startprocess).TotalMilliseconds} ms");
            Console.WriteLine($"Total physical memory usage     : {myProcess.WorkingSet64} byte");
            Console.WriteLine($"Peak physical memory usage      : {peakWorkingSet} byte \n");
            Console.WriteLine("All Actor Query ended\n");
            Console.WriteLine("--------------------------------------\n");
        }

        public void rembrandtQuery()
        {
            Console.WriteLine("--------------------------------------\n");
            Console.WriteLine("Rembrandt Query started\n");
            DateTime start;
            DateTime end;
            Process myProcess = Process.GetCurrentProcess();

            DateTime startprocess = DateTime.Now;

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
            DateTime endprocess = DateTime.Now;

            long peakWorkingSet = myProcess.PeakWorkingSet64;

            Console.WriteLine($"Total query time                : {(end-start).TotalMilliseconds} ms");
            Console.WriteLine($"Total process time              : {(endprocess - startprocess).TotalMilliseconds} ms");
            Console.WriteLine($"Total physical memory usage     : {myProcess.WorkingSet64} byte");
            Console.WriteLine($"Peak physical memory usage      : {peakWorkingSet} byte \n");
            Console.WriteLine("Rembrandt Query ended\n");
            Console.WriteLine("--------------------------------------\n");
        }

        public void giovanniQuery()
        {
            Console.WriteLine("--------------------------------------\n");
            Console.WriteLine("Giovanni Actor Query started\n");
            DateTime start;
            DateTime end;
            Process myProcess = Process.GetCurrentProcess();

            DateTime startprocess = DateTime.Now;

            var actor = new RDFVariable("?actor");
            var name = new RDFVariable("?name");

            var selectQuery =
                 new RDFSelectQuery().AddPatternGroup(
                     new RDFPatternGroup("pg1").AddPattern(
                         new RDFPattern(
                             actor,
                             RDFVocabulary.RDF.TYPE,
                             new RDFPlainLiteral("ecrm:E39_Actor")
                         )
                     )
                     .AddPattern(
                         new RDFPattern(
                             actor,
                             RDFVocabulary.RDFS.LABEL,
                             name
                         )
                     )
                     .AddFilter(
                         new RDFRegexFilter(
                             name,
                             new Regex(@"Giovanni", RegexOptions.IgnoreCase)
                         )
                     )
                 )
                 .AddProjectionVariable(actor);

            start = DateTime.Now;
            var queryResult = selectQuery.ApplyToGraph(Graph);
            end = DateTime.Now;
            DateTime endprocess = DateTime.Now;

            long peakWorkingSet = myProcess.PeakWorkingSet64;

            Console.WriteLine($"Total query time                : {(end - start).TotalMilliseconds} ms");
            Console.WriteLine($"Total process time              : {(endprocess - startprocess).TotalMilliseconds} ms");
            Console.WriteLine($"Total physical memory usage     : {myProcess.WorkingSet64} byte");
            Console.WriteLine($"Peak physical memory usage      : {peakWorkingSet} byte \n");
            Console.WriteLine("Giovanni Actor Query ended\n");
            Console.WriteLine("--------------------------------------\n");
        }

        public void stress()
        {
            Console.WriteLine("--------------------------------------\n");
            Console.WriteLine("Stress test started\n");
            Process myProcess = Process.GetCurrentProcess();
            DateTime startprocess = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                var actor = new RDFVariable("?actor");

                var selectQuery =
                    new RDFSelectQuery().AddPatternGroup(
                        new RDFPatternGroup("pg1").AddPattern(
                            new RDFPattern(
                                actor,
                                RDFVocabulary.RDF.TYPE,
                                new RDFPlainLiteral("ecrm:E39_Actor")
                            )
                        )
                    )
                    .AddProjectionVariable(actor);
            }
            DateTime endprocess = DateTime.Now;

            long peakWorkingSet = myProcess.PeakWorkingSet64;

            Console.WriteLine($"Total process time              : {(endprocess - startprocess).TotalMilliseconds} ms");
            Console.WriteLine($"Total physical memory usage     : {myProcess.WorkingSet64} byte");
            Console.WriteLine($"Peak physical memory usage      : {peakWorkingSet} byte \n");
            Console.WriteLine("Stress test ended\n");
            Console.WriteLine("--------------------------------------\n");
        }
    }
}
