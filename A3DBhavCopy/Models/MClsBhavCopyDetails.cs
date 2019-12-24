using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3DBhavCopy.Models
{
    [Table("BhavCopyDetails")]
    class MClsBhavCopyDetails
    {
        [Key]
        public int iID { get; set; }
        public string cSYMBOL { get; set; }
        public string cSERIES { get; set; }
        public string cOPEN { get; set; }
        public string cHIGH { get; set; }
        public string cLOW { get; set; }
        public string cCLOSE { get; set; }
        public string cLAST { get; set; }
        public string cPREVCLOSE { get; set; }
        public string cTOTTRDQTY { get; set; }
        public string cTOTTRDVAL { get; set; }
        public string cTIMESTAMP { get; set; }
        public string cTOTALTRADES { get; set; }
        public string cISIN { get; set; }
        public DateTime dTIMESTAMP { get; set; }
        public int iFileID { get; set; }
    }
}
