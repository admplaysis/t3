using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class MedicaoManual
    {
        public int tipo { get; set; }
        public T_Metas Meta { get; set; }
        public List<T_Medicoes> Medicoes { get; set; }
    }
}