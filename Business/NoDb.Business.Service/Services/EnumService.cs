using CoreCommon.Infrastructure.Helpers;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.DbModels;
using System.IO;

namespace NoDb.Business.Service.Services
{
    public class EnumService
    {
        private readonly NoDbService noDbService;

        public EnumService(NoDbService noDbService)
        {
            this.noDbService = noDbService;
            ReadFromSettingsFolder();
        }

        public NoDbEnum Enums { get; private set; }

        public void ReadFromSettingsFolder()
        {
            if (!File.Exists(noDbService.EnumFilePath))
            {
                Enums = new NoDbEnum();
            }
            else
            {
                var jsonEnum = File.ReadAllText(noDbService.EnumFilePath);
                Enums = ConversionHelper.Deserialize<NoDbEnum>(jsonEnum);
            }

            StaticManager.Enums = Enums.EnumList;
        }

        public void Save()
        {
            var json = ConversionHelper.Serialize(Enums, isIndented: true);
            File.WriteAllText(noDbService.EnumFilePath, json);

            StaticManager.Enums = Enums.EnumList;
        }
    }
}
