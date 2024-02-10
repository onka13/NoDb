using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace NoDb.Data.Domain.Converters
{
    public class TableConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(StaticManager.Tables.Select(x => x.Detail.Name).ToList());
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
            if (value is string)
            {
                //var table = ConverterManager.GetTable(value.ToString());
                //if (table != null)
                //{
                //    return table.Detail.Name;
                //}
                return value?.ToString();
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
