using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace TicketClientApp
{
    internal class ClsFuncs
    {
        const string DESKey = "ERTYHJKI";
        const string DESIV = "PLUDUMBG";

        public static string DESDecrypt(string stringToDecrypt)//Decrypt the content
        {

            byte[] key;
            byte[] IV;

            byte[] inputByteArray;
            try
            {

                key = Convert2ByteArray(DESKey);

                IV = Convert2ByteArray(DESIV);

                int len = stringToDecrypt.Length; inputByteArray = Convert.FromBase64String(stringToDecrypt);


                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                MemoryStream ms = new MemoryStream();

                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);

                cs.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8; return encoding.GetString(ms.ToArray());
            }

            catch
            {
                //return ex.Message;
                return "ExeptionError";
                //throw ex;
            }


        }

        public static string DESEncrypt(string stringToEncrypt)// Encrypt the content
        {

            byte[] key;
            byte[] IV;

            byte[] inputByteArray;
            try
            {

                key = Convert2ByteArray(DESKey);

                IV = Convert2ByteArray(DESIV);

                inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                MemoryStream ms = new MemoryStream(); CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);

                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }

            catch (System.Exception ex)
            {

                throw ex;
            }

        }

        static byte[] Convert2ByteArray(string strInput)
        {
            int intCounter; char[] arrChar;
            arrChar = strInput.ToCharArray();

            byte[] arrByte = new byte[arrChar.Length];

            for (intCounter = 0; intCounter <= arrByte.Length - 1; intCounter++)
                arrByte[intCounter] = Convert.ToByte(arrChar[intCounter]);

            return arrByte;
        }


        public static void WriteTotextFile(TicketWebService.UserAuthenticateRet authenticate_ret)
        {

            string fileName = Application.StartupPath + "\\Ticket.txt";
            FileInfo fi = new FileInfo(fileName);

            try
            {
                System.IO.File.WriteAllText(fileName, String.Empty);

                //if (fi.Exists)
                //{
                //    fi.Delete();
                //}


                // Create a new file     
                //using (FileStream fs = fi.Create())
                using (FileStream fs = System.IO.File.Create(fileName))
                {
                    Byte[] txt1 = new UTF8Encoding(true).GetBytes(ClsFuncs.DESEncrypt(authenticate_ret.user_san) + "\n");
                    fs.Write(txt1, 0, txt1.Length);

                    Byte[] txt2 = new UTF8Encoding(true).GetBytes(ClsFuncs.DESEncrypt(authenticate_ret.user_cn) + "\n");
                    fs.Write(txt2, 0, txt2.Length);

                    Byte[] txt3 = new UTF8Encoding(true).GetBytes(authenticate_ret.token + "\n");
                    fs.Write(txt3, 0, txt3.Length);

                    Byte[] img = new UTF8Encoding(true).GetBytes(authenticate_ret.user_image + "\n");
                    fs.Write(img, 0, img.Length);

                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        public static void DeleteFiles_for_BeforeInstalation_notUsed()
        {

            string fileName = Application.StartupPath + "\\config.txt";
            FileInfo fi = new FileInfo(fileName);

            // Check if file already exists. If yes, delete it.     
            if (fi.Exists)
            {
                string fileName2 = Application.StartupPath + "\\temp.txt";
                FileInfo fi2 = new FileInfo(fileName2);
                if (fi2.Exists)
                    fi2.Delete();


                fi.Delete();

                //string path_dsk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\111.lnk";
                //FileInfo dsk_fi = new FileInfo(path_dsk);
                //if (dsk_fi.Exists)
                //    dsk_fi.Delete();


                DeleteStartupFolderShortcuts(Path.GetFileName(Application.ExecutablePath));

                CreateStartupFolderShortcut();

                //string path = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                //CreateShortcut(path, Application.ExecutablePath, "TicketClientApp");


                //string path2 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                //CreateShortcut(path, Application.ExecutablePath, "TicketClientApp");
            }


        }

        public static void CreateStartupFolderShortcut()
        {
            IWshShell wshShell = new WshShell();

            //WshShellClass wshShell = new WshShellClass();

            IWshShortcut shortcut;
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Create the shortcut
            shortcut = (IWshShortcut)wshShell.CreateShortcut(startUpFolderPath + "\\" + Application.ProductName + "2.lnk");

            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.WorkingDirectory = Application.StartupPath;
            shortcut.Description = "Launch My Application";
            // shortcut.IconLocation = Application.StartupPath + @"\App.ico";
            shortcut.Save();
        }

        public static void DeleteStartupFolderShortcuts(string targetExeName)
        {
            string startUpFolderPath =
              Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            DirectoryInfo di = new DirectoryInfo(startUpFolderPath);
            FileInfo[] files = di.GetFiles("*.lnk");

            foreach (FileInfo fi in files)
            {
                string shortcutTargetFile = GetShortcutTargetFile(fi.FullName);

                if (shortcutTargetFile.EndsWith(targetExeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    System.IO.File.Delete(fi.FullName);
                }
            }
        }

        public static string GetShortcutTargetFile(string shortcutFilename)
        {
            string pathOnly = Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = Path.GetFileName(shortcutFilename);

            Shell32.Shell shell = new Shell32.Shell();
            Shell32.Folder folder = shell.NameSpace(pathOnly);
            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }

            return String.Empty; // Not found
        }



        public static void CreateShortcut(string path, string targetpath, string shortcutname)
        {
            var wsh = new IWshShell_Class();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
              path + "\\" + shortcutname + ".lnk") as IWshRuntimeLibrary.IWshShortcut;
            shortcut.TargetPath = targetpath;
            shortcut.Save();
        }
    }
}
