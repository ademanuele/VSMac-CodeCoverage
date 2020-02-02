using System.Collections.Generic;
using CoverletCoverageSummary = Coverlet.Core.CoverageSummary;
using CoverletCoverageResult = Coverlet.Core.CoverageResult;
using System.IO;
using Newtonsoft.Json;

namespace CodeCoverage.Coverage
{
  class CoverletCoverageResults : ICoverageResults
  {
    readonly CoverletCoverageResult result;

    public CoverletCoverageResults(CoverletCoverageResult result)
    {
      this.result = result;
    }

    public Dictionary<string, CoverageSummary> ModuleCoverage
    {
      get
      {
        var modulesCoverage = new Dictionary<string, CoverageSummary>();
        var summary = new CoverletCoverageSummary();

        foreach (var moduleInfo in result.Modules)
        {
          var moduleLineCoverage = summary.CalculateLineCoverage(moduleInfo.Value);
          var moduleBranchCoverage = summary.CalculateBranchCoverage(moduleInfo.Value);
          modulesCoverage[moduleInfo.Key] = new CoverageSummary(moduleLineCoverage.Percent, moduleBranchCoverage.Percent);
        }

        return modulesCoverage;
      }
    }

    public void SaveTo(Stream stream)
    {
      using (StreamWriter writer = new StreamWriter(stream))
      using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
      {
        JsonSerializer serializer = new JsonSerializer();
        serializer.Serialize(jsonWriter, result);
      }
    }
  }
}
