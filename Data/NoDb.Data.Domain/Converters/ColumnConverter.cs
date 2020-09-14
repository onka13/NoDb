using NoDb.Data.Domain.Attributes;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace NoDb.Data.Domain.Converters
{
    public class ColumnConverter : TypeConverter
    {
        List<NoDbColumn> GetColumns(ITypeDescriptorContext context)
        {
            List<NoDbColumn> response = null;
            var onkafAttribute = (NoDbColumnAttribute)context.PropertyDescriptor.Attributes?[typeof(NoDbColumnAttribute)];
            if (onkafAttribute != null)
            {
                //if (onkafAttribute.ColumnType == NoDbColumnType.ForeignColumn)
                var project = StaticManager.GetSolution().Projects.FirstOrDefault(x => x.Project.Name == StaticManager.SelectedProject);
                var table = project.Tables.FirstOrDefault(x => x.Detail.Name == StaticManager.SelectedForeignTable);
                if (table != null)
                    response = table.ColumnsWithRelated();

                //else if (onkafAttribute.ColumnType == NoDbColumnType.ForeignColumnPrimaryKey)
                //    response = ConverterManager.GetSelectedForeignTable()?.GetPkColumns();
            }
            else
            {
                response = StaticManager.GetSelectedTable()?.ColumnsWithRelated();
            }
            return response ?? new List<NoDbColumn>();
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(GetColumns(context).Select(x => x.Name).ToList());
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value != null)
            {
                return value.ToString();
                //foreach (var b in GetColumns(context))
                //{
                //    if (b.Name == value.ToString())
                //    {
                //        return b.Name;
                //    }
                //}
            }
            try
            {
                return base.ConvertFrom(context, culture, value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }

        }
    }
}
