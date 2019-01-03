using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class TipoMovimentoEstoque
    {
        public TipoMovimentoEstoque()
        {
            MovimentosEstoque = new HashSet<MovimentoEstoque>();
        }
        public string Id { get; set; }
        public string Descricao { get; set; }
        public int SPR { get; set; }//Sistema proprietario: Indica se o valor do campo pode ou não ser manipulado
        public virtual ICollection<MovimentoEstoque> MovimentosEstoque { get; set; }
        public int SeguenciaTansformacaoId { get; set; }
        public int SequencaRepeticaoId { get; set; }
    }
}