using System.IO;
using Newtonsoft.Json;

namespace CodeCoverage.Coverage
{
  class CoverletResultsParser : ICoverageResultsParser
  {
    public string FileExtension => "json";

    public ICoverageResults ParseFrom(Stream stream)
    {
      using StreamReader reader = new StreamReader(stream);
      using JsonTextReader jsonReader = new JsonTextReader(reader);
      JsonSerializer serializer = new JsonSerializer();
      var coverletResults = serializer.Deserialize<Coverlet.Core.CoverageResult>(jsonReader);
      return new CoverletCoverageResults(coverletResults);
    }
  }
}
