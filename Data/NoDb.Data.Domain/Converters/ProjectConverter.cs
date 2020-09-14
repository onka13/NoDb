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
            var values = StaticManager.GetSolution().Projects.Select(x => x.Project.Name).ToList();
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
                //var projectModel = ConverterManager.GetSolution().Projects.FirstOrDefault(x => x.Project.Name == value?.ToString());
                //if (projectModel != null)
                //{
                //    return projectModel.Project.Name;
                //}
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
