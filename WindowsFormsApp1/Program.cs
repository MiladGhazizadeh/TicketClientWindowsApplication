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
                //MessageBox.Show("یک نسخه دیگر از برنامه مورد نظر هم اکنون در حال اجرا میباشد");
                return;
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                #region MyRegion

                //ClsFuncs.DeleteFiles_for_BeforeInstalation();

                //string fileName = Application.StartupPath + "\\temp.txt";
                //FileInfo fi = new FileInfo(fileName);

                //if (fi.Exists)
                //    Application.Run(new Form1());
                //else
                //    Application.Run(new FrmLogin());


                #endregion

                #region determine startup form

                bool exist_autenticate_data = false;

                string fileName = Application.StartupPath + "\\Ticket.txt";
                FileInfo fi = new FileInfo(fileName);

                if (fi.Exists)
                {
                    using (StreamReader sr = File.OpenText(fileName))
                    {

                        string user_san = "";
                        string user_cn = "";
                        string token = "";

                        string line1 = sr.ReadLine();
                        if (line1 != null)
                        {
                            user_san = ClsFuncs.DESDecrypt(line1);

                            string line2 = sr.ReadLine();
                            if (line2 != null)
                            {
                                user_cn = ClsFuncs.DESDecrypt(line2);

                                if (user_san != "ExeptionError" && user_cn != "ExeptionError")
                                {
                                    string line3 = sr.ReadLine();
                                    if (line3 != null)
                                    {
                                        token = line3;
                                        
                                        if (!String.IsNullOrEmpty(token))
                                        {
                                            exist_autenticate_data = true;
                                        }

                                    }
                                }
                                
                            }
                        }


                        sr.Close();
                    }

                    if (exist_autenticate_data)
                        Application.Run(new Form1());
                    else
                        Application.Run(new FrmLogin());

                }else
                {
                    MessageBox.Show("please reinstall application");
                }

                #endregion

                //Application.Run(new FormTest());
            }

            //GC.KeepAlive(mutex);

        }
    }
}
