using CoreCommon.Infra.Helpers;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace NoDb.Business.Service.Services
{
    public class SettingsService
    {
        NoDbService _noDbService;
        public string NoDbSettingsFolder { get; private set; }

        public List<NoDbSetting> Settings { get; set; }

        public SettingsService(NoDbService noDbService)
        {
            _noDbService = noDbService;
            Settings = new List<NoDbSetting>();
            CheckSettingsFolder();
        }

        public void CheckSettingsFolder()
        {
            var noDbFolderInfo = new DirectoryInfo(_noDbService.NoDbFolder);
            var dirs = new[] {
                noDbFolderInfo.FullName,
                noDbFolderInfo.Parent?.FullName,
                noDbFolderInfo.Parent?.Parent?.FullName,
                noDbFolderInfo.Parent?.Parent?.Parent?.FullName,
            };
            foreach (var folder in dirs)
            {
                if (folder == null) continue;
                NoDbSettingsFolder = Path.Combine(folder, "NoDbSettings");
                if (Directory.Exists(NoDbSettingsFolder))
                {
                    break;
                }
            }
            if (!Directory.Exists(NoDbSettingsFolder))
            {
                // create default
                NoDbSettingsFolder = Path.Combine(noDbFolderInfo.FullName, "NoDbSettings");
                Directory.CreateDirectory(NoDbSettingsFolder);
                Save(new NoDbSetting
                {
                    SettingsKey = "Development"
                });
            }
            ReadFromSettingsFolder();
        }

        public void ReadFromSettingsFolder()
        {
            Settings.Clear();
            var settingFiles = Directory.GetFiles(NoDbSettingsFolder, "*.json");
            foreach (var settingFile in settingFiles)
            {
                var json = File.ReadAllText(settingFile);
                var setting = ConversionHelper.Deserialize<NoDbSetting>(json);
                Settings.Add(setting);
            }
        }

        public void Save(NoDbSetting setting)
        {
            var json = ConversionHelper.Serialize(setting);
            var path = Path.Combine(NoDbSettingsFolder, setting.SettingsKey + ".json");
            File.WriteAllText(path, json);
            ReadFromSettingsFolder();
        }

        public void New(NoDbSetting setting)
        {
            if (string.IsNullOrWhiteSpace(setting.SettingsKey))
            {
                throw new Exception("Empty setting key!");
            }
            var path = Path.Combine(NoDbSettingsFolder, setting.SettingsKey + ".json");
            if (File.Exists(path))
            {
                throw new Exception("Setting key already exists!");
            }
            Save(setting);
        }

        public void Delete(NoDbSetting setting)
        {
            var path = Path.Combine(NoDbSettingsFolder, setting.SettingsKey + ".json");
            File.Delete(path);
            ReadFromSettingsFolder();
        }
    }
}
