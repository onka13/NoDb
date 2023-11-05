using CoreCommon.Data.EntityFrameworkBase.Models;
using NoDb.Business.Service.Managers;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NoDb.Business.Service.Services
{
    public class ImportService
    {
        NoDbService _noDbService;

        public ImportService(NoDbService noDbService)
        {
            _noDbService = noDbService;
        }

        public NoDbColumn ConvertToNoDbColumn(InformationSchemaColumn informationSchemaColumn)
        {
            var column = new NoDbColumn();
            column.Name = informationSchemaColumn.ColumnName;
            column.Length = informationSchemaColumn.CharacterMaximumLength ?? 0;
            column.Precision = informationSchemaColumn.NumericPrecision ?? 0;
            column.Scale = informationSchemaColumn.NumericScale ?? 0;
            column.Required = informationSchemaColumn.IsNullable == "YES";
            return column;
        }

        public NoDbTable ConvertToNoDbTable(NoDbConnectionType dbConnectionType, InformationSchemaTable informationSchemaTable, List<InformationSchemaColumn> informationSchemaColumns)
        {
            var table = new NoDbTable();
            table.Detail.Name = informationSchemaTable.TableName;
            table.Detail.SystemName = informationSchemaTable.TableName;
            table.Detail.Schema = informationSchemaTable.TableSchema;
            table.Detail.ConnectionType = dbConnectionType;

            var queryService = QueryManager.GetNoDbQueryService(dbConnectionType);

            var pattern = new Regex(@"\W");

            foreach (var informationSchemaColumn in informationSchemaColumns)
            {
                var column = new NoDbColumn();
                column.ShortName = informationSchemaColumn.ColumnName;
                column.Name = pattern.Replace(informationSchemaColumn.ColumnName, string.Empty);
                column.Length = informationSchemaColumn.CharacterMaximumLength ?? 0;
                column.Precision = informationSchemaColumn.NumericPrecision ?? 0;
                column.Scale = informationSchemaColumn.NumericScale ?? 0;
                column.Required = informationSchemaColumn.IsNullable == "NO";
                try
                {
                    column.DataType = queryService.DbTypeToNoDbDataType(informationSchemaColumn.DataType);
                }
                catch
                {
                    column.DataType = NoDbDataType.OBJECT;
                }
                table.Columns.Add(column);

                if (table.Detail.TitleColumn == null)
                {
                    table.Detail.TitleColumn = column.Name;
                }
            }

            return table;
        }

        public void SyncFromDb(NoDbSetting noDbSetting, NoDbConnectionType dbConnectionType, List<InformationSchemaTable> dbTables, List<InformationSchemaColumn> dbColumns)
        {
            var allNoDbTables = dbTables.Select(x => ConvertToNoDbTable(dbConnectionType, x, dbColumns.Where(y => y.TableName == x.TableName).ToList()));

            foreach (var noDbTable in allNoDbTables)
            {
                var tableOriginal = _noDbService.TableService.Tables.FirstOrDefault(x => x.Detail.SystemName == noDbTable.Detail.SystemName);
                if (tableOriginal == null)
                {
                    _noDbService.TableService.New(noDbTable);
                    continue;
                }

                if (tableOriginal.Detail.ConnectionType == NoDbConnectionType.None)
                {
                    if (noDbSetting.ConnectionType != noDbTable.Detail.ConnectionType)
                    {
                        continue;
                    }
                }
                else if (tableOriginal.Detail.ConnectionType != noDbTable.Detail.ConnectionType)
                {
                    continue;
                }

                SyncDbTableWithNoDbTable(tableOriginal, noDbTable);
            }
        }

        public void SyncDbTableWithNoDbTable(NoDbTable tableOriginal, NoDbTable table)
        {
            var isUpdated = false;
            foreach (var column in table.Columns)
            {
                var originalColumn = tableOriginal.Columns.FirstOrDefault(x =>
                column.ShortName.Equals(x.Name, System.StringComparison.InvariantCultureIgnoreCase) || column.ShortName.Equals(x.ShortName, System.StringComparison.InvariantCultureIgnoreCase));
                if (originalColumn == null)
                {
                    tableOriginal.Columns.Add(column);
                    isUpdated = true;
                    continue;
                }
                if (column.DataType == NoDbDataType.STRING && column.Length != originalColumn.Length)
                {
                    originalColumn.Length = column.Length;
                    isUpdated = true;
                }
                if (column.Required != originalColumn.Required)
                {
                    originalColumn.Required = column.Required;
                    isUpdated = true;
                }
                if (column.DataType != originalColumn.DataType)
                {
                    originalColumn.DataType = column.DataType;
                    isUpdated = true;
                }
            }

            if (isUpdated)
            {
                _noDbService.TableService.UpdateTable(tableOriginal, false);
            }
        }
    }
}
