using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace NoDb.Data.Domain.Converters
{
    public class EnumColumnConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(StaticManager.Enums.Select(x => x.Name).ToList());
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
                foreach (var b in StaticManager.Enums)
                {
                    if (b.Name == value.ToString())
                    {
                        return b.Name;
                    }
                }
            }
            try
            {
                return base.ConvertFrom(context, culture, value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return value;
            }
        }
    }
}
