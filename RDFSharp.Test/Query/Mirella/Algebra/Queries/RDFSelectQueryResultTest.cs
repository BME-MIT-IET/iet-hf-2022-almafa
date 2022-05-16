
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using RDFSharp.Query;
using FluentAssertions;

namespace RDFSharp.Test.Query
{
    [TestClass]
    public class RDFSelectQueryResultTest
    {
        [TestMethod]
        public void ShouldCreateSelectQueryResult()
        {
            // Arrange and Act
            RDFSelectQueryResult result = new RDFSelectQueryResult();

            // Assert
            result.Should().NotBeNull();
            result.SelectResults.Should().NotBeNull();
            result.SelectResultsCount.Should().Be(0);
        }
        [TestMethod]
        public void ShouldSerializeSelectQueryResultToStream()
        {
            // Arrange
            RDFSelectQueryResult selectResult = new RDFSelectQueryResult();
            selectResult.SelectResults.Columns.Add(new DataColumn("?gyumolcs", typeof(string)));
            selectResult.SelectResults.Columns.Add(new DataColumn("?allat", typeof(string)));
            DataRow row0 = selectResult.SelectResults.NewRow();
            row0["?gyumolcs"] = "alma";
            row0["?allat"] = "kutya";
            selectResult.SelectResults.Rows.Add(row0);
            DataRow row1 = selectResult.SelectResults.NewRow();
            row1["?gyumolcs"] = "barack";
            row1["?allat"] = "paci";
            selectResult.SelectResults.Rows.Add(row1);
            DataRow row2 = selectResult.SelectResults.NewRow();
            row2["?gyumolcs"] = "citrom";
            row2["?allat"] = "cica";
            selectResult.SelectResults.Rows.Add(row2);
            selectResult.SelectResults.AcceptChanges();

            MemoryStream stream = new MemoryStream();
            selectResult.ToSparqlXmlResult(stream);
            byte[] streamData = stream.ToArray();

            streamData.Length.Should().BeGreaterThan(100); // Just a pre check 

            // Act
            RDFSelectQueryResult selectResult2 = RDFSelectQueryResult.FromSparqlXmlResult(new MemoryStream(streamData));

            // Assert
            selectResult2.Should().NotBeNull();
            selectResult2.SelectResults.Should().NotBeNull();
            selectResult2.SelectResultsCount.Should().Be(3);
            selectResult2.SelectResults.Columns.Count.Should().Be(2);
            selectResult2.SelectResults.Columns.Contains("?gyumolcs").Should().BeTrue();
            selectResult2.SelectResults.Columns.Contains("?allat").Should().BeTrue();
            selectResult2.SelectResults.Rows[0]["?gyumolcs"].Should().Be("alma");
            selectResult2.SelectResults.Rows[0]["?allat"].Should().Be("kutya");
            selectResult2.SelectResults.Rows[1]["?gyumolcs"].Should().Be("barack");
            selectResult2.SelectResults.Rows[1]["?allat"].Should().Be("paci");
            selectResult2.SelectResults.Rows[2]["?gyumolcs"].Should().Be("citrom");
            selectResult2.SelectResults.Rows[2]["?allat"].Should().Be("cica");
        }

        [TestMethod]
        public async Task ShouldSerializeSelectQueryResultToStreamAsync()
        {
            // Arrange
            RDFSelectQueryResult selectResult = new RDFSelectQueryResult();
            selectResult.SelectResults.Columns.Add(new DataColumn("?gyumolcs", typeof(string)));
            selectResult.SelectResults.Columns.Add(new DataColumn("?allat", typeof(string)));
            DataRow row = selectResult.SelectResults.NewRow();
            row["?gyumolcs"] = "alma";
            row["?allat"] = "kutya";
            selectResult.SelectResults.Rows.Add(row);
            selectResult.SelectResults.AcceptChanges();

            MemoryStream stream = new MemoryStream();
            await selectResult.ToSparqlXmlResultAsync(stream);
            byte[] streamData = stream.ToArray();

            streamData.Length.Should().BeGreaterThan(100); // Just a pre check 

            // Act
            RDFSelectQueryResult selectResult2 = await RDFSelectQueryResult.FromSparqlXmlResultAsync(new MemoryStream(streamData));

            // Assert
            selectResult2.Should().NotBeNull();
            selectResult2.SelectResults.Should().NotBeNull();
            selectResult2.SelectResultsCount.Should().Be(1);
            selectResult2.SelectResults.Columns.Count.Should().Be(2);
            selectResult2.SelectResults.Rows[0]["?gyumolcs"].Should().Be("alma");
            selectResult2.SelectResults.Rows[0]["?allat"].Should().Be("kutya");
        }

