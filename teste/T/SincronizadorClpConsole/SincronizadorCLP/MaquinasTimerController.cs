using SGI.Areas.PlugAndPlay.Models;
using SGI.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SincronizadorCLP
{
    class MaquinasTimerController
    {
        private List<MaquinaTimer> _maquinasTimerControl = new List<MaquinaTimer>();
        public void IniciarSincronizacao()
        {
            System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "LogEntrouIniSinc.txt");
            try
            {
                using (JSgi bd = new JSgi())
                {
                    List<Maquina> maquinas = bd.Maquina.ToList();
                    foreach (var m in maquinas)
                    {
                        if (m.ControlIp != null)
                        {
                            MaquinasTimerControl.Add(new MaquinaTimer(m));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "LogErroIniSinc.txt");
            }

        }
        internal List<MaquinaTimer> MaquinasTimerControl { get => _maquinasTimerControl; set => _maquinasTimerControl = value; }
    }
}
