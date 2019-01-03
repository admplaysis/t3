namespace SincronizadorCLP
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceSincClpProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceSincClpInstaller1 = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceSincClpProcessInstaller1
            // 
            this.serviceSincClpProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceSincClpProcessInstaller1.Password = null;
            this.serviceSincClpProcessInstaller1.Username = null;
            // 
            // serviceSincClpInstaller1
            // 
            this.serviceSincClpInstaller1.Description = "d1";
            this.serviceSincClpInstaller1.DisplayName = "d1";
            this.serviceSincClpInstaller1.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceSincClpProcessInstaller1});
            this.serviceSincClpInstaller1.ServiceName = "SincronizadorClp";
            this.serviceSincClpInstaller1.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.serviceSincClpInstaller1_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceSincClpInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceSincClpProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceSincClpInstaller1;
    }
}