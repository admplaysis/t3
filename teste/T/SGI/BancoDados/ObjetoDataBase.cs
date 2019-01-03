using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.BancoDados
{
    public class ObjetoDataBase
    {
        public Tipo.Objeto Tipo { get; set; }
        public string Nome { get; set; }
        public string Comando { get; set; }
        public Boolean checkVersao()
        {
            /*
             
             EXEC sp_rename '[SGI].[dbo].[T_FILA_PRODUCAO].FPR_TEMPO_RESTANTE_PERFORMANCE', 
            'FPR_TEMPO_RESTANTE_PERFORMANC', 'COLUMN';  
            EXEC sp_rename '[SGI].[dbo].[T_FILA_PRODUCAO].FPR_TEMPO_DECORRIDO_PERFORMANCE', 
            'FPR_TEMPO_DECORRIDO_PERFORMANC', 'COLUMN';  
            EXEC sp_rename '[SGI].[dbo].[T_FILA_PRODUCAO].FPR_TEMPO_DECORRIDO_PEQUENAS_PARADAS', 
            'FPR_TEMPO_DECO_PEQUENA_PARADA', 'COLUMN';  

             ALTERAR CAMPO FPR_OBS DE TEXT PARA VARCHAR 8000 
             
             */

            return true;
        }
    }
}