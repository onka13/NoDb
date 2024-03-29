﻿using CoreCommon.Infrastructure.Helpers;
using NoDb.Business.Service.Templates;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NoDb.Business.Service.Services
{
    public class TableService
    {
        NoDbService _noDbService;
        public string TableFilePath
        {
            get
            {
                return _noDbService.NoDbFolder + Path.DirectorySeparatorChar + "Tables.json";
            }
        }

        public List<NoDbTable> Tables { get; set; }

        public TableService(NoDbService noDbService)
        {
            _noDbService = noDbService;
            ReadFromSettingsFolder();
        }

        public void ReadFromSettingsFolder()
        {
            if (!File.Exists(TableFilePath))
            {
                Tables = new List<NoDbTable>();
            }
            else
            {
                var json = File.ReadAllText(TableFilePath);
                Tables = ConversionHelper.Deserialize<List<NoDbTable>>(json).OrderBy(x => x?.Detail?.Name).ToList();
            }
        }

        public NoDbTable New(NoDbTable table)
        {
            Tables.Add(table);
            WriteToFile();
            return table;
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
            if(saveRevision)
            {
                _noDbService.RevisionService.SaveRevision(originalTable, updatedTable);
            }

            Tables[index] = updatedTable;
            WriteToFile();
        }
        
        private void WriteToFile()
        {
            var json = ConversionHelper.Serialize(Tables, isIndented: true);
            File.WriteAllText(TableFilePath, json);
        }
    }
}
