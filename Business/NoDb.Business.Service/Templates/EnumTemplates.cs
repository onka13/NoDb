using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;

namespace NoDb.Business.Service.Templates
{
    public class EnumTemplates
    {
        public static NoDbEnumDetail Default()
        {
            return new NoDbEnumDetail
            {
                Name = "EnumName",
                EnumType = NoDbDataEnumType.INT,
                Items = new List<NoDbEnumItem>
                {
                    new NoDbEnumItem { Name = "EnumItem1", Value = 1 },
                    new NoDbEnumItem { Name = "EnumItem2", Value = 2 }
                }
            };
        }
    }
}
