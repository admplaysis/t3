using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.BancoDados
{
    public class Tipo
    {
        public enum Banco { SqlServer, Oracle, Mysql, PostgreeSql };
        public enum Objeto { View, Procedure, Trigger, Function }
    }
}