        [TestMethod]
        public void ShouldSerializeSelectQueryResultToFile()
        {
            // Arrange
            RDFSelectQueryResult selectResult = new RDFSelectQueryResult();
            selectResult.SelectResults.Columns.Add(new DataColumn("?gyumolcs", typeof(string)));
            selectResult.SelectResults.Columns.Add(new DataColumn("?allat", typeof(string)));
            DataRow row = selectResult.SelectResults.NewRow();
            row["?gyumolcs"] = "alma";
            row["?allat"] = "kutya";
            selectResult.SelectResults.Rows.Add(row);
            selectResult.SelectResults.AcceptChanges();

            selectResult.ToSparqlXmlResult(Path.Combine(Environment.CurrentDirectory, "testFile.srx"));

            File.Exists(Path.Combine(Environment.CurrentDirectory, "testFile.srx")).Should().BeTrue(); // Just a pre check 

            // Act
            RDFSelectQueryResult selectResult2 = RDFSelectQueryResult.FromSparqlXmlResult(Path.Combine(Environment.CurrentDirectory, "testFile.srx"));

            // Assert
            selectResult2.Should().NotBeNull();
            selectResult2.SelectResults.Should().NotBeNull();
            selectResult2.SelectResultsCount.Should().Be(1);
            selectResult2.SelectResults.Columns.Count.Should().Be(2);
            selectResult2.SelectResults.Columns.Contains("?gyumolcs").Should().BeTrue();
            selectResult2.SelectResults.Columns.Contains("?allat").Should().BeTrue();
            selectResult2.SelectResults.Rows[0]["?gyumolcs"].Should().Be("alma");
            selectResult2.SelectResults.Rows[0]["?allat"].Should().Be("kutya");
        }

        [TestMethod]
        public async Task ShouldSerializeSelectQueryResultToFileAsync()
        {
            // Arrange
            RDFSelectQueryResult selectResult = new RDFSelectQueryResult();
            selectResult.SelectResults.Columns.Add(new DataColumn("?gyumolcs", typeof(string)));
            selectResult.SelectResults.Columns.Add(new DataColumn("?allat", typeof(string)));
            DataRow row = selectResult.SelectResults.NewRow();
            row["?gyumolcs"] = "alma";
            row["?allat"] = "kutya";
            selectResult.SelectResults.Rows.Add(row);
            selectResult.SelectResults.AcceptChanges();

            await selectResult.ToSparqlXmlResultAsync(Path.Combine(Environment.CurrentDirectory, "testFileAsync.srx"));

            File.Exists(Path.Combine(Environment.CurrentDirectory, "testFileAsync.srx")).Should().BeTrue(); // Just a pre check 

            // Act
            RDFSelectQueryResult selectResult2 = await RDFSelectQueryResult.FromSparqlXmlResultAsync(Path.Combine(Environment.CurrentDirectory, "testFileAsync.srx"));

            // Assert
            selectResult2.Should().NotBeNull();
            selectResult2.SelectResults.Should().NotBeNull();
            selectResult2.SelectResultsCount.Should().Be(1);
            selectResult2.SelectResults.Columns.Count.Should().Be(2);
            selectResult2.SelectResults.Columns.Contains("?gyumolcs").Should().BeTrue();
            selectResult2.SelectResults.Columns.Contains("?allat").Should().BeTrue();
            selectResult2.SelectResults.Rows[0]["?gyumolcs"].Should().Be("alma");
            selectResult2.SelectResults.Rows[0]["?allat"].Should().Be("kutya");
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