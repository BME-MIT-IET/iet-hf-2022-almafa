/*
   Copyright 2012-2022 Marco De Salvo

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using RDFSharp.Model;
using RDFSharp.Query;

namespace RDFSharp.Test.Query
{
    [TestClass]
    public class RDFAskQueryTests
    {
        private WireMockServer server;

        [TestInitialize]
        public void Initialize() { server = WireMockServer.Start(); }

        [TestCleanup]
        public void Cleanup()  { server.Stop(); server.Dispose(); }
        
        #region Tests
        [TestMethod]
        public void ShouldCreateAskQuery()
        {
            RDFAskQuery query = new RDFAskQuery();

            Assert.IsNotNull(query);
            Assert.IsNotNull(query.QueryMembers);
            Assert.IsTrue(query.QueryMembers.Count == 0);
            Assert.IsNotNull(query.Prefixes);
            Assert.IsTrue(query.Prefixes.Count == 0);
            Assert.IsTrue(query.IsEvaluable);
            Assert.IsFalse(query.IsOptional);
            Assert.IsFalse(query.JoinAsUnion);
            Assert.IsFalse(query.IsSubQuery);
            Assert.IsTrue(query.ToString().Equals("ASK" + Environment.NewLine + "WHERE {" + Environment.NewLine + "}"));
            Assert.IsTrue(query.QueryMemberID.Equals(RDFModelUtilities.CreateHash(query.QueryMemberStringID)));
            Assert.IsTrue(query.GetEvaluableQueryMembers().Count() == 0);
            Assert.IsTrue(query.GetPatternGroups().Count() == 0);
            Assert.IsTrue(query.GetSubQueries().Count() == 0);
            Assert.IsTrue(query.GetValues().Count() == 0);
            Assert.IsTrue(query.GetModifiers().Count() == 0);
            Assert.IsTrue(query.GetPrefixes().Count() == 0);
        }

        [TestMethod]
        public void ShouldCreateAskQueryWithQueryMembers()
        {
            RDFAskQuery query = new RDFAskQuery();
            query.AddPrefix(RDFNamespaceRegister.GetByPrefix("rdf"));
            query.AddPatternGroup(
                new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS))
                    .AddFilter(new RDFIsUriFilter(new RDFVariable("?S"))));
            query.AddSubQuery(
                new RDFSelectQuery()
                    .AddPrefix(RDFNamespaceRegister.GetByPrefix("owl"))
                    .AddPatternGroup(
                        new RDFPatternGroup("PG1")
                            .AddPattern(new RDFPattern(new RDFVariable("?S"), new RDFVariable("?P"), RDFVocabulary.OWL.CLASS))
                            .AddValues(new RDFValues().AddColumn(new RDFVariable("?S"), new List<RDFPatternMember>() { RDFVocabulary.RDFS.CLASS })))
                    .AddProjectionVariable(new RDFVariable("?S")));

            Assert.IsTrue(query.ToString().Equals("PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>"+Environment.NewLine+"PREFIX owl: <http://www.w3.org/2002/07/owl#>"+Environment.NewLine+Environment.NewLine+"ASK"+Environment.NewLine+"WHERE {"+Environment.NewLine+"  {"+Environment.NewLine+"    ?S rdf:type <http://www.w3.org/2000/01/rdf-schema#Class> ."+Environment.NewLine+"    FILTER ( ISURI(?S) ) "+Environment.NewLine+"  }"+Environment.NewLine+"  {"+Environment.NewLine+"    SELECT ?S"+Environment.NewLine+"    WHERE {"+Environment.NewLine+"      {"+Environment.NewLine+"        ?S ?P owl:Class ."+Environment.NewLine+"        VALUES ?S { <http://www.w3.org/2000/01/rdf-schema#Class> } ."+Environment.NewLine+"      }"+Environment.NewLine+"    }"+Environment.NewLine+"  }"+Environment.NewLine+"}"));
            Assert.IsTrue(query.QueryMemberID.Equals(RDFModelUtilities.CreateHash(query.QueryMemberStringID)));
            Assert.IsTrue(query.GetEvaluableQueryMembers().Count() == 2); //SPARQL Values is managed by Mirella
            Assert.IsTrue(query.GetPatternGroups().Count() == 1);
            Assert.IsTrue(query.GetSubQueries().Count() == 1);
            Assert.IsTrue(query.GetValues().Count() == 1);
            Assert.IsTrue(query.GetModifiers().Count() == 0); //ASK query doesn't have modifiers
            Assert.IsTrue(query.GetPrefixes().Count() == 2);
        }

        [TestMethod]
        public void ShouldApplyAskQueryToGraphAndHaveTrueResult()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:flower"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS));
            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFAskQueryResult result = query.ApplyToGraph(graph);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.AskResult);
        }

        [TestMethod]
        public void ShouldApplyAskQueryToGraphAndHaveFalseResult()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:flower"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE));
            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFAskQueryResult result = query.ApplyToGraph(graph);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }

        [TestMethod]
        public void ShouldApplyAskQueryToNullGraphAndHaveFalseResult()
        {
            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFAskQueryResult result = query.ApplyToGraph(null);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }

        [TestMethod]
        public async Task ShouldApplyAskQueryToGraphAsyncAndHaveTrueResult()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:flower"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS));
            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFAskQueryResult result = await query.ApplyToGraphAsync(graph);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.AskResult);
        }

        [TestMethod]
        public async Task ShouldApplyAskQueryToGraphAsyncAndHaveFalseResult()
        {
            RDFGraph graph = new RDFGraph();
            graph.AddTriple(new RDFTriple(new RDFResource("ex:flower"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.DATATYPE));
            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFAskQueryResult result = await query.ApplyToGraphAsync(graph);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }

        [TestMethod]
        public async Task ShouldApplyAskQueryToNullGraphAsyncAndHaveFalseResult()
        {
            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFAskQueryResult result = await query.ApplyToGraphAsync(null);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }

        [TestMethod]
        public void ShouldApplyAskQueryToSPARQLEndpointAndHaveTrueResult()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointAndHaveTrueResult/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>true</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithHeader("Content-Type", "application/sparql-results+xml")
                            .WithStatusCode(HttpStatusCode.OK));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointAndHaveTrueResult/sparql"));
            RDFAskQueryResult result = query.ApplyToSPARQLEndpoint(endpoint);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.AskResult);
        }

        [TestMethod]
        public void ShouldApplyAskQueryToSPARQLEndpointAndHaveFalseResult()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointAndHaveFalseResult/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>false</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithHeader("Content-Type", "application/sparql-results+xml")
                            .WithStatusCode(HttpStatusCode.OK));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointAndHaveFalseResult/sparql"));
            RDFAskQueryResult result = query.ApplyToSPARQLEndpoint(endpoint);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }

        [TestMethod]
        public async Task ShouldApplyAskQueryToSPARQLEndpointAsyncAndHaveTrueResult()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointAsyncAndHaveTrueResult/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>true</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithHeader("Content-Type", "application/sparql-results+xml")
                            .WithStatusCode(HttpStatusCode.OK));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointAsyncAndHaveTrueResult/sparql"));
            RDFAskQueryResult result = await query.ApplyToSPARQLEndpointAsync(endpoint);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.AskResult);
        }

        [TestMethod]
        public async Task ShouldApplyAskQueryToSPARQLEndpointAsyncAndHaveFalseResult()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointAsyncAndHaveFalseResult/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>false</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithHeader("Content-Type", "application/sparql-results+xml")
                            .WithStatusCode(HttpStatusCode.OK));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointAsyncAndHaveFalseResult/sparql"));
            RDFAskQueryResult result = await query.ApplyToSPARQLEndpointAsync(endpoint);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }

        [TestMethod]
        public void ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAndHaveTrueResult()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAndHaveTrueResult/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>true</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithHeader("Content-Type", "application/sparql-results+xml")
                            .WithStatusCode(HttpStatusCode.OK));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAndHaveTrueResult/sparql"));
            RDFAskQueryResult result = query.ApplyToSPARQLEndpoint(endpoint, new RDFSPARQLEndpointQueryOptions());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.AskResult);
        }

        [TestMethod]
        public void ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAndHaveFalseResult()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAndHaveFalseResult/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>false</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithHeader("Content-Type", "application/sparql-results+xml")
                            .WithStatusCode(HttpStatusCode.OK));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAndHaveFalseResult/sparql"));
            RDFAskQueryResult result = query.ApplyToSPARQLEndpoint(endpoint, new RDFSPARQLEndpointQueryOptions());

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }

        [TestMethod]
        public async Task ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAsyncAndHaveTrueResult()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAsyncAndHaveTrueResult/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>true</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithHeader("Content-Type", "application/sparql-results+xml")
                            .WithStatusCode(HttpStatusCode.OK));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAsyncAndHaveTrueResult/sparql"));
            RDFAskQueryResult result = await query.ApplyToSPARQLEndpointAsync(endpoint, new RDFSPARQLEndpointQueryOptions());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.AskResult);
        }

        [TestMethod]
        public async Task ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAsyncAndHaveFalseResult()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAsyncAndHaveFalseResult/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>false</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithHeader("Content-Type", "application/sparql-results+xml")
                            .WithStatusCode(HttpStatusCode.OK));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithDefaultOptionsAsyncAndHaveFalseResult/sparql"));
            RDFAskQueryResult result = await query.ApplyToSPARQLEndpointAsync(endpoint, new RDFSPARQLEndpointQueryOptions());

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenApplyingAskQueryToSPARQLEndpointAccordingToTimingAndBehavior()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldThrowExceptionWhenApplyingAskQueryToSPARQLEndpointAccordingToTimingAndBehavior/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>false</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithStatusCode(HttpStatusCode.OK)
                            .WithDelay(1000));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldThrowExceptionWhenApplyingAskQueryToSPARQLEndpointAccordingToTimingAndBehavior/sparql"));

            Assert.ThrowsException<RDFQueryException>(() => query.ApplyToSPARQLEndpoint(endpoint, new RDFSPARQLEndpointQueryOptions(250, RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.ThrowException)));
        }

        [TestMethod]
        public void ShouldGiveEmptyResultWhenApplyingAskQueryToSPARQLEndpointAccordingToTimingAndBehavior()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldThrowExceptionWhenApplyingAskQueryToSPARQLEndpointAccordingToTimingAndBehavior/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody(
@"<?xml version=""1.0""?>
<sparql xmlns='http://www.w3.org/2005/sparql-results#'>
    <head />
    <boolean>false</boolean>
</sparql>", encoding: Encoding.UTF8)
                            .WithStatusCode(HttpStatusCode.OK)
                            .WithDelay(1000));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldThrowExceptionWhenApplyingAskQueryToSPARQLEndpointAccordingToTimingAndBehavior/sparql"));

            RDFAskQueryResult result = query.ApplyToSPARQLEndpoint(endpoint, new RDFSPARQLEndpointQueryOptions(250, RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.GiveEmptyResult));

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenApplyingAskQueryToSPARQLEndpointAccordingToBehavior()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithOptionsToThrowException/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody("Internal Server Error")
                            .WithStatusCode(HttpStatusCode.InternalServerError)
                            .WithFault(FaultType.NONE));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldApplyAskQueryToSPARQLEndpointWithOptionsToThrowException/sparql"));

            Assert.ThrowsException<RDFQueryException>(() => query.ApplyToSPARQLEndpoint(endpoint, new RDFSPARQLEndpointQueryOptions(1000, RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.ThrowException)));
        }

        [TestMethod]
        public void ShouldGiveEmptyResultWhenApplyingAskQueryToSPARQLEndpointAccordingToBehavior()
        {
            server
                .Given(
                    Request.Create()
                           .WithPath("/RDFAskQueryTest/ShouldGiveEmptyResultWhenApplyingAskQueryToSPARQLEndpointAccordingToBehavior/sparql")
                           .UsingGet()
                           .WithParam(queryParams => queryParams.ContainsKey("query")))
                .RespondWith(
                    Response.Create()
                            .WithBody("Internal Server Error")
                            .WithStatusCode(HttpStatusCode.InternalServerError)
                            .WithFault(FaultType.NONE));

            RDFAskQuery query = new RDFAskQuery()
                .AddPrefix(RDFNamespaceRegister.GetByPrefix(RDFVocabulary.RDF.PREFIX))
                .AddPatternGroup(new RDFPatternGroup("PG1")
                    .AddPattern(new RDFPattern(new RDFVariable("?S"), RDFVocabulary.RDF.TYPE, RDFVocabulary.RDFS.CLASS)));
            RDFSPARQLEndpoint endpoint = new RDFSPARQLEndpoint(new Uri(server.Url + "/RDFAskQueryTest/ShouldGiveEmptyResultWhenApplyingAskQueryToSPARQLEndpointAccordingToBehavior/sparql"));

            RDFAskQueryResult result = query.ApplyToSPARQLEndpoint(endpoint, new RDFSPARQLEndpointQueryOptions(1000, RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.GiveEmptyResult));

            Assert.IsNotNull(result);
            Assert.IsFalse(result.AskResult);
        }
        #endregion
    }
}
