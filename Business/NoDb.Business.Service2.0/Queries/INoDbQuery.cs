using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoDb.Business.Service.Queries
{
    public interface INoDbQuery
    {
        abstract string DropTableQuery(NoDbTable table);
        abstract string CreateTableQuery(NoDbTable table);
        abstract string DropIndexQuery(NoDbTable table, NoDbIndex index);
        abstract string CreateIndexQuery(NoDbTable table, NoDbIndex index);
        abstract string RenameIndexQuery(NoDbTable table, NoDbIndex oldIndex, NoDbIndex newIndex);
        abstract string DropColumnQuery(NoDbTable table, NoDbColumn column);
        abstract string AddColumnQuery(NoDbTable table, NoDbColumn column);
        abstract string RenameColumnQuery(NoDbTable table, NoDbColumn oldColumn, NoDbColumn newColumn);
        abstract string UpdateColumnQuery(NoDbTable table, NoDbColumn column);
        abstract string CreateRelationQuery(NoDbTable table, NoDbRelation relation);
        abstract string DeleteRelationQuery(NoDbTable table, NoDbRelation relation);
        abstract string RenameRelationQuery(NoDbTable table, NoDbRelation oldRelation, NoDbRelation newRelation);
        abstract string TableConstraintPrimaryKeyQuery(NoDbTable table);
        abstract string ColumnQuery(NoDbColumn column);
        abstract string ColumnDataType(NoDbColumn column);

    }
}
