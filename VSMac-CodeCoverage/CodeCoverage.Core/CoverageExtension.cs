using CodeCoverage.Core.Presentation;

namespace CodeCoverage.Core
{
  public static class CoverageExtension
  {
    static ILoggingService loggingService;
    public static ICoverageResultsRepository Repository { get; private set; }
    static ICoverageProvider provider;

    static CoveragePadPresenter presenter;

    public static void Setup(ILoggingService loggingService, ICoverageProvider provider, ICoverageResultsParser parser)
    {
      CoverageExtension.loggingService = loggingService;
      Repository = new CoverageResultsRepository(parser);
      CoverageExtension.provider = provider;
    }

    public static CoveragePadPresenter Presenter(ICoveragePad pad)
    {
      return presenter ??= new CoveragePadPresenter(pad, loggingService, Repository, provider);
    }
  }
}
