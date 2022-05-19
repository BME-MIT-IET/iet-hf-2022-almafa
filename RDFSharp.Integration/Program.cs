using RDFSharp.Model;
using RDFSharp.Query;

/*

SELECT ?description
WHERE
{
    #PG1
    {
        ?actor rdfs:label "Rembrandt"@en .
        ?actor dbo:abstract ?description
        FILTER ( LANG ( ?description ) = 'en' )
    }
}
LIMIT 1

*/

// CREATE SELECT QUERY
// Create variables
var actor = new RDFVariable("?actor");
var description = new RDFVariable("?description");
// Compose query
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

// CREATE SPARQL ENDPOINTS
var dbPediaEndpnt = new RDFSPARQLEndpoint(new Uri("https://dbpedia.org/sparql"));

// APPLY SELECT QUERY TO SPARQL ENDPOINT
var selectQueryResult = await selectQuery.ApplyToSPARQLEndpointAsync(dbPediaEndpnt);

var about = selectQueryResult.SelectResults.Rows[0].ItemArray[0]?.ToString();

Console.WriteLine(about);
