using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DirectXBaseCSharp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += (o, e) =>
                {
                    MessageBox.Show(e.ExceptionObject.ToString());
                };
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
