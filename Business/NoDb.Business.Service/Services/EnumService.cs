using CoreCommon.Infrastructure.Helpers;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.DbModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public List<NoDbEnumDetail> Enums { get; private set; }

        public void ReadFromSettingsFolder()
        {
            Enums = new List<NoDbEnumDetail>();

            var files = Directory.GetFiles(noDbService.EnumFolderPath);
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var @enum = ConversionHelper.Deserialize<NoDbEnumDetail>(json);
                Enums.Add(@enum);
            }

            Enums = Enums.OrderBy(x => x?.Name).ToList();

            StaticManager.Enums = Enums;
        }

        public void UpdateEnums(List<NoDbEnumDetail> updatedEnums)
        {
            Enums = updatedEnums;
            Save();
        }

        public void Save()
        {
            foreach (var @enum in Enums)
            {
                var json = ConversionHelper.Serialize(@enum, isIndented: true, minimise: true);
                File.WriteAllText(Path.Combine(noDbService.EnumFolderPath, @enum.Name + ".json"), json);
            }

            StaticManager.Enums = Enums;
        }
    }
}
