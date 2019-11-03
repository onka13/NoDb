using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoDb.Business.Service.Templates
{
    public class TableTemplates
    {
        public static Dictionary<string, Func<string, NoDbTable>> DefinedTemplates = new Dictionary<string, Func<string, NoDbTable>>
        {
            { "Default" ,  DefaultTable },
            { "Id-Name" ,  IdNameTable }
        };

        public static NoDbTable Get(string tableName, string template)
        {
            if (!DefinedTemplates.ContainsKey(template))
            {
                template = "Default";
            }
            return DefinedTemplates[template](tableName);
        }

        public static NoDbTable DefaultTable(string tableName)
        {
            return new NoDbTable
            {
                Detail = new NoDbTableDetail
                {
                    Name = tableName
                },
                Indices = new List<NoDbIndex>
                {
                    new NoDbIndex
                    {
                        Name = "PK_" + tableName + "_Id",
                        IsPrimaryKey = true,
                        Columns = new List<NoDbIndexColumn>
                        {
                            new NoDbIndexColumn() {ColumnName = "Id"}
                        }
                    }
                },
                Columns = new List<NoDbColumn>()
                {
                    new NoDbColumn
                    {
                        Name = "Id",
                        DataType = NoDbDataType.INT,
                        Required = true,
                        IsAutoIncrement = true
                    }
                },
                Relations = new List<NoDbRelation>()
            };
        }

        public static NoDbTable IdNameTable(string tableName)
        {
            return new NoDbTable
            {
                Detail = new NoDbTableDetail
                {
                    Name = tableName
                },
                Indices = new List<NoDbIndex>
                {
                    new NoDbIndex
                    {
                        Name = "PK_" + tableName + "_Id",
                        IsPrimaryKey = true,
                        Columns = new List<NoDbIndexColumn>
                        {
                            new NoDbIndexColumn() {ColumnName = "Id"}
                        }
                    },
                    new NoDbIndex
                    {
                        Name = "PK_" + tableName + "_Name",
                        Columns = new List<NoDbIndexColumn>
                        {
                            new NoDbIndexColumn() {ColumnName = "Name"}
                        }
                    }
                },
                Columns = new List<NoDbColumn>()
                {
                    new NoDbColumn
                    {
                        Name = "Id",
                        DataType = NoDbDataType.INT,
                        Required = true,
                        IsAutoIncrement = true
                    },
                    new NoDbColumn
                    {
                        Name = "Name",
                        DataType = NoDbDataType.STRING,
                        Required = true,
                    }
                },
                Relations = new List<NoDbRelation>()
            };
        }
    }
}
