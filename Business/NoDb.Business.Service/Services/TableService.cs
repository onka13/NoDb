using CoreCommon.Infrastructure.Helpers;
using Google.Protobuf.WellKnownTypes;
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
            Tables = new List<NoDbTable>();

            var files = Directory.GetFiles(noDbService.TableFolderPath);
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var table = ConversionHelper.Deserialize<NoDbTable>(json);
                Tables.Add(table);
            }

            Tables = Tables.OrderBy(x => x?.Detail?.Name).ToList();

            StaticManager.Tables = Tables;
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
            if (saveRevision)
            {
                noDbService.RevisionService.SaveRevision(originalTable, updatedTable);
            }

            Tables[index] = updatedTable;
            WriteToFile();
        }

        public static void UpgradeToVersion8(string from, string to)
        {
            // Transform tables.json
            string tablesJsonPath = Path.Combine(from, "Tables.json");
            string targetTablesFolder = Path.Combine(to, NoDbService.NODB_FOLDER_NAME, "Tables");
            Directory.CreateDirectory(targetTablesFolder);
            var jsonContent = File.ReadAllText(tablesJsonPath);
            var tables = ConversionHelper.Deserialize<List<NoDbTable>>(jsonContent);
            foreach (var table in tables)
            {
                var json = ConversionHelper.Serialize(table, isIndented: true, minimise: true);
                File.WriteAllText(Path.Combine(targetTablesFolder, table.Detail.Name + ".json"), json);
            }

            // Move enums
            string enumsPath = Path.Combine(from, "Enums.json");
            string targetEnumsFolder = Path.Combine(to, NoDbService.NODB_FOLDER_NAME, "Enums");
            Directory.CreateDirectory(targetEnumsFolder);
            var jsonEnum = File.ReadAllText(enumsPath);
            var enums = ConversionHelper.Deserialize<NoDbEnum>(jsonEnum);
            foreach (var @enum in enums.EnumList)
            {
                var json = ConversionHelper.Serialize(@enum, isIndented: true, minimise: true);
                File.WriteAllText(Path.Combine(targetEnumsFolder, @enum.Name + ".json"), json);
            }
        }

        private void WriteToFile()
        {
            foreach (var table in Tables)
            {
                table.Detail.Name = StringHelper.FirstCharToUpper(table.Detail.Name);
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    table.Columns[i].Name = StringHelper.FirstCharToUpper(table.Columns[i].Name);
                }

                for (int i = 0; i < table.Indices.Count; i++)
                {
                    for (int j = 0; j < table.Indices[i].Columns.Count; j++)
                    {
                        table.Indices[i].Columns[j].ColumnName = StringHelper.FirstCharToUpper(table.Indices[i].Columns[j].ColumnName);
                    }
                }

                for (int i = 0; i < table.Relations.Count; i++)
                {
                    for (int j = 0; j < table.Relations[i].Items.Count; j++)
                    {
                        table.Relations[i].Items[j].ColumnName = StringHelper.FirstCharToUpper(table.Relations[i].Items[j].ColumnName);
                        table.Relations[i].Items[j].ForeignColumn = StringHelper.FirstCharToUpper(table.Relations[i].Items[j].ForeignColumn);
                    }
                }

                for (int i = 0; i < table.SearchItems.Count; i++)
                {
                    for (int j = 0; j < table.SearchItems[i].Columns.Count; j++)
                    {
                        table.SearchItems[i].Columns[j].ColumnName = StringHelper.FirstCharToUpper(table.SearchItems[i].Columns[j].ColumnName);
                    }

                    for (int j = 0; j < table.SearchItems[i].AllColumns.Count; j++)
                    {
                        table.SearchItems[i].AllColumns[j].Name = StringHelper.FirstCharToUpper(table.SearchItems[i].AllColumns[j].Name);
                    }

                    for (int j = 0; j < table.SearchItems[i].DisplayedColumns.Count; j++)
                    {
                        table.SearchItems[i].DisplayedColumns[j].ColumnName = StringHelper.FirstCharToUpper(table.SearchItems[i].DisplayedColumns[j].ColumnName);
                        table.SearchItems[i].DisplayedColumns[j].RelationColumnName = StringHelper.FirstCharToUpper(table.SearchItems[i].DisplayedColumns[j].RelationColumnName);
                    }
                }

                var json = ConversionHelper.Serialize(table, isIndented: true, minimise: true);
                File.WriteAllText(Path.Combine(noDbService.TableFolderPath, table.Detail.Name + ".json"), json);
            }

            StaticManager.Tables = Tables;
        }
    }
}
