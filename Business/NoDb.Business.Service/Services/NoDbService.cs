using NoDb.Business.Service.Managers;
using Org.BouncyCastle.Tsp;
using System.IO;

namespace NoDb.Business.Service.Services
{
    public class NoDbService
    {
        public const string NODB_FOLDER_NAME = "__NoDb__";

        public string Solution { get; private set; }
        public string SolutionFolder { get; private set; }
        public string NoDbFolder { get; private set; }

        public string ProjectsFilePath => Path.Combine(NoDbFolder, "projects.json");
        public string TableFilePath => Path.Combine(NoDbFolder, "Tables.json");
        public string EnumFilePath => Path.Combine(NoDbFolder, "Enums.json");
        public string RevisionFolder => Path.Combine(NoDbFolder, "Revisions");
        public string SettingsFolder => Path.Combine(NoDbFolder, "Settings");
        public string QueriesFolder => Path.Combine(NoDbFolder, "Queries");

        public TableService TableService { get; set; }
        public EnumService EnumService { get; set; }
        public RevisionService RevisionService { get; set; }
        public SearchService SearchService { get; set; }
        public NoDbSolutionService NoDbSolutionService { get; set; }
        public SettingsService SettingsService { get; set; }
        public QueryHistoryService QueryHistoryService { get; set; }

        public NoDbService(string solution)
        {
            Init(solution);
        }

        public void Init(string solution)
        {
            Solution = solution;
            SolutionFolder = Path.GetDirectoryName(solution);
            NoDbFolder = Path.Combine(SolutionFolder, NODB_FOLDER_NAME);

            foreach (var folder in new string[] { NoDbFolder, RevisionFolder, SettingsFolder, QueriesFolder })
            {
                Directory.CreateDirectory(folder);
            }

            NoDbSolutionService = new NoDbSolutionService(this);
            TableService = new TableService(this);
            EnumService = new EnumService(this);
            RevisionService = new RevisionService(this);
            SearchService = new SearchService(this);
            SettingsService = new SettingsService(this);
            QueryHistoryService = new QueryHistoryService(this);
        }
    }
}
