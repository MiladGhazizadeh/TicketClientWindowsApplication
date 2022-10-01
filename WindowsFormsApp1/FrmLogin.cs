using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace TicketClientApp
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "نام کاربری...")
            {
                textBox1.Text = "";
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox1.Text = "نام کاربری...";
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "رمز عبور...")
            {
                textBox2.Text = "";
            }

        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                textBox2.Text = "رمز عبور...";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(textBox1.Text.Trim()) || textBox1.Text == "نام کاربری...")
                    textBox1.Focus();
                else
                if (string.IsNullOrEmpty(textBox2.Text.Trim()) || textBox2.Text == "رمز عبور...")
                    textBox2.Focus();
                else
                {
                    TicketWebService.AD_AuthenticateWebService TicketService = new TicketWebService.AD_AuthenticateWebService();

                    var authenticate_ret = TicketService.UserAuthenticate(textBox1.Text.Trim(), textBox2.Text.Trim());

                    if (authenticate_ret != null)
                    {
                        if (!String.IsNullOrEmpty(authenticate_ret.message))
                            MessageBox.Show(authenticate_ret.message, "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                        {
                            var token = authenticate_ret.token;

                            if (chk_remind.Checked)
                            {
                               ClsFuncs.WriteTotextFile(authenticate_ret);

                                _tk = token.ToString();
                                san = authenticate_ret.user_san;
                                cn = authenticate_ret.user_cn;
                                img = authenticate_ret.user_image;
                            }
                            else
                            {
                                string fileName = Application.StartupPath + "\\temp.txt";
                                FileInfo fi = new FileInfo(fileName);

                                if (fi.Exists)
                                {
                                    fi.Delete();
                                }

                            }

                            this.Hide();
                            Form1 frm = new Form1();
                            frm.ShowDialog();
                        }
                    }
                    else
                        MessageBox.Show("عدم خروجی وب سرویس", "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }
            catch (Exception em)
            {
                MessageBox.Show(em.Message, "بروز خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void WriteTotextFile(TicketWebService.UserAuthenticateRet authenticate_ret)
        //{


        //    string fileName = Application.StartupPath + "\\temp.txt";
        //    FileInfo fi = new FileInfo(fileName);

        //    try
        //    {
        //        // Check if file already exists. If yes, delete it.     
        //        if (fi.Exists)
        //        {
        //            fi.Delete();
        //        }

        //        // Create a new file     
        //        using (FileStream fs = fi.Create())
        //        {
        //            Byte[] txt1 = new UTF8Encoding(true).GetBytes(ClsFuncs.DESEncrypt(authenticate_ret.user_san) + "\n");
        //            fs.Write(txt1, 0, txt1.Length);

        //            Byte[] txt2 = new UTF8Encoding(true).GetBytes(ClsFuncs.DESEncrypt(authenticate_ret.user_cn) + "\n");
        //            fs.Write(txt2, 0, txt2.Length);

        //            Byte[] txt3 = new UTF8Encoding(true).GetBytes(authenticate_ret.token + "\n");
        //            fs.Write(txt3, 0, txt3.Length);

        //            Byte[] img = new UTF8Encoding(true).GetBytes(authenticate_ret.user_image + "\n");
        //            fs.Write(img, 0, img.Length);

        //        }

        //        //using (StreamReader sr = File.OpenText(fileName))
        //        //{
        //        //    string s = "";
        //        //    while ((s = sr.ReadLine()) != null)
        //        //    {
        //        //        Console.WriteLine(s);
        //        //    }
        //        //}
        //    }
        //    catch (Exception Ex)
        //    {
        //        Console.WriteLine(Ex.ToString());
        //    }
        //}


        public static string _tk = "";
        public static string san = "";
        public static string cn = "";
        public static string img = "";


        private void FrmLogin_Load(object sender, EventArgs e)
        {
            textBox1.Text = "نام کاربری...";
            textBox2.Text = "رمز عبور...";


            //string fileName = Application.StartupPath + "\\temp.txt";
            //FileInfo fi = new FileInfo(fileName);

            //if (fi.Exists)
            //{
            //    using (StreamReader sr = File.OpenText(fileName))
            //    {
            //        string user_san = "";
            //        string user_cn = "";
            //        string token = "";
            //        string user_image = "";

            //        string line1 = sr.ReadLine();
            //        if (line1 != null)
            //        {
            //            user_san = ClsFuncs.DESDecrypt(line1);

            //            string line2 = sr.ReadLine();
            //            if (line2 != null)
            //            {
            //                user_cn = ClsFuncs.DESDecrypt(line2);


            //                string line3 = sr.ReadLine();
            //                if (line3 != null)
            //                {

            //                    token = line3;

            //                    string line4 = sr.ReadLine();
            //                    if (line4 != null)
            //                        user_image = line4;

            //                    _tk = token;
            //                    san = user_san;
            //                    cn = user_cn;
            //                    img = user_image;

            //                    this.Hide();
            //                    Form1 frm = new Form1();
            //                    frm.Show();
            //                }
            //            }
            //        }
            //    }
            //}

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (!string.IsNullOrEmpty(textBox1.Text.Trim()) && textBox1.Text != "نام کاربری...")
                    textBox2.Focus();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                if (!string.IsNullOrEmpty(textBox2.Text.Trim()) && textBox2.Text != "رمز عبور...")
                    btnLogin.Focus();
        }

        private void FrmLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
