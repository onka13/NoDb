using CoreCommon.Infrastructure.Helpers;
using NoDb.Business.Service.Templates;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NoDb.Business.Service.Services
{
    public class TableService
    {
        NoDbService _noDbService;

        public string TableFolderPath
        {
            get
            {
                return _noDbService.NoDbFolder + Path.DirectorySeparatorChar + "Tables";
            }
        }

        public string NewTableFilePath(string tableName)
        {
            return TableFolderPath + Path.DirectorySeparatorChar + tableName;
        }

        public List<NoDbTable> Tables { get; set; }

        public TableService(NoDbService noDbService)
        {
            _noDbService = noDbService;
            if (!Directory.Exists(TableFolderPath))
            {
                Directory.CreateDirectory(TableFolderPath);
            }

            ReadFromSettingsFolder();
        }

        public void ReadFromSettingsFolder()
        {
            Tables = new List<NoDbTable>();

            var files = Directory.GetFiles(TableFolderPath);
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var table = ConversionHelper.Deserialize<NoDbTable>(json);
                Tables.Add(table);
            }

            Tables = Tables.OrderBy(x => x?.Detail?.Name).ToList();
        }

        public NoDbTable New(string tableName, string template = "")
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new Exception("Tablename can not be empty!");
            }
            if (Tables.Any(x => x.Detail.Name.ToLower() == tableName.ToLower()))
            {
                throw new Exception("Table already exist!");
            }

            var table = TableTemplates.Get(tableName, template);
            Tables.Add(table);
            WriteToFile();
            return table;
        }

        public void New(NoDbTable table)
        {
            if (string.IsNullOrWhiteSpace(table.Detail.TitleColumn))
            {
                throw new Exception("Please enter TitleColumn field!");
            }
            Tables.Add(table);
            WriteToFile();
        }

        public void Delete(string tick)
        {
            Delete(Tables[Tables.FindIndex(x => x.Hash == tick)]);
        }

        public void Delete(NoDbTable table)
        {
            _noDbService.RevisionService.SaveRevision(table, null);
            Tables.Remove(table);
            WriteToFile();
        }

        public void UpdateTable(NoDbTable updatedTable, bool saveRevision = true)
        {
            if (string.IsNullOrWhiteSpace(updatedTable.Detail.TitleColumn))
            {
                throw new Exception("Please enter TitleColumn field!");
            }
            var index = Tables.FindIndex(x => x.Hash == updatedTable.Hash);
            var originalTable = Tables[index];
            if (saveRevision)
            {
                _noDbService.RevisionService.SaveRevision(originalTable, updatedTable);
            }

            Tables[index] = updatedTable;
            WriteToFile();
        }

        public static void SplitOldTableJsonFile(string tablesJsonPath, string tablesFolderPath)
        {
            var jsonContent = File.ReadAllText(tablesJsonPath);
            var tables = ConversionHelper.Deserialize<List<NoDbTable>>(jsonContent);
            foreach (var table in tables)
            {
                var json = ConversionHelper.Serialize(table, isIndented: true, minimise: true);
                File.WriteAllText($"{tablesFolderPath}{Path.DirectorySeparatorChar}{table.Detail.Name}.json", json);
            }
        }

        private void WriteToFile()
        {
            foreach (var table in Tables)
            {
                var json = ConversionHelper.Serialize(table, isIndented: true, minimise: true);
                File.WriteAllText(NewTableFilePath(table.Detail.Name), json);
            }
        }
    }
}
