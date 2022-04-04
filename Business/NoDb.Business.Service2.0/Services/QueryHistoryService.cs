using CoreCommon.Infra.Helpers;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NoDb.Business.Service.Services
{
    public class QueryHistoryService
    {
        public const string HISTORY_FOLDER = "queries";

        readonly string _settingsFolder;
        public List<string> HistoryFiles { get; set; }
        public List<string> HistoryFileNames { get; set; }

        public QueryHistoryService(string settingsFolder)
        {
            _settingsFolder = settingsFolder;
            ReadFromSettingsFolder();
        }

        public void ReadFromSettingsFolder()
        {
            var dir = Path.Combine(_settingsFolder, HISTORY_FOLDER);
            Directory.CreateDirectory(dir);
            HistoryFiles = Directory.GetFiles(dir, "*.txt").ToList();
            HistoryFileNames = HistoryFiles.Select(x => new FileInfo(x).Name.Replace(".txt", "")).ToList();
        }

        public string GetPath(string name)
        {
            return Path.Combine(_settingsFolder, HISTORY_FOLDER, name + ".txt");
        }

        public void Save(string name, string content, bool append = false)
        {
            var path = GetPath(name);
            if (append)
            {
                var oldContent = GetContent(name);
                content = oldContent + "\n\n" + content;
            }
            File.WriteAllText(path, content, Encoding.UTF8);
            ReadFromSettingsFolder();
        }

        public void New(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Empty name!");
            }
            var path = GetPath(name);
            if (File.Exists(path))
            {
                throw new Exception("Setting key already exists!");
            }
            Save(name, "");
        }

        public void Delete(string name)
        {
            File.Delete(GetPath(name));
            ReadFromSettingsFolder();
        }

        public string GetContent(string name)
        {
            var path = GetPath(name);
            if (!File.Exists(path)) return "";
            return File.ReadAllText(path);
        }
    }
}
