using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKRY.Database
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("id_user")]
        public int id { get; set; }
        public string user_name { get; set; }
        public string hash { get; set; }
        public string salt { get; set; }
    }
}
