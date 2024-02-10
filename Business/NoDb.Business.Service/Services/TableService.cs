using CoreCommon.Infrastructure.Helpers;
using NoDb.Business.Service.Templates;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NoDb.Business.Service.Services
{
    public class TableService
    {
        private readonly NoDbService noDbService;        

        public TableService(NoDbService noDbService)
        {
            this.noDbService = noDbService;
            ReadFromSettingsFolder();
        }

        public List<NoDbTable> Tables { get; private set; }

        public void ReadFromSettingsFolder()
        {
            if (!File.Exists(noDbService.TableFilePath))
            {
                Tables = new List<NoDbTable>();
            }
            else
            {
                var json = File.ReadAllText(noDbService.TableFilePath);
                Tables = ConversionHelper.Deserialize<List<NoDbTable>>(json).OrderBy(x => x?.Detail?.Name).ToList();
            }

            StaticManager.Tables = Tables;
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
            noDbService.RevisionService.SaveRevision(table, null);
            Tables.Remove(table);
            WriteToFile();
        }

        public void UpdateTable(NoDbTable updatedTable, bool saveRevision = true)
        {
            if (string.IsNullOrWhiteSpace(updatedTable.Detail.TitleColumn))
            {
                throw new Exception("Please enter TitleColumn field!");
            }

            if (updatedTable.Columns.Count == 0)
            {
                throw new Exception("Please enter one column at least!");
            }
            
            if (updatedTable.Columns.Any(x => string.IsNullOrWhiteSpace(x.Name)))
            {
                throw new Exception("Empty column name not allowed!");
            }
            
            if (updatedTable.Columns.Select(x => x.Name).Distinct().Count() != updatedTable.Columns.Count)
            {
                throw new Exception("Duplicated column names not allowed!");
            }

            var index = Tables.FindIndex(x => x.Hash == updatedTable.Hash);
            var originalTable = Tables[index];
            if(saveRevision)
            {
                noDbService.RevisionService.SaveRevision(originalTable, updatedTable);
            }

            Tables[index] = updatedTable;
            WriteToFile();
        }
        
        private void WriteToFile()
        {
            var json = ConversionHelper.Serialize(Tables, isIndented: true);
            File.WriteAllText(noDbService.TableFilePath, json);
            StaticManager.Tables = Tables;
        }
    }
}
