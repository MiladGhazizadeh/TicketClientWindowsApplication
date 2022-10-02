using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicketClientApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            const string appName = "NkumsWindowsDesktopApp"; //"UniqueAppId";
            bool result;
            var mutex = new System.Threading.Mutex(true, appName, out result);

            if (!result)
            {
                MessageBox.Show("یک نسخه دیگر از برنامه مورد نظر هم اکنون در حال اجرا میباشد");
                return;
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                #region MyRegion

                //Application.Run(new Form2());


                ClsFuncs.DeleteFiles_for_BeforeInstalation();

                string fileName = Application.StartupPath + "\\temp.txt";
                FileInfo fi = new FileInfo(fileName);

                if (fi.Exists)
                    Application.Run(new Form1());
                else
                    Application.Run(new FrmLogin());
                #endregion
            }

            //GC.KeepAlive(mutex);

        }
    }
}
