using System.IO;
using System.Reflection;
using CodeCoverage.Coverlet;
using NUnit.Framework;

namespace UnitTests.Coverlet
{
  public class CoverletResultParsingTests
  {
    [Test]
    public void TestCase()
    {
      Stream resultsStream = SampleResultsStream();

      CoverletResultsParser parser = new CoverletResultsParser();

      var parsedResults = parser.ParseFrom(resultsStream);
      Assert.AreEqual(parsedResults.ModuleCoverage.Count, 2);
    }

    Stream SampleResultsStream()
    {
      Assembly assembly = typeof(CoverletResultParsingTests).Assembly;
      return assembly.GetManifestResourceStream("UnitTests.Coverlet.result_sample.json");
    }
  }
}
