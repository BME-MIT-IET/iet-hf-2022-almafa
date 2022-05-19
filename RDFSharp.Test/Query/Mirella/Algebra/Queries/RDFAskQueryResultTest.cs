using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using RDFSharp.Query;
using FluentAssertions;

namespace RDFSharp.Test.Query
{
    [TestClass]
    public class RDFAskQueryResultTest
    {
        [TestMethod]
        public void ShouldCreateAskQueryResult()
        {
            // Arrange and Act
            RDFAskQueryResult result = new RDFAskQueryResult();

            // Assert
            result.Should().NotBeNull();
            result.AskResult.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldSerializeFalseAskQueryResultToStream()
        {
            // Arrange
            MemoryStream stream = new MemoryStream();
            RDFAskQueryResult askResult = new RDFAskQueryResult();
            askResult.ToSparqlXmlResult(stream);
            byte[] streamData = stream.ToArray();

            streamData.Length.Should().BeGreaterThan(100); // Just a pre check 

            // Act
            RDFAskQueryResult askResult2 = RDFAskQueryResult.FromSparqlXmlResult(new MemoryStream(streamData));

            // Assert
            askResult2.Should().NotBeNull();
            askResult2.AskResult.Should().BeFalse();
        }

        [TestMethod]
        public async Task ShouldSerializeFalseAskQueryResultToStreamAsync()
        {
            // Arrange
            MemoryStream stream = new MemoryStream();
            RDFAskQueryResult askResult = new RDFAskQueryResult();
            await askResult.ToSparqlXmlResultAsync(stream);
            byte[] streamData = stream.ToArray();

            streamData.Length.Should().BeGreaterThan(100); // Just a pre check 

            // Act
            RDFAskQueryResult askResult2 = await RDFAskQueryResult.FromSparqlXmlResultAsync(new MemoryStream(streamData));

            // Assert
            askResult2.Should().NotBeNull();
            askResult2.AskResult.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldSerializeTrueAskQueryResultToStream()
        {
            // Arrange
            MemoryStream stream = new MemoryStream();
            RDFAskQueryResult askResult = new RDFAskQueryResult();
            askResult.AskResult = true;
            askResult.ToSparqlXmlResult(stream);
            byte[] streamData = stream.ToArray();

            streamData.Length.Should().BeGreaterThan(100); // Just a pre check 

            // Act
            RDFAskQueryResult askResult2 = RDFAskQueryResult.FromSparqlXmlResult(new MemoryStream(streamData));

            // Assert
            askResult2.Should().NotBeNull();
            askResult2.AskResult.Should().BeTrue();
        }

        [TestMethod]
        public async Task ShouldSerializeTrueAskQueryResultToStreamAsync()
        {
            // Arrange
            MemoryStream stream = new MemoryStream();
            RDFAskQueryResult askResult = new RDFAskQueryResult();
            askResult.AskResult = true;
            await askResult.ToSparqlXmlResultAsync(stream);
            byte[] streamData = stream.ToArray();

            streamData.Length.Should().BeGreaterThan(100); // Just a pre check 

            // Act
            RDFAskQueryResult askResult2 = await RDFAskQueryResult.FromSparqlXmlResultAsync(new MemoryStream(streamData));

            // Assert
            askResult2.Should().NotBeNull();
            askResult2.AskResult.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldSerializeFalseAskQueryResultToFile()
        {
            // Arrange
            RDFAskQueryResult askResult = new RDFAskQueryResult();
            askResult.ToSparqlXmlResult(Path.Combine(Environment.CurrentDirectory, "testFileFalse.srx"));

            File.Exists(Path.Combine(Environment.CurrentDirectory, "testFileFalse.srx")).Should().BeTrue();

            // Act
            RDFAskQueryResult askResult2 = RDFAskQueryResult.FromSparqlXmlResult(Path.Combine(Environment.CurrentDirectory, "testFileFalse.srx"));

            // Assert
            askResult2.Should().NotBeNull();
            askResult2.AskResult.Should().BeFalse();
        }

        [TestMethod]
        public async Task ShouldSerializeFalseAskQueryResultToFileAsync()
        {
            // Arrange
            RDFAskQueryResult askResult = new RDFAskQueryResult();
            await askResult.ToSparqlXmlResultAsync(Path.Combine(Environment.CurrentDirectory, "testFileFalseAsync.srx"));

            File.Exists(Path.Combine(Environment.CurrentDirectory, "testFileFalseAsync.srx")).Should().BeTrue();

            // Act
            RDFAskQueryResult askResult2 = await RDFAskQueryResult.FromSparqlXmlResultAsync(Path.Combine(Environment.CurrentDirectory, "testFileFalseAsync.srx"));

            // Assert
            askResult2.Should().NotBeNull();
            askResult2.AskResult.Should().BeFalse();
        }

        [TestMethod]
        public void ShouldSerializeTrueAskQueryResultToFile()
        {
            // Arrange
            RDFAskQueryResult askResult = new RDFAskQueryResult();
            askResult.AskResult = true;
            askResult.ToSparqlXmlResult(Path.Combine(Environment.CurrentDirectory, "testFileTrue.srx"));

            File.Exists(Path.Combine(Environment.CurrentDirectory, "testFileTrue.srx")).Should().BeTrue();

            // Act
            RDFAskQueryResult askResult2 = RDFAskQueryResult.FromSparqlXmlResult(Path.Combine(Environment.CurrentDirectory, "testFileTrue.srx"));

            // Assert
            askResult2.Should().NotBeNull();
            askResult2.AskResult.Should().BeTrue();
        }

        [TestMethod]
        public async Task ShouldSerializeTrueAskQueryResultToFileAsync()
        {
            // Arrange
            RDFAskQueryResult askResult = new RDFAskQueryResult();
            askResult.AskResult = true;
            await askResult.ToSparqlXmlResultAsync(Path.Combine(Environment.CurrentDirectory, "testFileTrueAsync.srx"));

            File.Exists(Path.Combine(Environment.CurrentDirectory, "testFileTrueAsync.srx")).Should().BeTrue();

            // Act
            RDFAskQueryResult askResult2 = await RDFAskQueryResult.FromSparqlXmlResultAsync(Path.Combine(Environment.CurrentDirectory, "testFileTrueAsync.srx"));

            // Assert
            askResult2.Should().NotBeNull();
            askResult2.AskResult.Should().BeTrue();
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (string file in Directory.EnumerateFiles(Environment.CurrentDirectory, "testFile*"))
            {
                File.Delete(file);
            }
        }
    }
}