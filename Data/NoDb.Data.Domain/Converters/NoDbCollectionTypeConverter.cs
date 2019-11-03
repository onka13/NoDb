using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.SearchModels;
using System;
using System.Collections;
using System.Collections.Generic;
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
                if (value is List<NoDbIndexColumn>)
                {
                    return string.Join(",", (value as List<NoDbIndexColumn>).Select(x => x.ColumnName));
                }
                if (value is List<NoDbRelationItem>)
                {
                    return string.Join(",", (value as List<NoDbRelationItem>).Select(x => x.ColumnName + "-" + x.ForeignColumn));
                }
                if (value is List<NoDbSearchColumnDetail>)
                {
                    return string.Join(",", (value as List<NoDbSearchColumnDetail>).Select(x => x.ColumnName));
                }
                if (value is List<NoDbSearchDisplayedColumnDetail>)
                {
                    return string.Join(",", (value as List<NoDbSearchDisplayedColumnDetail>).Select(x => x.ColumnName));
                }
                if (value is List<string>)
                {
                    return string.Join(",", ((List<string>)value).Take(5).Select(x => x));
                }
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
