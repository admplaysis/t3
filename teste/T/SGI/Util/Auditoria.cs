using SGI.Context;
using SGI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Util
{
    public static class Auditoria
    {
        /// <summary>
        /// Metódo para incluir auditoria de movimentações realizadas no sistema.
        /// </summary>
        /// <param name="auditor">Objeto do tipo T_Auditoria</param>
        public static void Registrar(T_Auditoria auditor)
        {
            JSgi db = new JSgi();
            auditor.T_Usuario = db.T_Usuario.Find(auditor.ID_USUARIO);
            db.T_Auditoria.Add(auditor);
            db.SaveChanges();
        }
    }
}
