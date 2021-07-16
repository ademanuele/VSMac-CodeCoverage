using System.IO;
using CodeCoverage.Core;
using Coverlet.Core;
using Newtonsoft.Json;

namespace CodeCoverage.Coverlet
{
  public class CoverletResultsParser : ICoverageResultsParser
  {
    public string FileExtension => "json";

    public ICoverageResults ParseFrom(Stream stream)
    {
      using StreamReader reader = new StreamReader(stream);
      using JsonTextReader jsonReader = new JsonTextReader(reader);
      JsonSerializer serializer = new JsonSerializer();
      serializer.Error += Serializer_Error;
      var coverletResults = serializer.Deserialize<CoverageResult>(jsonReader);
      return new CoverletCoverageResults(coverletResults);
    }

    private void Serializer_Error(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
    {
      // Currently Coverlet is serializing the BranchKey object as this string. I think in error.
      // This value is not used currently to display coverage results.
      // This allows the results to be deserialized successfully, ignoring this error of not being able to deserialize this object.
      if (e.ErrorContext.Member is string stringMember && stringMember == "Coverlet.Core.Instrumentation.BranchKey")      
        e.ErrorContext.Handled = true;      
    }
  }
}
