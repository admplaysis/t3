using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Models
{
    public class Dre
    {
        public List<Tr_Visoes> Tr_Visoes { get; set; }
        public List<DreView> DreView { get; set; }
        public List<Tr_Unidade> Unidades { get; set; }
    }
}