using NoDb.Data.Domain.DbModels;
using System.Collections.Generic;

namespace NoDb.Data.Domain.Converters
{
    public static class StaticManager
    {
        public static List<NoDbTable> Tables { get; set; }

        public static List<NoDbEnumDetail> Enums { get; set; }

        public static List<NoDbProject> Projects { get; set; }

        public static NoDbTable SelectedTable { get; set; }

        public static string SelectedForeignTable { get; set; }
    }
}
