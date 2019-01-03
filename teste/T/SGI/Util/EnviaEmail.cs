using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace SGI.Util
{
    public static class EnviaEmail
    {
        public static bool EnviarEmail(string assunto, string destinatarios, string mensagem, string titulo)
        {
            string remetente = "workflow@jaepel.com.br";
            string smtp = "10.0.10.13";
            string senha = "";
            int porta = 25;
            char[] separadores = { ';' };
            destinatarios = "washington@libertsolutions.com.br;angelo.fontana@liertsolutions.com.br";
            string[] emailsDestinos = destinatarios.Split(separadores);
            try
            {
                MailMessage mailMessage = new MailMessage();
                //Endereço que irá aparecer no e-mail do usuário 
                mailMessage.From = new MailAddress(remetente, "Workflow " + titulo);
                //destinatarios do e-mail, para incluir mais de um basta separar por ponto e virgula  
                mailMessage.To.Add(emailsDestinos[0]);
                mailMessage.Subject = assunto;
                mailMessage.IsBodyHtml = true;
                //Envia copia de e-mail
                if (emailsDestinos.Count() > 1)
                {
                    for (int i = 1; i < emailsDestinos.Count(); i++)
                    {
                        MailAddress copy = new MailAddress(emailsDestinos[i]);
                        mailMessage.CC.Add(copy);
                    }
                }

                //Envia e-mail 
                //mailMessage.CC.Add(new MailAddress("angelo.fontana@jaepel.com.br"));
                //conteudo do corpo do e-mail 
                mailMessage.Body = mensagem;
                mailMessage.Priority = MailPriority.High;
                //smtp do e-mail que irá enviar 
                SmtpClient smtpClient = new SmtpClient(smtp, porta);
                smtpClient.EnableSsl = false;
                //credenciais da conta que utilizará para enviar o e-mail 
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(remetente, senha);
                //smtpClient.UseDefaultCredentials = true;
                smtpClient.Send(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Texto de e-mail para aprovações referente a pedido de compras
        /// </summary>
        /// <param name="pedCustom">Objeto do tipo PedidoCustom</param>
        /// <returns>Retorna texto a ser enviado por e-mail</returns>
        public static string TextoEmailPedido(Models.Custom.PedidoCustom pedCustom)
        {
            string texto = "";
            texto = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/Util/pcaprovado.html"));
            texto = texto.Replace("%PEDIDO%", pedCustom.Pedido);
            texto = texto.Replace("%EMPRESA%", pedCustom.Empresa);
            texto = texto.Replace("%C7_EMISSAO%", (string.IsNullOrEmpty(pedCustom.Emissao) ? "" : DateTime.ParseExact(pedCustom.Emissao, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")));
            texto = texto.Replace("%CR_TOTAL%", pedCustom.Total.ToString());
            texto = texto.Replace("%C7_USER%", pedCustom.Comprador != null ? pedCustom.Comprador.Nome : "");
            texto = texto.Replace("%CR_DESCONTO%", pedCustom.ItensPedCompra.Sum(x => x.ValDesc).ToString());
            texto = texto.Replace("%A2_NOME%", pedCustom.ForNome);
            texto = texto.Replace("%CR_LIQUIDO%", "");
            texto = texto.Replace("%FRETE%", pedCustom.TpFrete);
            texto = texto.Replace("%E4_DESCRI%", pedCustom.CondPag);
            texto = texto.Replace("%Y_JPSOLICIT%", pedCustom.Solicitante != null ? pedCustom.Solicitante.Nome : "");
            //Busca itens do pedido de compras
            string itens = "";
            foreach (var item in pedCustom.ItensPedCompra)
            {
                itens += "<tr bgcolor='#FFFFFF' class='texto-layer'>";
                itens += "<td height='1'><div align='center' class='style12'>" + item.Item + "</div></td>";//Item
                itens += "<td height='1'><div align='center' class='style12'>" + item.CodProduto + "</div></td>";//Código produto
                itens += "<td height='1'><div align='center' class='style12'>" + item.Produto + "</div></td>";//Desrição produto
                itens += "<td height='1'><div align='center' class='style12'>" + item.Un + "</div></td>";//Unidade de medida
                itens += "<td height='1'><div align='center' class='style12'>" + item.Qtd.ToString() + "</div></td>";//Quantidade
                itens += "<td height='1'><div align='center' class='style12'>" + item.Preco.ToString() + "</div></td>";//Vlr.unitário
                itens += "<td height='1'><div align='center' class='style12'>" + item.Ipi.ToString() + "</div></td>";//Ipi
                itens += "<td height='1'><div align='center' class='style12'>" + item.Total.ToString() + "</div></td>";//Valor Total
                itens += "<td height='1'><div align='center' class='style12'>" + (string.IsNullOrEmpty(item.DtEntrega) ? "" : DateTime.ParseExact(item.DtEntrega, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")) + "</div></td>";//Data entrega
                itens += "<td height='1'><div align='center' class='style12'>" + item.UltPrcCopra.ToString() + "</div></td>";//Data Ultimo preço de compra
                itens += "<td height='1'><div align='center' class='style12'>" + (string.IsNullOrEmpty(item.DtUlPrc) ? "" : DateTime.ParseExact(item.DtUlPrc, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")) + "</div></td>";//Ultimo preço de compra
                itens += "<td height='1'><div align='center' class='style12'>" + item.CCusto + "</div></td>";//Centro de custos
                itens += "<td height='1'><div align='center' class='style12'>" + item.NumSc + "</div></td>";//Número SC
                itens += "</tr>";
            }
            texto = texto.Replace("%ITENS%", itens);

            //Busca aprovadores do pedido
            string aprov = "";
            foreach (var item in pedCustom.LstAprovacoes)
            {
                aprov += "<tr bgcolor='#FFFFFF' class='texto-layer'>";
                aprov += "<td height='1'><div align='center'>" + item.Nivel + "</div></td>";//Nível
                //Status da aprovação
                if (item.Status == "02")
                {
                    aprov += "<td height='1'><div align='center'>Aguardando Aprovação</div></td>";//Status
                }
                else if (item.Status == "03")
                {
                    aprov += "<td height='1'><div align='center'>Aprovado</div></td>";//Status
                }
                else if (item.Status == "04")
                {
                    aprov += "<td height='1'><div align='center'>Rejeitado</div></td>";//Status
                }
                aprov += "<td height='1'><div align='center'>" + item.Nome + "</div></td>";//Aprovado por
                aprov += "<td height='1'><div align='center'>" + (string.IsNullOrEmpty(item.DtLib) ? "" : DateTime.ParseExact(item.DtLib, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")) + "</div></td>";//Data aprovação
                aprov += "<td height='1'><div align='center'>" + item.Obs + "</div></td>";//Observação
                aprov += "</tr>";
            }
            texto = texto.Replace("%APROVADORES%", aprov);
            return texto;
        }

        /// <summary>
        /// Texto de e-mail para aprovações referente a pedido de compras
        /// </summary>
        /// <param name="nfeCustom">Objeto do tipo PedidoCustom</param>
        /// <returns>Retorna texto a ser enviado por e-mail</returns>
        public static string TextoEmailNfFinan(Models.Custom.NfeCustom nfeCustom)
        {
            string texto = "";
            return texto;
        }
    }
}