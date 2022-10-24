using System;
using System.Collections.Generic;
using System.Text;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;

namespace NoDb.Business.Service.Queries
{
    public abstract class NoDbQueryBase// : INoDbQuery
    {
        public abstract string Escape(string name);
        public abstract string AddColumnQuery(NoDbTable table, NoDbColumn column);
        public abstract string ColumnDataType(NoDbColumn column);
        public abstract NoDbDataType DbTypeToNoDbDataType(string columnDbType);
        public abstract string ColumnQuery(NoDbColumn column);
        public abstract string CreateIndexQuery(NoDbTable table, NoDbIndex index);
        public abstract string CreateRelationQuery(NoDbTable table, NoDbRelation relation);
        public abstract string CreateTableQuery(NoDbTable table, bool onlyTable = false);
        public abstract string CreateAllIndexQuery(NoDbTable table);
        public abstract string CreateAllRelationQuery(NoDbTable table);
        public abstract string DeleteRelationQuery(NoDbTable table, NoDbRelation relation);
        public abstract string DropColumnQuery(NoDbTable table, NoDbColumn column);
        public abstract string DropIndexQuery(NoDbTable table, NoDbIndex index);
        public abstract string DropTableQuery(NoDbTable table);
        public abstract string RenameColumnQuery(NoDbTable table, NoDbColumn oldColumn, NoDbColumn newColumn);
        public abstract string RenameIndexQuery(NoDbTable table, NoDbIndex oldIndex, NoDbIndex newIndex);
        public abstract string RenameRelationQuery(NoDbTable table, NoDbRelation oldRelation, NoDbRelation newRelation);
        public abstract string TableConstraintPrimaryKeyQuery(NoDbTable table);
        public abstract string UpdateColumnQuery(NoDbTable table, NoDbColumn column);

        public string GetSchema(NoDbTable table)
        {
            return table.Detail.Schema ?? "dbo";
        }
    }
}
