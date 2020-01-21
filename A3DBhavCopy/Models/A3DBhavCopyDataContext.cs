using System.Data.Entity;

namespace A3DBhavCopy.Models
{
    class A3DBhavCopyDataContext:DbContext
    {
        public A3DBhavCopyDataContext(string StrConnectionString) : base(StrConnectionString)
        {

    //         < connectionStrings >
    //< add name = "A3DBhavCopyData"  connectionString = "Server=(localdb)\v11.0;Integrated Security=true; Database=A3DBhavCopyData" providerName = "System.Data.SqlClient" />
     
    //   </ connectionStrings >
                 //\\AttachDbFileName = C:\MyFolder\MyData.mdf;"A3DBhavCopyData"
                 Database.SetInitializer<A3DBhavCopyDataContext>(new CreateDatabaseIfNotExists<A3DBhavCopyDataContext>());
        }
        public DbSet<MClsBhavCopyHead> _MBhavCopyHead { get; set; }
        public DbSet<MClsBhavCopyDetails> _MClsBhavCopyDetails { get; set; }
    }
}
