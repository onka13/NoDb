using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System;

namespace NoDb.Business.Service.Managers
{
    public class NoDbHelper
    {
        public static bool IsNumber(NoDbDataType dataType)
        {
            return dataType == NoDbDataType.BYTE || dataType == NoDbDataType.DECIMAL || dataType == NoDbDataType.FLOAT || dataType == NoDbDataType.INT
                     || dataType == NoDbDataType.LONG || dataType == NoDbDataType.SHORT;
        }

        public static bool IsDate(NoDbDataType dataType)
        {
            return dataType == NoDbDataType.DATE || dataType == NoDbDataType.DATETIME || dataType == NoDbDataType.TIMESPAN;
        }

        public static string GetCondition(string val1, string val2, NoDbSearchSign sign)
        {
            switch (sign)
            {
                case NoDbSearchSign.Equal:
                    return string.Format("{0}.Equals({1})", val1, val2);
                case NoDbSearchSign.Contain:
                    return string.Format("{0}.ToLower().Contains({1}.ToLower())", val1, val2);
                case NoDbSearchSign.Less:
                    return string.Format("{0} < {1}", val1, val2);
                case NoDbSearchSign.LessEq:
                    return string.Format("{0} <= {1}", val1, val2);
                case NoDbSearchSign.Greater:
                    return string.Format("{0} > {1}", val1, val2); ;
                case NoDbSearchSign.GreaterEq:
                    return string.Format("{0} >= {1}", val1, val2); ;
                default:
                    throw new Exception("not valid sign");
            }
        }

        public static string ColumnDataType(NoDbColumn column)
        {
            if (column == null) return "";
            if (!string.IsNullOrEmpty(column.EnumName))
            {
                return column.EnumName + (column.Required ? "" : "?");
            }
            string output;
            switch (column.DataType)
            {
                case NoDbDataType.BYTE:
                    output = "byte";
                    break;
                case NoDbDataType.SHORT:
                    output = "short";
                    break;
                case NoDbDataType.INT:
                    output = "int";
                    break;
                case NoDbDataType.LONG:
                    output = "long";
                    break;
                case NoDbDataType.BOOL:
                    output = "bool";
                    break;
                case NoDbDataType.FLOAT:
                    output = "double";
                    break;
                case NoDbDataType.DECIMAL:
                    output = "decimal";
                    break;
                case NoDbDataType.DATE:
                    output = "DateTime";
                    break;
                case NoDbDataType.DATETIME:
                    output = "DateTime";
                    break;
                case NoDbDataType.TIMESPAN:
                    output = "TimeSpan";
                    break;
                case NoDbDataType.GUID:
                    output = "Guid";
                    break;
                case NoDbDataType.STRING:
                    output = "string";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (column.DataType != NoDbDataType.STRING && !column.Required)
            {
                output += "?";
            }

            return output;
        }
    }
}
