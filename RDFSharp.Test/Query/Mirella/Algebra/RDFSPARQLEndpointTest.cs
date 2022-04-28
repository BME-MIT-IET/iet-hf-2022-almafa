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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDFSharp.Query;

namespace RDFSharp.Test.Query
{
    [TestClass]
    public class RDFSPARQLEndpointTest
    {
        #region Tests
        [TestMethod]
        public void ShouldCreateSPARQLEndpoint()
        {
            RDFSPARQLEndpoint sparqlEndpoint = new RDFSPARQLEndpoint(new Uri("http://sparql/query"));

            Assert.IsNotNull(sparqlEndpoint);
            Assert.IsTrue(sparqlEndpoint.BaseAddress.Equals(new Uri("http://sparql/query")));
            Assert.IsNotNull(sparqlEndpoint.QueryParams);
            Assert.IsTrue(sparqlEndpoint.QueryParams.Count == 0);
            Assert.IsTrue(sparqlEndpoint.ToString().Equals("http://sparql/query"));
        }

        [TestMethod]
        public void ShouldThrowExceptionOnCreatingSPARQLEndpointBecauseNullUri()
            => Assert.ThrowsException<RDFQueryException>(() => new RDFSPARQLEndpoint(null));

        [TestMethod]
        public void ShouldAddQueryOptions()
        {
            RDFSPARQLEndpoint sparqlEndpoint = new RDFSPARQLEndpoint(new Uri("http://sparql/query"));
            sparqlEndpoint.AddDefaultGraphUri("http://ex1.org/");
            sparqlEndpoint.AddNamedGraphUri("http://ex2.org/");

            Assert.IsNotNull(sparqlEndpoint);
            Assert.IsTrue(sparqlEndpoint.BaseAddress.Equals(new Uri("http://sparql/query")));
            Assert.IsNotNull(sparqlEndpoint.QueryParams);
            Assert.IsTrue(sparqlEndpoint.QueryParams.Count == 2);
            Assert.IsTrue(sparqlEndpoint.QueryParams["default-graph-uri"].Equals("http://ex1.org/"));
            Assert.IsTrue(sparqlEndpoint.QueryParams["named-graph-uri"].Equals("http://ex2.org/"));
            Assert.IsTrue(sparqlEndpoint.ToString().Equals("http://sparql/query"));
        }

        [TestMethod]
        public void ShouldCreateSPARQLEndpointQueryOptions()
        {
            RDFSPARQLEndpointQueryOptions sparqlEndpointQueryOptions = new RDFSPARQLEndpointQueryOptions();

            Assert.IsNotNull(sparqlEndpointQueryOptions);
            Assert.IsTrue(sparqlEndpointQueryOptions.TimeoutMilliseconds == -1);
            Assert.IsTrue(sparqlEndpointQueryOptions.ErrorBehavior == RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.ThrowException);
        }

        [TestMethod]
        public void ShouldCreateSPARQLEndpointQueryOptionsWithTimeoutMilliseconds()
        {
            RDFSPARQLEndpointQueryOptions sparqlEndpointQueryOptions = new RDFSPARQLEndpointQueryOptions(2000);

            Assert.IsNotNull(sparqlEndpointQueryOptions);
            Assert.IsTrue(sparqlEndpointQueryOptions.TimeoutMilliseconds == 2000);
            Assert.IsTrue(sparqlEndpointQueryOptions.ErrorBehavior == RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.ThrowException);
        }

        [TestMethod]
        public void ShouldCreateSPARQLEndpointQueryOptionsWithNegativeTimeoutMilliseconds()
        {
            RDFSPARQLEndpointQueryOptions sparqlEndpointQueryOptions = new RDFSPARQLEndpointQueryOptions(-2000);

            Assert.IsNotNull(sparqlEndpointQueryOptions);
            Assert.IsTrue(sparqlEndpointQueryOptions.TimeoutMilliseconds == -1);
            Assert.IsTrue(sparqlEndpointQueryOptions.ErrorBehavior == RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.ThrowException);
        }

        [TestMethod]
        public void ShouldCreateSPARQLEndpointQueryOptionsWithTimeoutMillisecondsAndErrorBehavior()
        {
            RDFSPARQLEndpointQueryOptions sparqlEndpointQueryOptions = new RDFSPARQLEndpointQueryOptions(2000, RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.GiveEmptyResult);

            Assert.IsNotNull(sparqlEndpointQueryOptions);
            Assert.IsTrue(sparqlEndpointQueryOptions.TimeoutMilliseconds == 2000);
            Assert.IsTrue(sparqlEndpointQueryOptions.ErrorBehavior == RDFQueryEnums.RDFSPARQLEndpointQueryErrorBehaviors.GiveEmptyResult);
        }

        [TestMethod]
        public void ShouldCreateSPARQLEndpointOperationOptions()
        {
            RDFSPARQLEndpointOperationOptions sparqlEndpointOperationOptions = new RDFSPARQLEndpointOperationOptions();

            Assert.IsNotNull(sparqlEndpointOperationOptions);
            Assert.IsTrue(sparqlEndpointOperationOptions.RequestContentType == RDFQueryEnums.RDFSPARQLEndpointOperationContentTypes.Sparql_Update);
        }

        [TestMethod]
        public void ShouldCreateSPARQLEndpointOperationOptionsWithRequestContentType()
        {
            RDFSPARQLEndpointOperationOptions sparqlEndpointOperationOptions = new RDFSPARQLEndpointOperationOptions(RDFQueryEnums.RDFSPARQLEndpointOperationContentTypes.X_WWW_FormUrlencoded);

            Assert.IsNotNull(sparqlEndpointOperationOptions);
            Assert.IsTrue(sparqlEndpointOperationOptions.RequestContentType == RDFQueryEnums.RDFSPARQLEndpointOperationContentTypes.X_WWW_FormUrlencoded);
        }
        #endregion
    }
}