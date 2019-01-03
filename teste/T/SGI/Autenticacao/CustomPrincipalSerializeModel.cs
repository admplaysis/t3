using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Autenticacao
{
    public class CustomPrincipalSerializeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Roles { get; set; }
    }
}