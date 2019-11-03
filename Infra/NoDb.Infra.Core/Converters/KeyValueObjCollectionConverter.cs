using System;
using System.Collections;
using System.ComponentModel;

namespace NoDb.Infra.Core.Converters
{
    /// <summary>
    /// This is a special type converter which will be associated with the KeyValueObjCollection class.
    /// It converts an KeyValueObjCollection object to a string representation for use in a property grid.
    /// </summary>
    public class KeyValueObjCollectionConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destType)
        {
            if (destType == typeof(string) && value is IList)
            {
                if (((IList)value).Count > 0)
                    return ((IList)value)[0] + "...";
                return "";
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
