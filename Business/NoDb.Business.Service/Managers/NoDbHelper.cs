using NoDb.Data.Domain.Enums;

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
    }
}
