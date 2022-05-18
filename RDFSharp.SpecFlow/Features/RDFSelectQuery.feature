Feature: RDFSelectQuery

 Background:
	Given a Variable named actor
	And a Resource named label with "http://www.w3.org/2000/01/rdf-schema#label" uri
	And a PlainLiteral called Rembrandt with language en
	And a Resource named abstract with "http://dbpedia.org/ontology/abstract" uri 
	And a Variable named description
    And a SelectQuery named selectQuery
	And a SPARQLEndpoint named dbPediaEndpoint with "https://dbpedia.org/sparql" uri

Scenario: Something useful
    And a PatternGroup named pg1 is added to selectQuery
	And a Pattern with actor and label and Rembrandt is added to pg1 
	And a Pattern with actor and abstract and description is added to pg1
	And a filter for pg1 to keep only results with description in en
	And the number of results for selectQuery is limited to 1
	And we are interested in the description in the selectQuery
	When we apply the selectQuery to the dbPediaEndpoint
	Then the result should start with "Rembrandt Harmenszoon van Rijn"
