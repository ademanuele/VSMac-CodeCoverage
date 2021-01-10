using System.Linq;
using System.Xml.Serialization;
using Coverlet.Core;

namespace CodeCoverage.Coverage
{
  [XmlRoot(ElementName = "Configuration")]
  public class CoverletRunSettingsConfiguration
  {
    [XmlElement(IsNullable = true)]
    public string Include { get; set; }

    [XmlElement(IsNullable = true)]
    public string IncludeDirectory { get; set; }

    [XmlElement(IsNullable = true)]
    public string Exclude { get; set; }

    [XmlElement(IsNullable = true)]
    public string ExcludeByFile { get; set; }

    [XmlElement(IsNullable = true)]
    public string ExcludeByAttribute { get; set; }

    [XmlElement(IsNullable = true)]
    public bool? IncludeTestAssembly { get; set; }

    [XmlElement(IsNullable = true)]
    public bool? SingleHit { get; set; }

    [XmlElement(IsNullable = true)]
    public bool? UseSourceLink { get; set; }

    [XmlElement(IsNullable = true)]
    public bool? SkipAutoProps { get; set; }

    public CoverageParameters ToParameters()
    {
      return new CoverageParameters
      {
        IncludeFilters = ParseCommaSeparated(Include),
        IncludeDirectories = ParseCommaSeparated(IncludeDirectory),
        ExcludeFilters = ParseCommaSeparated(Exclude),
        ExcludedSourceFiles = ParseCommaSeparated(ExcludeByFile),
        ExcludeAttributes = ParseCommaSeparated(ExcludeByAttribute),
        IncludeTestAssembly = IncludeTestAssembly ?? false,
        SingleHit = SingleHit ?? false,
        UseSourceLink = UseSourceLink ?? false,
        SkipAutoProps = SkipAutoProps ?? false,
      };
    }

    private string[] ParseCommaSeparated(string s)
    {
      if (s is null) return new string[0];

      return s.Split(',').Select(i => i.Trim())
                         .Where(i => !i.Equals(string.Empty))
                         .ToArray();
    }
  }
}
