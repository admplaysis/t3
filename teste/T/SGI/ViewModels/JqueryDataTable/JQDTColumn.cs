using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.ViewModels.JqueryDataTable
{
    public class JQDTColumn
    {
        public string data { get; set; }
        public string name { get; set; }
        public Boolean searchable { get; set; }
        public Boolean orderable { get; set; }
        public JQDTColumnSearch search { get; set; }
    }
}