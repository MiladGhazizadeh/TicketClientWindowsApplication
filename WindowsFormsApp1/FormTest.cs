using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace TicketClientApp
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
        }

        private void FormTest_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            //CreateShortcut(path, Application.ExecutablePath, "TicketClientApp");

            //string path2 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //CreateShortcut(path2, Application.ExecutablePath, "TicketClientApp");


            //string path2 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\111.lnk";
            //FileInfo fi3 = new FileInfo(path2);
            //if (fi3.Exists)
            //    fi3.Delete();




            DeleteStartupFolderShortcuts(Path.GetFileName(Application.ExecutablePath));

            CreateStartupFolderShortcut();

        }


        void CreateShortcut(string path, string targetpath, string shortcutname)
        {
            var wsh = new IWshShell_Class();
            IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
              path + "\\" + shortcutname + ".lnk") as IWshRuntimeLibrary.IWshShortcut;
            shortcut.TargetPath = targetpath;
            shortcut.Save();
        }


        public static void CreateStartupFolderShortcut()
        {
            IWshShell wshShell = new WshShell();

            //WshShellClass wshShell = new WshShellClass();

            IWshShortcut shortcut;
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Create the shortcut
            shortcut = (IWshShortcut)wshShell.CreateShortcut(startUpFolderPath + "\\" + Application.ProductName + ".lnk");

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

    }
}
