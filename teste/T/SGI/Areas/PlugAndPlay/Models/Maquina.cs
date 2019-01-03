using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SGI.Areas.PlugAndPlay.Models
{
    public class Maquina
    {
        public Maquina()
        {
            Roteiros = new HashSet<Roteiro>();
            Feedbacks = new HashSet<Feedback>();
            TargetsProduto = new HashSet<TargetProduto>();
            FilasProducao = new HashSet<FilaProducao>();
            MovimentosEstoque = new HashSet<MovimentoEstoque>();
        }
        [Required(ErrorMessage = "Prencimento Obrigatório.")]
        [MaxLength(10, ErrorMessage = "Permitido no máximo 10 caracteres.")]
        [Display(Name = "ID")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Prencimento Obrigatório.")]
        [MaxLength(100, ErrorMessage = "Permitido no máximo 100 caracteres.")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [MaxLength(30, ErrorMessage = "Permitido no máximo 100 caracteres.")]
        [Display(Name = "IP do Sensor")]
        public string ControlIp { get; set; }
        public int CalendarioId { get; set; }

        public int Sirene { get; set; }
        public string CorSemafaro { get; set; }
        [Required(ErrorMessage = "Prencimento Obrigatório.")]
        [Display(Name = "Tipo do contador : 1 - Produção em Série , 2 - Contagem para Gestao da Identificação")]
        [Range(1, 2, ErrorMessage = "Valores para tipo COntador :  CONTADOR_PRODUCAO_EM_SERIE = 1, CONTADOR_GESTAO_DE_IDENTIFICACAO=2 ")]
        public int TipoContador { get; set; }
        public string GrupoMaquinaId { get; set; }
        public Calendario Calendario { get; set; }
        public GrupoMaquina GrupoMaquina { get; set; }
        public string MaqIdMaqPai { get; set; }
        public virtual ICollection<Roteiro> Roteiros { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<TargetProduto> TargetsProduto { get; set; }
        public virtual ICollection<FilaProducao> FilasProducao { get; set; }
        public virtual ICollection<MovimentoEstoque> MovimentosEstoque { get; set; }
    }
}