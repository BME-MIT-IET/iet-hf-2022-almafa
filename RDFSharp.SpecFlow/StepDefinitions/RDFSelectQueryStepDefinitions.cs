using RDFSharp.Model;
using RDFSharp.Query;
using System;
using TechTalk.SpecFlow;

namespace RDFSharp.SpecFlow.StepDefinitions
{
    [Binding]
    public class RDFSelectQueryStepDefinitions
    {
        private Dictionary<string, RDFPatternMember> patternMembers = new Dictionary<string, RDFPatternMember>();
        private Dictionary<string, RDFSelectQuery> selectQueries = new Dictionary<string, RDFSelectQuery>();
        private Dictionary<string, RDFSPARQLEndpoint> SPARQLEndpoints = new Dictionary<string, RDFSPARQLEndpoint>();
        private Dictionary<string, RDFPatternGroup> patternGroups = new Dictionary<string, RDFPatternGroup>();

        private RDFSelectQueryResult selectQueryResult = null!;

        [Given(@"a Variable named (.*)")]
        public void GivenAVariable(string variableName)
        {
            var variable = new RDFVariable(variableName);
            patternMembers[variableName] = variable;
        }

        [Given(@"a Resource named (.*) with ""([^""]*)"" uri")]
        public void GivenAResource(string resourceName, string uri)
        {
            var resource = new RDFResource(uri);
            patternMembers[resourceName] = resource;
        }

        [Given(@"a PlainLiteral called (.*) with language (.*)")]
        public void GivenAPlainLiteral(string value, string language)
        {
            var plainLiteral = new RDFPlainLiteral(value, language);
            patternMembers[value] = plainLiteral;
        }

        [Given(@"a SelectQuery named (.*)")]
        public void GivenASelectQuery(string queryName)
        {
            var selectQuery = new RDFSelectQuery();
            selectQueries[queryName] = selectQuery;
        }

        [Given(@"a SPARQLEndpoint named (.*) with ""([^""]*)"" uri")]
        public void GivenASPARQLEndpoint(string endpointName, string uri)
        {
            var SPARQLEndpoint = new RDFSPARQLEndpoint(new Uri(uri));
            SPARQLEndpoints[endpointName] = SPARQLEndpoint;
        }

        [Given(@"a PatternGroup named (.*) is added to (.*)")]
        public void GivenAPatternGroupAddedToSelectQuery(string patternGroupName, string selectQueryName)
        {
            var patternGroup = new RDFPatternGroup(patternGroupName);
            patternGroups[patternGroupName] = patternGroup;
            selectQueries[selectQueryName].AddPatternGroup(patternGroup);
        }

        [Given(@"a Pattern with (.*) and (.*) and (.*) is added to (.*)")]
        public void GivenAPatternAddedToAPatternGroup(string subjectName, string predicateName, string objectName, string patternGroupName)
        {
            var subject = patternMembers[subjectName];
            var predicate = patternMembers[predicateName];
            var objLit = patternMembers[objectName];
            var pattern = new RDFPattern(subject, predicate, objLit);
            patternGroups[patternGroupName].AddPattern(pattern);
        }

        [Given(@"a filter for (.*) to keep only results with (.*) in (.*)")]
        public void GivenAFilterForPatternGroup(string patternGroupName, string variableName, string language)
        {
            var variable = patternMembers[variableName] as RDFVariable;
            var filter = new RDFLangMatchesFilter(variable, language);
            patternGroups[patternGroupName].AddFilter(filter);
        }

        [Given(@"the number of results for (.*) is limited to (.*)")]
        public void GivenTheNumberOfResultsIsLimitedTo(string selectQueryName, int limit)
        {
            selectQueries[selectQueryName].AddModifier(new RDFLimitModifier(limit));
        }

        [Given(@"we are interested in the (.*) in the (.*)")]
        public void GivenWeAreInterestedInTheDescription(string variableName, string selectQueryName)
        {
            var variable = patternMembers[variableName] as RDFVariable;
            selectQueries[selectQueryName].AddProjectionVariable(variable);
        }

        [When(@"we apply the (.*) to the (.*)")]
        public void WhenWeApplyTheSelectQuery(string selectQueryName, string SPARQLEndpointName)
        {
            var selectQuery = selectQueries[selectQueryName];
            var SPARQLEndpoint = SPARQLEndpoints[SPARQLEndpointName];
            selectQueryResult = selectQuery.ApplyToSPARQLEndpoint(SPARQLEndpoint);
        }

        [Then(@"the result should start with ""([^""]*)""")]
        public void ThenTheResultShouldStartWith(string firstPart)
        {
            var result = selectQueryResult.SelectResults.Rows[0].ItemArray[0]?.ToString();
            result.Should().StartWith(firstPart);
        }
    }
}
