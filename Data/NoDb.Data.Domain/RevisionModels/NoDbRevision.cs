using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;

namespace NoDb.Data.Domain.RevisionModels
{
    public class NoDbRevision
    {
        public NoDbTable OldTable { get; set; }
        public NoDbTable NewTable { get; set; }
        public DateTime ActionDate { get; set; }

        public List<NoDbRevisionDetail> Revisions { get; set; }

        public NoDbRevision()
        {
            Revisions = new List<NoDbRevisionDetail>();
            ActionDate = DateTime.Now;
        }
    }
}
