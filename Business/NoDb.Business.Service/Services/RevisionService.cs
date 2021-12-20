using CoreCommon.Infra.Helpers;
using NoDb.Business.Service.Managers;
using NoDb.Business.Service.Templates;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using NoDb.Data.Domain.RevisionModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NoDb.Business.Service.Services
{
    public class RevisionService
    {
        NoDbService _noDbService;

        public string RevisionFolder
        {
            get
            {
                return _noDbService.NoDbFolder + Path.DirectorySeparatorChar + "Revisions";
            }
        }

        public RevisionService(NoDbService noDbService)
        {
            _noDbService = noDbService;
        }

        public List<string> GetRevisionFiles()
        {
            try
            {
                return Directory.GetFiles(RevisionFolder).Select(x => x.Replace(RevisionFolder + Path.DirectorySeparatorChar, "").Replace(".json", "")).OrderByDescending(x => x).ToList();
            }
            catch
            {
                // ignored
            }
            return new List<string>();
        }

        public NoDbRevision ReadRevision(string revisionFileName)
        {
            var revisionFilePath = RevisionFolder + Path.DirectorySeparatorChar + revisionFileName + ".json";
            var json = File.ReadAllText(revisionFilePath);
            return ConversionHelper.Deserialize<NoDbRevision>(json);
        }

        public void DeleteRevision(string revisionFileName)
        {
            var revisionFilePath = RevisionFolder + Path.DirectorySeparatorChar + revisionFileName + ".json";
            File.Delete(revisionFilePath);
        }

        public void SaveRevision(NoDbTable oldTable, NoDbTable newTable)
        {
            var revision = CheckRevision(oldTable, newTable);
            SaveRevision(revision);
        }

        public void SaveRevision(NoDbRevision revision)
        {
            if (revision == null) return;
            var revisionPath = RevisionFolder;
            if (!Directory.Exists(revisionPath))
            {
                Directory.CreateDirectory(revisionPath);
            }
            revisionPath += Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + (revision.NewTable ?? revision.OldTable)?.Detail.Name + ".json";
            var jsonRev = ConversionHelper.Serialize(revision, isIndented: true);
            File.WriteAllText(revisionPath, jsonRev);
        }

        public static NoDbRevision CheckRevision(NoDbTable oldTable, NoDbTable newTable)
        {
            var revision = new NoDbRevision
            {
                OldTable = oldTable,
                NewTable = newTable
            };

            if (oldTable == null)
            {
                revision.Revisions.Add(new NoDbRevisionDetail
                {
                    Action = NoDbRevisionAction.Added,
                    ObjectType = NoDbRevisionType.Table,
                    NewValue = newTable
                });
                return revision;
            }

            if (newTable == null)
            {
                revision.Revisions.Add(new NoDbRevisionDetail
                {
                    Action = NoDbRevisionAction.Removed,
                    ObjectType = NoDbRevisionType.Table,
                    OldValue = oldTable
                });
                return revision;
            }

            foreach (var oldColumn in oldTable.Columns)
            {
                var newColumn = newTable.Columns.FirstOrDefault(x => x.Hash == oldColumn.Hash);
                if (newColumn == null)
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Removed,
                        ObjectType = NoDbRevisionType.Column,
                        OldValue = oldColumn
                    });
                    continue;
                }
                if (oldColumn.Name != newColumn.Name)
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Renamed,
                        ObjectType = NoDbRevisionType.Column,
                        OldValue = oldColumn,
                        NewValue = newColumn,
                    });
                    // check other revisions after renaming 
                }
                if (oldColumn.DataType != newColumn.DataType ||
                    oldColumn.Length != newColumn.Length ||
                    oldColumn.Precision != newColumn.Precision ||
                    oldColumn.Scale != newColumn.Scale ||
                    oldColumn.Required != newColumn.Required ||
                    oldColumn.IsAutoIncrement != newColumn.IsAutoIncrement
                    )
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Updated,
                        ObjectType = NoDbRevisionType.Column,
                        OldValue = oldColumn,
                        NewValue = newColumn,
                    });
                }
                // TODO: default value
            }

            foreach (var newColumn in newTable.Columns)
            {
                var oldColum = oldTable.Columns.FirstOrDefault(x => x.Hash == newColumn.Hash);
                if (oldColum == null)
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Added,
                        ObjectType = NoDbRevisionType.Column,
                        NewValue = newColumn
                    });
                }
            }

            foreach (var oldIndex in oldTable.Indices)
            {
                var newIndex = newTable.Indices.FirstOrDefault(x => x.Hash == oldIndex.Hash);
                if (newIndex == null)
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Removed,
                        ObjectType = NoDbRevisionType.Index,
                        OldValue = oldIndex
                    });
                    continue;
                }
                if (oldIndex.Name != newIndex.Name)
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Renamed,
                        ObjectType = NoDbRevisionType.Index,
                        OldValue = oldIndex,
                        NewValue = newIndex
                    });
                    // güncelleme olmuşsa aşağıdaki gibi update ekle
                }

                if (oldIndex.IsPrimaryKey != newIndex.IsPrimaryKey ||
                    oldIndex.IsUnique != newIndex.IsUnique ||
                    oldIndex.Columns.Any(x => newIndex.Columns.FirstOrDefault(y => y.ColumnName == x.ColumnName && y.Sort == x.Sort) == null)
                    )
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Updated,
                        ObjectType = NoDbRevisionType.Index,
                        OldValue = oldIndex,
                        NewValue = newIndex
                    });
                }
            }

            foreach (var newIndex in newTable.Indices)
            {
                var oldIndex = oldTable.Indices.FirstOrDefault(x => x.Hash == newIndex.Hash);
                if (oldIndex == null)
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Added,
                        ObjectType = NoDbRevisionType.Index,
                        NewValue = newIndex
                    });
                }
            }

            foreach (var oldRelation in oldTable.Relations)
            {
                var newRelation = newTable.Relations.FirstOrDefault(x => x.Hash == oldRelation.Hash);
                if (newRelation == null)
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Removed,
                        ObjectType = NoDbRevisionType.Relation,
                        OldValue = oldRelation
                    });
                    continue;
                }
                if (oldRelation.Name != newRelation.Name)
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Renamed,
                        ObjectType = NoDbRevisionType.Relation,
                        OldValue = oldRelation,
                        NewValue = newRelation
                    });
                    // güncelleme varsa sonra ekle
                }

                string itemsOld = string.Join(",", oldRelation.Items.Select(x => x.ToString()));
                string itemsNew = string.Join(",", newRelation.Items.Select(x => x.ToString()));

                if (
                    oldRelation.ForeignTable != newRelation.ForeignTable ||
                     oldRelation.DeleteRule != newRelation.DeleteRule ||
                     oldRelation.UpdateRule != newRelation.UpdateRule ||
                     itemsOld != itemsNew
                    )
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Updated,
                        ObjectType = NoDbRevisionType.Relation,
                        OldValue = oldRelation,
                        NewValue = newRelation
                    });
                }
            }

            foreach (var newRelation in newTable.Relations)
            {
                var oldRelation = oldTable.Relations.FirstOrDefault(x => x.Hash == newRelation.Hash);
                if (oldRelation == null)
                {
                    revision.Revisions.Add(new NoDbRevisionDetail
                    {
                        Action = NoDbRevisionAction.Added,
                        ObjectType = NoDbRevisionType.Relation,
                        NewValue = newRelation
                    });
                }
            }

            return revision;
        }

        public object GetDetailByType(NoDbRevisionDetail detail, object value)
        {
            if (value == null) return null;

            if (detail.ObjectType == NoDbRevisionType.Table)
            {
                return ConversionHelper.ConvertTo<NoDbTable>(value);
            }
            if (detail.ObjectType == NoDbRevisionType.Column)
            {
                return ConversionHelper.ConvertTo<NoDbColumn>(value);
            }
            if (detail.ObjectType == NoDbRevisionType.Index)
            {
                return ConversionHelper.ConvertTo<NoDbIndex>(value);
            }
            if (detail.ObjectType == NoDbRevisionType.Relation)
            {
                return ConversionHelper.ConvertTo<NoDbRelation>(value);
            }
            return value;
        }

        public string GetRevisionQuery(NoDbRevision revision, NoDbRevisionDetail detail, NoDbConnectionType connectionType)
        {
            var queryService = QueryManager.GetNoDbQueryService(connectionType);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("-- Action: {0}, Type: {1}\n", detail.Action, detail.ObjectType);
            if (detail.ObjectType == NoDbRevisionType.Table)
            {
                var oldTable = detail.OldValue is NoDbTable ? (NoDbTable)detail.OldValue : ConversionHelper.ConvertTo<NoDbTable>(detail.OldValue);
                var newTable = detail.NewValue is NoDbTable ? (NoDbTable)detail.NewValue : ConversionHelper.ConvertTo<NoDbTable>(detail.NewValue);
                if (detail.Action == NoDbRevisionAction.Removed)
                {
                    stringBuilder.Append(queryService.DropTableQuery(oldTable) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Added)
                {
                    stringBuilder.Append(queryService.CreateTableQuery(newTable) + "\n");
                }
            }
            else if (detail.ObjectType == NoDbRevisionType.Column)
            {
                var oldColumn = ConversionHelper.ConvertTo<NoDbColumn>(detail.OldValue);
                var newColumn = ConversionHelper.ConvertTo<NoDbColumn>(detail.NewValue);
                if (detail.Action == NoDbRevisionAction.Removed)
                {
                    stringBuilder.Append(queryService.DropColumnQuery(revision.NewTable, oldColumn) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Added)
                {
                    stringBuilder.Append(queryService.AddColumnQuery(revision.NewTable, newColumn) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Renamed)
                {
                    stringBuilder.Append(queryService.RenameColumnQuery(revision.NewTable, oldColumn, newColumn) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Updated)
                {
                    stringBuilder.Append(queryService.UpdateColumnQuery(revision.NewTable, newColumn) + "\n");
                }
            }
            else if (detail.ObjectType == NoDbRevisionType.Index)
            {
                var oldIndex = ConversionHelper.ConvertTo<NoDbIndex>(detail.OldValue);
                var newIndex = ConversionHelper.ConvertTo<NoDbIndex>(detail.NewValue);
                if (detail.Action == NoDbRevisionAction.Removed)
                {
                    stringBuilder.Append(queryService.DropIndexQuery(revision.NewTable, oldIndex) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Added)
                {
                    stringBuilder.Append(queryService.CreateIndexQuery(revision.NewTable, newIndex) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Renamed)
                {
                    stringBuilder.Append(queryService.RenameIndexQuery(revision.NewTable, oldIndex, newIndex) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Updated)
                {
                    stringBuilder.Append(queryService.DropIndexQuery(revision.NewTable, oldIndex) + "\n");
                    stringBuilder.Append(queryService.CreateIndexQuery(revision.NewTable, newIndex) + "\n");
                }
            }
            else if (detail.ObjectType == NoDbRevisionType.Relation)
            {
                var oldRelation = ConversionHelper.ConvertTo<NoDbRelation>(detail.OldValue);
                var newRelation = ConversionHelper.ConvertTo<NoDbRelation>(detail.NewValue);
                if (detail.Action == NoDbRevisionAction.Removed)
                {
                    stringBuilder.AppendFormat(queryService.DeleteRelationQuery(revision.NewTable, oldRelation) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Added)
                {
                    stringBuilder.AppendFormat(queryService.CreateRelationQuery(revision.NewTable, newRelation) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Renamed)
                {
                    stringBuilder.AppendFormat(queryService.RenameRelationQuery(revision.NewTable, oldRelation, newRelation) + "\n");
                }
                else if (detail.Action == NoDbRevisionAction.Updated)
                {
                    stringBuilder.AppendFormat(queryService.DeleteRelationQuery(revision.NewTable, oldRelation) + "\n");
                    stringBuilder.AppendFormat(queryService.CreateRelationQuery(revision.NewTable, newRelation) + "\n");
                }
            }
            return stringBuilder.ToString();
        }
    }
}
