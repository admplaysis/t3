using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SGI.Autenticacao
{
    interface ICustomPrincipal : IPrincipal
    {
        int Id { get; set; }
        string Name { get; set; }
        string[] Roles { get; set; }
    }
}