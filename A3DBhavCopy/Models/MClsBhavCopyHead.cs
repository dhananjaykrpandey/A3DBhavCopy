using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3DBhavCopy.Models
{
    [Table("BhavCopyHead")]
    class MClsBhavCopyHead
    {
        [Key]
        public int iFileID { get; set; }
        public string cFileName { get; set; }
        public DateTime dFileDate { get; set; }
        public DateTime dFileUploadDate { get; set; }
    }
}
