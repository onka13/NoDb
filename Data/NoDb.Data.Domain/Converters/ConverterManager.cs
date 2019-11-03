using NoDb.Data.Domain.DbModels;
using System.Collections.Generic;
using System.Linq;

namespace NoDb.Data.Domain.Converters
{
    public static class ConverterManager
    {
        private static NoDbEnum NoDbEnum;
        private static List<NoDbTable> Tables;
        private static NoDbTable SelectedTable;
        private static string SelectedForeignTable;

        #region " Enums"

        public static List<NoDbEnumDetail> GetEnums()
        {
            if (NoDbEnum == null) NoDbEnum = new NoDbEnum();
            return NoDbEnum.EnumList;
        }

        public static void SetEnums(NoDbEnum noDbEnum)
        {
            NoDbEnum = noDbEnum;
        }
        
        #endregion

        #region " Tables "

        public static List<NoDbTable> GetTables()
        {
            if (Tables == null) Tables = new List<NoDbTable>();
            return Tables;
        }

        public static void SetTables(List<NoDbTable> tables)
        {
            Tables = tables;
        }

        public static NoDbTable GetTable(string name)
        {
            return GetTables().FirstOrDefault(x => x.Detail.Name == name);
        }

        public static List<string> GetTableNames()
        {
            return GetTables().Select(x => x.Detail.Name).ToList();
        }

        public static NoDbTable GetSelectedTable()
        {
            return SelectedTable;
        }

        public static void SetSelectedTable(NoDbTable table)
        {
            SelectedTable = table;
        }

        public static NoDbTable GetSelectedForeignTable()
        {
            return GetTable(SelectedForeignTable);
        }

        public static void SetSelectedForeignTable(string table)
        {
            SelectedForeignTable = table;
        }

        #endregion
    }
}
