using CoreCommon.Infrastructure.Helpers;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NoDb.Business.Service.Services
{
    public class SettingsService
    {
        private readonly NoDbService noDbService;

        public SettingsService(NoDbService noDbService)
        {
            this.noDbService = noDbService;
            ReadFromSettingsFolder();
        }

        public List<NoDbSetting> Settings { get; set; } = new List<NoDbSetting>();

        public void ReadFromSettingsFolder()
        {
            Settings.Clear();
            var settingFiles = Directory.GetFiles(noDbService.SettingsFolder, "*.json");
            foreach (var settingFile in settingFiles)
            {
                var json = File.ReadAllText(settingFile);
                var setting = ConversionHelper.Deserialize<NoDbSetting>(json);
                Settings.Add(setting);
            }

            if (settingFiles.Length == 0)
            {
                New("dev");
            }
        }

        public void Save(NoDbSetting setting)
        {
            var json = ConversionHelper.Serialize(setting, isIndented: true);
            File.WriteAllText(GetSettingFilePath(setting), json);
            ReadFromSettingsFolder();
        }

        public void New(string key)
        {
            var projectPrefix = "ABC";
            if (noDbService.NoDbSolutionService.Projects.Count > 0)
            {
                projectPrefix = noDbService.NoDbSolutionService.Projects.FirstOrDefault().Name.Split('.').FirstOrDefault() ?? "ABC";
            }

            New(new NoDbSetting
            {
                SettingsKey = key,
                Schema = "dbo",
                ConnectionType = Data.Domain.Enums.NoDbConnectionType.Mssql,
                ConnectionName = "MainConnection",
                ProjectSetting = new ProjectSetting
                {
                    ApiProject = $"{projectPrefix}.API",
                    ApplicationProject = $"{projectPrefix}.Application",
                    InfrastructureProject = $"{projectPrefix}.Infrastructure",
                    DomainProject = $"{projectPrefix}.Domain",
                    CoreCommonDataNamespace = "DotNetCommon",
        }
            });
        }
        
        public void New(NoDbSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.SettingsKey))
            {
                throw new Exception("Empty setting key!");
            }
            var path = GetSettingFilePath(setting);
            if (File.Exists(path))
            {
                throw new Exception("Setting key already exists!");
            }
            Save(setting);
        }

        public void Delete(NoDbSetting setting)
        {
            File.Delete(GetSettingFilePath(setting));
            ReadFromSettingsFolder();
        }

        private string GetSettingFilePath(NoDbSetting setting)
        {
            return Path.Combine(noDbService.SettingsFolder, setting.SettingsKey + ".json");
        }
    }
}
