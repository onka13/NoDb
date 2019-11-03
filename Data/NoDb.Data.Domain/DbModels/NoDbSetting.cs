using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbSetting
    {
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbSettingConnection> Connections { get; set; }

        public string SettingsKey { get; set; }

        public string Schema { get; set; }

        public NoDbSetting()
        {
            Connections = new List<NoDbSettingConnection>();
        }

        public override string ToString()
        {
            return SettingsKey ?? "?";
        }
    }

    public class NoDbSettingConnection
    {
        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public NoDbConnectionType ConnectionType { get; set; }

        public override string ToString()
        {
            return ConnectionType + "-" + Name;
        }
    }
}
