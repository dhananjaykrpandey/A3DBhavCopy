using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3DBhavCopy.Models
{
    class A3DBhavCopyDataContext:DbContext
    {
        public A3DBhavCopyDataContext() : base("A3DBhavCopyData")
        {
            Database.SetInitializer<A3DBhavCopyDataContext>(new CreateDatabaseIfNotExists<A3DBhavCopyDataContext>());
        }
        public DbSet<MClsBhavCopyHead> _MBhavCopyHead { get; set; }
        public DbSet<MClsBhavCopyDetails> _MClsBhavCopyDetails { get; set; }
    }
}
