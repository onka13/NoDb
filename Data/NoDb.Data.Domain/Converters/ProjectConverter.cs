using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace NoDb.Data.Domain.Converters
{
    public class ProjectConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var values = StaticManager.Projects.Select(x => x.Name).ToList();
            values.Insert(0, "");
            return new StandardValuesCollection(values);
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
                return value?.ToString();
            }
            try
            {
                return base.ConvertFrom(context, culture, value);
            }
            catch (Exception e)
            {
                return value;
            }
        }
    }
}
