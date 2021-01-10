using System.Collections.Generic;
using CoverletCoverageSummary = Coverlet.Core.CoverageSummary;
using CoverletCoverageResult = Coverlet.Core.CoverageResult;
using System.IO;
using Newtonsoft.Json;

namespace CodeCoverage.Coverage
{
  class CoverletCoverageResults : ICoverageResults
  {
    public Dictionary<string, CoverageSummary> ModuleCoverage { get; private set; }
    
    readonly CoverletCoverageResult result;

    public CoverletCoverageResults(CoverletCoverageResult result)
    {
      this.result = result;
      SetModuleCoverage();
    }

    void SetModuleCoverage()
    {
      var modulesCoverage = new Dictionary<string, CoverageSummary>();
      var summary = new CoverletCoverageSummary();

      foreach (var moduleInfo in result.Modules)
      {
        var moduleLineCoverage = summary.CalculateLineCoverage(moduleInfo.Value);
        var moduleBranchCoverage = summary.CalculateBranchCoverage(moduleInfo.Value);
        var moduleName = moduleInfo.Key.Replace(".dll", "");
        modulesCoverage[moduleName] = new CoverageSummary(moduleLineCoverage.Percent, moduleBranchCoverage.Percent);
      }

      ModuleCoverage = modulesCoverage;
    }

    public Dictionary<int, int> CoverageForFile(string path)
    {
      Dictionary<int, int> fileCoverage = new Dictionary<int, int>();

      foreach (var module in result.Modules)
        foreach (var document in module.Value)
          if (document.Key == path)
            foreach (var c in document.Value)
              foreach (var method in c.Value)
                foreach (var line in method.Value.Lines)
                {
                  if (!fileCoverage.ContainsKey(line.Key))
                    fileCoverage[line.Key] = 0;
                  fileCoverage[line.Key] += line.Value;
                }

      return fileCoverage;
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
