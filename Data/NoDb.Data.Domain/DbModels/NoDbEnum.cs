using NoDb.Data.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel;

namespace NoDb.Data.Domain.DbModels
{
    public class NoDbEnum : NoDbBase
    {
        [Category("General")]
        public List<NoDbEnumDetail> EnumList { get; set; }

        public NoDbEnum()
        {
            EnumList = new List<NoDbEnumDetail>();
        }
    }
}
