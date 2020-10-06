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
                var project = StaticManager.GetSelectedProject(StaticManager.SelectedProject);
                var table = project.Tables.FirstOrDefault(x => x.Detail.Name == StaticManager.SelectedForeignTable);
                if (table != null)
                    response = table.ColumnsWithRelated();
            }
            else
            {
                response = StaticManager.SelectedTable?.ColumnsWithRelated();
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
