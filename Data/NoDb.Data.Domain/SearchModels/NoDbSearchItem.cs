﻿using NoDb.Data.Domain.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NoDb.Data.Domain.SearchModels
{
    public class NoDbSearchItem
    {
        [Category("General")]
        public string Description { get; set; }

        [Category("Advance")]
        public bool IsExportable { get; set; }

        [Category("Advance")]
        public bool IsEditable { get; set; }

        [Category("Advance")]
        public bool IsCreateable { get; set; }

        [Category("Advance")]
        public bool IsDeleteable { get; set; }

        [Category("Advance")]
        public bool HasDetail { get; set; }

        [Category("Advance")]
        public string RepositoryMethod { get; set; }

        [Category("UI")]
        public bool HideMenu { get; set; }

        [Category("General")]
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbSearchColumnDetail> Columns { get; set; }

        [Category("General")]
        [TypeConverter(typeof(NoDbCollectionTypeConverter))]
        public List<NoDbSearchDisplayedColumnDetail> DisplayedColumns { get; set; }

        public NoDbSearchItem()
        {
            Columns = new List<NoDbSearchColumnDetail>();
            DisplayedColumns = new List<NoDbSearchDisplayedColumnDetail>();
            IsExportable = IsEditable = HasDetail = IsDeleteable = IsCreateable = true;
        }

        public override string ToString()
        {
            return RepositoryMethod;
        }
    }
}
