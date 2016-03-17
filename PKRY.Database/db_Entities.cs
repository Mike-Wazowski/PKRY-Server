using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRY.Database
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public partial class db_Entities : DbContext
    {
        public db_Entities() : base(nameOrConnectionString: "Database=PKRY;Data Source=eu-cdbr-azure-west-d.cloudapp.net;User Id=b3ee8b5f16eb90;Password=593c690c") { }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
