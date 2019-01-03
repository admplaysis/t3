using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SGI.Autenticacao
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) {
            return Roles.Contains(role);
        }
        public CustomPrincipal(string email, string[] roles)
        {
            this.Roles = roles;
            this.Identity = new GenericIdentity(email);
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Roles { get; set; }
    }
}