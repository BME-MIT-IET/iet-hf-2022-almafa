using RDFSharp.Model;
using RDFSharp.Query;

// CREATE SELECT QUERY
var selectQuery =
    new RDFSelectQuery().AddPatternGroup(
        new RDFPatternGroup("pg1").AddPattern(
            new RDFPattern(
                new RDFVariable("?actor"),
                RDFVocabulary.RDFS.LABEL,
                new RDFPlainLiteral("Rembrandt", "en")
            )
        )
    );

// CREATE SPARQL ENDPOINTS
var dbPediaEndpnt = new RDFSPARQLEndpoint(new Uri("https://dbpedia.org/sparql"));

// APPLY SELECT QUERY TO SPARQL ENDPOINT
var selectQueryResult = await selectQuery.ApplyToSPARQLEndpointAsync(dbPediaEndpnt);

// SHOW RESULTS
selectQueryResult.ToSparqlXmlResult(Console.OpenStandardOutput());
