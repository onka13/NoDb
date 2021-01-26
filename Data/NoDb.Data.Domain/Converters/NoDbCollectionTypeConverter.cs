using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace NoDb.Data.Domain.Converters
{
    public class NoDbCollectionTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value != null && destinationType == typeof(string))
            {
                if (value is IList)
                {
                    var text = ((IList)value).Cast<object>().Take(5);
                    return string.Join(",", text);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
