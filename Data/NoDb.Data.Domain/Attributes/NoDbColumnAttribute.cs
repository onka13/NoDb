using System;

namespace NoDb.Data.Domain.Attributes
{
    public enum NoDbColumnType
    {
        Column,
        ForeignColumn,
        ForeignColumnPrimaryKey,
    }
    public class NoDbColumnAttribute : Attribute
    {
        public NoDbColumnType ColumnType { get; set; }
        public NoDbColumnAttribute()
        {
            ColumnType = NoDbColumnType.Column;
        }
        public NoDbColumnAttribute(NoDbColumnType columnType)
        {
            ColumnType = columnType;
        }
    }
}
