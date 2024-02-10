using CoreCommon.Infrastructure.Helpers;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.IO;

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
        }

        public void Save(NoDbSetting setting)
        {
            var json = ConversionHelper.Serialize(setting, isIndented: true);
            File.WriteAllText(GetSettingFilePath(setting), json);
            ReadFromSettingsFolder();
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
