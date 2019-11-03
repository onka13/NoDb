using CoreCommon.Infra.Helpers;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.DbModels;
using System.IO;

namespace NoDb.Business.Service.Services
{
    public class EnumService
    {
        NoDbService _noDbService;

        public string EnumFilePath
        {
            get
            {
                return _noDbService.NoDbFolder + Path.DirectorySeparatorChar + "Enums.json";
            }
        }

        public NoDbEnum Enums { get; set; }

        public EnumService(NoDbService noDbService)
        {
            _noDbService = noDbService;
            ReadFromSettingsFolder();
        }

        public void ReadFromSettingsFolder()
        {
            if (!File.Exists(EnumFilePath))
            {
                Enums = new NoDbEnum();
            }
            else
            {
                var jsonEnum = File.ReadAllText(EnumFilePath);
                Enums = ConversionHelper.Deserialize<NoDbEnum>(jsonEnum);
            }
            ConverterManager.SetEnums(Enums);
        }

        public void Save()
        {
            ConverterManager.SetEnums(Enums);

            var json = ConversionHelper.Serialize(Enums);
            File.WriteAllText(EnumFilePath, json);
        }
    }
}
