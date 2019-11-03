using NoDb.Data.Domain.Enums;

namespace NoDb.Data.Domain.RevisionModels
{
    public class NoDbRevisionDetail
    {
        public NoDbRevisionAction Action { get; set; }
        public NoDbRevisionType ObjectType { get; set; }

        public object OldValue { get; set; }
        //[TypeConverter(typeof(NoDbRevisionConverter))]
        public object NewValue { get; set; }
    }
}
