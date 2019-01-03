using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using SGI.Context;


namespace SincronizadorClpConsole
{
    class Program
    {

        static bool ExtratorRodou =false;
        static void Main(string[] args)
        {
            Timer timerExtrator;
            new MaquinasTimerController().IniciarSincronizacao();
            timerExtrator = new Timer(new TimerCallback(Extrator), null, 1000, 1000);
            Console.ReadKey();
        }


        static void Extrator(object sender)
        {
            try
            {
                if (DateTime.Now.ToString("HHmm") == "1658")
                {
                    if ( ExtratorRodou == false)
                    {
                        JSgi db = new JSgi();
                        ExtratorRodou = true;
                        Console.WriteLine("Executando-> " + " SP_SGI_EXTRATOR '" + DateTime.Now.ToString("yyyyMMdd") + "', ''");
                        db.Database.ExecuteSqlCommand(" SP_SGI_EXTRATOR "+ DateTime.Now.ToString("yyyyMMdd")+", ''");
                    }
                }
                else
                {
                    ExtratorRodou = false;
                }


                StreamWriter vWriter = new StreamWriter(@"c:\Play\Extrator.txt", true);
                vWriter.WriteLine("Servico Rodando: " + DateTime.Now.ToString());
                vWriter.Flush();
                vWriter.Close();
                //Console.WriteLine("->"+ DateTime.Now.ToString("HHmm")+"    "+ DateTime.Now.ToString("HH:mm:ss"));
                

            }
            catch (Exception)
            {
                StreamWriter vWriter = new StreamWriter(@"c:\Play\Extrator.txt", true);
                vWriter.WriteLine("ERRO Extrator: " + DateTime.Now.ToString());
                Console.WriteLine("ERRO Extractor;");

                throw;
            }



        }



    }
}
