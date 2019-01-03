using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.ViewModels.JqueryDataTable
{
    public class JQDTParams
    {
        public int draw { get; set; }

        public int start { get; set; }
        public int length { get; set; }
        public JQDTColumnSearch search { get; set; }
        public List<JQDTColumnOrder> order { get; set; }
        public List<JQDTColumn> columns { get; set; }
    }
}