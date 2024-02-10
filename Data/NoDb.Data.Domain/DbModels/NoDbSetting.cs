using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbSetting
    {
        [Category("DB")]
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbSettingConnection> Connections { get; set; }

        [Description("Unique settings key")]
        [Category("General")]
        public string SettingsKey { get; set; }

        [Description("Default schema")]
        [Category("DB")]
        public string Schema { get; set; }

        [Description("Default connection type")]
        [Category("DB")]
        public NoDbConnectionType ConnectionType { get; set; }

        [Description("Default connection name")]
        [Category("DB")]
        public string ConnectionName { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        [Category("Project")]
        public ProjectSetting ProjectSetting { get; set; }

        [Category("Project")]
        public string JsPanelProjectPath { get; set; }
        
        [Category("Project")]
        public NoDbAdminPanelType JsPanelProjectType { get; set; }

        [Category("General")]
        public string[] ExcludeTablePrefixes { get; set; }

        public NoDbSetting()
        {
            Connections = new List<NoDbSettingConnection>();
            ProjectSetting = new ProjectSetting();
            Schema = "dbo";
            ConnectionType = NoDbConnectionType.Mssql;
            ConnectionName = "MainConnection";
            JsPanelProjectType = NoDbAdminPanelType.Reactv2;
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

    public class ProjectSetting
    {
        [Description("Root namespace")]
        [Category("General")]
        public string RootNamespace { get; set; }
        
        [Description("CoreCommon Data project namespace")]
        [Category("General")]
        public string CoreCommonDataNamespace { get; set; }

        [Category("Search")]
        public string SearchRequestBase { get; set; }

        public string DomainProject { get; set; }

        public string ApplicationProject { get; set; }

        public string InfrastructureProject { get; set; }

        public string ApiProject { get; set; }

        public ProjectSetting()
        {
            CoreCommonDataNamespace = "CoreCommon.Data";
        }

        public override string ToString()
        {
            return "Project Settings";
        }
    }
}
