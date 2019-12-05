﻿using NoDb.Data.Domain.Converters;
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
        public DotNetCoreProject DotNetCoreProject { get; set; }

        public NoDbSetting()
        {
            Connections = new List<NoDbSettingConnection>();
            DotNetCoreProject = new DotNetCoreProject();
            Schema = "dbo";
            ConnectionType = NoDbConnectionType.Mssql;
            ConnectionName = "MainConnection";
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

    public class DotNetCoreProject
    {
        [Category("Entity")]
        public string EntityNamespaces { get; set; }
        [Category("Entity")]
        public string ElasticEntityBase { get; set; }
        [Category("Entity")]
        public string EntityBase { get; set; }

        [Category("Context")]
        public string DbContextNamespaces { get; set; }

        [Category("Repository")]
        public string ElasticRepoBase { get; set; }
        [Category("Repository")]
        public string EntityRepoBase { get; set; }
        [Category("Repository")]
        public string RepositoryNamespaces { get; set; }
        [Category("Repository")]
        public string EntityRepoInterfaceBase { get; set; }

        [Category("Service")]
        public string ServiceNamespaces { get; set; }
        [Category("Service")]
        public string EntityServiceBase { get; set; }
        [Category("Service")]
        public string EntityServiceInterfaceBase { get; set; }

        public DotNetCoreProject()
        {
            EntityNamespaces = "using CoreCommon.Data.Domain.Entitites;\r\nusing CoreCommon.Data.Domain.Enums;";
            RepositoryNamespaces = "using CoreCommon.Data.Domain.Entitites;\r\nusing CoreCommon.Data.Domain.Enums;\r\nusing CoreCommon.Data.EntityFrameworkBase.Base;\r\nusing CoreCommon.Data.ElasticSearch.Base;\r\nusing CoreCommon.Data.Domain.Business;";
            ServiceNamespaces = "using CoreCommon.Business.Service.Base;\r\nusing CoreCommon.Data.Domain.Business;\r\nusing CoreCommon.Data.Domain.Entitites;\r\nusing CoreCommon.Data.Domain.Enums;";
            DbContextNamespaces = "CoreCommon.Data.EntityFrameworkBase.Base";
            EntityBase = "IEntityBase";
            EntityRepoBase = "EntityFrameworkBaseRepository";
            EntityServiceBase = "BusinessLogicBase";
            ElasticEntityBase = "IElasticSearchEntity";
            ElasticRepoBase = "ElasticSearchRepositoryBase";
            EntityRepoInterfaceBase = "IRepositoryBase";
            EntityServiceInterfaceBase = "IBusinessLogicBase";
        }

        public override string ToString()
        {
            return ".NET Core Project";
        }
    }
}
