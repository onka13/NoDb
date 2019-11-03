using NoDb.Data.Domain.Base;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbEnumItem : NoDbBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
