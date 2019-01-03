using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SGI.Models.Custom
{
    [NotMapped]
    public class AprovadorCustom
    {
        public string Codigo { get; set; }
        public string Tipo { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}