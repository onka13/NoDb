using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NoDb.Business.Service.Services
{
    public class QueryHistoryService
    {
        private readonly NoDbService noDbService;

        public QueryHistoryService(NoDbService noDbService)
        {
            this.noDbService = noDbService;
            ReadFromSettingsFolder();
        }

        public List<string> HistoryFiles { get; private set; }
        public List<string> HistoryFileNames { get; private set; }

        public void ReadFromSettingsFolder()
        {
            HistoryFiles = Directory.GetFiles(noDbService.QueriesFolder, "*.txt").ToList();
            HistoryFileNames = HistoryFiles.Select(x => new FileInfo(x).Name.Replace(".txt", "")).ToList();
        }

        public string GetPath(string name)
        {
            return Path.Combine(noDbService.QueriesFolder, name + ".txt");
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
