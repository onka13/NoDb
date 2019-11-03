using NoDb.Business.Service.Managers;
using System.IO;

namespace NoDb.Business.Service.Services
{
    public class NoDbService
    {
        public string NoDbFolder { get; private set; }
        public TableService TableService { get; set; }
        public EnumService EnumService { get; set; }
        public RevisionService RevisionService { get; set; }
        public SearchService SearchService { get; set; }
        public SettingsService SettingsService { get; set; }

        public NoDbService(string folder)
        {
            NoDbFolder = folder;
            Init();
        }

        public void Init()
        {
            if (!Directory.Exists(NoDbFolder))
            {
                Directory.CreateDirectory(NoDbFolder);
            }
            TableService = new TableService(this);
            EnumService = new EnumService(this);
            RevisionService = new RevisionService(this);
            SearchService = new SearchService(this);
            SettingsService = new SettingsService(this);
        }
    }
}
