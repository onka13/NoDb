﻿using NoDb.Business.Service.Managers;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using NoDb.Data.Domain.SearchModels;
using System.Collections.Generic;

namespace NoDb.Business.Service.Services
{
    public class SearchService
    {
        NoDbService _noDbService;

        public SearchService(NoDbService noDbService)
        {
            _noDbService = noDbService;
        }

        public NoDbSearchItem GetDefaultSearchItem(NoDbTable table, List<NoDbSearchItem> currentItems)
        {
            var defaultItem = new NoDbSearchItem
            {
                RepositoryMethod = "Search" + (currentItems?.Count > 0 ? "_" + currentItems?.Count : "")
            };
            foreach (var column in table.Columns)
            {
                defaultItem.DisplayedColumns.Add(new NoDbSearchDisplayedColumnDetail
                {
                    ColumnName = column.Name
                });

                if (NoDbHelper.IsDate(column.DataType))
                {
                    defaultItem.Columns.Add(new NoDbSearchColumnDetail
                    {
                        ColumnName = column.Name,
                        Title = column.Name + ">",
                        Sign = NoDbSearchSign.Greater
                    });
                    defaultItem.Columns.Add(new NoDbSearchColumnDetail
                    {
                        ColumnName = column.Name,
                        Title = column.Name + "<=",
                        Sign = NoDbSearchSign.LessEq
                    });
                    continue;
                }

                if (column.DataType == NoDbDataType.STRING)
                {
                    defaultItem.Columns.Add(new NoDbSearchColumnDetail
                    {
                        ColumnName = column.Name,
                        Sign = NoDbSearchSign.Contain
                    });
                    continue;
                }
                defaultItem.Columns.Add(new NoDbSearchColumnDetail
                {
                    ColumnName = column.Name,
                    Sign = NoDbSearchSign.Equal
                });
            }

            return defaultItem;
        }
    }
}
