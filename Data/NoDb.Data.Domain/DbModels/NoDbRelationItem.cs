using NoDb.Data.Domain.Attributes;
using NoDb.Data.Domain.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbRelationItem
    {
        [Description("Column name")]
        [Category("General")]
        [TypeConverter(typeof(ColumnConverter))]
        public string ColumnName { get; set; }

        [Description("Foreign column")]
        [NoDbColumn(NoDbColumnType.ForeignColumnPrimaryKey)]
        [Category("General")]
        [TypeConverter(typeof(ColumnConverter))]
        public string ForeignColumn { get; set; }

        public override string ToString()
        {
            return ColumnName + " - " + ForeignColumn;
        }
    }
}
