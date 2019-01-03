using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class ItensCalendario
    {
        public int Id { get; set; }
        public DateTime DataDe { get; set; }
        public DateTime DataAte { get; set; }
        public string Observacao { get; set; }

        public int Tipo { get; set; }   //enumerator: 1=Expediente Normal, 2=Sem Expediente de trabalho, 3=Folga, 4=Feriado, 5=Troca de Feriado, 6=Ferias Coletivas, 7=Outros.
        public string TurnoId { get; set; }
        public string TurmaId { get; set; }
        public virtual Turno Turno { get; set; }
        public virtual Turma Turma { get; set; }
        public int CalendarioId { get; set; }
        public Calendario Calendario { get; set; }
    }
}