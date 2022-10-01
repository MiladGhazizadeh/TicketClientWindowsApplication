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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form2());


            string fileName = Application.StartupPath + "\\temp.txt";
            FileInfo fi = new FileInfo(fileName);

            if (fi.Exists)
                Application.Run(new Form1());
            else
                Application.Run(new FrmLogin());

        }
    }
}
