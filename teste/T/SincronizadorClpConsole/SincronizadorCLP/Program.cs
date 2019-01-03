using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using SGI.Context;

namespace SincronizadorCLP
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        static void Main()
        {
            //System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "Log4.txt");
            //MaquinasTimerController maqControler = new MaquinasTimerController();
            //maqControler.IniciarSincronizacao();
            //while (true)
            //{

            //}
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
