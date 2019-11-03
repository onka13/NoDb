using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NoDb.Data.Domain.Base
{
    public class NoDbBase
    {
        [Browsable(false)]
        [ReadOnly(true)]
        public string Hash { get; set; }

        public NoDbBase()
        {
            Hash = Guid.NewGuid().ToString("N");
        }
    }
}
